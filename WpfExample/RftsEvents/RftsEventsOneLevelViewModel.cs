using System.Collections.Generic;
using System.Data;
using Optixsoft.SorExaminer.OtdrDataFormat;
using Optixsoft.SorExaminer.OtdrDataFormat.Structures;

namespace WpfExample
{
    public class RftsEventsOneLevelViewModel
    {
        private readonly OtdrDataKnownBlocks _sorData;
        public string ComparisonResult { get; set; }

        public DataTable BindableTable { get; set; }

        public RftsEventsOneLevelEeltViewModel EeltViewModel { get; set; }

        public RftsEventsOneLevelViewModel(OtdrDataKnownBlocks sorData, RftsLevel rftsLevel)
        {
            _sorData = sorData;

            CreateTable();
            PopulateTable();
            EeltViewModel = new RftsEventsOneLevelEeltViewModel(rftsLevel, _sorData.KeyEvents.EndToEndLoss);
        }

        private List<string> LineList => new List<string>()
        {
            "       Common Information",
            "Landmark Name",
            "Landmark Type",
            "State",
            "Damage Type",
            "Distance, km",
            "Enabled",
            "Event Type",
            "       Current Measurement", 
            "Reflectance coefficient, dB",
            "Attenuation in Closure, dB",
            "Attenuation coefficient, dB/km",
            "       Monitoring Thresholds", 
            "Reflectance coefficient, dB",
            "Attenuation in Closure, dB",
            "Attenuation coefficient, dB/km",
            "       Deviations from Base", 
            "Reflectance coefficient, dB",
            "Attenuation in Closure, dB",
            "Attenuation coefficient, dB/km",
            "",
        };


        private void PopulateTable()
        {
            foreach (var line in LineList)
            {
                DataRow newRow = BindableTable.NewRow();
                newRow["Parameters"] = line;
                BindableTable.Rows.Add(newRow);
            }
        }

        private void CreateTable()
        {
            BindableTable = new DataTable();

            DataColumn parametersColumn = new DataColumn() { ColumnName = "Parameters" };
            BindableTable.Columns.Add(parametersColumn);

            for (int i = 0; i < _sorData.RftsEvents.EventsCount; i++)
            {
                BindableTable.Columns.Add(new DataColumn($"Event N{i}") { DataType = typeof(string) });
            }
        }

    }
}
