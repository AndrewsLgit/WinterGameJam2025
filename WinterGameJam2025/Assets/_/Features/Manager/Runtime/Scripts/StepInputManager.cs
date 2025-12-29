using System;
using UnityEngine;
using Foundation.Runtime;
using Player.Runtime;
using SharedData.Runtime;
using Tools.Runtime;

namespace Manager.Runtime
{
    public class StepInputManager : FBehaviour
    {
        #region Public

        public static StepInputManager Instance { get; private set; }
   
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

        private void Start()
        {
            _inputRouter = PlayerInputRouter.Instance;
            _currentStep.Reset();
            
            SubscribeToInputEvents();

            _currentStep.OnCurrentStepComplete += HandleStepComplete;
        }

        private void Update()
        {
            if (_stepTimer is {IsRunning: true})
                _stepTimer.Tick(Time.deltaTime);
        }

        private void OnEnable()
        {
            // SubscribeToInputEvents();
            //
            // _currentStep.OnCurrentStepComplete += HandleStepComplete;
        }

        private void OnDisable()
        {
            UnsubscribeFromInputEvents();
            _currentStep.OnCurrentStepComplete -= HandleStepComplete;
        }

        #endregion

        #region Main Methods

        private void HandleTimer(bool isBeingPressed)
        {
            if (!_isStepInProgress && isBeingPressed)
            {
                if (_currentStep.CurrentStepDuration <= 0)
                {
                    SetTimerProgressToData(0f);
                    _currentStep.IncrementCurrentStepIndex();
                    _isStepComplete = true;
                    _stepTimer?.Stop();
                    if (_stepTimer != null)
                    {
                        _stepTimer.OnTimerStop -= _currentStep.IncrementCurrentStepIndex;
                        _stepTimer.OnTimerTick -= SetTimerProgressToData;
                    }
                    _stepTimer = null;
                    
                    return;
                }
                
                _stepTimer = new CountdownTimer(_currentStep.CurrentStepDuration * 0.001f); // * 0.001f to turn milliseconds into seconds
                _stepTimer?.Start();
                _stepTimer.OnTimerStop += _currentStep.IncrementCurrentStepIndex;
                _stepTimer.OnTimerTick += SetTimerProgressToData;

                _isStepInProgress = true;
                _isStepComplete = false;

                return;
            }

            if (!isBeingPressed && _stepTimer != null)
            {
                _stepTimer?.Pause();
                return;
            }
            
            _stepTimer?.Resume();
        }
        
   
        #endregion

        #region Utils

        private void HandleStepComplete()
        {
            _isStepInProgress = false;
            _isStepComplete = true;

            if (_stepTimer == null) return;
            _stepTimer.OnTimerStop -= _currentStep.IncrementCurrentStepIndex;
            _stepTimer.OnTimerTick -= SetTimerProgressToData;
            _stepTimer = null;
            
            Info($"Completed step, new index: {_currentStep.CurrentStepIndex}", this);
        }

        private void SetTimerProgressToData(float progress)
        {
            // if (_stepTimer == null) return;
            
            _currentStep.CurrentStepProgress = 1f - progress;
            Info($"Timer running with progress {progress} || Step index: {_currentStep.CurrentStepIndex}", this);
        }

        private void SubscribeToInputEvents()
        {
            _inputRouter ??= PlayerInputRouter.Instance;

            if (_inputRouter is null)
            {
                Error("Input router singleton was not found.");
                return;
            }
            _inputRouter.OnLeftStepTriggered += InputRouter_OnLeftStepTriggered;
            _inputRouter.OnRightStepTriggered += InputRouter_OnRightStepTriggered;
        }

        private void UnsubscribeFromInputEvents()
        {
            _inputRouter ??= PlayerInputRouter.Instance;

            if (_inputRouter is null)
            {
                Error("Input router singleton was not found.");
                return;
            }
            _inputRouter.OnLeftStepTriggered -= InputRouter_OnLeftStepTriggered;
            _inputRouter.OnRightStepTriggered -= InputRouter_OnRightStepTriggered;
        }

        private void InputRouter_OnRightStepTriggered(bool isBeingPressed)
        {
            if (_currentStep.IsLeftStep) return;
            HandleTimer(isBeingPressed);
        }

        private void InputRouter_OnLeftStepTriggered(bool isBeingPressed)
        {
            if (!_currentStep.IsLeftStep) return;
            HandleTimer(isBeingPressed);
        }

        #endregion

        #region Private & Protected

        [SerializeField] private CurrentStep_Data _currentStep;

        private PlayerInputRouter _inputRouter;
        private float? _currentStepTimer;
        private bool _isStepComplete;
        private bool _isStepInProgress;
        private CountdownTimer _stepTimer;

        #endregion
    }
}