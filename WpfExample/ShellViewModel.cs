using Caliburn.Micro;
using IitOtdrLibrary;

namespace WpfExample
{
    public class ShellViewModel : Caliburn.Micro.PropertyChangedBase, IShell
    {
        public OtdrManager OtdrManager { get; set; }
        private bool _isOtdrInited;
        public bool IsOtdrInited
        {
            get
            {
                return _isOtdrInited;
            }
            set
            {
                if (Equals(value, _isOtdrInited)) return;
                _isOtdrInited = value;
                NotifyOfPropertyChange(()=>IsOtdrInited);
            }
        }
        public string IpAddress { get; set; }

        public ShellViewModel()
        {
            IpAddress = "192.168.96.52";
//            IpAddress = "172.16.4.10";
            //IpAddress = "192.168.88.101";
        }

        public void Init()
        {
            OtdrManager = new OtdrManager();
            if (!OtdrManager.LoadDll())
                return;
            using (new WaitCursor())
            {
                OtdrManager.InitializeLibrary(IpAddress);
                if (OtdrManager.IsInitializedSuccessfully)
                    IsOtdrInited = true;
            }
        }

        public void LaunchOtdrParamView()
        {
            var vm = new OtdrParamViewModel(OtdrManager.IitOtdr);
            IWindowManager windowManager = new WindowManager();
            windowManager.ShowDialog(vm);
        }
    }
}