using Mirror;
using UnityEngine;

public class CanvasSetting : NetworkBehaviour
{
    public Canvas canvas;

    void Start()
    {
        if (isServer)
        {
            canvas = GetComponent<Canvas>();
            canvas.worldCamera = Camera.main;
        }
    }

    private void Update()
    {
        Vector3  cameraPosition = Camera.main.transform.position;
        transform.LookAt( transform.position - cameraPosition);
    }
}