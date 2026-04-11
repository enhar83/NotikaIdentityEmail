using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Layer.Abstract
{
    public interface IEmailActivationService
    {
        Task SendConfirmEmailAsync(string receiverEmail, string code);
        Task SendPasswordResetEmailAsync(string receiverEmail, string resetTokenLink);
    }
}
