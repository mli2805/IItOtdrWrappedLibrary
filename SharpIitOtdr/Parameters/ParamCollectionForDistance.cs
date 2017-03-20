namespace IitOtdrLibrary
{
    public class ParamCollectionForDistance
    {
        public string[] Resolutions { get; set; }
        public string SelectedResolution { get; set; }
        public string[] PulseDurations { get; set; }
        public string SelectedPulseDuration { get; set; }
        public string[] AveragingTime { get; set; }
        public string SelectedAveragingTime { get; set; }
        public string[] AveragingNumber { get; set; }
        public string SelectedAveragingNumber { get; set; }

        public double Bc { get; set; }
        public double Ob { get; set; }
    }
}