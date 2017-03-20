using System.Collections.Generic;
using System.Linq;
using Caliburn.Micro;
using IitOtdrLibrary;

namespace WpfExample
{
    public class OtdrParamViewModel : Screen
    {
        private readonly ParamCollectionForOtdr _paramCollectionForOtdr;
        private ParamCollectionForWaveLength _paramCollectionForUnit;
        private ParamCollectionForDistance _paramCollectionForDistance;

        private string _selectedUnit;
        private double _backscatteredCoefficient;
        private double _refractiveIndex;
        private List<string> _distances;
        private string _selectedDistance;
        private List<string> _resolutions;
        private string _selectedResolution;
        private List<string> _pulseDurations;
        private string _selectedPulseDuration;
        private List<string> _measCountsToAverage;
        private string _selectedMeasCountToAverage;
        private List<string> _periodsToAverage;
        private string _selectedPeriodToAverage;

        public List<string> Units { get; set; }

        public string SelectedUnit
        {
            get { return _selectedUnit; }
            set
            {
                if (value == _selectedUnit) return;
                _selectedUnit = value;
                NotifyOfPropertyChange();
                InitializeForSelectedUnit();
            }
        }

        public double BackscatteredCoefficient
        {
            get { return _backscatteredCoefficient; }
            set
            {
                if (value.Equals(_backscatteredCoefficient)) return;
                _backscatteredCoefficient = value;
                NotifyOfPropertyChange();
            }
        }

        public double RefractiveIndex
        {
            get { return _refractiveIndex; }
            set
            {
                if (value.Equals(_refractiveIndex)) return;
                _refractiveIndex = value;
                NotifyOfPropertyChange();
            }
        }

        public List<string> Distances
        {
            get { return _distances; }
            set
            {
                if (Equals(value, _distances)) return;
                _distances = value;
                NotifyOfPropertyChange();
            }
        }

        public string SelectedDistance
        {
            get { return _selectedDistance; }
            set
            {
                if (value == _selectedDistance) return;
                _selectedDistance = value;
                NotifyOfPropertyChange();
            }
        }

        public List<string> Resolutions
        {
            get { return _resolutions; }
            set
            {
                if (Equals(value, _resolutions)) return;
                _resolutions = value;
                NotifyOfPropertyChange();
            }
        }

        public string SelectedResolution
        {
            get { return _selectedResolution; }
            set
            {
                if (value == _selectedResolution) return;
                _selectedResolution = value;
                NotifyOfPropertyChange();
            }
        }

        public List<string> PulseDurations
        {
            get { return _pulseDurations; }
            set
            {
                if (Equals(value, _pulseDurations)) return;
                _pulseDurations = value;
                NotifyOfPropertyChange();
            }
        }

        public string SelectedPulseDuration
        {
            get { return _selectedPulseDuration; }
            set
            {
                if (value == _selectedPulseDuration) return;
                _selectedPulseDuration = value;
                NotifyOfPropertyChange();
            }
        }

        public List<string> MeasCountsToAverage
        {
            get { return _measCountsToAverage; }
            set
            {
                if (Equals(value, _measCountsToAverage)) return;
                _measCountsToAverage = value;
                NotifyOfPropertyChange();
            }
        }

        public string SelectedMeasCountToAverage
        {
            get { return _selectedMeasCountToAverage; }
            set
            {
                if (value == _selectedMeasCountToAverage) return;
                _selectedMeasCountToAverage = value;
                NotifyOfPropertyChange();
            }
        }

        public List<string> PeriodsToAverage
        {
            get { return _periodsToAverage; }
            set
            {
                if (Equals(value, _periodsToAverage)) return;
                _periodsToAverage = value;
                NotifyOfPropertyChange();
            }
        }

        public string SelectedPeriodToAverage
        {
            get { return _selectedPeriodToAverage; }
            set
            {
                if (value == _selectedPeriodToAverage) return;
                _selectedPeriodToAverage = value;
                NotifyOfPropertyChange();
            }
        }


        public OtdrParamViewModel(IitOtdrWrapper otdrWrapper)
        {
            var paramGetter = new OtdrParamsGetter(otdrWrapper);
            _paramCollectionForOtdr = paramGetter.GetParamCollectionForOtdr();
            InitilizeInputControls();
        }

        private void InitilizeInputControls()
        {
            Units = _paramCollectionForOtdr.Units.Keys.ToList();
            SelectedUnit = Units.First();

            InitializeForSelectedUnit();
        }

        private void InitializeForSelectedUnit()
        {
            _paramCollectionForUnit = _paramCollectionForOtdr.Units[SelectedUnit];
            Distances = _paramCollectionForUnit.Distances.Keys.ToList();
            SelectedDistance = Distances.First();

            InitializeFromSelectedDistance();
        }

        private void InitializeFromSelectedDistance()
        {
            _paramCollectionForDistance = _paramCollectionForUnit.Distances[SelectedDistance];
            Resolutions = _paramCollectionForDistance.Resolutions.ToList();
            SelectedResolution = Resolutions.First();
            PulseDurations = _paramCollectionForDistance.PulseDurations.ToList();
            SelectedPulseDuration = PulseDurations.First();
            MeasCountsToAverage = _paramCollectionForDistance.AveragingNumber.ToList();
            SelectedMeasCountToAverage = MeasCountsToAverage.First();
            PeriodsToAverage = _paramCollectionForDistance.AveragingTime.ToList();
            SelectedPeriodToAverage = PeriodsToAverage.First();
            BackscatteredCoefficient = _paramCollectionForDistance.Bc;
            RefractiveIndex = _paramCollectionForDistance.Ob;
        }

        protected override void OnViewLoaded(object view)
        {
            DisplayName = "Measurement parameters";
        }

        public void SetParams()
        {
            _paramCollectionForOtdr.SelectedUnit = SelectedUnit;
            _paramCollectionForOtdr.Units[SelectedUnit].SelectedDistance = SelectedDistance;
            _paramCollectionForOtdr.Units[SelectedUnit].Distances[SelectedDistance].SelectedResolution = SelectedResolution;
            _paramCollectionForOtdr.Units[SelectedUnit].Distances[SelectedDistance].SelectedPulseDuration = SelectedPulseDuration;
            _paramCollectionForOtdr.Units[SelectedUnit].Distances[SelectedDistance].SelectedAveragingTime = SelectedPeriodToAverage;
            _paramCollectionForOtdr.Units[SelectedUnit].Distances[SelectedDistance].SelectedAveragingNumber = SelectedMeasCountToAverage;
            _paramCollectionForOtdr.Units[SelectedUnit].Distances[SelectedDistance].Bc = BackscatteredCoefficient;
            _paramCollectionForOtdr.Units[SelectedUnit].Distances[SelectedDistance].Ob = RefractiveIndex;

            TryClose();
        }

        public void Cancel()
        {
            TryClose();
        }


    }
}
