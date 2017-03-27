using System.Collections.Generic;
using System.Globalization;
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
        private ParamCollectionForResolution _paramCollectionForResolution;

        #region Comboboxes
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
        private string _timeCorrespondingToCount;
        private string _countCorrespondingToTime;

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
                InitializeFromSelectedDistance();
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
                InitializeFromSelectedResolution();
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

                _otdrWrapper.SetParam((int)ServiceCmdParam.Navr, MeasCountsToAverage.IndexOf(SelectedMeasCountToAverage));
                TimeCorrespondingToCount = _otdrWrapper.GetManyVariantsForParam((int) ServiceCmdParam.Time)[0];
            }
        }

        public string TimeCorrespondingToCount
        {
            get { return _timeCorrespondingToCount; }
            set
            {
                if (value == _timeCorrespondingToCount) return;
                _timeCorrespondingToCount = value;
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

                _otdrWrapper.SetParam((int)ServiceCmdParam.Time, PeriodsToAverage.IndexOf(SelectedPeriodToAverage));
                CountCorrespondingToTime = _otdrWrapper.GetManyVariantsForParam((int)ServiceCmdParam.Navr)[0];
            }
        }

        public string CountCorrespondingToTime
        {
            get { return _countCorrespondingToTime; }
            set
            {
                if (value == _countCorrespondingToTime) return;
                _countCorrespondingToTime = value;
                NotifyOfPropertyChange();
            }
        }

        public bool IsTimeToAverageSelected
        {
            get { return _isTimeToAverageSelected; }
            set
            {
                if (value == _isTimeToAverageSelected) return;
                _isTimeToAverageSelected = value;
                NotifyOfPropertyChange();

                _otdrWrapper.SetParam((int)ServiceCmdParam.IsTime, IsTimeToAverageSelected ? 1 : 0);
            }
        }

        #endregion

        private IitOtdrWrapper _otdrWrapper;
        private bool _isTimeToAverageSelected;

        public OtdrParamViewModel(IitOtdrWrapper otdrWrapper)
        {
            _otdrWrapper = otdrWrapper;

            IsTimeToAverageSelected = true;
            Units = _otdrWrapper.GetManyVariantsForParam((int)ServiceCmdParam.Unit).ToList();
            SelectedUnit = Units.First();
        }

        private void InitializeForSelectedUnit()
        {
            _otdrWrapper.SetParam((int)ServiceCmdParam.Unit, Units.IndexOf(SelectedUnit));

            _backscatteredCoefficient = double.Parse(_otdrWrapper.GetManyVariantsForParam((int)ServiceCmdParam.Bc)[0], new CultureInfo("en-US"));
            _refractiveIndex = double.Parse(_otdrWrapper.GetManyVariantsForParam((int)ServiceCmdParam.Ri)[0], new CultureInfo("en-US"));
            Distances = _otdrWrapper.GetManyVariantsForParam((int)ServiceCmdParam.Lmax).ToList();
            SelectedDistance = Distances.First();
        }

        private void InitializeFromSelectedDistance()
        {
            _otdrWrapper.SetParam((int)ServiceCmdParam.Lmax, Distances.IndexOf(SelectedDistance));

            Resolutions = _otdrWrapper.GetManyVariantsForParam((int)ServiceCmdParam.Res).ToList();
            SelectedResolution = Resolutions.First();
            PulseDurations = _otdrWrapper.GetManyVariantsForParam((int)ServiceCmdParam.Res).ToList();
            SelectedPulseDuration = PulseDurations.First();
        }

        private void InitializeFromSelectedResolution()
        {
            _otdrWrapper.SetParam((int)ServiceCmdParam.Res, Resolutions.IndexOf(SelectedResolution));

            MeasCountsToAverage = _otdrWrapper.GetManyVariantsForParam((int)ServiceCmdParam.Navr).ToList();
            SelectedMeasCountToAverage = MeasCountsToAverage.First();
            PeriodsToAverage = _otdrWrapper.GetManyVariantsForParam((int)ServiceCmdParam.Time).ToList();
            SelectedPeriodToAverage = PeriodsToAverage.First();
        }

        protected override void OnViewLoaded(object view)
        {
            DisplayName = "Measurement parameters";
        }

        public void SetParams()
        {
            ApplySelections();

            TryClose();
        }

        private void ApplySelections()
        {
            _paramCollectionForOtdr.SelectedUnit = SelectedUnit;
            _paramCollectionForOtdr.Units[SelectedUnit].SelectedDistance = SelectedDistance;
            _paramCollectionForOtdr.Units[SelectedUnit].Distances[SelectedDistance].SelectedResolution = SelectedResolution;

            _paramCollectionForOtdr.Units[SelectedUnit].Distances[SelectedDistance].Resolutions[SelectedResolution].SelectedPulseDuration =
                SelectedPulseDuration;
            _paramCollectionForOtdr.Units[SelectedUnit].Distances[SelectedDistance].Resolutions[SelectedResolution].SelectedTimeToAverage =
                SelectedPeriodToAverage;
            _paramCollectionForOtdr.Units[SelectedUnit].Distances[SelectedDistance].Resolutions[SelectedResolution].SelectedMeasurementCountToAverage =
                SelectedMeasCountToAverage;
            _paramCollectionForOtdr.Units[SelectedUnit].Distances[SelectedDistance].Resolutions[SelectedResolution].Bc = BackscatteredCoefficient;
            _paramCollectionForOtdr.Units[SelectedUnit].Distances[SelectedDistance].Resolutions[SelectedResolution].Ob = RefractiveIndex;
        }

        public void Cancel()
        {
            TryClose();
        }


    }
}
