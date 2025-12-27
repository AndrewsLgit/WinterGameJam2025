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
        public bool IsStepComplete = false;
        
        public event Action OnCurrentStepComplete;
        
        public void IncrementCurrentStepIndex() 
        {
            CurrentStepIndex++;
            OnCurrentStepComplete?.Invoke();
        }

        public void Reset()
        {
            CurrentStepIndex = 0;
            CurrentStepDuration = 0f;
            IsStepComplete = false;
        }
    }
}
