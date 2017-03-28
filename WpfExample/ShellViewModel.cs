using System.Threading.Tasks;
using Caliburn.Micro;
using IitOtdrLibrary;

namespace WpfExample
{
    public class ShellViewModel : Caliburn.Micro.PropertyChangedBase, IShell
    {
        public OtdrManager OtdrManager { get; set; }
        private bool _isParamButtonEnabled;
        public bool IsParamButtonEnabled
        {
            get { return _isParamButtonEnabled; }
            set
            {
                if (Equals(value, _isParamButtonEnabled)) return;
                _isParamButtonEnabled = value;
                NotifyOfPropertyChange(()=>IsParamButtonEnabled);
            }
        }
        public string IpAddress { get; set; }

        public ShellViewModel()
        {
//            IpAddress = "192.168.96.52";
            IpAddress = "172.16.4.10";
            //IpAddress = "192.168.88.101";
        }

        public async Task Init()
        {
            OtdrManager = new OtdrManager();
            if (!OtdrManager.LoadDll())
                return;

            await RunInitializationProcess();
        }

        private async Task RunInitializationProcess()
        {
            using (new WaitCursor())
            {
                await Task.Run(()=> OtdrManager.InitializeLibrary(IpAddress));
                if (OtdrManager.IsInitializedSuccessfully)
                    IsParamButtonEnabled = true;
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