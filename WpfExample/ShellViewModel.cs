using System;
using System.IO;
using System.Threading.Tasks;
using Caliburn.Micro;
using IitOtdrLibrary;
using DirectCharonLibrary;
using Microsoft.Win32;

namespace WpfExample
{
    public class ShellViewModel : Caliburn.Micro.PropertyChangedBase, IShell
    {
        public OtdrManager OtdrManager { get; set; }
        public Charon MainCharon { get; set; }

        public bool ShouldForceLmax { get; set; } = true;

        public string _baseFileName;
        public string BaseFileName
        {
            get { return _baseFileName; }
            set
            {
                if (Equals(value, _baseFileName)) return;
                _baseFileName = value;
                NotifyOfPropertyChange(() => BaseFileName);
            }
        }

        public string CurrentFileName { get; set; } = @"c:\temp\123.sor";

        public string _initializationMessage;
        public string InitializationMessage
        {
            get { return _initializationMessage; }
            set
            {
                if (Equals(value, _initializationMessage)) return;
                _initializationMessage = value;
                NotifyOfPropertyChange(() => InitializationMessage);
            }
        }

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

        private int _activePort;
        public int ActivePort
        {
            get { return _activePort; }
            set
            {
                if (Equals(value, _activePort)) return;
                _activePort = value;
                NotifyOfPropertyChange(() => ActivePort);
            }
        }


        public ShellViewModel()
        {
            IpAddress = "192.168.96.52";
            //            IpAddress = "172.16.4.10";
            //IpAddress = "192.168.88.101";
            OtauPort = 23;

            BaseFileName = @"c:\temp\base.sor";
        }

        public async Task InitOtdr()
        {
            InitializationMessage = "Wait, please...";

            OtdrManager = new OtdrManager();
            var initializationResult = OtdrManager.LoadDll();
            if (initializationResult != "")
            {
                InitializationMessage = initializationResult;
                return;
            }

            await RunInitializationProcess();

            InitializationMessage = IsOtdrInitialized ? "OTDR initialized successfully!" : "OTDR initialization failed!";
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
            InitializationMessage = "Wait, please...";
            MainCharon = new Charon(new NetAddress() { IpAddress = IpAddress, TcpPort = OtauPort });
            await RunOtauInitialization();
            InitializationMessage = MainCharon.IsLastCommandSuccessful ? "OTAU initialized successfully!" : MainCharon.LastErrorMessage;

            if (!MainCharon.IsLastCommandSuccessful)
                return;
            ActivePort = MainCharon.GetExtendedActivePort();
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
            InitializationMessage = newActivePort != -1 ? $"Otau toggled to port {newActivePort}" : MainCharon.LastErrorMessage;
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

                var sorData = OtdrManager.GetLastSorData();
                sorData.Save(CurrentFileName);
            }
        }

        public void ChooseBaseFilename()
        {
            var fd = new OpenFileDialog();
            fd.Filter = "Sor files (*.sor)|*.sor";
            fd.InitialDirectory = @"c:\temp\";
            if (fd.ShowDialog() == true)
                BaseFileName = fd.FileName;
        }

        public async Task StartMeasurementWithBase()
        {
            using (new WaitCursor())
            {
                IsMeasurementInProgress = true;
                Message = "Wait, please...";

                byte[] buffer = File.ReadAllBytes(BaseFileName);
                await Task.Run(() => OtdrManager.MeasureWithBase(buffer));

                IsMeasurementInProgress = false;
                Message = "Measurement is finished.";

                var sorData = OtdrManager.GetLastSorData();
                sorData.Save(CurrentFileName);
            }
        }

        public void CompareMeasurementWithBase()
        {
            var bufferBase = File.ReadAllBytes(@"c:\temp\base.sor");
            var bufferMeas = File.ReadAllBytes(@"c:\temp\123.sor");

            var moniResult = OtdrManager.CompareMeasureWithBase(bufferBase, bufferMeas, true);
            Console.WriteLine($"Comparison end. IsFiberBreak = {moniResult.IsFiberBreak}, IsNoFiber = {moniResult.IsNoFiber}");
        }

        private bool _isMonitoringCycleCanceled;
        private object _cycleLockOb = new object();
        public async Task StartCycle()
        {
            lock (_cycleLockOb)
            {
                _isMonitoringCycleCanceled = false;
            }

            int c = 0;
            while (true)
            {
                lock (_cycleLockOb)
                {
                    if (_isMonitoringCycleCanceled)
                    {
                        OtdrManager.InterruptMeasurement();
                        break;
                    }
                }
                using (new WaitCursor())
                {
                    IsMeasurementInProgress = true;
                    Message = $"Monitoring cycle {c}. Wait, please...";

                    byte[] buffer = File.ReadAllBytes(BaseFileName);
                    await Task.Run(() => OtdrManager.MeasureWithBase(buffer));

                    IsMeasurementInProgress = false;
                    Message = $"{c}th measurement is finished.";

                    var sorData = OtdrManager.GetLastSorData();
                    sorData.Save(CurrentFileName);
                    CompareMeasurementWithBase();
                }
                c++;
            }

        }
        public void StopCycle()
        {
            lock (_cycleLockOb)
            {
                _isMonitoringCycleCanceled = true;
                InterruptMeasurement();
            }
        }


        public void InterruptMeasurement()
        {
            OtdrManager.InterruptMeasurement();
            Message = "Stop command is sent";
        }
    }
}