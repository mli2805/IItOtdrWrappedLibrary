using System;
using System.Data;
using Optixsoft.SorExaminer.OtdrDataFormat;
using Optixsoft.SorExaminer.OtdrDataFormat.Structures;

namespace WpfExample
{
    public class RftsEventsOneLevelViewModel
    {
        private readonly OtdrDataKnownBlocks _sorData;
        private readonly RftsLevel _rftsLevel;
        public string ComparisonResult { get; set; }

        public DataTable BindableTable { get; set; }

        public RftsEventsOneLevelEeltViewModel EeltViewModel { get; set; }

        public RftsEventsOneLevelViewModel(OtdrDataKnownBlocks sorData, RftsLevel rftsLevel)
        {
            _sorData = sorData;
            _rftsLevel = rftsLevel;

            CreateTable();
            PopulateTable();
            EeltViewModel = new RftsEventsOneLevelEeltViewModel(_rftsLevel, _sorData.KeyEvents.EndToEndLoss);

            ComparisonResult = $"State: ";
        }


        private void PopulateTable()
        {
            DataRow newRow = BindableTable.NewRow();
            newRow["Parameters"] = "Landmark Name";
            BindableTable.Rows.Add(newRow);

            newRow = BindableTable.NewRow();
            newRow["Parameters"] = "Landmark Type";
            BindableTable.Rows.Add(newRow);
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
