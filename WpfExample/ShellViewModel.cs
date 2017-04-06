using System;
using System.Threading.Tasks;
using Caliburn.Micro;
using IitOtdrLibrary;
using DirectCharonLibrary;

namespace WpfExample
{
    public class ShellViewModel : Caliburn.Micro.PropertyChangedBase, IShell
    {
        public OtdrManager OtdrManager { get; set; }
        public Charon MainCharon { get; set; }

        public bool ShouldForceLmax { get; set; } = true;

        public string BaseFileName { get; set; } = @"c:\temp\base.sor";

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

        public string CharonInfo => $"charon {MainCharon?.Serial} has {MainCharon?.OwnPortCount}/{MainCharon?.FullPortCount} ports";

        private bool _isOtdrInitialized;
        public bool IsOtdrInitialized
        {
            get { return _isOtdrInitialized; }
            set
            {
                if (Equals(value, _isOtdrInitialized)) return;
                _isOtdrInitialized = value;
                NotifyOfPropertyChange(() => IsOtdrInitialized);
            }
        }

        private bool _isOtauInitialized;
        public bool IsOtauInitialized
        {
            get { return _isOtauInitialized; }
            set
            {
                if (Equals(value, _isOtauInitialized)) return;
                _isOtauInitialized = value;
                NotifyOfPropertyChange(() => IsOtauInitialized);
            }
        }

        private bool _isMeasurementInProgress;
        public bool IsMeasurementInProgress
        {
            get { return _isMeasurementInProgress; }
            set
            {
                if (Equals(value, _isMeasurementInProgress)) return;
                _isMeasurementInProgress = value;
                NotifyOfPropertyChange(() => IsMeasurementInProgress);
            }
        }


        public string IpAddress { get; set; }
        public int OtauPort { get; set; }
        public int ActivePort { get; set; }


        public ShellViewModel()
        {
            IpAddress = "192.168.96.52";
            //            IpAddress = "172.16.4.10";
            //IpAddress = "192.168.88.101";
            OtauPort = 23;
        }

        public async Task InitOtdr()
        {
            Message = "Wait, please...";

            OtdrManager = new OtdrManager();
            var initializationResult = OtdrManager.LoadDll();
            if (initializationResult != "")
            {
                Message = initializationResult;
                return;
            }

            await RunInitializationProcess();

            Message = IsOtdrInitialized ? "OTDR initialized successfully!" : "OTDR initialization failed!";
        }

        private async Task RunInitializationProcess()
        {
            using (new WaitCursor())
            {
                await Task.Run(() => OtdrManager.InitializeLibrary(IpAddress));
                if (OtdrManager.IsInitializedSuccessfully)
                    IsOtdrInitialized = true;
            }
        }

        public async Task InitOtau()
        {
            Message = "Wait, please...";
            MainCharon = new Charon(new NetAddress() { IpAddress = IpAddress, TcpPort = OtauPort });
            await RunOtauInitialization();

            Message = MainCharon.IsLastCommandSuccessful ? "OTAU initialized successfully!" : "OTAU initialization failed!";
        }

        public async Task RunOtauInitialization()
        {
            using (new WaitCursor())
            {
                await Task.Run(() => MainCharon.Initialize());
                if (MainCharon.IsLastCommandSuccessful)
                {
                    IsOtauInitialized = true;
                    NotifyOfPropertyChange(() => CharonInfo);
                }
            }
        }

        public void SetActivePort()
        {
            var newActivePort = MainCharon.SetExtendedActivePort(ActivePort);
            Message = newActivePort != -1 ? $"Otau toggled to port {newActivePort}" : "Failed to toggle otau!";
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
                IsMeasurementInProgress = true;
                Message = "Wait, please...";

                await Task.Run(() => OtdrManager.DoManualMeasurement(ShouldForceLmax));

                IsMeasurementInProgress = false;
                Message = "Measurement is finished.";

                OtdrManager.GetLastSorData();
            }
        }

        public async Task StartMeasurementWithBase()
        {
            using (new WaitCursor())
            {
                IsMeasurementInProgress = true;
                Message = "Wait, please...";

                await Task.Run(() => OtdrManager.MeasureWithBase(BaseFileName));

                IsMeasurementInProgress = false;
                Message = "Measurement is finished.";

                OtdrManager.GetLastSorData();
            }
        }

        public void InterruptMeasurement()
        {
            OtdrManager.InterruptMeasurement();
            Message = "Stop command is sent";
        }
    }
}