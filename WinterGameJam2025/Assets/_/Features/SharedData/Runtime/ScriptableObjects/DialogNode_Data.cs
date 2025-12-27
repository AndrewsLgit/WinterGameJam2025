using UnityEngine;

namespace SharedData.Runtime.ScriptableObjects
{
    [CreateAssetMenu(fileName = "SO_DialogNode", menuName = "Data/DialogNode")]
    public class DialogNode_Data : ScriptableObject
    {
        [TextArea, Tooltip("Dialog text to be displayed.")] 
        public string DialogText;
    }
}
