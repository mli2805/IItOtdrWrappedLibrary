using Optixsoft.SorExaminer.OtdrDataFormat;
using Optixsoft.SorExaminer.OtdrDataFormat.Structures;

namespace WpfExample
{
    public class RftsEventsOneLevelViewModel
    {
        private readonly OtdrDataKnownBlocks _sorData;
        private readonly RftsLevel _rftsLevel;
        public string ComparisonResult { get; set; }

        public RftsEventsOneLevelEeltViewModel EeltViewModel { get; set; }

        public RftsEventsOneLevelViewModel(OtdrDataKnownBlocks sorData, RftsLevel rftsLevel)
        {
            _sorData = sorData;
            _rftsLevel = rftsLevel;

            EeltViewModel = new RftsEventsOneLevelEeltViewModel(_rftsLevel, _sorData.KeyEvents.EndToEndLoss);

            ComparisonResult = $"State: ";
        }
    }
}
