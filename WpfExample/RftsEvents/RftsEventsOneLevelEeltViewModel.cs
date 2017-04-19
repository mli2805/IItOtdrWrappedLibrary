using Optixsoft.SorExaminer.OtdrDataFormat.Structures;

namespace WpfExample
{
    public class RftsEventsOneLevelEeltViewModel
    {
        private readonly RftsLevel _rftsLevel;

        public double AttenuationValue { get; set; }
        public string Threshold { get; set; }
        public double DeviationValue { get; set; }
        public string StateValue { get; set; }

        public RftsEventsOneLevelEeltViewModel(RftsLevel rftsLevel, double value)
        {
            _rftsLevel = rftsLevel;

            AttenuationValue = value;
            var thresholdValue = _rftsLevel.EELT.IsAbsolute
                ? _rftsLevel.EELT.AbsoluteThreshold / 1000.0
                : _rftsLevel.EELT.RelativeThreshold / 1000.0;
            var thresholdType = _rftsLevel.EELT.IsAbsolute ? "(abs.)" : "(rel.)";

            Threshold = string.Format( $"{thresholdValue: 0.000} {thresholdType}"); 


        }
    }
}
