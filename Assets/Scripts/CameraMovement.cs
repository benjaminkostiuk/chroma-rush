using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private Transform target;
    [SerializeField] private float zoomSpeed = 2f;
    [SerializeField] private float xOffset = 45;

    private Vector3 previousPosition;
    private float zPosition;
    private float maxZoom = -35f;
    private float minZoom = -8f;

    private void Start()
    {
        zPosition = cam.transform.position.z;
        
        cam.transform.position = target.position;
        // Rotate initial viewpoint of camera based on xoffset
        cam.transform.Rotate(new Vector3(1, 0, 0), xOffset);
        cam.transform.Translate(new Vector3(0, 0, zPosition));
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            previousPosition = cam.ScreenToViewportPoint(Input.mousePosition);
        }

        if (Input.GetMouseButton(0))
        {
            Vector3 direction = previousPosition - cam.ScreenToViewportPoint(Input.mousePosition);
            // Set initial position
            cam.transform.position = target.position;

            // Rotate around inital position based on movement
            cam.transform.Rotate(new Vector3(1, 0, 0), direction.y * 180);
            cam.transform.Rotate(new Vector3(0, 1, 0), -direction.x * 180, Space.World);
            cam.transform.Translate(new Vector3(0, 0, zPosition));

            // Update previous position
            previousPosition = cam.ScreenToViewportPoint(Input.mousePosition);
        }

        // Zoom for scroll wheel
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if ((zPosition > maxZoom && scroll < 0) || (zPosition < minZoom && scroll > 0))
        {
            cam.transform.Translate(0, 0, scroll * zoomSpeed);
            zPosition += scroll * zoomSpeed;
        }
    }
}
