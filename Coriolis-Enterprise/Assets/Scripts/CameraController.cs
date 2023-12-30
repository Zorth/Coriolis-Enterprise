using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float slowSpeed, slowRotateSpeed, movementTime, fastMultiplier, minHeight, maxHeight, maxDepth, minDepth;
    private Transform cameraTransform;

    private float cameraSpeed, cameraRotateSpeed, cameraTime, zoomLevel, newZoom;


    private Vector3 newPos;
    private Quaternion newRot;

    // Start is called before the first frame update
    void Start()
    {
        newPos = transform.position;
        newZoom = 0.5f;
        newRot = Quaternion.identity;
        cameraTransform = Camera.main.transform;
    }

    // Update is called once per frame
    void Update()
    {
        HandleInput();
    }

    void HandleInput()
    {
        float shiftMultiplier = ((fastMultiplier - 1f) * Input.GetAxis("Fast") + 1f);

        handlePosition(shiftMultiplier);
        handleRotation(shiftMultiplier);
        handleZoom(shiftMultiplier);

        handleReset();
    }

    private void handleReset()
    {
        if (Input.GetAxis("Reset") > 0.5f)
        {
            Start();
        }
    }

    private void handleRotation(float shiftMultiplier)
    {
        cameraRotateSpeed = slowRotateSpeed * shiftMultiplier;

        newRot *= Quaternion.AngleAxis(Input.GetAxis("Rotate") * cameraRotateSpeed, Vector3.up);

        transform.rotation = Quaternion.Lerp(transform.rotation, newRot, Time.deltaTime * cameraRotateSpeed * 10f);
    }

    private void handleZoom(float shiftMultiplier)
    {
        newZoom = Mathf.Clamp(newZoom - Input.GetAxis("Mouse ScrollWheel") * shiftMultiplier, 0f, 1f);
        zoomLevel = zoomLevel + (newZoom - zoomLevel) * Time.deltaTime * movementTime;

        cameraTransform.localPosition = new Vector3(0, minHeight + (maxHeight - minHeight) * zoomLevel, -(minDepth + (maxDepth - minDepth) * (1f - zoomLevel)));

    }

    private void handlePosition(float shiftMultiplier)
    {
        cameraSpeed = slowSpeed * shiftMultiplier;
        cameraTime = movementTime * shiftMultiplier;

        newPos += transform.forward * Input.GetAxis("Vertical") * cameraSpeed;
        newPos += transform.right * Input.GetAxis("Horizontal") * cameraSpeed;

        transform.position = Vector3.Lerp(transform.position, newPos, Time.deltaTime * cameraTime);

    }
}
