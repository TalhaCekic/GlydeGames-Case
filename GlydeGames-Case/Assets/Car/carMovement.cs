using UnityEngine;
using System;
using System.Collections.Generic;
using Mirror;
using UnityEngine.UI;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class carMovement : NetworkBehaviour
{
    private bool isOwned = false;
    
    public enum ControlMode
    {
        Keyboard,
        Buttons
    };

    public enum Axel
    {
        Front,
        Rear
    }

    [Serializable]
    public struct Wheel
    {
        public GameObject wheelModel;
        public WheelCollider wheelCollider;
        public GameObject wheelEffectObj;
        public ParticleSystem smokeParticle;
        public Axel axel;
    }

    public ControlMode control;

    public float maxAcceleration = 30.0f;
    public float brakeAcceleration = 50.0f;

    public float turnSensitivity = 1.0f;
    public float maxSteerAngle = 30.0f;

    public Vector3 _centerOfMass;

    public List<Wheel> wheels;

    float moveInput;
    float steerInput;

    private Rigidbody carRb;

    private PlayerListObjController _playerListObjController;
    [SyncVar] public float speed;
    [Header("HUD")] 
    private ProgressBar speedBar;
    public TMPro.TMP_Text speedText;
    public NetworkIdentity netIdentity;
// private CarLights carLights;

    [Header("Car system")]
    [SyncVar] public bool isDriving;
    public Transform Sit1Pos;
    public Transform Sit2Pos;
    public Transform Out1Pos;
    public Transform Out2Pos;

    public Transform SpawnPos;

    public override void OnStartClient()
    {
        transform.position = SpawnPos.position;
    }
    public override void OnStartServer()
    {
        transform.position = SpawnPos.position;
    }

    public void CarStartFunction(Transform ts)
    {
        SpawnPos = ts;
    }

    void Start()
    {
        if (SceneManager.GetActiveScene().buildIndex == 0) return;
        if (isLocalPlayer)
        {
            GameObject ingameHud = GameObject.Find("PlayerHudDoc");
            speedBar = ingameHud.GetComponent<UIDocument>().rootVisualElement.Q("SpeedBar") as ProgressBar;
            
            carRb = GetComponent<Rigidbody>();
            carRb.centerOfMass = _centerOfMass;
            // carLights = GetComponent<CarLights>();
            //DontDestroyOnLoad(this);
        }
    }
 
    void Update()
    {
        if (SceneManager.GetActiveScene().buildIndex == 0) return;
        if (netIdentity.isOwned)
        {
            GetInputs();
            AnimateWheels();
            //WheelEffects();
            //speed = carRb.velocity.magnitude * 3.6f;
            //speedText.text = Mathf.Round(speed) + " km/h";   
            
            //speedBar.title = Mathf.Round(speed) + " km/h";
            //speedBar.value = speed;
        }
        else
        {
            moveInput = 0;
        }
    }

    void LateUpdate()
    {
        if (SceneManager.GetActiveScene().buildIndex == 0) return;
        if (netIdentity.isOwned)
        {
            Move();
            Steer();
            Brake();
        }
    }

    public void MoveInput(float input)
    {
        if (!isLocalPlayer) return;
        moveInput = input;
    }

    public void SteerInput(float input)
    {
        if (!isLocalPlayer) return;
        steerInput = input;
    }

    public void GetInputs()
    {
        if (control == ControlMode.Keyboard)
        {
            moveInput = Input.GetAxis("Vertical");
            steerInput = Input.GetAxis("Horizontal");
        }

        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            SteamLobby.instance.Restart();
        }
    }

    public void Move()
    {
        foreach (var wheel in wheels)
        {
            wheel.wheelCollider.motorTorque = moveInput * 600 * maxAcceleration * Time.deltaTime;
        }
    }

    public void Steer()
    {
        foreach (var wheel in wheels)
        {
            if (wheel.axel == Axel.Front)
            {
                var _steerAngle = steerInput * turnSensitivity * maxSteerAngle;
                wheel.wheelCollider.steerAngle = Mathf.Lerp(wheel.wheelCollider.steerAngle, _steerAngle, 0.6f);
            }
        }
    }

    public  void Brake()
    {
        if (Input.GetKey(KeyCode.Space) || moveInput == 0)
        {
            foreach (var wheel in wheels)
            {
                wheel.wheelCollider.brakeTorque = 300 * brakeAcceleration * Time.deltaTime;
            }
            //   carLights.isBackLightOn = true;
            //   carLights.OperateBackLights();
        }
        else
        {
            foreach (var wheel in wheels)
            {
                wheel.wheelCollider.brakeTorque = 0;
            }
            //   carLights.isBackLightOn = false;
            //   carLights.OperateBackLights();
        }
    }

   public void AnimateWheels()
    {
        foreach (var wheel in wheels)
        {
            Quaternion rot;
            Vector3 pos;
            wheel.wheelCollider.GetWorldPose(out pos, out rot);
            wheel.wheelModel.transform.position = pos;
            wheel.wheelModel.transform.rotation = rot;
        }
    }

    [Command(requiresAuthority = false)]
    void WheelEffects()
    {
        foreach (var wheel in wheels)
        {
            //var dirtParticleMainSettings = wheel.smokeParticle.main;

            if (Input.GetKey(KeyCode.Space) && wheel.axel == Axel.Rear && wheel.wheelCollider.isGrounded == true &&
                carRb.velocity.magnitude >= 10.0f)
            {
                wheel.wheelEffectObj.GetComponentInChildren<TrailRenderer>().emitting = true;
                wheel.smokeParticle.Emit(1);
            }
            else
            {
                //  wheel.wheelEffectObj.GetComponentInChildren<TrailRenderer>().emitting = false;
            }
        }
    }
}