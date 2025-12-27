using UnityEngine;

namespace SharedData.Runtime
{
    [CreateAssetMenu(fileName = "SO_CurrentStep", menuName = "Data/CurrentStep")]
    public class CurrentStep_Data : ScriptableObject
    {
        public int CurrentStepIndex;
        public float CurrentStepDuration;
        public bool IsStepComplete;
    }
}
