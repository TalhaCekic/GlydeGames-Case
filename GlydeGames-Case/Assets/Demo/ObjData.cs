using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjData : MonoBehaviour
{
    public int maxValue = 100; // Objeye ait maksimum deðer
    public int currentValue; // Objeye ait mevcut deðer
    public float playerPushForce = 500f; // Oyuncuya uygulanacak itme kuvveti
    public float impactThreshold = 10f; // Deðerin düþmesi ve oyuncunun itilmesi için gereken minimum çarpma hýzý

    private void Start()
    {
        currentValue = maxValue; // Baþlangýçta mevcut deðeri maksimum yap
    }
    private void Update()
    {
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Çarpma hýzýný kontrol et
        float impactForce = collision.relativeVelocity.magnitude;
        print($"Çarpma hýzý: {impactForce}"); // Çarpma hýzýný konsola yazdýr

        // Eðer çarpma hýzý belirli bir eþik deðeri aþarsa, deðeri düþür
        if (impactForce >= impactThreshold)
        {
            int damage = Mathf.RoundToInt(impactForce); // Çarpma hýzýný hasar olarak al
            currentValue -= damage; // Deðeri düþür
            currentValue = Mathf.Clamp(currentValue, 0, maxValue); // Deðeri 0 ile maxValue arasýnda sýnýrla
            Debug.Log($"Objeye çarpma oldu! Hasar: {damage}, Kalan Deðer: {currentValue}");

            // Eðer çarpýlan obje bir oyuncuysa, onu geri it
            if (collision.collider.CompareTag("Player"))
            {
                Rigidbody playerRigidbody = collision.collider.GetComponent<Rigidbody>();
                if (playerRigidbody != null)
                {
                    // Çarpma yönünü hesapla
                    Vector3 pushDirection = collision.contacts[0].normal;
                    playerRigidbody.AddForce(-pushDirection * playerPushForce, ForceMode.Impulse);
                    Debug.Log("Oyuncu geri itildi!");
                }
            }
        }
        else
        {
            Debug.Log($"Çarpma hýzý ({impactForce}) eþik deðerin altýnda, iþlem yapýlmadý.");
        }
    }
}
