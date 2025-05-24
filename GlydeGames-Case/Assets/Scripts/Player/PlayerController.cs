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
        public CharacterController CharacterController;

        [SerializeField] private float AnimBlendSpeed = 8.9f;
        [SerializeField] private Transform CameraRoot;
        [SerializeField] private Transform camera;
        [SerializeField] private float UpperLimit = -40f;
        [SerializeField] private float BottomLimit = 70f;
        [SerializeField] private float MouseSensitivity = 21.9f;
        [SerializeField, Range(10, 500)] private float JumpFactor = 260f;
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
        [SyncVar] private int _jumpHash;
        [SyncVar] private int _groundHash;
        [SyncVar] private int _fallingHash;
        [SyncVar] private int _zVelHash;
        [SyncVar] private int _crouchHash;
        [SyncVar] private float _xRotation;

        private const float _walkSpeed = 30f;
        private const float _runSpeed = 45f;
        private Vector3 _currentVelocity;

        public PlayerInteract playerInteract;
        public PlayerMenuManager playerMenuManager;

        [SerializeField] Transform HeadTarget, HeadLook;
        float kafaAci = 90;


        [SyncVar] public bool isCollisionItemDec;
        [SyncVar] public float CollisionDelay;

        public override void OnStartAuthority()
        {
            if (SceneManager.GetActiveScene().buildIndex == 0) return;
            camera = Camera.main.transform;
            Camera.main.transform.SetParent(transform);
            Camera.main.transform.position = CameraRoot.position;

        }

        [ClientRpc]
        public void StartFonction(GameObject pos)
        {
            if (!netIdentity.isOwned) return;
            transform.SetParent(null);
            transform.position = pos.GetComponent<carMovement>().Out1Pos.position;
            Camera.main.transform.SetParent(transform);
            Camera.main.transform.position = CameraRoot.position;

            print("çıkart");
        }

        public void StartEnter(Transform slot)
        {
            // print("parent yap");
        }

        private void Start()
        {
            playerInteract = GetComponent<PlayerInteract>();
            playerMenuManager = GetComponent<PlayerMenuManager>();
            _hasAnimator = TryGetComponent<Animator>(out _animator);
            _inputManager = GetComponent<InputManager>();


            _xVelHash = Animator.StringToHash("X_Velocity");
            _yVelHash = Animator.StringToHash("Y_Velocity");
            _zVelHash = Animator.StringToHash("Z_Velocity");
            _jumpHash = Animator.StringToHash("isJump");
            _groundHash = Animator.StringToHash("Grounded");
            _fallingHash = Animator.StringToHash("Falling");
            _crouchHash = Animator.StringToHash("Crouch");

            if (isLocalPlayer)
            {
                if (SceneManager.GetActiveScene().buildIndex == 0) return;
                camera = Camera.main.transform;
                Camera.main.transform.SetParent(transform);
                Camera.main.transform.position = CameraRoot.position;
                kafaAci = 0;
            }
        }

        private void Update()
        {
            if (SceneManager.GetActiveScene().buildIndex == 0) return;
            if (isCollisionItemDec)
            {
                ServerCollision();
            }

            if (isLocalPlayer)
            {
                if (playerMenuManager.mouseActivity) return;
                SampleGround();
                Move();
                HandleJump();
                CamMovements();
            }
        }

        [Server]
        private void ServerCollision()
        {
            if (CollisionDelay > 1)
            {
                //RpcCollisionOpen(0f);
                if (CollisionDelay > 2)
                {
                    CollisionDelay = 0;
                    isCollisionItemDec = false;
                }
                else
                {
                    CollisionDelay += Time.deltaTime;
                }
            }
            else
            {
                //RpcCollisionOpen(0.5f);
                CollisionDelay += Time.deltaTime;
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

            //_xRotation -= Mouse_Y * MouseSensitivity * Time.smoothDeltaTime;
            _xRotation -= Mouse_Y * MouseSensitivity;
            _xRotation = Mathf.Clamp(_xRotation, 0, 180);

            kafaAci -= Mouse_Y * MouseSensitivity;
            kafaAci = Mathf.Clamp(kafaAci, -80, 90);

            Vector3 kafaPos = new Vector3(0, Mathf.Sin(kafaAci * Mathf.Deg2Rad), Mathf.Cos(kafaAci * Mathf.Deg2Rad));
            HeadTarget.transform.localPosition = kafaPos * 2;

            Vector3 kafaBakisPos = new Vector3(0, Mathf.Sin((-kafaAci) * Mathf.Deg2Rad),
                Mathf.Cos((-kafaAci) * Mathf.Deg2Rad));
            HeadLook.transform.localPosition = kafaBakisPos * 5;

            //camera.localRotation = Quaternion.Euler(_xRotation, 0, 0);
            camera.localRotation = Quaternion.Euler(kafaAci, 0, 0);
            //_playerRigidbody.MoveRotation(_playerRigidbody.rotation * Quaternion.Euler(0, Mouse_X * MouseSensitivity * Time.smoothDeltaTime, 0));
            transform.Rotate(Vector3.up * Mouse_X * MouseSensitivity);
        }

        private void HandleCrouch()
        {
            _animator.SetBool(_crouchHash, _inputManager.Crouch);

            if (_inputManager.Crouch)
            {
                if (CharacterController.height >= 0.5f && CharacterController.height < 1.6f)
                {
                    CharacterController.height -= Time.deltaTime * 10;
                    CharacterController.center = new Vector3(0, 0.5f, 0);
                }
            }
            else
            {
                CharacterController.height = 1.5f;
                CharacterController.center = new Vector3(0, 0.75f, 0);
            }
        }

        private void HandleJump()
        {
            // if (!_hasAnimator) return;
            // if (!_inputManager.Jump) return;
            // if (!_grounded) return;
            // _animator.SetBool(_jumpHash, true);
            // _playerRigidbody.AddForce(-_playerRigidbody.velocity.y * Vector3.up, ForceMode.VelocityChange);
            // _playerRigidbody.AddForce(Vector3.up * JumpFactor, ForceMode.Impulse);
        }

        private void ServerHandleJump()
        {
            _animator.SetTrigger(_jumpHash);
        }

        public void JumpAddForce()
        {
            _playerRigidbody.AddForce(-_playerRigidbody.velocity.y * Vector3.up, ForceMode.VelocityChange);
            _playerRigidbody.AddForce(Vector3.up * JumpFactor, ForceMode.Impulse);
            _animator.ResetTrigger(_jumpHash);
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

        [Command]
        public void rbForce(Vector3 forcePos)
        {
            RpcrbForce(forcePos);
        }

        [ClientRpc]
        public void RpcrbForce(Vector3 forcePos)
        {
            CharacterController.Move(forcePos * 20 * Time.deltaTime);
            isCollisionItemDec = true;
            CollisionDelay = 0;
        }
    }
}