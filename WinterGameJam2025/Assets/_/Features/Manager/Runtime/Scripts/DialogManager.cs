using UnityEngine;
using Foundation.Runtime;
using SharedData.Runtime;
using SharedData.Runtime.Structs;

namespace Manager.Runtime
{
    public class DialogManager : FBehaviour
    {
        #region Public

        public StepDialogEvent[] StepDialogEvents { get; private set; }
        
        public static DialogManager Instance { get; private set; }
   
        #endregion

        #region Unity API

        private void Awake()
        {
            if (Instance is not null && Instance != this)
            {
                Destroy(gameObject);
                Warning("Another instance of DialogManager was found in the scene. Destroying this instance!");
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            StepDialogEvents = _dialogStepEventContainer.StepDialogEvents;
            _eventArrayLength = StepDialogEvents.Length;
            
            if(_eventArrayLength > 0)
                _currentStepData.CurrentStepDuration = StepDialogEvents[0].TriggerPressure;
            
            CurrentStepData_OnStepIndexChanged();

            _currentStepData.OnCurrentStepComplete += CurrentStepData_OnStepIndexChanged;
        }

        
        private void Update()
        {
            if (_eventIndex < 0)
                return;

            var evt = StepDialogEvents[_eventIndex];
            int stepIndex = _currentStepData.CurrentStepIndex;
            float timerProgress = _currentStepData.CurrentStepProgress;

            if (stepIndex >= evt.DialogEntryRange.From &&
                stepIndex <= evt.DialogEntryRange.To)
            {
                float entryProgress = ComputeRangeProgress(
                    stepIndex,
                    evt.DialogEntryRange,
                    timerProgress
                );

                // Feed entryProgress to dialog UI (typewriter)
                // Example: DialogView.SetRevealProgress(entryProgress);
                Info($"Setting progress for DialogView {evt.DialogNode.DialogText} to {entryProgress}");
                _dialogView.SetRevealProgress(entryProgress);
                _dialogView.SetOpacity(1f);
                return;
            }
            
            if (stepIndex >= evt.DialogExitRange.From &&
                     stepIndex <= evt.DialogExitRange.To)
            {
                float exitProgress = ComputeRangeProgress(
                    stepIndex,
                    evt.DialogExitRange,
                    timerProgress
                );

                float opacity = 1f - exitProgress;
                // Feed opacity to dialog UI
                // Example: DialogView.SetOpacity(opacity);
                _dialogView.SetOpacity(opacity);
                Info($"Setting opacity for DialogView {evt.DialogNode.DialogText} to {opacity}");
                return;
            }
            
            _dialogView.SetOpacity(1f);
        }

        private void OnDestroy()
        {
            _currentStepData.OnCurrentStepComplete -= CurrentStepData_OnStepIndexChanged;
        }
        #endregion

        #region Main Methods

        private void CurrentStepData_OnStepIndexChanged()
        {
            _eventIndex = -1;
            
            int currentStepIndex = _currentStepData.CurrentStepIndex;

            for (int i = 0; i < _eventArrayLength; i++)
            {
                var evt = StepDialogEvents[i];
                bool inEntry = currentStepIndex >= evt.DialogEntryRange.From && _currentStepData.CurrentStepIndex <= evt.DialogEntryRange.To;
                bool inExit = currentStepIndex >= evt.DialogExitRange.From && _currentStepData.CurrentStepIndex <= evt.DialogExitRange.To;

                if (!inEntry && !inExit)
                {
                    continue;
                }
                
                _eventIndex = i;
                _currentStepData.CurrentStepDuration = evt.TriggerPressure;

                if (_eventIndex != _previousEventIndex)
                {
                    _dialogView.SetDialog(evt.DialogNode.DialogText);
                    _dialogView.SetOpacity(1f); 
                    _previousEventIndex = _eventIndex;
                }
                
                return;
            }
            
            // if no dialog
            _previousEventIndex = -1;
            _dialogView.SetOpacity(1f);
            //_dialogView.Hide();
        }
   
        #endregion

        #region Utils

        private float ComputeRangeProgress(int stepIndex, StepRange range, float timerProgress)
        {
            if (stepIndex < range.From) return 0f;
            if (stepIndex > range.To) return 1f;

            int rangeLength = range.To - range.From + 1;
            int stepOffset = stepIndex - range.From;
            
            float progress = (stepOffset + timerProgress) / rangeLength;
            
            return Mathf.Clamp01(progress);
        }
   
        #endregion

        #region Private & Protected

        [SerializeField]
        private CurrentStep_Data _currentStepData;
        [SerializeField]
        private DialogStepEventContainer_Data _dialogStepEventContainer;
        [SerializeField]
        private DialogView _dialogView;
   
        private int _eventIndex = 0;
        private int _eventArrayLength;
        private int _previousEventIndex;

        #endregion
    }
}