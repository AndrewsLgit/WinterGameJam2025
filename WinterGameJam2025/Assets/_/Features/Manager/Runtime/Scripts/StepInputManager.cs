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
            
            _endScreen.alpha = 0;
            _isGameOver = false;
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

        private void OnApplicationQuit()
        {
            _currentStep.Reset();
        }

        #endregion

        #region Main Methods

        private void HandleTimer(bool isBeingPressed)
        {
            
            if (!_isStepInProgress && isBeingPressed)
            {
                // if (_currentStep.CurrentStepDuration <= 0) // TODO: create timer for 100-150ms when duration == 0
                // {
                //     SetTimerProgressToData(0f);
                //     _currentStep.IncrementCurrentStepIndex();
                //     _isStepComplete = true;
                //     _stepTimer?.Stop();
                //     if (_stepTimer != null)
                //     {
                //         _stepTimer.OnTimerStop -= _currentStep.IncrementCurrentStepIndex;
                //         _stepTimer.OnTimerTick -= SetTimerProgressToData;
                //     }
                //     _stepTimer = null;
                //     
                //     return;
                // }


                var stepTime = _currentStep.CurrentStepDuration == 0
                    ? 0.200f
                    : _currentStep.CurrentStepDuration * 0.001f;
                
                
                _stepTimer = new CountdownTimer(stepTime); // * 0.001f to turn milliseconds into seconds
                _stepTimer?.Start();
                _currentStep.IsWalking = true;
                _stepTimer.OnTimerStop += _currentStep.IncrementCurrentStepIndex;
                _stepTimer.OnTimerTick += SetTimerProgressToData;

                _isStepInProgress = true;
                _isStepComplete = false;

                return;
            }

            if (!isBeingPressed && _stepTimer != null)
            {
                _stepTimer?.Pause();
                _currentStep.IsWalking = false;
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
            _currentStep.IsWalking = false;
            PlayRandomStepSound();

            if (_currentStep.CurrentStepIndex >= _lastStepIndex)
                _isGameOver = true;
            
            Info($"Completed step, new index: {_currentStep.CurrentStepIndex}", this);
        }

        private void SetTimerProgressToData(float progress)
        {
            // if (_stepTimer == null) return;
            var fromZeroToOne = 1f - progress;
            _currentStep.CurrentStepProgress = Mathf.Clamp01(fromZeroToOne);
            _currentStep.IsWalking = true;
            // _currentStep.CurrentStepProgress = fromZeroToOne;
            Info($"Timer running with progress {_currentStep.CurrentStepProgress} || Step index: {_currentStep.CurrentStepIndex}", this);

            if (_currentStep.CurrentStepIndex > -1)
            {
                _isGameStart = false;
                _tutorialScreen.alpha = 0;
            }

            if (_isGameStart)
            {
                _tutorialScreen.alpha = progress;
                _endScreen.alpha = 0;
            }

            if (_isGameOver && _currentStep.CurrentStepIndex == _lastStepIndex)
            {
                _endScreen.alpha = fromZeroToOne;
            }
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
            if (_isGameOver && _currentStep.CurrentStepIndex == _lastStepIndex + 1) return;
            HandleTimer(isBeingPressed);
        }

        private void InputRouter_OnLeftStepTriggered(bool isBeingPressed)
        {
            if (!_currentStep.IsLeftStep) return;
            if (_isGameOver && _currentStep.CurrentStepIndex == _lastStepIndex + 1) return;

            HandleTimer(isBeingPressed);
        }

        private void PlayRandomStepSound()
        {
            var rnd = UnityEngine.Random.Range(0, _stepSounds.Length);
            _stepAudioSource.clip = _stepSounds[rnd];
            _stepAudioSource.Play(); 
        }

        #endregion

        #region Private & Protected

        [SerializeField] private CurrentStep_Data _currentStep;
        [SerializeField] private AudioSource _stepAudioSource;
        [SerializeField] private AudioClip[] _stepSounds;
        [SerializeField] private int _lastStepIndex;
        [SerializeField] private CanvasGroup _tutorialScreen;
        [SerializeField] private CanvasGroup _endScreen;

        private PlayerInputRouter _inputRouter;
        private float? _currentStepTimer;
        private bool _isStepComplete;
        private bool _isStepInProgress;
        private CountdownTimer _stepTimer;
        private bool _isGameStart = true;
        private bool _isGameOver = false;

        #endregion
    }
}