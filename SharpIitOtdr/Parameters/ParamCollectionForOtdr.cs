using System.Collections.Generic;

namespace IitOtdrLibrary
{
    public class ParamCollectionForOtdr
    {
        public Dictionary<string, ParamCollectionForWaveLength> Units { get; set; } = new Dictionary<string, ParamCollectionForWaveLength>();
        public string SelectedUnit { get; set; }
    }
}