using Mirror;
using UnityEngine;
using Player.Manager;
using Player.PlayerMenu;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

namespace Player.PlayerControl
{
    public class PlayerController : NetworkBehaviour
    {
        [SerializeField] private float AnimBlendSpeed = 8.9f;
        [SerializeField] private Transform CameraRoot;
        [SerializeField] private Transform camera;
        [SerializeField] private float MouseSensitivity = 21.9f;
        public Transform groundCheck;
        [SerializeField] private float Dis2Ground = 0.8f;
        [SerializeField] private LayerMask GroundCheck;
        [SerializeField] private float AirResistance = 0.8f;
        public Rigidbody _playerRigidbody;
        private InputManager _inputManager;
        public Animator _animator;
        [SyncVar] private bool _grounded = false;
        [SyncVar] private bool _hasAnimator;
        [SyncVar] private int _xVelHash;
        [SyncVar] private int _yVelHash;
        [SyncVar] private int _groundHash;
        [SyncVar] private int _fallingHash;
        [SyncVar] private float _xRotation;

        private const float _walkSpeed = 30f;
        private const float _runSpeed = 45f;
        private Vector3 _currentVelocity;

        public PlayerMenuManager playerMenuManager;

        [SerializeField] Transform HeadTarget, HeadLook;
        float headAngle = 90;

        public override void OnStartAuthority()
        {
            if (SceneManager.GetActiveScene().buildIndex == 0) return;
            camera = Camera.main.transform;
            Camera.main.transform.SetParent(transform);
            Camera.main.transform.position = CameraRoot.position;
        }

        private void Start()
        {
            playerMenuManager = GetComponent<PlayerMenuManager>();
            _hasAnimator = TryGetComponent<Animator>(out _animator);
            _inputManager = GetComponent<InputManager>();


            _xVelHash = Animator.StringToHash("X_Velocity");
            _yVelHash = Animator.StringToHash("Y_Velocity");
            _groundHash = Animator.StringToHash("Grounded");
            _fallingHash = Animator.StringToHash("Falling");

            if (isLocalPlayer)
            {
                if (SceneManager.GetActiveScene().buildIndex == 0) return;
                camera = Camera.main.transform;
                Camera.main.transform.SetParent(transform);
                Camera.main.transform.position = CameraRoot.position;
                headAngle = 0;
            }
        }

        private void Update()
        {
            if (SceneManager.GetActiveScene().buildIndex == 0) return;

            if (isLocalPlayer)
            {
                if (playerMenuManager.mouseActivity) return;
                SampleGround();
                Move();
                CamMovements();
            }
        }
        private void Move()
        {
            if (!_hasAnimator) return;

            float targetSpeed = _inputManager.Run ? _runSpeed : _walkSpeed;
            if (_inputManager.Crouch) targetSpeed = 1.5f;
            if (_inputManager.Move == Vector2.zero) targetSpeed = 0;

            if (_grounded)
            {
                _currentVelocity.x = Mathf.Lerp(_currentVelocity.x, _inputManager.Move.x * targetSpeed,
                    AnimBlendSpeed * Time.deltaTime);
                _currentVelocity.y = Mathf.Lerp(_currentVelocity.y, _inputManager.Move.y * targetSpeed,
                    AnimBlendSpeed * Time.deltaTime);

                var xVelDifference = _currentVelocity.x - _playerRigidbody.velocity.x;
                var zVelDifference = _currentVelocity.y - _playerRigidbody.velocity.z;

                _playerRigidbody.AddForce(transform.TransformVector(new Vector3(xVelDifference, 0, zVelDifference)),
                    ForceMode.Force);
            }
            else
            {
                _playerRigidbody.AddForce(
                    transform.TransformVector(new Vector3(_currentVelocity.x * AirResistance, 0,
                        _currentVelocity.y * AirResistance)), ForceMode.Force);
            }

            _animator.SetFloat(_xVelHash, _currentVelocity.x);
            _animator.SetFloat(_yVelHash, _currentVelocity.y);
        }

        private void CamMovements()
        {
            if (!_hasAnimator) return;
            var Mouse_X = _inputManager.Look.x * Time.deltaTime;
            var Mouse_Y = _inputManager.Look.y * Time.deltaTime;
            camera.position = CameraRoot.position;

            _xRotation -= Mouse_Y * MouseSensitivity;
            _xRotation = Mathf.Clamp(_xRotation, 0, 180);

            headAngle -= Mouse_Y * MouseSensitivity;
            headAngle = Mathf.Clamp(headAngle, -80, 90);

            Vector3 kafaPos = new Vector3(0, Mathf.Sin(headAngle * Mathf.Deg2Rad), Mathf.Cos(headAngle * Mathf.Deg2Rad));
            HeadTarget.transform.localPosition = kafaPos * 2;

            Vector3 kafaBakisPos = new Vector3(0, Mathf.Sin((-headAngle) * Mathf.Deg2Rad),
                Mathf.Cos((-headAngle) * Mathf.Deg2Rad));
            HeadLook.transform.localPosition = kafaBakisPos * 5;

            camera.localRotation = Quaternion.Euler(headAngle, 0, 0);

            transform.Rotate(Vector3.up * Mouse_X * MouseSensitivity);
        }

        private void SampleGround()
        {
            if (!_hasAnimator) return;

            _grounded = Physics.CheckSphere(groundCheck.position, Dis2Ground, GroundCheck);
            SetAnimationGrounding();
            return;
        }

        private void SetAnimationGrounding()
        {
            _animator.SetBool(_fallingHash, !_grounded);
            _animator.SetBool(_groundHash, _grounded);
        }
    }
}