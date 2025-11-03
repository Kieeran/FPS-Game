using Unity.Netcode;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace PlayerAssets
{
    [RequireComponent(typeof(CharacterController))]
#if ENABLE_INPUT_SYSTEM
    [RequireComponent(typeof(PlayerInput))]
#endif
    public class PlayerController : PlayerBehaviour
    {
        [Header("Player Movement")]
        public float MoveSpeed = 2.0f;
        public float SprintSpeed = 5.335f;
        [Range(0.0f, 0.3f)] public float RotationSmoothTime = 0.12f;
        public float SpeedChangeRate = 10.0f;

        [Header("Jumping and Gravity")]
        public float JumpHeight = 1.2f;
        public float Gravity = -15.0f;
        public float JumpTimeout = 0.50f;
        public float FallTimeout = 0.15f;

        [Header("Ground Check")]
        public bool Grounded = true;
        public float GroundedOffset = -0.14f;
        public float GroundedRadius = 0.28f;
        public LayerMask GroundLayers;

        [Header("Camera Settings")]
        public GameObject CinemachineCameraTarget;
        public float TopClamp = 70.0f;
        public float BottomClamp = -30.0f;
        public float CameraAngleOverride = 0.0f;
        public bool LockCameraPosition = false;

        [Header("Audio")]
        public AudioClip LandingAudioClip;
        public AudioClip[] FootstepAudioClips;
        [Range(0, 1)] public float FootstepAudioVolume = 0.5f;

        // Private variables
        private float _cinemachineTargetYaw;
        private float _cinemachineTargetPitch;

        private float _speed;
        private float _animationBlend;
        private float _targetRotation = 0.0f;
        private float _rotationVelocity;
        private float _verticalVelocity;
        private float _terminalVelocity = 53.0f;

        private float _jumpTimeoutDelta;
        private float _fallTimeoutDelta;

        [SerializeField] GameObject _playerModel;
        [SerializeField] Animator _animator;
        [SerializeField] CharacterController _controller;

        bool _toggleCameraRotation = true;

#if ENABLE_INPUT_SYSTEM
        private PlayerInput _playerInput;
#endif
        // private CharacterController _controller;
        private PlayerAssetsInputs _input;
        private GameObject _mainCamera;

        private const float _threshold = 0.01f;
        // public bool _hasAnimator;

        // Animator parameters
        private int _animIDSpeed;
        private int _animIDGrounded;
        private int _animIDJump;
        private int _animIDFreeFall;
        private int _animIDMotionSpeed;
        private int _animIDVelocityX;
        private int _animIDVelocityY;

        Vector3 _currentPos;
        Quaternion _currentRot;

        private bool IsCurrentDeviceMouse
        {
            get
            {
#if ENABLE_INPUT_SYSTEM
                return _playerInput != null && _playerInput.currentControlScheme == "KeyboardMouse";
#else
                return false;
#endif
            }
        }

        public override void InitializeAwake()
        {
            base.InitializeAwake();
            PlayerRoot.Events.ToggleEscapeUI += () =>
            {
                _toggleCameraRotation = !_toggleCameraRotation;
            };
        }

        public override void OnInGameManagerReady(InGameManager manager)
        {
            base.OnInGameManagerReady(manager);
            manager.OnGameEnd += () =>
            {
                _toggleCameraRotation = false;
            };
            _mainCamera = InGameManager.Instance.PlayerCamera;
        }

        public override void InitializeStart()
        {
            base.InitializeStart();
            _cinemachineTargetYaw = CinemachineCameraTarget.transform.rotation.eulerAngles.y;

            // _hasAnimator = TryGetComponent(out _animator);
            // _controller = GetComponent<CharacterController>();
            _input = GetComponent<PlayerAssetsInputs>();

#if ENABLE_INPUT_SYSTEM
            _playerInput = GetComponent<PlayerInput>();
#endif

            AssignAnimationIDs();

            _jumpTimeoutDelta = JumpTimeout;
            _fallTimeoutDelta = FallTimeout;

            _currentPos = transform.localPosition;
            _currentRot = transform.localRotation;
        }

        void Update()
        {
            if (!IsOwner) return;

            // _hasAnimator = TryGetComponent(out _animator);

            GroundedCheck();
            JumpAndGravity();
            Move();
            Shoot();

            // _playerModel.transform.rotation = Quaternion.Euler(0f, _cinemachineTargetYaw, 0f);
            // transform.SetLocalPositionAndRotation(_currentPos, transform.localRotation);
            _playerModel.transform.rotation = Quaternion.Euler(0, CinemachineCameraTarget.transform.eulerAngles.y, 0);
        }

        private void LateUpdate()
        {
            CameraRotation();
        }

        private void AssignAnimationIDs()
        {
            _animIDSpeed = Animator.StringToHash("Speed");
            _animIDGrounded = Animator.StringToHash("Grounded");
            _animIDJump = Animator.StringToHash("Jump");
            _animIDFreeFall = Animator.StringToHash("FreeFall");
            _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
            _animIDVelocityX = Animator.StringToHash("VelocityX");
            _animIDVelocityY = Animator.StringToHash("VelocityY");
        }

        private void GroundedCheck()
        {
            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z);
            Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers, QueryTriggerInteraction.Ignore);

            // if (_hasAnimator)
            // {
            //     _animator.SetBool(_animIDGrounded, Grounded);
            // }

            _animator.SetBool(_animIDGrounded, Grounded);
        }

        private void Move()
        {
            float targetSpeed = _input.sprint ? SprintSpeed : MoveSpeed;
            if (_input.move == Vector2.zero) targetSpeed = 0.0f;

            float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;
            float speedOffset = 0.1f;
            float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;

            if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
            {
                _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * SpeedChangeRate);
                _speed = Mathf.Round(_speed * 1000f) / 1000f;
            }
            else
            {
                _speed = targetSpeed;
            }

            _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * SpeedChangeRate);
            if (_animationBlend < 0.01f) _animationBlend = 0f;

            Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;

            if (_input.move != Vector2.zero)
            {
                _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + _mainCamera.transform.eulerAngles.y;
                float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity, RotationSmoothTime);

                // Move Foward
                if (_input.move.y > 0 || Input.GetMouseButtonDown(0))
                {
                    // if (_hasAnimator)
                    // {
                    //     _animator.SetFloat(_animIDVelocityY, 10);
                    //     _animator.SetFloat(_animIDVelocityX, 0);
                    // }

                    _animator.SetFloat(_animIDVelocityY, 10);
                    _animator.SetFloat(_animIDVelocityX, 0);
                }

                // Move Left
                if (_input.move.x < 0 && _input.move.y == 0)
                {
                    // if (_hasAnimator)
                    // {
                    //     _animator.SetFloat(_animIDVelocityY, 0);
                    //     _animator.SetFloat(_animIDVelocityX, -10);
                    // }

                    _animator.SetFloat(_animIDVelocityY, 0);
                    _animator.SetFloat(_animIDVelocityX, -10);
                }

                // Move Right
                if (_input.move.x > 0 && _input.move.y == 0)
                {
                    // if (_hasAnimator)
                    // {
                    //     _animator.SetFloat(_animIDVelocityY, 0);
                    //     _animator.SetFloat(_animIDVelocityX, 10);
                    // }

                    _animator.SetFloat(_animIDVelocityY, 0);
                    _animator.SetFloat(_animIDVelocityX, 10);
                }

                // Move Foward
                if (_input.move.y < 0)
                {
                    // if (_hasAnimator)
                    // {
                    //     _animator.SetFloat(_animIDVelocityY, -10);
                    //     _animator.SetFloat(_animIDVelocityX, 0);
                    // }

                    _animator.SetFloat(_animIDVelocityY, -10);
                    _animator.SetFloat(_animIDVelocityX, 0);
                }

                transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
            }

            Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;

            _controller.Move(targetDirection.normalized * (_speed * Time.deltaTime) + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);

            // if (_hasAnimator)
            // {
            //     _animator.SetFloat(_animIDSpeed, _animationBlend);
            //     _animator.SetFloat(_animIDMotionSpeed, inputMagnitude);
            // }

            _animator.SetFloat(_animIDSpeed, _animationBlend);
            _animator.SetFloat(_animIDMotionSpeed, inputMagnitude);

            // transform.SetLocalPositionAndRotation(
            //     Vector3.Lerp(_currentPos, _currentPos, 0),
            //     _playerModel.transform.rotation
            // );
        }

        private void Shoot()
        {
            if (_input.shoot)
            {
                _animator.SetLayerWeight(1, Mathf.Lerp(_animator.GetLayerWeight(1), 1f, Time.deltaTime * 10f));
            }
            else _animator.SetLayerWeight(1, Mathf.Lerp(_animator.GetLayerWeight(1), 0f, Time.deltaTime * 10f));
        }

        private void JumpAndGravity()
        {
            if (Grounded)
            {
                _fallTimeoutDelta = FallTimeout;

                // if (_hasAnimator)
                // {
                //     _animator.SetBool(_animIDJump, false);
                //     _animator.SetBool(_animIDFreeFall, false);
                // }

                _animator.SetBool(_animIDJump, false);
                _animator.SetBool(_animIDFreeFall, false);

                if (_verticalVelocity < 0.0f)
                {
                    _verticalVelocity = -2f;
                }

                if (_input.jump && _jumpTimeoutDelta <= 0.0f)
                {
                    _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);

                    // if (_hasAnimator)
                    // {
                    //     _animator.SetBool(_animIDJump, true);
                    // }

                    _animator.SetBool(_animIDJump, true);
                }

                if (_jumpTimeoutDelta >= 0.0f)
                {
                    _jumpTimeoutDelta -= Time.deltaTime;
                }
            }
            else
            {
                _jumpTimeoutDelta = JumpTimeout;

                if (_fallTimeoutDelta >= 0.0f)
                {
                    _fallTimeoutDelta -= Time.deltaTime;
                }
                else
                {
                    // if (_hasAnimator)
                    // {
                    //     _animator.SetBool(_animIDFreeFall, true);
                    // }

                    _animator.SetBool(_animIDFreeFall, true);
                }

                _input.jump = false;
            }

            if (_verticalVelocity < _terminalVelocity)
            {
                _verticalVelocity += Gravity * Time.deltaTime;
            }
        }

        private void CameraRotation()
        {
            if (!_toggleCameraRotation) return;

            if (_input.look.sqrMagnitude >= _threshold && !LockCameraPosition)
            {
                float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;

                _cinemachineTargetYaw += _input.look.x * deltaTimeMultiplier;
                _cinemachineTargetPitch += _input.look.y * deltaTimeMultiplier;
            }

            _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
            _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

            CinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + CameraAngleOverride, _cinemachineTargetYaw, 0.0f);
        }

        public void RotateCameraTo(Quaternion targetRotation)
        {
            CinemachineCameraTarget.transform.rotation = targetRotation;

            _input.look = Vector2.zero;

            Vector3 euler = targetRotation.eulerAngles;
            _cinemachineTargetYaw = euler.y;
            _cinemachineTargetPitch = euler.x - CameraAngleOverride;
        }

        private static float ClampAngle(float angle, float min, float max)
        {
            if (angle < -360f) angle += 360f;
            if (angle > 360f) angle -= 360f;
            return Mathf.Clamp(angle, min, max);
        }

        private void OnDrawGizmosSelected()
        {
            Color gizmoColor = Grounded ? new Color(0f, 1f, 0f, 0.35f) : new Color(1f, 0f, 0f, 0.35f);
            Gizmos.color = gizmoColor;

            Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z), GroundedRadius);
        }

        public void OnFootstep(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f && FootstepAudioClips.Length > 0)
            {
                var index = Random.Range(0, FootstepAudioClips.Length);
                AudioSource.PlayClipAtPoint(FootstepAudioClips[index], transform.TransformPoint(_controller.center), FootstepAudioVolume);
            }
        }

        public void OnLand(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                AudioSource.PlayClipAtPoint(LandingAudioClip, transform.TransformPoint(_controller.center), FootstepAudioVolume);
            }
        }
    }
}