
using System.Collections.Generic;
using UnityEngine;
using Foundation.Runtime;
using SharedData.Runtime;
using SharedData.Runtime.Structs;

namespace Manager.Runtime
{
    public class SpawnManager : FBehaviour
    {
         #region Public

         [field: SerializeField]
         public ObjectSpawnEvent[] SpawnEvent { get; private set; }
         
         public static SpawnManager Instance { get; private set; }
   
        #endregion

        #region Unity API
        
        private void Awake()
        {
            if (Instance is not null && Instance != this)
            {
                Destroy(gameObject);
                Warning("Another instance of SpawnManager was found in the scene. Destroying this instance!");
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            _currentStepData.OnCurrentStepComplete += StepData_OnCurrentStepComplete;

            if (_camera != null) return;
            _camera = Camera.main;
        }
        
        #endregion

        #region Main Methods

        private void StepData_OnCurrentStepComplete()
        {
            foreach (ObjectSpawnEvent spawnEvent in SpawnEvent)
            {
                if (spawnEvent.Prefab == null) continue;
                
                if (spawnEvent.StepSpawnIndex == _currentStepData.CurrentStepIndex)
                {
                    var obj = Instantiate(spawnEvent.Prefab);
                    obj.transform.position = _camera.transform.position;
                    var spawnPos = obj.transform.position;
                    spawnPos.y -= 1.8f;
                    obj.transform.position = spawnPos;
                    obj.name = spawnEvent.Prefab.name;
                    _spawnedObjects.Add(obj);
                }
                if (spawnEvent.StepDisappearIndex == _currentStepData.CurrentStepIndex)
                {
                    var obj = _spawnedObjects.Find(x => x.name == spawnEvent.Prefab.name);
                    _spawnedObjects.Remove(obj);
                    Destroy(obj);
                }
                    
            }
        }
   
        #endregion

        #region Utils

   
        #endregion

        #region Private & Protected

        [SerializeField] private CurrentStep_Data _currentStepData;
        [SerializeField] private Camera _camera;
        
        private List<GameObject> _spawnedObjects = new List<GameObject>();

        #endregion
    }
}