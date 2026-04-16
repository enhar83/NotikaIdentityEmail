using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entity_Layer.DTOs.CommentDtos.MLForComment;

namespace Business_Layer.Abstract
{
    public interface IToxicityService
    {
        void TrainModel();
        ToxicityAnalysisDto AnalyzeComment(string text);
    }
}
