using UnityEngine;

namespace SharedData.Runtime.ScriptableObjects
{
    [CreateAssetMenu(fileName = "SO_DialogNode", menuName = "Data/DialogNode")]
    public class SO_DialogNode : ScriptableObject
    {
        [TextArea, Tooltip("Dialog text to be displayed.")] 
        public string DialogText;
    }
}
