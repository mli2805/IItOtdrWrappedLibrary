using System.Threading.Tasks;
using Caliburn.Micro;
using IitOtdrLibrary;
using Action = System.Action;

namespace WpfExample
{
    public class ShellViewModel : Caliburn.Micro.PropertyChangedBase, IShell
    {
        public OtdrManager OtdrManager { get; set; }

        public string _message;

        public string Message
        {
            get { return _message; }
            set
            {
                if (Equals(value, _message)) return;
                _message = value;
                NotifyOfPropertyChange(() => Message);
            }
        }

        private bool _isOtdrInitialized;
        public bool IsOtdrInitialized
        {
            get { return _isOtdrInitialized; }
            set
            {
                if (Equals(value, _isOtdrInitialized)) return;
                _isOtdrInitialized = value;
                NotifyOfPropertyChange(()=>IsOtdrInitialized);
            }
        }
        public string IpAddress { get; set; }

        public ShellViewModel()
        {
            IpAddress = "192.168.96.52";
//            IpAddress = "172.16.4.10";
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
                    IsOtdrInitialized = true;
            }
        }

        public void LaunchOtdrParamView()
        {
            var vm = new OtdrParamViewModel(OtdrManager.IitOtdr);
            IWindowManager windowManager = new WindowManager();
            windowManager.ShowDialog(vm);
        }

        public async Task StartMeasurement()
        {
            using (new WaitCursor())
            {
                Message = "Wait, please...";
                await Task.Run(() => OtdrManager.Measure());
                Message = "Done.";
            }
        }
    }
}