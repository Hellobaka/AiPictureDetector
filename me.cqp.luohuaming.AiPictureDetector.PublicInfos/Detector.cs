using PublicInfos.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;

namespace PublicInfos
{
    public static class Detector
    {
        private static string BaseUrl { get; set; } = "https://ajax.thehive.ai/api/demo/classify?endpoint=ai_generated_image_detection&data_url=&hash=&image_url=";

        public static DetectorResult? Get(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return null;
            }
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
            var t = http.PostAsync(BaseUrl, form).Result;
            // t.EnsureSuccessStatusCode();

            var json = JsonConvert.DeserializeObject<DetectorResult>(t.Content.ReadAsStringAsync().Result);
            return json;
        }
    }
}
