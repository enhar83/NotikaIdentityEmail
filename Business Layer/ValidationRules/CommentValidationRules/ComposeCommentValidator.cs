using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entity_Layer.DTOs.CommentDtos;
using FluentValidation;
using FluentValidation.AspNetCore;

namespace Business_Layer.ValidationRules.CommentValidationRules
{
    public class ComposeCommentValidator:AbstractValidator<ComposeCommentDto>
    {
        public ComposeCommentValidator()
        {
            RuleFor(c=>c.Subject)
                .NotEmpty().WithMessage("Yorum başlığı boş olamaz.");

            RuleFor(c => c.CommentDetail)
                .NotEmpty().WithMessage("Yorum detayı boş olamaz.");
        }
    }
}
