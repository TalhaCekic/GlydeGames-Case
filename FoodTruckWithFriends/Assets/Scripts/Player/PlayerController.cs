using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using Player.Manager;
using Player.PlayerMenu;
using DG.Tweening;

namespace Player.PlayerControl
{
    public class PlayerController : NetworkBehaviour
    {
        [SerializeField] private float AnimBlendSpeed = 8.9f;
        [SerializeField] private Transform CameraRoot;
        [SerializeField] private Transform camera;
        [SerializeField] private float UpperLimit = -40f;
        [SerializeField] private float BottomLimit = 70f;
        [SerializeField] private float MouseSensitivity = 21.9f;
        [SerializeField, Range(10, 500)] private float JumpFactor = 260f;
        [SerializeField] private float Dis2Ground = 0.8f;
        [SerializeField] private LayerMask GroundCheck;
        [SerializeField] private float AirResistance = 0.8f;
        private Rigidbody _playerRigidbody;
        private InputManager _inputManager;
        private Animator _animator;
        [SyncVar] private bool _grounded = false;
        [SyncVar]  private bool _hasAnimator;
        [SyncVar]  private int _xVelHash;
        [SyncVar]  private int _yVelHash;
        [SyncVar]  private int _jumpHash;
        [SyncVar]  private int _groundHash;
        [SyncVar]  private int _fallingHash;
        [SyncVar]  private int _zVelHash;
        [SyncVar]  private int _crouchHash;
        [SyncVar]  private float _xRotation;

         private const float _walkSpeed = 2f;
         private const float _runSpeed = 6f;
        private Vector2 _currentVelocity;

        public PlayerInteract playerInteract;
        public PlayerMenuManager playerMenuManager;

        [SerializeField] Transform HeadTarget, HeadLook;
        float kafaAci = 90;

        public override void OnStartLocalPlayer()
        {
            camera = Camera.main.transform;
            Camera.main.transform.SetParent(this.transform);
            Camera.main.transform.localPosition =
                new Vector3(CameraRoot.position.x, CameraRoot.position.y, CameraRoot.position.z);
        }

        private void Start()
        {
            playerInteract = GetComponent<PlayerInteract>();
            playerMenuManager = GetComponent<PlayerMenuManager>();
            _hasAnimator = TryGetComponent<Animator>(out _animator);
            _playerRigidbody = GetComponent<Rigidbody>();
            _inputManager = GetComponent<InputManager>();


            _xVelHash = Animator.StringToHash("X_Velocity");
            _yVelHash = Animator.StringToHash("Y_Velocity");
            _zVelHash = Animator.StringToHash("Z_Velocity");
            _jumpHash = Animator.StringToHash("Jump");
            _groundHash = Animator.StringToHash("Grounded");
            _fallingHash = Animator.StringToHash("Falling");
            _crouchHash = Animator.StringToHash("Crouch");
        }

        private void FixedUpdate()
        {
            if (isLocalPlayer)
            {
                if (playerInteract.mouseActivity)
                {
                    _currentVelocity.x = 0;
                    _currentVelocity.y = 0;
                    
                    _animator.SetFloat(_yVelHash, _currentVelocity.y);
                    _animator.SetFloat(_xVelHash, _currentVelocity.x);
                }
                else
                {
                    SampleGround();
                    Move();
                    HandleJump();
                    HandleCrouch();
                
                }
            }
        }

        private void LateUpdate()
        {
            if (isLocalPlayer)
            {
                if (playerInteract.mouseActivity)
                {
                    //CamMovements();
                }
                else
                {
                    CamMovements();
                }
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
                    AnimBlendSpeed * Time.fixedDeltaTime);
                _currentVelocity.y = Mathf.Lerp(_currentVelocity.y, _inputManager.Move.y * targetSpeed,
                    AnimBlendSpeed * Time.fixedDeltaTime);

                var xVelDifference = _currentVelocity.x - _playerRigidbody.velocity.x;
                var zVelDifference = _currentVelocity.y - _playerRigidbody.velocity.z;

                _playerRigidbody.AddForce(transform.TransformVector(new Vector3(xVelDifference, 0, zVelDifference)),
                    ForceMode.VelocityChange);
            }
            else
            {
                _playerRigidbody.AddForce(
                    transform.TransformVector(new Vector3(_currentVelocity.x * AirResistance, 0,
                        _currentVelocity.y * AirResistance)), ForceMode.VelocityChange);
            }


            _animator.SetFloat(_xVelHash, _currentVelocity.x);
            _animator.SetFloat(_yVelHash, _currentVelocity.y);
        }
        private void CamMovements()
        {
            if (!_hasAnimator) return;
            
            var Mouse_X = _inputManager.Look.x;
            var Mouse_Y = _inputManager.Look.y;
            camera.position = CameraRoot.position;
            
            _xRotation -= Mouse_Y * MouseSensitivity * Time.smoothDeltaTime;
            _xRotation = Mathf.Clamp(_xRotation, UpperLimit, BottomLimit);
            
            kafaAci +=  Mouse_Y * MouseSensitivity * Time.smoothDeltaTime;
            kafaAci = Mathf.Clamp (kafaAci, 30, 150);
            
            Vector3 kafaPos = new Vector3 (0, Mathf.Sin (kafaAci* Mathf.Deg2Rad), Mathf.Cos (kafaAci * Mathf.Deg2Rad));
            HeadTarget.transform.localPosition = kafaPos*2;
            
            Vector3 kafaBakisPos = new Vector3 (0, Mathf.Sin ((kafaAci - 90) * Mathf.Deg2Rad), Mathf.Cos ((kafaAci - 90) * Mathf.Deg2Rad));
            HeadLook.transform.localPosition = kafaBakisPos*5;
            
            camera.localRotation = Quaternion.Euler(_xRotation, 0, 0);
            _playerRigidbody.MoveRotation(_playerRigidbody.rotation *
                                          Quaternion.Euler(0, Mouse_X * MouseSensitivity * Time.smoothDeltaTime, 0));
        }   

        private void HandleCrouch() => _animator.SetBool(_crouchHash, _inputManager.Crouch);
        
        private void HandleJump()
        {
            if (!_hasAnimator) return;
            if (!_inputManager.Jump) return;
            if (!_grounded) return;
            //_animator.SetTrigger(_jumpHash);
            ServerHandleJump();
            _playerRigidbody.AddForce(-_playerRigidbody.velocity.y * Vector3.up, ForceMode.VelocityChange);
            _playerRigidbody.AddForce(Vector3.up * JumpFactor, ForceMode.Impulse);
            //_animator.ResetTrigger(_jumpHash);
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

            RaycastHit hitInfo;
            if (Physics.Raycast(_playerRigidbody.worldCenterOfMass, Vector3.down, out hitInfo, Dis2Ground + 0.1f,
                    GroundCheck))
            {
                //Grounded
                _grounded = true;
                SetAnimationGrounding();
                return;
            }

            //Falling
            _grounded = false;
            //_animator.SetFloat(_zVelHash, _playerRigidbody.velocity.y);
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