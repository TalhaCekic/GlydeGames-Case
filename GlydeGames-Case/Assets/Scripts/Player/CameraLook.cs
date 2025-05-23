using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLook : MonoBehaviour
{
    public float mouseSensitvity = 100f;
    public Transform playerBody;
    float xRotation = 0f;

    public Transform target;  
    public float smoothSpeed = 0.125f;  
    public float shakeAmplitude = 0.1f; 
    public float shakeFrequency = 10f; 

    private Vector3 offset;  
    private bool isMoving = false;  
    void Start()
    {
        //Cursor.lockState = CursorLockMode.Locked;
        //offset = transform.position - target.position;  // Kameran�n ba�lang�� pozisyonu
    }

    
    void Update()
    {
       float mouseX = Input.GetAxis("Mouse X") * mouseSensitvity * Time.deltaTime;
       float mouseY = Input.GetAxis("Mouse Y") * mouseSensitvity * Time.deltaTime;

        xRotation -= mouseY;
        //kilitleme i�lemi
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        playerBody.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerBody.Rotate(Vector3.up * mouseX);


        cameraShake();

    }
    private void cameraShake()
    {
        // Oyuncu hareket ederken sars�nt� efektini uygulama
        if (isMoving)
        {
            float shakeX = Mathf.Sin(Time.time * shakeFrequency) * shakeAmplitude;
            float shakeY = Mathf.Sin((Time.time + 1f) * shakeFrequency) * shakeAmplitude;
            playerBody.transform.position += new Vector3(shakeX, shakeY, 0f);
        }

        // Kameran�n pozisyonunu belirleme
        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        playerBody.transform.position = smoothedPosition;
    }
    public void StartMoving()
    {
        isMoving = true;
    }

    // Oyuncunun hareket etmeyi b�rakt���n� belirten fonksiyon
    public void StopMoving()
    {
        isMoving = false;
    }
}
