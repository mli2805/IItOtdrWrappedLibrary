using System.Collections.Generic;

namespace IitOtdrLibrary
{
    public class ParamCollectionForDistance
    {
        public Dictionary<string, ParamCollectionForResolution> Resolutions { get; set; } = new Dictionary<string, ParamCollectionForResolution>();
        public string SelectedResolution { get; set; }
    }
}