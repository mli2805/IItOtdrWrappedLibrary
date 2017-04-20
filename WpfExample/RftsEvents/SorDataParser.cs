using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Optixsoft.SorExaminer.OtdrDataFormat;
using Optixsoft.SorExaminer.OtdrDataFormat.Structures;

namespace WpfExample
{
    public class SorDataParser
    {
        private readonly OtdrDataKnownBlocks _sorData;
        private int _eventCount;

        public SorDataParser(OtdrDataKnownBlocks sorData)
        {
            _sorData = sorData;
        }

        private Dictionary<int, string> LineNameList => new Dictionary<int, string>
        {
            { 100, "       Common Information"       },
            { 101, "Landmark Name"                   },
            { 102, "Landmark Type"                   },
            { 103, "State"                           },
            { 104, "Damage Type"                     },
            { 105, "Distance, km"                    },
            { 106, "Enabled"                         },
            { 107, "Event Type"                      },
            { 200, "       Current Measurement"      },
            { 201, "Reflectance coefficient, dB"     },
            { 202, "Attenuation in Closure, dB"      },
            { 203, "Attenuation coefficient, dB/km " },
            { 300, "       Monitoring Thresholds"    },
            { 301, "Reflectance coefficient, dB"     },
            { 302, "Attenuation in Closure, dB"      },
            { 303, "Attenuation coefficient, dB/km " },
            { 400, "       Deviations from Base"     },
            { 401, "Reflectance coefficient, dB"     },
            { 402, "Attenuation in Closure, dB"      },
            { 403, "Attenuation coefficient, dB/km " },
            { 900, ""                                },
        };


        private Dictionary<int, string[]> PrepareEmptyDictionary()
        {
            var eventTable = new Dictionary<int, string[]>();
            foreach (var pair in LineNameList)
            {
                var cells = new string[_eventCount + 1];
                cells[0] = pair.Value;
                eventTable.Add(pair.Key, cells);
            }
            return eventTable;
        }

        public Dictionary<int, string[]> Parse(RftsLevelType rftsLevel)
        {
            _eventCount = _sorData.LinkParameters.LandmarksCount;
            var eventTable = PrepareEmptyDictionary();

            ParseCommonInformation(eventTable);
            ParseCurrentMeasurement(eventTable);
            ParseMonitoringThresholds(eventTable, rftsLevel);
            ParseDeviationFromBase(eventTable);
            return eventTable;
        }

        private void ParseCommonInformation(Dictionary<int, string[]> eventTable)
        {
            for (int i = 0; i < _eventCount; i++)
            {
                eventTable[101][i + 1] = _sorData.LinkParameters.LandmarkBlocks[i].Comment;
                eventTable[102][i + 1] = _sorData.LinkParameters.LandmarkBlocks[i].Code.ForTable();
                eventTable[106][i + 1] = _sorData.RftsEvents.Events[i].EventTypes.ForTable();
            }
        }
        private void ParseCurrentMeasurement(Dictionary<int, string[]> eventTable)
        {
            for (int i = 0; i < _eventCount; i++)
            {
                eventTable[201][i + 1] = _sorData.KeyEvents.KeyEvents[i].EventReflectance.ToString(CultureInfo.CurrentCulture);
                eventTable[202][i + 1] = _sorData.KeyEvents.KeyEvents[i].EventLoss.ToString(CultureInfo.CurrentCulture);
            }
        }

        private void ParseMonitoringThresholds(Dictionary<int, string[]> eventTable, RftsLevelType rftsLevel)
        {
            var level = _sorData.RftsParameters.Levels.First(l => l.LevelName == rftsLevel);

            for (int i = 0; i < _eventCount; i++)
            {
                eventTable[301][i + 1] = level.ThresholdSets[i].ReflectanceThreshold.ForTable();
                eventTable[302][i + 1] = level.ThresholdSets[i].AttenuationThreshold.ForTable();
                eventTable[303][i + 1] = level.ThresholdSets[i].AttenuationCoefThreshold.ForTable();
            }
        }

        private void ParseDeviationFromBase(Dictionary<int, string[]> eventTable)
        {
        }


    }
}