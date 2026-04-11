using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Business_Layer.Abstract;
using Business_Layer.Configurations;
using Entity_Layer.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Business_Layer.Concrete
{
    public class TokenManager:ITokenService
    {
        private readonly JwtSettings _jwtSettings;

        //IOptions<JwtSettings> sayesinde appsettings içerisindeki verileri buraya otomatik gelir.
        //ayarları doğrudan okumak yerine wrapper yapısı kullanılıyor. .Value ile Key,Issuer gibi verilere ulaşılır.
        public TokenManager(IOptions<JwtSettings> jwtSettings)
        {
            _jwtSettings = jwtSettings.Value;
        }

        //bir kullanıcı alır ve ona karşılık bir Token (string) verir.
        public string CreateToken(AppUser user)
        {
            //ilk olarak claimler hazırlanır. bir API endpointidir ve görevi kullanıcının gönderdiği bilgileri JWT claimlerine çevirmektir.
            //Claim: bu kullanıcı kim sorusunun cevabıdır. 
            //Token çözüldüğünde bu bilgiler dbye gitmeden okunabilir.

            var claims = new List<Claim>
            {
                //standart .net clamileri (authorize attributeları bunlar otomatik tanır)
                // ?? "" kullanılmasının sebebi boş bir prop varsa, uygulamanın çokmesi engellenmek içindir. NullReferenceException önlemek için boş string ataması yapılıyor.
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email ?? ""),
                new Claim(ClaimTypes.Name, user.UserName ?? ""),

                //custom claimler. kullanıcı tarafından tanımlanan özel proplardır. 
                new Claim("name",user.Name ?? ""),
                new Claim("surname",user.Surname ?? ""),
                new Claim("city",user.City ?? ""),

                //JTI: her tokena özel benzersiz bir id verir. güvenlik için önemlidir.
                //aynı saniyede iki token üretilise bile birbirinden farklı olmalarını sağlamak içindir. replay ttack denilen saldırılar zorlaşır.
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            //şifreleme anahtarı oluşturulur.
            //appsettings içerisindeki Key stringini byte dizisine çevirip simetrik bir anahtar yapma işlemidir.
            //simetrik denme sebebi aynı anahtarın hem tokenı imzalamak hem de sunucuya geri geldiğinde doğrulamak içim kullanılmasıdır.
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));

            //bu anahtarı kullanarak HMAC-SHA256 algortmasıyla bir imzalama kimliği oluşturuluyor. Böylece JWT web token dijital olarak imzalanır. 
            //bu mühür tokenın içeriği değişirse imzanın bozulmasını sağlar. 
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            //burada token nesnesi oluşturulur.
            //tüm parçalar birleştirilir, kim bastı?, kimin için?, hangi bilgilerle?, ne kadar süreyle? 
            var tokenOptions = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer, //yayınlayan (localhost)
                audience: _jwtSettings.Audience, //alıcı (localhost)
                claims: claims, //kullanıcı bilgileir
                expires: DateTime.Now.AddMinutes(_jwtSettings.ExpireMinutes), //bitiş süresi
                signingCredentials: creds //güvenlik mührü
            );

            //oluşturulan nesneyi Header.Payload.Signature formatında bir string olarak döner. 
            return new JwtSecurityTokenHandler().WriteToken(tokenOptions);
        }
    }
}
