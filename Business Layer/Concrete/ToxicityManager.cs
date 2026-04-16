using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business_Layer.Abstract;
using Entity_Layer.DTOs.CommentDtos.MLForComment;
using Microsoft.ML;

namespace Business_Layer.Concrete
{
    public class ToxicityManager: IToxicityService
    {
        private readonly MLContext _mlContext;
        private readonly string _modelPath;
        private readonly string _dataPath;

        public ToxicityManager()
        {
            _mlContext = new MLContext();

            _modelPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "MLModels", "ToxicityModel.zip");
            _dataPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "MLModels", "toxicity_data.csv");

            string folder = Path.GetDirectoryName(_modelPath);
            if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
        }

        public ToxicityAnalysisDto AnalyzeComment(string text)
        {
            if (!File.Exists(_modelPath))
            {

                TrainModel();
            }

            ITransformer loadedModel = _mlContext.Model.Load(_modelPath, out var modelInputSchema);

            var predictionEngine = _mlContext.Model.CreatePredictionEngine<ModelInputDto, ModelOutputDto>(loadedModel);

            var input = new ModelInputDto { CommentText = text };
            var result = predictionEngine.Predict(input);

            return new ToxicityAnalysisDto
            {
                Text = text,
                Score = result.Score
            };
        }

        public void TrainModel()
        {
            IDataView trainingData = _mlContext.Data.LoadFromTextFile<ModelInputDto>(
                path: _dataPath, hasHeader: true, separatorChar: ',');

            var pipeline = _mlContext.Transforms.Text.FeaturizeText("Features", nameof(ModelInputDto.CommentText))
                .Append(_mlContext.BinaryClassification.Trainers.SdcaLogisticRegression(
                    labelColumnName: nameof(ModelInputDto.Label), featureColumnName: "Features"));

            var model = pipeline.Fit(trainingData);

            _mlContext.Model.Save(model, trainingData.Schema, _modelPath);
        }
    }
}
