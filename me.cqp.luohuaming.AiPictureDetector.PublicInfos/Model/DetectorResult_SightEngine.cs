namespace PublicInfos.Model
{
    public class DetectorResult_SightEngine
    {
        public string status { get; set; }

        public Type type { get; set; }

        public class Type
        {
            public float ai_generated { get; set; }
        }
    }
}
