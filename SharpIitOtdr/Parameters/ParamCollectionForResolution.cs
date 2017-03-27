using System.Collections.Generic;

namespace IitOtdrLibrary
{
    public class ParamCollectionForResolution
    {
        public string[] PulseDurations { get; set; }
        public string SelectedPulseDuration { get; set; }
        public Dictionary<string, string> TimeToAverage { get; set; }
        public string SelectedTimeToAverage { get; set; }
        public Dictionary<string, string> MeasurementCountToAverage { get; set; }
        public string SelectedMeasurementCountToAverage { get; set; }

        public double Bc { get; set; }
        public double Ob { get; set; }
    }
}