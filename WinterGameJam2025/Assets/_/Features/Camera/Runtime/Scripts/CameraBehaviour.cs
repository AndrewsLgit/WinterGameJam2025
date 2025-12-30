using System.Collections.Generic;
using Foundation.Runtime;
using SharedData.Runtime;
using UnityEngine;

namespace Camera.Runtime
{
    public class CameraBehaviour : FBehaviour 
    {
        #region Public

   
        #endregion

        #region Unity API

        private void Awake()
        {
            if ( _camera == null )
                _camera = GetComponent<UnityEngine.Camera>();

            _baseLookDirection = transform.rotation;
            
            _pendingStepSpawn = true;
            _isForcingLookDown = true;
            _isLookingDown = true;
            _lookForwardTime = 8f;
            _isFirstStep = true;
        }

        private void Start()
        {
            _stepData.OnCurrentStepComplete += StepData_OnCurrentStepComplete;
        }

        private void OnDestroy()
        {
            _stepData.OnCurrentStepComplete -= StepData_OnCurrentStepComplete;
        }

        private void Update()
        {
            HandleWalkingState(Time.deltaTime);
            UpdateStepOpacity();
            UpdateSmoothMovement(Time.deltaTime);
        }

        
        #endregion

        #region Main Methods
        
   
        #endregion

        #region Utils

        private void StepData_OnCurrentStepComplete()
        {
            _pendingStepSpawn = true;

            // If camera is already looking down, resolve
            if (IsCameraLookingDown())
            {
                ResolvePendingStep();
            }
            else
            {    
                _isLookingDown = true;
                _isForcingLookDown = true;
                _lookForwardTime = _timeBeforeLookForward;
            }
        }

        private void ResolvePendingStep()
        {
            if(!_pendingStepSpawn) return;
            
            SpawnStepAtLookPoint();
            _pendingStepSpawn = false;
        }

        private void SpawnStepAtLookPoint()
        {
            var stepPrefab = _stepData.IsLeftStep ? _leftStepPrefab : _rightStepPrefab;
            var offset = _offsetDistance;
            if (_isFirstStep) offset.z -= offset.z;
            offset.x = _stepData.IsLeftStep ? -offset.x : offset.x; 
            
            //transform.position += transform.up * _moveDistance;

            if (Physics.Raycast(_camera.transform.position, _camera.transform.forward, out RaycastHit hit, 1000, _groundLayerMask))
            {
                Debug.DrawRay(_camera.transform.position, _camera.transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);

                Vector3 spawnPos = hit.point + offset;
                _currentlySpawnedStep = Instantiate(stepPrefab, spawnPos, Quaternion.identity);
                
                CacheStepRenderer();
                if (_isFirstStep)
                {
                    _isFirstStep = false;
                }
                else
                    BeginSmoothMoveForward();
            }
        }

        private void CacheStepRenderer()
        {
            _currentlySpawnedStepRenderer = _currentlySpawnedStep.GetComponentInChildren<Renderer>();

            if (_currentlySpawnedStepRenderer == null) return;
            
            Color color = _currentlySpawnedStepRenderer.material.color;
            color.a = 0f;
            _currentlySpawnedStepRenderer.material.color = color;

            _activeStepRenderers.Add(_currentlySpawnedStepRenderer);
            
        }

        private void BeginSmoothMoveForward()
        {
            if (_isFirstStep)
                return;
            
            // if (_isMovingForward)
            // return;
            
            _moveStartPos = transform.position;
            Vector3 planarForward = GetPlanarForward();
            _moveTargetPos = transform.position + planarForward * _moveDistance;
            _moveProgress = 0f;
            _isMovingForward = true;
        }

        private Vector3 GetPlanarForward()
        {
            float yaw = transform.eulerAngles.y;
            Vector3 forward = Quaternion.Euler(0f, yaw, 0f) * Vector3.forward;
            
            return forward.normalized;
        }

        private void UpdateSmoothMovement(float deltaTime)
        {
            if (!_isMovingForward) return;
            
            _moveProgress += deltaTime *  _moveSpeed;
            float t = Mathf.Clamp01(_moveProgress);
            
            transform.position = Vector3.Lerp(_moveStartPos, _moveTargetPos, t);
            
            if (t >= 1f)
                _isMovingForward = false;
        }

        private void UpdateStepOpacity()
        {
            // if(_currentlySpawnedStepRenderer == null) return;
            if (_activeStepRenderers.Count <= 0) return;

            float alpha = Mathf.Clamp01(_stepData.CurrentStepProgress);

            for (int i = 0; i < _activeStepRenderers.Count; i++)
            {
                var r = _activeStepRenderers[i];
                if (r == null) continue;

                Color color = r.material.color;
                if (r == _currentlySpawnedStepRenderer)
                {
                    if (alpha >= 0.95f)
                    {
                        alpha = 1f;
                        _currentlySpawnedStepRenderer = null;
                    }
                    color.a = alpha;
                }
                else
                {
                    color.a = 1f;
                }

                r.material.color = color;
            }
        }
        
        private void SmoothLookDown(float deltaTime)
        {
            // var desiredRotation = transform.rotation.eulerAngles;
            // desiredRotation.y = _downAngle;
            // transform.rotation = Quaternion.LookRotation(desiredRotation, Vector3.up);
            
            Quaternion target = Quaternion.Euler(_downAngle, transform.eulerAngles.y, 0f);
            //transform.rotation = target;
            
            transform.rotation = Quaternion.Slerp(transform.rotation, target, _rotationSpeed * deltaTime);
        }

        private void SmoothLookForward(float deltaTime)
        {
            // transform.rotation = Quaternion.LookRotation(transform.forward, Vector3.up);
            // Quaternion target = Quaternion.Euler(_forwardAngle, transform.eulerAngles.y, 0f);
            
            // transform.rotation = _baseLookDirection;
            
            transform.rotation = Quaternion.Slerp(transform.rotation, _baseLookDirection, _rotationSpeed * deltaTime);
        }

        private void HandleWalkingState(float deltaTime)
        {
            bool isWalkingNow = _stepData.IsWalking;

            if (isWalkingNow && !_isForcingLookDown)
            {
                _isLookingDown = true;
                _isForcingLookDown = true;
                _lookForwardTime = _timeBeforeLookForward;
                // SmoothLookDown(deltaTime);
            }

            if (_isForcingLookDown)
            {
                SmoothLookDown(deltaTime);
                
                // float currentXAngle = NormalizeAngle(transform.eulerAngles.x);
                // if (Mathf.Abs(currentXAngle - _downAngle) <= _lookDownCompleteThreshold)
                if (IsCameraLookingDown())
                {
                    _isForcingLookDown = false;

                    ResolvePendingStep();
                    // if (!_isForcingLookDown && _pendingStepSpawn)
                    // {
                    //     SpawnStepAtLookPoint();
                    //     // BeginSmoothMoveForward();
                    //     _pendingStepSpawn = false;
                    // }
                }

                return;
            }
            
            // else
            if (!isWalkingNow)
            {
                _lookForwardTime -= deltaTime;

                if (_lookForwardTime <= 0f)
                {
                    _isLookingDown = false;
                    SmoothLookForward(deltaTime);
                }
            }
            
            _wasWalking = isWalkingNow;
        }

        private bool IsCameraLookingDown()
        {
            float currentXAngle = NormalizeAngle(transform.eulerAngles.x);
            return Mathf.Abs(currentXAngle - _downAngle) <= _lookDownCompleteThreshold;
        }

        private float NormalizeAngle(float angle)
        {
            angle %= 360f;
            if (angle > 180f) angle -= 360f;
            return angle;
        }
        #endregion

        #region Private & Protected

        [Header("References")]
        [SerializeField] 
        private UnityEngine.Camera  _camera;
        [SerializeField] 
        private CurrentStep_Data _stepData;
        [SerializeField] 
        private GameObject _leftStepPrefab;
        [SerializeField] 
        private GameObject _rightStepPrefab;
        [SerializeField] 
        private LayerMask _groundLayerMask;

        [Header("Rotation Settings")] 
        [SerializeField] 
        private float _rotationSpeed;
        [SerializeField] 
        private float _downAngle;
        [SerializeField] 
        private float _forwardAngle = 0f;
        [SerializeField] 
        private float _timeBeforeLookForward = 2f;
        [SerializeField] 
        private float _lookDownCompleteThreshold = 1f;

        [Header("Movement Settings")]
        [SerializeField] 
        private float _moveSpeed;
        [SerializeField] 
        private float _moveDistance;
        
        [Header("SpawnSettings")]
        [SerializeField, Tooltip("Offset from center of the camera")] 
        private Vector3 _offsetDistance;
        
        private GameObject _currentlySpawnedStep;
        private Renderer _currentlySpawnedStepRenderer;
        private bool _isLookingDown;
        private bool _wasWalking;
        private float _lookForwardTime;
        private Quaternion _baseLookDirection;
        private bool _isForcingLookDown;
        private bool _pendingStepSpawn;
        private Vector3 _moveStartPos;
        private Vector3 _moveTargetPos;
        private float _moveProgress;
        private bool _isMovingForward;
        private List<Renderer> _activeStepRenderers = new List<Renderer>();
        private bool _isFirstStep;

        #endregion
    }
}