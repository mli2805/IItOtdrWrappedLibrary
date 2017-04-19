using Optixsoft.SorExaminer.OtdrDataFormat.Structures;

namespace WpfExample
{
    public class RftsEventsOneLevelViewModel
    {
        private readonly RftsLevel _rftsLevel;
        public string ComparisonResult { get; set; }

        public RftsEventsOneLevelViewModel(RftsLevel rftsLevel)
        {
            _rftsLevel = rftsLevel;

            ComparisonResult = $"State: ";
        }
    }
}
