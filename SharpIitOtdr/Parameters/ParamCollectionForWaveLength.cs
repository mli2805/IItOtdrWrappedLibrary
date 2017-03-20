using System.Collections.Generic;

namespace IitOtdrLibrary
{
    public class ParamCollectionForWaveLength
    {
        public Dictionary<string, ParamCollectionForDistance> Distances { get; set; } = new Dictionary<string, ParamCollectionForDistance>();
        public string SelectedDistance { get; set; }
    }
}