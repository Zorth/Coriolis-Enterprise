using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float slowSpeed, slowRotateSpeed, movementTime, fastMultiplier, minHeight, maxHeight, maxDepth, minDepth;
    private Transform cameraTransform;

    private float cameraSpeed, cameraRotateSpeed, cameraTime, zoomLevel, newZoom;
    public GameObject worldGenerator;
    private float worldSize, worldRadius;
    private float tileRadius;
    private Dictionary<string, Vector3> offsets;

    private Vector3 newPos;
    private Quaternion newRot;

    // Start is called before the first frame update
    void Start()
    {
        newPos = transform.position;
        cameraTransform = Camera.main.transform;

        worldSize = worldGenerator.GetComponent<WorldGenerator>().worldSize;
        offsets = worldGenerator.GetComponent<WorldGenerator>().offsets;
        tileRadius = WorldGenerator.tileRadius;
        worldRadius = worldSize * tileRadius * 2;

        ResetZoomRot();
    }
    // Update is called once per frame
    void Update()
    {
        HandleInput();
        LoopTeleport();

        UpdateRenderDistance();
    }

    void HandleInput()
    {
        float cameraSpeedMultiplier = ((fastMultiplier - 1f) * Input.GetAxis("Fast") + 1f) * (zoomLevel/2 + .5f);

        HandlePosition(cameraSpeedMultiplier);
        HandleRotation(cameraSpeedMultiplier);
        HandleZoom(cameraSpeedMultiplier);

        HandleReset();
    }

    private void HandleReset()
    {
        if (Input.GetButton("Reset"))
        {
            ResetZoomRot();
        }
    }

    private void ResetZoomRot()
    {
        newZoom = 0.5f;
        newRot = Quaternion.identity;
    }


    private void HandleRotation(float shiftMultiplier)
    {
        cameraRotateSpeed = slowRotateSpeed * shiftMultiplier;

        newRot *= Quaternion.AngleAxis(Input.GetAxis("Rotate") * cameraRotateSpeed, Vector3.up);

        transform.rotation = Quaternion.Lerp(transform.rotation, newRot, Time.deltaTime * cameraRotateSpeed * 10f);
    }

    private void HandleZoom(float shiftMultiplier)
    {
        newZoom = Mathf.Clamp(newZoom - Input.GetAxis("Mouse ScrollWheel") * shiftMultiplier, 0f, 1f);
        zoomLevel = zoomLevel + (newZoom - zoomLevel) * Time.deltaTime * movementTime;

        cameraTransform.localPosition = new Vector3(0, minHeight + (maxHeight - minHeight) * zoomLevel, -(minDepth + (maxDepth - minDepth) * (1f - zoomLevel)));

    }

    private void HandlePosition(float shiftMultiplier)
    {
        cameraSpeed = slowSpeed * shiftMultiplier;
        cameraTime = movementTime * shiftMultiplier;

        newPos += transform.forward * Input.GetAxis("Vertical") * cameraSpeed;
        newPos += transform.right * Input.GetAxis("Horizontal") * cameraSpeed;

        transform.position = Vector3.Lerp(transform.position, newPos, Time.deltaTime * cameraTime);

    }

    private void LoopTeleport()
    {
        while (transform.position.magnitude > ((worldRadius+1) * tileRadius))
        {
            Vector3 tpDist;
            if (Vector3.SignedAngle(transform.position, Vector3.forward, Vector3.up) < -120f)
            {
                tpDist = offsets["SE"];
            }
            else if(Vector3.SignedAngle(transform.position, Vector3.forward, Vector3.up) < -60f)
            {
                tpDist = offsets["E"];
            }
            else if(Vector3.SignedAngle(transform.position, Vector3.forward, Vector3.up) < 0f)
            {
                tpDist = offsets["NE"];
            }
            else if(Vector3.SignedAngle(transform.position, Vector3.forward, Vector3.up) < 60f)
            {
                tpDist = offsets["NW"];
            }
            else if(Vector3.SignedAngle(transform.position, Vector3.forward, Vector3.up) < 120f)
            {
                tpDist = offsets["W"];
            }
            else
            {
                tpDist = offsets["SW"];
            }

            transform.position -= tpDist;
            newPos -= tpDist;

        }
    }

    private void UpdateRenderDistance()
    {

    }
}
