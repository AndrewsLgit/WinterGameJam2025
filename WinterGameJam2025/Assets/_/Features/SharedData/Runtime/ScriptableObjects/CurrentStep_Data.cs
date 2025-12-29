using System;
using UnityEngine;

namespace SharedData.Runtime
{
    [CreateAssetMenu(fileName = "SO_CurrentStep", menuName = "Data/CurrentStep")]
    public class CurrentStep_Data : ScriptableObject
    {
        [field: SerializeField]
        public int CurrentStepIndex { get; private set; }
        public float CurrentStepDuration;
        public float CurrentStepProgress;
        public bool IsStepComplete = false;
        public bool IsLeftStep = true;
        
        public event Action OnCurrentStepComplete;
        
        public void IncrementCurrentStepIndex() 
        {
            CurrentStepIndex++;
            IsLeftStep = !IsLeftStep;
            OnCurrentStepComplete?.Invoke();
        }

        public void Reset()
        {
            CurrentStepIndex = 0;
            //CurrentStepDuration = 0f;
            IsStepComplete = false;
            CurrentStepProgress = 0;
        }
    }
}
