using SharedData.Runtime.Structs;
using UnityEngine;

namespace SharedData.Runtime
{
    [CreateAssetMenu(fileName = "SO_DialogStepEventContainer", menuName = "Data/DialogStepEventContainer")]
    public class DialogStepEventContainer_Data : ScriptableObject
    {
        public StepDialogEvent[] StepDialogEvents;
    }
}
