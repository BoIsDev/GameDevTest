using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
[System.Serializable]
//Get the data of the gameObject
public class GameObjectData
{
    public string type;
    public Vector3 position;
    public Quaternion rotation;
}

[System.Serializable]
//Get the data of the game
public class GameData
{
    public List<GameObjectData> objects = new List<GameObjectData>();
    public int redCount;
    public int blueCount;
    public int yellowCount;
}

public class GridManager : MonoBehaviour
{
    public float cellSize = 1f;
    public LayerMask raycastLayerMask;
    public LayerMask raycastBackground;
    public GameObject[] objects;
    public int redCount, blueCount, yellowCount = 10;
    public GameObject selectedObject; // Selected object
    [SerializeField] private Text txtRed, txtBlue, txtYellow;
    private Vector3 pos;
    void Start()
    {
        LoadData();
        UpdateUI();
    }

    void Update()
    {
        // Check if the user clicks the left mouse button to select an object
        if (Input.GetMouseButtonDown(0))
        {
            SelectObject();
        }

        // Release the left mouse button to drop the object
        if (Input.GetMouseButtonUp(0))
        {
            DropObject();
            Audio.Instance.PlaySFX(Audio.Instance.PickUp);

        }

        // Check if the user clicks the right mouse button to handle the object
        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            HandleRightClick(ray);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SaveAndQuit();
        }
    }

    void FixedUpdate()
    {
        // Check if the user holds the left mouse button to drag the object
        if (selectedObject != null && Input.GetMouseButton(0))
        {
            DragObject();
        }
    }
    public void CreateBox(int index)
    {
        if (selectedObject != null) return;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, raycastBackground))
        {
            pos = hit.point;
            selectedObject = Instantiate(objects[index], pos, Quaternion.identity);
            selectedObject.name = objects[index].name;
            UpdateColor(index, -1);
        }
        else
        {
            Debug.Log("Raycast did not hit");
        }
    }

    private void HandleRightClick(Ray ray)
    {
        RaycastHit hit;
        // Check if the object is a block and destroy it
        if (Physics.Raycast(ray, out hit, 1000, raycastLayerMask))
        {
            if (hit.collider != null && hit.collider.gameObject.CompareTag("Blocks"))
            {
                Audio.Instance.PlaySFX(Audio.Instance.Get);
                Destroy(hit.collider.gameObject);
                hit.collider.gameObject.SetActive(false);
                UpdateColorByName(hit.collider.transform.name, 1);
            }
        }
    }

    // Method to select an object
    void SelectObject()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, raycastLayerMask))
        {
            selectedObject = hit.collider.gameObject;
        }
        else
        {
            Debug.Log("No object selected");
        }
    }

    void DragObject()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (selectedObject != null)
        {
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, raycastBackground))
            {
                Vector3 worldPosition = hit.point;
                selectedObject.transform.position = new Vector3(worldPosition.x, worldPosition.y, worldPosition.z);

            }
            else
            {
                Debug.Log("Raycast did not hit");
            }
        }
    }
    void DropObject()
    {
        if (selectedObject != null)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, raycastBackground))
            {
                if (hit.transform.CompareTag("Blocks"))
                {
                    Vector3 spawnPosition = new Vector3(Mathf.RoundToInt(hit.point.x + hit.normal.x / 2), Mathf.RoundToInt(hit.point.y + hit.normal.y / 2), Mathf.RoundToInt(hit.point.z + hit.normal.z / 2));
                    selectedObject.transform.position = spawnPosition;
                    selectedObject = null;
                    Debug.Log("Placed");
                }
                else if (hit.transform.CompareTag("Background"))
                {
                    Vector3 spawnPosition = new Vector3(Mathf.RoundToInt(hit.point.x), Mathf.RoundToInt(hit.point.y), Mathf.RoundToInt(hit.point.z));
                    selectedObject.transform.position = spawnPosition;
                    selectedObject = null;
                }
            }
        }
    }
    public void UpdateUI()
    {
        txtBlue.text = blueCount.ToString();
        txtRed.text = redCount.ToString();
        txtYellow.text = yellowCount.ToString();
    }

    public bool CheckCount(int index)
    {
        switch (index)
        {
            case 0: return blueCount > 0;
            case 1: return redCount > 0;
            case 2: return yellowCount > 0;
            default: return false;
        }
    }
    public void UpdateColor(int index, int amount)
    {
        switch (index)
        {
            case 0: if (blueCount > 0) blueCount += amount; break;
            case 1: redCount += amount; break;
            case 2: yellowCount += amount; break;
        }
        UpdateUI();
    }

    // Get the color name and amount to update
    private void UpdateColorByName(string name, int amount)
    {
        switch (name)
        {
            case "Blue": UpdateColor(0, amount); break;
            case "Red": UpdateColor(1, amount); break;
            case "Yellow": UpdateColor(2, amount); break;
        }
    }
    private void SaveData()
    {
        GameData gameData = new GameData
        {
            redCount = redCount,
            blueCount = blueCount,
            yellowCount = yellowCount
        };

        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Blocks"))
        {
            GameObjectData data = new GameObjectData
            {
                type = obj.name,
                position = obj.transform.position,
                rotation = obj.transform.rotation
            };
            gameData.objects.Add(data);
        }

        string json = JsonUtility.ToJson(gameData, true);
        string path = Application.persistentDataPath + "/gameData.json";
        File.WriteAllText(path, json);
    }
    private void LoadData()
    {
        string path = Application.persistentDataPath + "/gameData.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            GameData gameData = JsonUtility.FromJson<GameData>(json);

            redCount = gameData.redCount;
            blueCount = gameData.blueCount;
            yellowCount = gameData.yellowCount;

            foreach (GameObjectData data in gameData.objects)
            {
                GameObject obj = Instantiate(GetPrefabByName(data.type), data.position, data.rotation);
                obj.tag = "Blocks";
                obj.name = data.type;
            }
        }
    }
        private GameObject GetPrefabByName(string name)
    {
        foreach (GameObject obj in objects)
        {
            if (obj.name == name)
            {
                return obj;
            }
        }
        return null;
    }
    private void SaveAndQuit()
    {
        SaveData();
        Application.Quit();
    }
}
