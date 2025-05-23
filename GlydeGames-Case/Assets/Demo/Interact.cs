using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening; // DOTween kütüphanesi için gerekli

public class Interact : MonoBehaviour
{
    public LayerMask interactableLayer; // Etkileþim yapýlacak layer
    public Transform holdPoint; // Objeyi tutacaðýmýz nokta
    public float moveSpeed = 10f; // Hareket hýzý
    public float throwForce = 500f; // Fýrlatma kuvveti

    private Rigidbody selectedRigidbody; // Seçilen objenin Rigidbody'si

    private void Update()
    {
        // Ray'i görselleþtirmek için Debug.DrawRay ekliyoruz
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Debug.DrawRay(ray.origin, ray.direction * 100f, Color.red);

        if (Input.GetMouseButtonDown(0)) // Sol fare tuþuna basýldýðýnda
        {
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, interactableLayer)) // Raycast ile layer kontrolü
            {
                if (hit.rigidbody != null) // Çarpýlan objede Rigidbody var mý kontrol et
                {
                    selectedRigidbody = hit.rigidbody; // Çarpýlan objenin Rigidbody'sini seç
                    selectedRigidbody.isKinematic = true; // Fizik motorunu devre dýþý býrak
                    Debug.Log("Obje tutuldu: " + selectedRigidbody.name);
                }
                else
                {
                    Debug.LogWarning("Raycast bir Rigidbody'ye çarpmadý!");
                }
            }
            else
            {
                Debug.LogWarning("Raycast hiçbir objeye çarpmadý!");
            }
        }

        if (Input.GetMouseButton(0) && selectedRigidbody != null) // Sol fare tuþu basýlý tutuluyorsa
        {
            // Objeyi tutma noktasýna doðru hareket ettir
            selectedRigidbody.MovePosition(Vector3.Lerp(
                selectedRigidbody.position,
                holdPoint.position,
                moveSpeed * Time.deltaTime
            ));
        }

        if (Input.GetMouseButtonUp(0) && selectedRigidbody != null) // Sol fare tuþu býrakýldýðýnda
        {
            selectedRigidbody.isKinematic = false; // Fizik motorunu tekrar etkinleþtir
            selectedRigidbody = null; // Seçilen objeyi sýfýrla
        }

        if (Input.GetMouseButtonDown(1) && selectedRigidbody != null) // Sað fare tuþuna basýldýðýnda
        {
            ThrowObject(); // Objeyi fýrlat
        }
    }

    private void ThrowObject()
    {
        // Kameranýn baktýðý yönü al
        Vector3 throwDirection = Camera.main.transform.forward;

        // Objeye kuvvet uygula
        selectedRigidbody.isKinematic = false; // Fizik motorunu tekrar etkinleþtir
        selectedRigidbody.AddForce(throwDirection * throwForce); // Kuvvet uygula

        // Objeyi serbest býrak
        selectedRigidbody = null;
    }
}
