using Mirror;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class carCameraController : NetworkBehaviour
{
    public NetworkIdentity NetworkIdentity;
    public carMovement carMovement;
    [SyncVar] public bool isChange;

    // kamerayı oyuncuya atar ve kamerayı tpp takip
    public override void OnStartLocalPlayer()
    {
        if (SceneManager.GetActiveScene().buildIndex == 0) return;
        Camera.main.transform.SetParent(transform);
        //Camera.main.transform.localPosition = new Vector3(0, 0, 0);
    }

    public void StartFonction()
    {
        if (!netIdentity.isOwned && carMovement.isDriving)
        {
            isChange = true;
        }
        else if (netIdentity.isOwned)
        {
            Camera.main.transform.SetParent(transform);
        }
    }

    public float distance = 5.0f; // Distance between the player and camera
    public float rotationSpeed = 5.0f; // Speed at which the camera rotates
    public float resetSpeed = 2.0f; // Speed at which the camera resets its rotation
    public Vector2 rotationLimits = new Vector2(-80f, 80f); // Vertical rotation limits

    private float currentX = 0.0f;
    private float currentY = 0.0f;

    private void Start()
    {
        if (SceneManager.GetActiveScene().buildIndex == 0) return;
        if (!isLocalPlayer) return;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if (SceneManager.GetActiveScene().buildIndex == 0) return;
        // if (!netIdentity.isOwned)return;
        if (netIdentity.isOwned)
        {
            currentX += Input.GetAxis("Mouse X") * rotationSpeed;
            currentY -= Input.GetAxis("Mouse Y") * rotationSpeed;

            currentY = Mathf.Clamp(currentY, rotationLimits.x, rotationLimits.y);

            Quaternion rotation = Quaternion.Euler(currentY, currentX, 0);
            Vector3 negDistance = new Vector3(0.0f, 0.0f, -distance);
            Vector3 position = rotation * negDistance + this.transform.position;

            Camera.main.transform.rotation = rotation;
            Camera.main.transform.position = position;
        }
        else
        {
            if (isChange)
            {
                currentX += Input.GetAxis("Mouse X") * rotationSpeed;
                currentY -= Input.GetAxis("Mouse Y") * rotationSpeed;

                currentY = Mathf.Clamp(currentY, rotationLimits.x, rotationLimits.y);

                Quaternion rotation = Quaternion.Euler(currentY, currentX, 0);
                Vector3 negDistance = new Vector3(0.0f, 0.0f, -distance);
                Vector3 position = rotation * negDistance + this.transform.position;

                Camera.main.transform.rotation = rotation;
                Camera.main.transform.position = position;
            }
        }
    }
}