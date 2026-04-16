using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ML.Data;

namespace Entity_Layer.DTOs.CommentDtos.MLForComment
{
    public class ModelOutputDto
    {
        [ColumnName("PredictedLabel")]
        public bool Prediction { get; set; } //model olasılığa bakar ve varsayılan olarak 0.5 üzeirndeyse burayı true yapar.

        [ColumnName("Probability")]
        public float Score { get; set; } //metnin toksik olma ihtimaline bakar 0.00 ile 1.00 arasındadır.
    }
}
