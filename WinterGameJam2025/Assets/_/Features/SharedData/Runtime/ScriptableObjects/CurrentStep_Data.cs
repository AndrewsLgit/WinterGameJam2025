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
        public bool IsWalking = false;
        public bool IsLeftStep = true;
        
        public event Action OnCurrentStepComplete;
        
        public void IncrementCurrentStepIndex() 
        {
            CurrentStepIndex++;
            IsLeftStep = !IsLeftStep;
            OnCurrentStepComplete?.Invoke();
            CurrentStepProgress = 0f;
        }

        public void Reset()
        {
            CurrentStepIndex = 0;
            IsWalking = false;
            CurrentStepProgress = 0;
        }
    }
}
