using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Caliburn.Micro;
using IitOtdrLibrary;

namespace WpfExample
{
    public class OtdrParamViewModel : Screen
    {
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

        public List<string> Units { get; set; }

        public string SelectedUnit
        {
            get { return _selectedUnit; }
            set
            {
                if (value == _selectedUnit) return;
                _selectedUnit = value;
                NotifyOfPropertyChange();

                _otdrWrapper.SetParam((int)ServiceCmdParam.Unit, Units.IndexOf(SelectedUnit));
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

                _otdrWrapper.SetParam((int)ServiceCmdParam.Ri, (int)(BackscatteredCoefficient*10));
                InitializeFromSelectedDistance();
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

                _otdrWrapper.SetParam((int)ServiceCmdParam.Ri, (int)(RefractiveIndex*100000));
                InitializeFromSelectedDistance();
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

                _otdrWrapper.SetParam((int)ServiceCmdParam.Lmax, Distances.IndexOf(SelectedDistance));
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

                _otdrWrapper.SetParam((int)ServiceCmdParam.Res, Resolutions.IndexOf(SelectedResolution));
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

                if (!IsTimeToAverageSelected)
                {
                    _otdrWrapper.SetParam((int) ServiceCmdParam.Navr,
                        MeasCountsToAverage.IndexOf(SelectedMeasCountToAverage));
                    PeriodsToAverage = _otdrWrapper.ParseLineOfVariantsForParam((int) ServiceCmdParam.Time).ToList();
                    SelectedPeriodToAverage = PeriodsToAverage.First();
                }
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

                if (IsTimeToAverageSelected)
                {
                    _otdrWrapper.SetParam((int) ServiceCmdParam.Time, PeriodsToAverage.IndexOf(SelectedPeriodToAverage));
                    MeasCountsToAverage = _otdrWrapper.ParseLineOfVariantsForParam((int) ServiceCmdParam.Navr).ToList();
                    SelectedMeasCountToAverage = MeasCountsToAverage.First();
                }
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
                if (IsTimeToAverageSelected)
                {
                    PeriodsToAverage = _otdrWrapper.ParseLineOfVariantsForParam((int) ServiceCmdParam.Time).ToList();
                    SelectedPeriodToAverage = PeriodsToAverage.First();
                }
                else
                {
                    MeasCountsToAverage = _otdrWrapper.ParseLineOfVariantsForParam((int) ServiceCmdParam.Navr).ToList();
                    SelectedMeasCountToAverage = MeasCountsToAverage[2];
                }
            }
        }

        #endregion

        private readonly IitOtdrWrapper _otdrWrapper;
        private bool _isTimeToAverageSelected;

        public OtdrParamViewModel(IitOtdrWrapper otdrWrapper)
        {
            _otdrWrapper = otdrWrapper;

            IsTimeToAverageSelected = true;
            Units = _otdrWrapper.ParseLineOfVariantsForParam((int)ServiceCmdParam.Unit).ToList();
            SelectedUnit = Units.First();
        }

        private void InitializeForSelectedUnit()
        {
            _backscatteredCoefficient = double.Parse(_otdrWrapper.ParseLineOfVariantsForParam((int)ServiceCmdParam.Bc)[0], new CultureInfo("en-US"));
            _refractiveIndex = double.Parse(_otdrWrapper.ParseLineOfVariantsForParam((int)ServiceCmdParam.Ri)[0], new CultureInfo("en-US"));
            Distances = _otdrWrapper.ParseLineOfVariantsForParam((int)ServiceCmdParam.Lmax).ToList();
            SelectedDistance = Distances.First();
        }

        private void InitializeFromSelectedDistance()
        {
            Resolutions = _otdrWrapper.ParseLineOfVariantsForParam((int)ServiceCmdParam.Res).Skip(1).ToList();
            SelectedResolution = Resolutions[1];
            PulseDurations = _otdrWrapper.ParseLineOfVariantsForParam((int)ServiceCmdParam.Pulse).ToList();
            SelectedPulseDuration = PulseDurations.Count > 3 ? PulseDurations[3] : PulseDurations.Last();
        }

        private void InitializeFromSelectedResolution()
        {
            IsTimeToAverageSelected =
                int.Parse(_otdrWrapper.ParseLineOfVariantsForParam((int) ServiceCmdParam.IsTime)[0]) == 1;
            if (IsTimeToAverageSelected)
            {
                PeriodsToAverage = _otdrWrapper.ParseLineOfVariantsForParam((int)ServiceCmdParam.Time).ToList();
                SelectedPeriodToAverage = PeriodsToAverage.First();
            }
            else
            {
                MeasCountsToAverage = _otdrWrapper.ParseLineOfVariantsForParam((int)ServiceCmdParam.Navr).ToList();
                SelectedMeasCountToAverage = MeasCountsToAverage[2];
            }
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
        }

        public void Cancel()
        {
            TryClose();
        }


    }
}
