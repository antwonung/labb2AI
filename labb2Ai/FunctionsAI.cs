using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using Microsoft.Extensions.Configuration;
namespace labb2Ai
{
    class FunctionsAI
    {
        ComputerVisionClient computerVision;
        string cogSvcEndpoint, cogSvcKey;
        public FunctionsAI()
        {
            // Get config settings from AppSettings
            IConfigurationBuilder builder = new ConfigurationBuilder().AddJsonFile("appSettings.json");
            IConfigurationRoot configuration = builder.Build();
            cogSvcEndpoint = configuration["CognitiveServicesEndpoint"];
            cogSvcKey = configuration["CognitiveServiceKey"];

        }
        public async Task Authorize()
        {
            await Task.Run(() =>
            {
                ApiKeyServiceClientCredentials credentials = new ApiKeyServiceClientCredentials(cogSvcKey);
                computerVision = new ComputerVisionClient(credentials) { Endpoint = cogSvcEndpoint };
            });
          
        }
    
        public async Task<List<string>> GetAiRespons(string filePath)
        {
            //Wait for authorize computer vision client
            await Authorize();
            
            // Analyze image features
            List<VisualFeatureTypes?> features = new List<VisualFeatureTypes?>()
            {
                VisualFeatureTypes.Description,
                VisualFeatureTypes.Tags,
                VisualFeatureTypes.Categories,
                VisualFeatureTypes.Brands,
                VisualFeatureTypes.Objects,
                VisualFeatureTypes.Adult
            };
            var t1 = await AnalyzeLocalAsync(filePath, features);
            return t1;
            
        }

        public async Task<List<string>> AnalyzeLocalAsync(string path, List<VisualFeatureTypes?> feat)
        {
            List<string> AiResult = new List<string>();
            //Get image analysis
            using (Stream imageData = File.OpenRead(path))
            {
                var analysis = await computerVision.AnalyzeImageInStreamAsync(imageData, feat);
                
                // get image captions
                foreach (var caption in analysis.Description.Captions)
                {
                    AiResult.Add($"Description: {caption.Text} (confidence:{caption.Confidence.ToString("P")})");
                }

                //Get image tags
                if (analysis.Tags.Count > 0)
                {
                    Console.WriteLine("Tags:");
                    foreach (var tag in analysis.Tags)
                    {
                        AiResult.Add($" -{tag.Name} (confidence:{ tag.Confidence.ToString("P")})");
                    }
                }


            }
            return AiResult;
        }


        public async Task<Image> GetThumbnail(string imageFile)
        {

            Image thumbnailImage;
            // Generate a thumbnail
            using (Stream imageData = File.OpenRead(imageFile))
            {
                // Get thumbnail data
                thumbnailImage = Image.FromStream(await computerVision.GenerateThumbnailInStreamAsync(100, 100, imageData, true));
          
            }
            return thumbnailImage;

        }


    }
}
