using System;
using UnityEngine;

namespace SharedData.Runtime.Structs
{
    [Serializable]
    public struct ObjectSpawnEvent
    {
        [field: SerializeField, Tooltip("Step index needed for the prefab to spawn")]
        public int StepSpawnIndex { get; private set; }
        
        [field: SerializeField, Tooltip("Prefab to spawn")]
        public GameObject Prefab { get; private set; }
        
        [field: SerializeField, Tooltip("Step index needed for the prefab to disappear")]
        public int StepDisappearIndex { get; private set; }
    }
}