using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entity_Layer.DTOs.MessageDtos;
using FluentValidation;

namespace Business_Layer.ValidationRules.MessageValidationRules
{
    public class ComposeMessageValidator:AbstractValidator<ComposeMessageDto>
    {
        public ComposeMessageValidator()
        {
            RuleFor(x => x.SenderEmail)
                .NotEmpty().WithMessage("Lütfen gönderen email adresi giriniz.");
            
            RuleFor(x => x.ReceiverEmail)
                .NotEmpty().WithMessage("Lütfen alıcı email adresi giriniz.");
            
            RuleFor(x => x.CategoryId)
                .NotEmpty().WithMessage("Lütfen bir mesaj kategorisi seçiniz.");
            
            RuleFor(x => x.Subject)
                .NotEmpty().WithMessage("Lütfen mail konusu giriniz.");
            
            RuleFor(x => x.MessageDetail)
                .NotEmpty().WithMessage("Lütfen mesaj içeriğini giriniz.");
        }
    }
}
