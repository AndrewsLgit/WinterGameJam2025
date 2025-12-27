
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
        
        #endregion

        #region Main Methods

   
        #endregion

        #region Utils

   
        #endregion

        #region Private & Protected

        [SerializeField] private CurrentStep_Data _currentStep;

        #endregion
    }
}