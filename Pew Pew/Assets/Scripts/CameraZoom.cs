using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraZoom : MonoBehaviour
{

    [SerializeField] float normal;
    [SerializeField] float zoom;
    [SerializeField] float speed;

    public void Update()
    {
        Camera cam = GetComponent<Camera>();

        if (Input.GetMouseButton(1))
        {
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, zoom, Time.deltaTime * speed);
        }
        else
        {
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, normal, Time.deltaTime * speed);
        }
    }
}
