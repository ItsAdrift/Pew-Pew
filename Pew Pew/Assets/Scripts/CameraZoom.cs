using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraZoom : MonoBehaviour
{

    [SerializeField] float normal;
    [SerializeField] float zoom;
    [SerializeField] float speed;

    public static bool isZoomed = false;

    public void Update()
    {
        Camera cam = GetComponent<Camera>();

        if (Input.GetMouseButton(1))
        {
            isZoomed = true;
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, zoom, Time.deltaTime * speed);
        }
        else
        {
            isZoomed = false;
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, normal, Time.deltaTime * speed);
        }
    }
}
