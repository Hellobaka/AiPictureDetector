using PublicInfos.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using me.cqp.luohuaming.AiPictureDetector.PublicInfos;

namespace PublicInfos
{
    public static class Detector
    {
        private static string BaseUrl_HiveAI { get; set; } = "https://ajax.thehive.ai/api/demo/classify?endpoint=ai_generated_image_detection&data_url=&hash=&image_url=";
       
        private static string BaseUrl_SightEngine { get; set; } = "https://api.sightengine.com/1.0/check.json";

        public static DetectorResult Detect(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return null;
            }
            DetectorResult r = new DetectorResult();
            double aiScore = 100;
            double notAIScore = 100;
            switch (AppConfig.DetectorType)
            {
                case DetectorType.HiveAI:
                    var result_hive = HiveAI_Detect(filePath);
                    if (result_hive == null || result_hive.return_code != "0"
                        || result_hive.response == null
                        || result_hive.response.output == null
                        || result_hive.response.output.Length == 0
                        || result_hive.response.output.First().classes.Length == 0)
                    {
                        return r;
                    }
                    var list = result_hive.response.output.First().classes.ToList();
                    var ai = list.FirstOrDefault(x => x.name == "ai_generated");
                    var notAI = list.FirstOrDefault(x => x.name == "not_ai_generated");
                    if (ai == null || notAI == null)
                    {
                        return r;
                    }
                    var highestModel = list.OrderByDescending(x => x.score).FirstOrDefault(x => x.name != "not_ai_generated" && x.name != "ai_generated");
                    aiScore *= ai.score * 100;
                    notAIScore *= notAI.score * 100;

                    r.Success = true;
                    r.AIScore = aiScore;
                    r.NotAIScore = notAIScore;
                    r.ModelName = highestModel.name;
                    break;

                case DetectorType.SightEngine:
                    var result_SightEngine = SightEngine_Detect(filePath);
                    if (result_SightEngine == null
                        || result_SightEngine.type == null)
                    {
                        return r;
                    }
                    r.Success = true;
                    aiScore *= result_SightEngine.type.ai_generated;
                    notAIScore *= 1 - result_SightEngine.type.ai_generated;
                    r.AIScore = aiScore;
                    r.NotAIScore = notAIScore;
                    break;
            }
            return r;
        }

        public static DetectorResult_SightEngine? SightEngine_Detect(string filePath)
        {
            using HttpClient http = new()
            {
                Timeout = TimeSpan.FromSeconds(10),
            };
            using var form = new MultipartFormDataContent();
            var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            var fileContent = new StreamContent(fileStream);
            fileContent.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse("image/jpeg");

            form.Add(fileContent, "media", Path.GetFileName(filePath));
            form.Add(new StringContent("genai"), "models");
            form.Add(new StringContent(AppConfig.SightEngine_UserID), "api_user");
            form.Add(new StringContent(AppConfig.SightEngine_UserSecret), "api_secret");
            var t = http.PostAsync(BaseUrl_SightEngine, form).Result;
            string json = t.Content.ReadAsStringAsync().Result;
            MainSave.CQLog.Debug("SightEngine_Detect", json);
            var r = JsonConvert.DeserializeObject<DetectorResult_SightEngine>(json);
            return r;
        }

        public static DetectorResult_HiveAI? HiveAI_Detect(string filePath)
        {
            using HttpClient http = new()
            {
                Timeout = TimeSpan.FromSeconds(10),
            };
            http.DefaultRequestHeaders.Add("Origin", "https://thehive.ai");
            http.DefaultRequestHeaders.Add("Referer", "https://thehive.ai");
            using var form = new MultipartFormDataContent();
            var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            var fileContent = new StreamContent(fileStream);
            fileContent.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse("image/jpeg");

            // 添加文件到请求
            form.Add(fileContent, "image", Path.GetFileName(filePath));
            form.Add(new StringContent("photo"), "media_type");
            form.Add(new StringContent("classification"), "model_type");
            // form.Add(new StringContent("true"), "require_captcha");
            var t = http.PostAsync(BaseUrl_HiveAI, form).Result;

            string json = t.Content.ReadAsStringAsync().Result;
            MainSave.CQLog.Debug("DetectorResult_HiveAI", json);

            var r = JsonConvert.DeserializeObject<DetectorResult_HiveAI>(json);
            return r;
        }
    }
}
