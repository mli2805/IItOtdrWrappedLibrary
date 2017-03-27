using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace IitOtdrLibrary
{
    public class OtdrParamsGetter
    {
        private readonly IitOtdrWrapper _iitOtdr;
        public OtdrParamsGetter(IitOtdrWrapper iitOtdrWrapper)
        {
            _iitOtdr = iitOtdrWrapper;
        }

        public ParamCollectionForOtdr GetParamCollectionForOtdr()
        {
            var result = new ParamCollectionForOtdr();
            var units = GetManyVariantsForParam((int)ServiceCmdParam.Unit); // WaveLength (Unit)
            for (int i = 0; i < units.Length; i++)
            {
                result.Units.Add(units[i], GetParamCollectionForWaveLength(i));
            }
            return result;
        }

        private ParamCollectionForWaveLength GetParamCollectionForWaveLength(int i)
        {
            _iitOtdr.SetParam((int)ServiceCmdParam.Unit, i);
            var paramCollectionForWaveLength = new ParamCollectionForWaveLength();

            var distanceRanges = GetManyVariantsForParam((int)ServiceCmdParam.Lmax); // LMax
            for (int j = 0; j < distanceRanges.Length; j++)
            {
                paramCollectionForWaveLength.Distances.Add(distanceRanges[j], GetParamCollectionForDistance(j));
            }
            return paramCollectionForWaveLength;
        }

        private ParamCollectionForDistance GetParamCollectionForDistance(int j)
        {
            _iitOtdr.SetParam((int)ServiceCmdParam.Lmax, j);
            var paramCollectionForDistance = new ParamCollectionForDistance();

            var resolutions = GetManyVariantsForParam((int) ServiceCmdParam.Res); // dL
            for (int k = 0; k < resolutions.Length; k++)
            {
                paramCollectionForDistance.Resolutions.Add(resolutions[k], GetParamCollectionForResolution(k));
            }
            return paramCollectionForDistance;
        }

        private ParamCollectionForResolution GetParamCollectionForResolution(int k)
        {
            _iitOtdr.SetParam((int)ServiceCmdParam.Res, k);
            var paramCollectionForResolution = new ParamCollectionForResolution()
            {
                PulseDurations = GetManyVariantsForParam((int) ServiceCmdParam.Pulse), // Tp
                Bc = double.Parse(GetManyVariantsForParam((int) ServiceCmdParam.Bc)[0], new CultureInfo("en-US")), // 
                Ob = double.Parse(GetManyVariantsForParam((int) ServiceCmdParam.Ri)[0], new CultureInfo("en-US")), //
            };

            _iitOtdr.SetParam((int)ServiceCmdParam.IsTime, 1);
            var timeVariants = GetManyVariantsForParam((int)ServiceCmdParam.Time);
            paramCollectionForResolution.TimeToAverage = new Dictionary<string, string>();   // Длительность измерения
            for (int l = 0; l < timeVariants.Length; l++)
            {
                _iitOtdr.SetParam((int)ServiceCmdParam.Time, l);
                var count = GetManyVariantsForParam((int)ServiceCmdParam.Navr);  // Число усреднений
                paramCollectionForResolution.TimeToAverage.Add(timeVariants[l], count[0]);
            }

            _iitOtdr.SetParam((int)ServiceCmdParam.IsTime, 0);
            var countVariants = GetManyVariantsForParam((int)ServiceCmdParam.Navr);
            paramCollectionForResolution.MeasurementCountToAverage = new Dictionary<string, string>();  // Число усреднений
            for (int m = 0; m < countVariants.Length; m++)
            {
                _iitOtdr.SetParam((int)ServiceCmdParam.Navr, m);
                var time = GetManyVariantsForParam((int) ServiceCmdParam.Time); // Длительность измерения
                paramCollectionForResolution.MeasurementCountToAverage.Add(countVariants[m], time[0]);
            }

            return paramCollectionForResolution;
        }

        private string[] GetManyVariantsForParam(int paramCode)
        {
            string value = _iitOtdr.GetLineOfVariantsForParam(paramCode);
            if (value == null)
                return null;

            // если вариант только 1 он возвращается без первого слэша
            if (value[0] != '/')
                return new[] { value };

            var strs = value.Split('/');
            return strs.Skip(1).ToArray();
        }

    }
}