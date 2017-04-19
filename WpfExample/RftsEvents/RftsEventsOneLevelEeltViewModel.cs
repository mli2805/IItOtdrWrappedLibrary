using Optixsoft.SorExaminer.OtdrDataFormat.Structures;

namespace WpfExample
{
    public class RftsEventsOneLevelEeltViewModel
    {
        public double AttenuationValue { get; set; }
        public string Threshold { get; set; }
        public double DeviationValue { get; set; }
        public string StateValue { get; set; }

        public RftsEventsOneLevelEeltViewModel(RftsLevel rftsLevel, double value)
        {
            AttenuationValue = value;
            var thresholdValue = rftsLevel.EELT.IsAbsolute
                ? rftsLevel.EELT.AbsoluteThreshold / 1000.0
                : rftsLevel.EELT.RelativeThreshold / 1000.0;
            var thresholdType = rftsLevel.EELT.IsAbsolute ? "(abs.)" : "(rel.)";

            Threshold = string.Format( $"{thresholdValue: 0.000} {thresholdType}"); 


        }
    }
}
