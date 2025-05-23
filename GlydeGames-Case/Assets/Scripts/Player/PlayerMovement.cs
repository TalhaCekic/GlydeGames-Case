using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.UI;
using Player.Manager;
using Player.PlayerMenu;
using UnityEngine.SceneManagement;

public class PlayerMovement : NetworkBehaviour
{
    public CharacterController CharacterController;
    public float speed = 1f;
    private float runSpeed = 0;
    public float jumpHeight = 2f;
    public Animator anims;

    [SerializeField] private Transform CameraRoot;
    [SyncVar] private int _xVelHash;
    [SyncVar] private int _zVelHash;
    
    Vector3 velocity;
    public float gravity = -9.81f;
    public bool isGround;

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    CameraLook cameraLook;
    
    public override void OnStartAuthority()
    {
        if (SceneManager.GetActiveScene().buildIndex == 0) return;
        //camera = Camera.main.transform;
        Camera.main.transform.SetParent(CameraRoot);
        //Camera.main.transform.localPosition = new Vector3(CameraRoot.position.x, CameraRoot.position.y, CameraRoot.position.z);
        Camera.main.transform.localPosition = Vector3.zero;
    }
    private void Start()
    {
        cameraLook = FindAnyObjectByType<CameraLook>();
        _xVelHash = Animator.StringToHash("X_Velocity");
        _zVelHash = Animator.StringToHash("Z_Velocity");
    }

    void Update()
    {
        if(!isLocalPlayer)return;
        Move();
        bend();
    }
    private void Move()
    {
        isGround = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        if (isGround == false)
        {
            Vector3 move = transform.right * x + transform.forward * z;
            CharacterController.Move(move * 2 * Time.deltaTime);
            shake();
        }
        if (isGround == true)
        {
            Vector3 move = transform.right * x + transform.forward * z;
            runSpeed = Mathf.Lerp(runSpeed, 1f, Time.deltaTime);
            CharacterController.Move(move * runSpeed * Time.deltaTime);
            CharacterController.Move(move * speed * Time.deltaTime);
            

            if (Input.GetKey(KeyCode.LeftShift))
            {
                runSpeed = Mathf.Lerp(runSpeed, 1f, Time.deltaTime);
                CharacterController.Move(move * (runSpeed += Time.deltaTime * 2) * Time.deltaTime);
            }
            shake();
        }
        if (isGround && velocity.y < 0)
        {
            velocity.y = -2f;
        }
        if (Input.GetButtonDown("Jump") && isGround)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
        velocity.y += gravity * Time.deltaTime;
        CharacterController.Move(velocity * Time.deltaTime);

    }
    private void shake()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        Vector3 move = transform.right * x + transform.forward * z;
        //hareket edip etmedi�ini kontrol eder ve sars�lmay� a�ar.
        if (move.magnitude > 0.05f)
        {
            cameraLook.StartMoving();
            //anims.SetBool("walk", true);
        }
        if (move.magnitude < 0.05f)
        {
            cameraLook.StopMoving();
            //anims.SetBool("walk", false);
        }
        anims.SetFloat(_xVelHash, x);
        anims.SetFloat(_zVelHash, z);
    }
    private void bend()
    {
        if (Input.GetKey(KeyCode.LeftControl))
        {
            CharacterController.height = 1f;
        }
        else if(!Input.GetKey(KeyCode.LeftControl))
        {
            if(CharacterController.height >= 1f && CharacterController.height < 2.52f)
            {
                CharacterController.height += Time.deltaTime *10;
            }          
        }
    }
}
