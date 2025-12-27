using System;
using SharedData.Runtime.ScriptableObjects;
using UnityEngine;

namespace SharedData.Runtime.Structs
{
    [Serializable]
    public struct StepDialogEvent
    {
        [field: SerializeField, Tooltip("Dialog to display")]
        public DialogNode_Data DialogNode { get; private set; }
        
        [field: SerializeField, Tooltip("Step index range for the dialog to start and complete.")]
        public StepRange DialogEntryRange { get; private set; }
        
        [field: SerializeField, Tooltip("Step index range for the dialog to disappear")]
        public StepRange DialogExitRange { get; private set; }
        
        [field: SerializeField, Tooltip("Time needed for the step to be considered as complete (in milliseconds). Instant step = 0")]
        public float TriggerPressure {get; private set;}
    }

    [Serializable]
    public struct StepRange
    {
        [field: SerializeField]
        public int From { get; private set; }
        
        [field: SerializeField]
        public int To { get; private set; }
    }
}