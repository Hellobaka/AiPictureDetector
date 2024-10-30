using Newtonsoft.Json;

namespace PublicInfos.Model
{
    public class DetectorResult
    {
        public string return_code { get; set; }
       
        public string message { get; set; }
      
        public Response response { get; set; }

        public class Response
        {
            public Output[] output { get; set; }
        }

        public class Output
        {
            public Classify[] classes { get; set; }

            public int time { get; set; }
        }

        public class Classify
        {
            public float score { get; set; }

            [JsonProperty("class")]
            public string name { get; set; }
        }
    }
}
