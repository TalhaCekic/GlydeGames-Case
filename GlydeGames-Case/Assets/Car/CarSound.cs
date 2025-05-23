using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CarSound : NetworkBehaviour
{
    public AudioSource motorAudioSource;
    //public AudioSource tireScreechAudioSource;
    
     public float minSpeed;
     public float maxSpeed;
    
    public float minPitch = 0.8f;
    public float maxPitch = 2.0f;
    public float maxRPM = 7000f;
    public Rigidbody carRigidbody;
    public float screechThreshold = 5.0f;
    private float pitchFromCar;

    private float currentRPM;
    private float carSpeed;
    
    public float minDistance = 10f;  // Sesin maksimum seviyede duyulacağı mesafe
    public float maxDistance = 50f;

    void Update()
    {
        if(!isServer)return;
        UpdateEngineSound();
    }

    [Server]
    void UpdateEngineSound()
    {
        RpcUpdateEngineSound();
    }

    [ClientRpc]
    void RpcUpdateEngineSound()
    {
        // if (motorAudioSource != null)
        // {
        //     motorAudioSource.spatialBlend = 1.0f;  // 3D ses
        //     motorAudioSource.minDistance = minDistance;
        //     motorAudioSource.maxDistance = maxDistance;
        //     motorAudioSource.rolloffMode = AudioRolloffMode.Logarithmic; // Gerçekçi ses azalma
        // }
        //
        // carSpeed = carRigidbody.velocity.magnitude;
        // currentRPM = Mathf.Clamp(carSpeed * 1000f, 0f, maxRPM);
        // print(currentRPM);
        //
        // float pitch = Mathf.Lerp(minPitch, maxPitch, currentRPM / maxRPM);
        // motorAudioSource.pitch = pitch;
        if (motorAudioSource != null)
        {
            motorAudioSource.spatialBlend = 1.0f;  
            motorAudioSource.minDistance = minDistance;
            motorAudioSource.maxDistance = maxDistance;
            motorAudioSource.rolloffMode = AudioRolloffMode.Logarithmic; 
        }
        
        carSpeed = carRigidbody.velocity.magnitude;
        pitchFromCar = carRigidbody.velocity.magnitude / 60f;        
        
        if (carSpeed < minSpeed)
        {
            motorAudioSource.pitch = minPitch;
        }

        if (carSpeed > minSpeed && carSpeed < maxSpeed)
        {
            motorAudioSource.pitch = minPitch + pitchFromCar;
        }

        if (carSpeed > maxSpeed)
        {
            motorAudioSource.pitch = maxPitch;
        }
        
    }


}
