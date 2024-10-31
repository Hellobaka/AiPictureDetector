namespace PublicInfos.Model
{
    public class DetectorResult
    {
        public bool Success { get; set; }

        public double AIScore { get; set; }

        public double NotAIScore { get; set; }

        public string ModelName { get; set; }
    }
}
