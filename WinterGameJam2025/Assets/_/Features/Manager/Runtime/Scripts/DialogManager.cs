
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
            _currentStepData.CurrentStepDuration = StepDialogEvents[0].TriggerPressure;
            _eventArrayLength = StepDialogEvents.Length;

            _currentStepData.OnCurrentStepComplete += CurrentStepData_OnStepIndexChanged;
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
            
            var currentStepIndex = _currentStepData.CurrentStepIndex;

            for (int i = 0; i < _eventArrayLength; i++)
            {
                var evt = StepDialogEvents[i];
                bool inEntry = currentStepIndex >= evt.DialogEntryRange.From && _currentStepData.CurrentStepIndex <= evt.DialogEntryRange.To;
                bool inExit = currentStepIndex >= evt.DialogExitRange.From && _currentStepData.CurrentStepIndex <= evt.DialogExitRange.To;
                
                if (!inEntry && !inExit) continue;
                
                _eventIndex = i;
                _currentStepData.CurrentStepDuration = evt.TriggerPressure;
            }
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
   
        private int _eventIndex = 0;
        private int _eventArrayLength;
        #endregion
    }
}