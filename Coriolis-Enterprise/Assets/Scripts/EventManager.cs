using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Xml;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Diagnostics;

public class EventSystem : MonoBehaviour
{
    public Dictionary<int, GameObject> selectedGOs = new Dictionary<int, GameObject>();

    private RaycastHit selectHit;
    private bool dragSelect;
    private float minDragDist = 40;

    private Vector3 clickPos;
    private Vector3[] selectionBoxVerts;
    private Rect selectionRect;
    private Texture2D selectBoxTexture;
    public Color selectionColor;

    // Start is called before the first frame update
    void Start()
    {
        setupSelectionVars();
    }

    private void setupSelectionVars()
    {
        dragSelect = false;
        selectionBoxVerts = new Vector3[2];
        selectBoxTexture = new Texture2D(1, 1);
        selectBoxTexture.SetPixel(0, 0, selectionColor);
        selectBoxTexture.Apply();
    }

    // Update is called once per frame
    void Update()
    {
        handleSelection();
    }

    private void handleSelection()
    {
        if (Input.GetButtonDown("Cancel"))
            deselectAll();

        if (Input.GetButtonDown("Select"))
            clickPos = Input.mousePosition;

        if (Input.GetButton("Select") && (clickPos - Input.mousePosition).magnitude > minDragDist)
            dragSelect = true;

        if (Input.GetButtonUp("Select"))
        {
            if (!Input.GetButton("Fast") && !Input.GetButton("Deselect"))
                deselectAll();

            if (dragSelect)
            {
                handleDragSelect();
            }
            else
            {
                handleClickSelect();
            }
            dragSelect = false;
        }

    }

    private void handleDragSelect()
    {
        selectionBoxVerts = new Vector3[3];
        RaycastHit hit1, hit2, hit3;
        Ray ray1 = Camera.main.ScreenPointToRay(clickPos);
        Ray ray2 = Camera.main.ScreenPointToRay(Input.mousePosition);
        Ray ray3 = Camera.main.ScreenPointToRay(new Vector3(clickPos.x, Input.mousePosition.y, 0));
        if (Physics.Raycast(ray1, out hit1, 1000.0f, LayerMask.GetMask("Terrain")) &&
            Physics.Raycast(ray2, out hit2, 1000.0f, LayerMask.GetMask("Terrain")) &&
            Physics.Raycast(ray3, out hit3, 1000.0f, LayerMask.GetMask("Terrain")))
        {
            selectionBoxVerts[0] = hit1.point;
            selectionBoxVerts[1] = hit2.point;
            selectionBoxVerts[2] = hit3.point;

            BoxCollider col = gameObject.AddComponent<BoxCollider>();

            Vector3 diagonal = selectionBoxVerts[1] - selectionBoxVerts[0];
            float angle = -Vector3.SignedAngle(selectionBoxVerts[2] - selectionBoxVerts[1], Vector3.right, Vector3.up);

            Vector3 scale;

            scale.x = Vector3.Distance(selectionBoxVerts[1], selectionBoxVerts[2]);
            scale.y = 10;
            scale.z = Vector3.Distance(selectionBoxVerts[0], selectionBoxVerts[2]);

            col.transform.position = Vector3.Lerp(selectionBoxVerts[0], selectionBoxVerts[1], 0.5f) + Vector3.up * 5;
            col.transform.localScale = scale;
            col.transform.rotation = Quaternion.Euler(0, angle, 0);

            col.isTrigger = true;
            col.enabled = true;
            col.includeLayers = LayerMask.GetMask("Units");

            Destroy(col, 0.2f);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (Input.GetButton("Deselect"))
            deselect(other.gameObject.GetInstanceID());
        else
            addSelected(other.gameObject);

    }

    private void handleClickSelect()
    {
        Ray ray = Camera.main.ScreenPointToRay(clickPos);

        if (Physics.Raycast(ray, out selectHit, 1000.0f, LayerMask.GetMask("Units")))
        {
            if (Input.GetButton("Deselect"))
                deselect(selectHit.transform.gameObject.GetInstanceID());
            else
                addSelected(selectHit.transform.gameObject);

        }
    }

    private void OnGUI()
    {
        if (dragSelect)
        {
            selectionRect.min = clickPos;
            selectionRect.max = Input.mousePosition;
            selectionRect.yMin = Screen.height - selectionRect.yMin;
            selectionRect.yMax = Screen.height - selectionRect.yMax;

            GUI.DrawTexture(selectionRect, selectBoxTexture, ScaleMode.StretchToFill, true);
        }
    }

    public void addSelected(GameObject go)
    {
        int id = go.GetInstanceID();

        if (!selectedGOs.ContainsKey(id))
        {
            selectedGOs.Add(id, go);
            go.AddComponent<SelectionComponent>();
        }
    }

    public void deselect(int id)
    {
        if (selectedGOs.ContainsKey(id))
        {
            Destroy(selectedGOs[id].GetComponent<SelectionComponent>());
            selectedGOs.Remove(id);
        }
    }

    public void deselectAll()
    {
        foreach (KeyValuePair<int, GameObject> pair in selectedGOs)
        {
            if (pair.Value != null)
            {
                Destroy(selectedGOs[pair.Key].GetComponent<SelectionComponent>());
            }
        }
        selectedGOs.Clear();
    }
}
