
using System;
using Foundation.Runtime;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player.Runtime
{
    [RequireComponent(typeof(Input))]
    public class PlayerInputRouter : FBehaviour 
    {
        #region Public

        public static PlayerInputRouter Instance { get; private set; }
        
        public event Action<bool> OnLeftStepTriggered;
        public event Action<bool> OnRightStepTriggered;
   
        #endregion

        #region Unity API

        private void Awake()
        {
            if (Instance is not null && Instance != this)
            {
                Destroy(gameObject);
                Warning("Another instance of PlayerInputRouter was found in the scene. Destroying this instance!");
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
   
        #endregion

        #region Main Methods

        public void LeftStep(InputAction.CallbackContext ctx)
        {
            if (ctx is { performed: false, canceled: false }) return;
            bool value = !ctx.canceled;
            
            Info($"Left step triggered with value: {value}");
            OnLeftStepTriggered?.Invoke(value);
        }

        public void RightStep(InputAction.CallbackContext ctx)
        {
            if (ctx is { performed: false, canceled: false }) return;
            bool value = !ctx.canceled;
            
            Info($"Right step triggered with value: {value}");
            
            OnRightStepTriggered?.Invoke(value);
        }
   
        #endregion

        #region Utils

   
        #endregion

        #region Private & Protected
   
        #endregion
    }
}