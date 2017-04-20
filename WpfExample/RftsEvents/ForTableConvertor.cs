using Optixsoft.SorExaminer.OtdrDataFormat;

namespace WpfExample
{
    public static class ForTableConvertor
    {
        public static string ForTable(this RftsEventTypes rftsEventType)
        {
            switch (rftsEventType)
            {
                case RftsEventTypes.None: return "no";
                case RftsEventTypes.IsMonitored: return "yes";
            }
            return "unexpected input";
        }

        public static string ForTable(this LandmarkCode landmarkCode)
        {
            switch (landmarkCode)
            {
                case LandmarkCode.FiberDistributingFrame: return "RTU";
                case LandmarkCode.Coupler: return "Sleeve";
                case LandmarkCode.RemoteTerminal: return "Terminal";
            }
            return "unexpected input";
        }

    }
}