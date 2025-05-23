using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening; // DOTween k�t�phanesi i�in gerekli

public class Interact : MonoBehaviour
{
    public LayerMask interactableLayer; // Etkile�im yap�lacak layer
    public Transform holdPoint; // Objeyi tutaca��m�z nokta
    public float moveSpeed = 10f; // Hareket h�z�
    public float throwForce = 500f; // F�rlatma kuvveti

    private Rigidbody selectedRigidbody; // Se�ilen objenin Rigidbody'si

    private void Update()
    {
        // Ray'i g�rselle�tirmek i�in Debug.DrawRay ekliyoruz
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Debug.DrawRay(ray.origin, ray.direction * 100f, Color.red);

        if (Input.GetMouseButtonDown(0)) // Sol fare tu�una bas�ld���nda
        {
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, interactableLayer)) // Raycast ile layer kontrol�
            {
                if (hit.rigidbody != null) // �arp�lan objede Rigidbody var m� kontrol et
                {
                    selectedRigidbody = hit.rigidbody; // �arp�lan objenin Rigidbody'sini se�
                    selectedRigidbody.isKinematic = true; // Fizik motorunu devre d��� b�rak
                    Debug.Log("Obje tutuldu: " + selectedRigidbody.name);
                }
                else
                {
                    Debug.LogWarning("Raycast bir Rigidbody'ye �arpmad�!");
                }
            }
            else
            {
                Debug.LogWarning("Raycast hi�bir objeye �arpmad�!");
            }
        }

        if (Input.GetMouseButton(0) && selectedRigidbody != null) // Sol fare tu�u bas�l� tutuluyorsa
        {
            // Objeyi tutma noktas�na do�ru hareket ettir
            selectedRigidbody.MovePosition(Vector3.Lerp(
                selectedRigidbody.position,
                holdPoint.position,
                moveSpeed * Time.deltaTime
            ));
        }

        if (Input.GetMouseButtonUp(0) && selectedRigidbody != null) // Sol fare tu�u b�rak�ld���nda
        {
            selectedRigidbody.isKinematic = false; // Fizik motorunu tekrar etkinle�tir
            selectedRigidbody = null; // Se�ilen objeyi s�f�rla
        }

        if (Input.GetMouseButtonDown(1) && selectedRigidbody != null) // Sa� fare tu�una bas�ld���nda
        {
            ThrowObject(); // Objeyi f�rlat
        }
    }

    private void ThrowObject()
    {
        // Kameran�n bakt��� y�n� al
        Vector3 throwDirection = Camera.main.transform.forward;

        // Objeye kuvvet uygula
        selectedRigidbody.isKinematic = false; // Fizik motorunu tekrar etkinle�tir
        selectedRigidbody.AddForce(throwDirection * throwForce); // Kuvvet uygula

        // Objeyi serbest b�rak
        selectedRigidbody = null;
    }
}
