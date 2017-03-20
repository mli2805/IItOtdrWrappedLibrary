using System.Windows.Input;
using Caliburn.Micro;
using IitOtdrLibrary;

namespace WpfExample
{
    public class ShellViewModel : Caliburn.Micro.PropertyChangedBase, IShell
    {
        public OtdrLogic OtdrLogic { get; set; }
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
            using (new WaitCursor())
            {
                OtdrLogic = new OtdrLogic(IpAddress);
                if (OtdrLogic.IsInitializedSuccessfully)
                    IsOtdrInited = true;
            }
        }

        public void LaunchOtdrParamView()
        {
            var vm = new OtdrParamViewModel(OtdrLogic.IitOtdr);
            IWindowManager windowManager = new WindowManager();
            windowManager.ShowDialog(vm);
        }
    }
}