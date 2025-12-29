
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
        }
   
        #endregion

        #region Main Methods

        private void CurrentStepData_OnStepIndexChanged(int oldIndex, int newIndex)
        {
            for (int i = 0; i < _eventArrayLength; i++)
            {
                if ((_currentStepData.CurrentStepIndex < StepDialogEvents[i].DialogEntryRange.From ||
                     _currentStepData.CurrentStepIndex > StepDialogEvents[i].DialogEntryRange.To) &&
                    (_currentStepData.CurrentStepIndex < StepDialogEvents[i].DialogExitRange.From ||
                     _currentStepData.CurrentStepIndex > StepDialogEvents[i].DialogExitRange.To)) continue;
                
                _eventIndex = i;
                _currentStepData.CurrentStepDuration = StepDialogEvents[i].TriggerPressure;
                break;
            }
        }
   
        #endregion

        #region Utils

   
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