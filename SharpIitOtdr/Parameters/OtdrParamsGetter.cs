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
            var paramSetForWaveLength = new ParamCollectionForWaveLength();

            var distanceRanges = GetManyVariantsForParam((int)ServiceCmdParam.Lmax); // LMax
            for (int j = 0; j < distanceRanges.Length; j++)
            {
                paramSetForWaveLength.Distances.Add(distanceRanges[j], GetParamCollectionForDistance(j));
            }
            return paramSetForWaveLength;
        }

        private ParamCollectionForDistance GetParamCollectionForDistance(int j)
        {
            _iitOtdr.SetParam((int)ServiceCmdParam.Lmax, j);
            return new ParamCollectionForDistance
            {
                Resolutions = GetManyVariantsForParam((int)ServiceCmdParam.Res),      // dL
                PulseDurations = GetManyVariantsForParam((int)ServiceCmdParam.Pulse),  // Tp
                AveragingTime = GetManyVariantsForParam((int)ServiceCmdParam.Time),   // Длительность измерения
                AveragingNumber = GetManyVariantsForParam((int)ServiceCmdParam.Navr),  // Число усреднений
                Bc = double.Parse(GetManyVariantsForParam((int)ServiceCmdParam.Bc)[0], new CultureInfo("en-US")), // 
                Ob = double.Parse(GetManyVariantsForParam((int)ServiceCmdParam.Ri)[0], new CultureInfo("en-US")), //
            };
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