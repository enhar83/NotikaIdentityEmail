using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business_Layer.Abstract;
using Entity_Layer.DTOs.UserSecrets;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using MailKit.Net.Smtp;

namespace Business_Layer.Concrete
{
    public class EmailActivationManager : IEmailActivationService
    {
        private readonly MailSettings _mailSettings;

        public EmailActivationManager(IOptions<MailSettings> mailSettings)
        {
            _mailSettings = mailSettings.Value;
        }

        //receiverEmail: alıcının email adresi
        //code: kullanıcıya gönderilecek olan 6 haneli aktivasyon kodu
        public async Task SendConfirmEmailAsync(string receiverEmail, string code)
        {
            var mimeMessage = new MimeMessage(); //mimemessage: MailKit kütüphanesinin temel sınıfıdır. boş bir eposta zarfı oluşturuyor gibi düşünülebilir. içine alıcı konu ve gövde bilgileri konulacak.

            //MailBoxAddress: email adresini ve yanında görünecek ismi birleştirir. From.Add ile bu zarfın gönderen kısmına buradaki bilgi yazılır. 
            MailboxAddress mailboxAddressFrom = new MailboxAddress(_mailSettings.SenderName, _mailSettings.SenderEmail);
            mimeMessage.From.Add(mailboxAddressFrom);

            //burada da aynı mantıkla alıcının adresini zarfın kime kısmına ekleriz.
            MailboxAddress mailboxAddressTo = new MailboxAddress("Sayın Kullanıcı", receiverEmail);
            mimeMessage.To.Add(mailboxAddressTo);

            //emailin başlığıdır.
            mimeMessage.Subject = "Notika Identity Aktivasyon Kodu";

            //emailin gövdesini oluşturmak için kullanılır. HtmlBody özelliği ile sadece düz metin değil, renkli  yazılar başlıklar ve divler içeren şık bir email gönderilebilir.
            //metnin içerisindeki {code} ifadesi metodun parametresi olan kodu oraya yerleştirir.
            var bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = $@"
            <div style='font-family: Arial, sans-serif;'>
                <h2>Hoş Geldiniz!</h2>
                <p>Hesabınızı onaylamak için kullanmanız gereken 6 haneli aktivasyon kodunuz aşağıdadır:</p>
                <h1 style='color: #4CAF50;'>{code}</h1>
                <p>Eğer bu işlemi siz yapmadıysanız lütfen bu e-postayı dikkate almayın.</p>
            </div>";

            mimeMessage.Body = bodyBuilder.ToMessageBody();

            //using ile bu SmtpClient nesnesinin bellekten güvenli bir şekilde silinmesi sağlanır ve açıkta klan bağlantılar kapanır.
            using (var client = new SmtpClient())
            {
                await client.ConnectAsync(_mailSettings.Server, _mailSettings.Port, SecureSocketOptions.StartTls); //gmail suncusuna bağlanır, 587 portunu ve StartTls güvenliği ile verilerin şifreli gitmesini sağlar.
                await client.AuthenticateAsync(_mailSettings.SenderEmail, _mailSettings.Password); //gmail hesbaına giriş yapar. burada secrets.json içerisine yazılan uygulama şifresi kullanılır ve gmail bu şifreyle doğrulama yapar.
                await client.SendAsync(mimeMessage); //hazırlanan email paketini sunucu üzerinden alıcıya yollar.
                await client.DisconnectAsync(true); //sunucu bağlantısının kapatır. true parametresi bağlantıyı kapatırlen sunucuya QUIT konutu gönderilmesini sağlar. 
            }
        }
    }
}