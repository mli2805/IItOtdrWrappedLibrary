using System;
using System.IO;
using System.Threading.Tasks;
using Caliburn.Micro;
using IitOtdrLibrary;
using DirectCharonLibrary;
using Iit.Fibertest.Utils;
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

        public string _measFileName;
        public string MeasFileName
        {
            get { return _measFileName; }
            set
            {
                if (Equals(value, _measFileName)) return;
                _measFileName = value;
                NotifyOfPropertyChange(() => MeasFileName);
            }
        }

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

        private bool _isLibraryInitialized;
        public bool IsLibraryInitialized
        {
            get { return _isLibraryInitialized; }
            set
            {
                if (Equals(value, _isLibraryInitialized)) return;
                _isLibraryInitialized = value;
                NotifyOfPropertyChange(() => IsLibraryInitialized);
            }
        }

        private bool _isOtdrConnected;
        public bool IsOtdrConnected
        {
            get { return _isOtdrConnected; }
            set
            {
                if (Equals(value, _isOtdrConnected)) return;
                _isOtdrConnected = value;
                NotifyOfPropertyChange(() => IsOtdrConnected);
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

        private readonly Logger _rtuLogger;

        public ShellViewModel()
        {
            _rtuLogger = new Logger("rtu.log");

            IpAddress = "192.168.96.52";
            //            IpAddress = "172.16.4.10";
            //IpAddress = "192.168.88.101";
            OtauPort = 23;

            BaseFileName = @"c:\temp\base.sor";
            MeasFileName = @"c:\temp\123.sor";


            OtdrManager = new OtdrManager(@"..\IitOtdr\", _rtuLogger);
            var initializationResult = OtdrManager.LoadDll();
            if (initializationResult != "")
                InitializationMessage = initializationResult;
            IsLibraryInitialized = OtdrManager.InitializeLibrary();
        }

        public async Task ConnectOtdr()
        {
            InitializationMessage = "Wait, please...";

            await ConnectionProcess();

            InitializationMessage = IsOtdrConnected ? "OTDR connected successfully!" : "OTDR connection failed!";
        }

        private async Task ConnectionProcess()
        {
            using (new WaitCursor())
            {
                await Task.Run(() => OtdrManager.ConnectOtdr(IpAddress));
                IsOtdrConnected = OtdrManager.IsOtdrConnected;
            }
        }

        public async Task InitOtau()
        {
            InitializationMessage = "Wait, please...";
            MainCharon = new Charon(new NetAddress() { IpAddress = IpAddress, TcpPort = OtauPort }, _rtuLogger);
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
                _rtuLogger.AppendLine("Otau initialization started");
                await Task.Run(() => MainCharon.Initialize());
                if (MainCharon.IsLastCommandSuccessful)
                {
                    _rtuLogger.AppendLine($"Otau initialized successfully");
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

                var lastSorDataBuffer = OtdrManager.GetLastSorDataBuffer();
                if (lastSorDataBuffer == null)
                    return;
                var sorData = OtdrManager.ApplyFilter(OtdrManager.ApplyAutoAnalysis(lastSorDataBuffer), false);
                sorData.Save(MeasFileName);
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

        public void ChooseMeasFilename()
        {
            var fd = new SaveFileDialog();
            fd.Filter = "Sor files (*.sor)|*.sor";
            fd.InitialDirectory = @"c:\temp\";
            if (fd.ShowDialog() == true)
                MeasFileName = fd.FileName;
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

                var lastSorDataBuffer = OtdrManager.GetLastSorDataBuffer();
                if (lastSorDataBuffer == null)
                    return;
                var sorData = OtdrManager.ApplyFilter(OtdrManager.ApplyAutoAnalysis(lastSorDataBuffer), OtdrManager.IsFilterOnInBase(buffer));
                sorData.Save(MeasFileName);
            }
        }

        // button
        public void CompareMeasurementWithBase()
        {
            var bufferBase = File.ReadAllBytes(BaseFileName);
            var bufferMeas = File.ReadAllBytes(MeasFileName);

            var moniResult = OtdrManager.CompareMeasureWithBase(bufferBase, bufferMeas, true);
            _rtuLogger.AppendLine($"Comparison end. IsFiberBreak = {moniResult.IsFiberBreak}, IsNoFiber = {moniResult.IsNoFiber}");
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
            byte[] baseBytes = File.ReadAllBytes(BaseFileName);
            var isFilterOn = OtdrManager.IsFilterOnInBase(baseBytes);

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
                    _rtuLogger.AppendLine($"Monitoring cycle {c}.");

                    await Task.Run(() => OtdrManager.MeasureWithBase(baseBytes));

                    IsMeasurementInProgress = false;
                    Message = $"{c}th measurement is finished.";

                    var moniResult = OtdrManager.CompareMeasureWithBase(baseBytes,
                        OtdrManager.ApplyAutoAnalysis(OtdrManager.GetLastSorDataBuffer()), true); // is ApplyAutoAnalysis necessary ?
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