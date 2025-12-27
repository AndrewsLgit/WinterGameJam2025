
using UnityEngine;
using Foundation.Runtime;
using SharedData.Runtime;

namespace Manager.Runtime
{
    public class StepInputManager : FBehaviour
    {
         #region Public

         public StepInputManager Instance { get; private set; }
   
        #endregion

        #region Unity API

        private void Awake()
        {
            if (Instance is not null && Instance != this)
            {
                Destroy(gameObject);
                Warning("Another instance of StepInputManager was found in the scene. Destroying this instance!");
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