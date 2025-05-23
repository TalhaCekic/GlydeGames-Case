using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjData : MonoBehaviour
{
    public int maxValue = 100; // Objeye ait maksimum de�er
    public int currentValue; // Objeye ait mevcut de�er
    public float playerPushForce = 500f; // Oyuncuya uygulanacak itme kuvveti
    public float impactThreshold = 10f; // De�erin d��mesi ve oyuncunun itilmesi i�in gereken minimum �arpma h�z�

    private void Start()
    {
        currentValue = maxValue; // Ba�lang��ta mevcut de�eri maksimum yap
    }
    private void Update()
    {
    }

    private void OnCollisionEnter(Collision collision)
    {
        // �arpma h�z�n� kontrol et
        float impactForce = collision.relativeVelocity.magnitude;
        print($"�arpma h�z�: {impactForce}"); // �arpma h�z�n� konsola yazd�r

        // E�er �arpma h�z� belirli bir e�ik de�eri a�arsa, de�eri d���r
        if (impactForce >= impactThreshold)
        {
            int damage = Mathf.RoundToInt(impactForce); // �arpma h�z�n� hasar olarak al
            currentValue -= damage; // De�eri d���r
            currentValue = Mathf.Clamp(currentValue, 0, maxValue); // De�eri 0 ile maxValue aras�nda s�n�rla
            Debug.Log($"Objeye �arpma oldu! Hasar: {damage}, Kalan De�er: {currentValue}");

            // E�er �arp�lan obje bir oyuncuysa, onu geri it
            if (collision.collider.CompareTag("Player"))
            {
                Rigidbody playerRigidbody = collision.collider.GetComponent<Rigidbody>();
                if (playerRigidbody != null)
                {
                    // �arpma y�n�n� hesapla
                    Vector3 pushDirection = collision.contacts[0].normal;
                    playerRigidbody.AddForce(-pushDirection * playerPushForce, ForceMode.Impulse);
                    Debug.Log("Oyuncu geri itildi!");
                }
            }
        }
        else
        {
            Debug.Log($"�arpma h�z� ({impactForce}) e�ik de�erin alt�nda, i�lem yap�lmad�.");
        }
    }
}
