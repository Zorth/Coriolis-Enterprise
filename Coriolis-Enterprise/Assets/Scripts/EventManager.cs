using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventSystem : MonoBehaviour
{
    public Dictionary<int, GameObject> selectedGOs = new Dictionary<int, GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
        Destroy(selectedGOs[id].GetComponent<SelectionComponent>());
        selectedGOs.Remove(id);
    }

    public void deselectAll()
    {
        foreach(KeyValuePair<int, GameObject> pair in selectedGOs)
        {
            if (pair.Value != null)
            {
                Destroy(selectedGOs[pair.Key].GetComponent<SelectionComponent>()); 
            }
        }
        selectedGOs.Clear();
    }
}
