using System.Linq;

namespace PublicInfos.Model
{
    public class DetectorResult_SightEngine
    {
        public string status { get; set; }

        public Type type { get; set; }

        public class Type
        {
            public float ai_generated { get; set; }

            public Ai_Generators ai_generators { get; set; }

            public float deepfake { get; set; }
        }

        public class Ai_Generators
        {
            public float dalle { get; set; }

            public float firefly { get; set; }

            public float flux { get; set; }

            public float gan { get; set; }

            public float ideogram { get; set; }

            public float midjourney { get; set; }

            public float stable_diffusion { get; set; }

            public float other { get; set; }

            public string GetHighestModel()
            {
                var r = GetType().GetProperties().Select(x => new
                {
                    score = (float)x.GetValue(this),
                    name = x.Name
                }).OrderByDescending(x => x.score);

                return r.FirstOrDefault()?.name ?? "null";
            }
        }
    }
}
