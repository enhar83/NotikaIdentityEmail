using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ML.Data;

namespace Entity_Layer.DTOs.CommentDtos.MLForComment
{
    public class ModelInputDto
    {
        [ColumnName("CommentText"), LoadColumn(0)]
        public string CommentText { get; set; }

        //toxic ise 1 değilse 0 
        [ColumnName("Label"), LoadColumn(1)]
        public bool Label { get; set; }
    }
}

// LoadColumn[0]: veri bir csv içerisinden okunacağı için 0. sütunun bu veriye ait olduğunu anlatır.
// LoadColumn[1]: veri setindeki 1. sütundur. 
