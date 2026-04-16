using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity_Layer.DTOs.CommentDtos.MLForComment
{
    public class ToxicityAnalysisDto
    {
        public string Text { get; set; } //analiz edilen metin
        public double Score { get; set; } //toxicity score değerine gelecek skor
    }
}
