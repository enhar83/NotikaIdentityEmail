using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entity_Layer.DTOs.MessageDtos;
using Entity_Layer.DTOs.NotificationDtos;
using Entity_Layer.Entities;
using FluentValidation;

namespace Business_Layer.ValidationRules.NotificationValidationRules
{
    public class ComposeNotificationValidator:AbstractValidator<ComposeNotificationDto>
    {
         public ComposeNotificationValidator()
        {
            RuleFor(x=>x.NotificationDetail)
                .NotEmpty().WithMessage("Bildirim içeriği boş olamaz.");

            RuleFor(x => x.NotificationImageUrl)
                .NotEmpty().WithMessage("Bildirim görseli boş olamaz.");

             RuleFor(x=>x.AppUserId)
                .NotEmpty().WithMessage("Bildirim yollanacak kullanıcı adını seçiniz.");
        }
    }
}
