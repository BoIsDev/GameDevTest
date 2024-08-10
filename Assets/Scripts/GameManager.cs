using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
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

public class GameManager : MonoBehaviour
{
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private LayerMask layerBlock;
    [SerializeField] private Text txtRed, txtBlue, txtYellow;
    private RaycastHit hit;
    private Vector3 pos;
    public GameObject[] objects;
    public GameObject pendingObj;
    public int redCount, blueCount, yellowCount = 10;
    public bool isPlaced;
    private static GameManager instance;
    public static GameManager Instance => instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        // Load saved data
        LoadData();
        UpdateUI();
    }

    void Update()
    {
        if (pendingObj != null)
        {
            pendingObj.transform.position = pos;

            if (Input.GetMouseButtonDown(0) && !isPlaced)
            {

                PlaceObject();
                UpdateUI();
            }
           
        }
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, 1000, layerMask))
        {
            pos = hit.point;
            if (Input.GetMouseButtonDown(1))
            {
                HandleRightClick(ray);
            }
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SaveData();
            Application.Quit();
        }
    }

    private void HandleRightClick(Ray ray)
    {
        // Check if the object is a block
        if (Physics.Raycast(ray, out hit, 1000, layerBlock))
        {
            if (hit.collider != null && hit.collider.gameObject.CompareTag("Blocks"))
            {
                SpawnItem.Instance.ReturnObjePool(hit.collider.gameObject);
                hit.collider.gameObject.SetActive(false);
                UpdateColorByName(hit.collider.transform.name, 1);
            }
        }
    }

    // Select the object by index
    public void SelectObject(int index)
    {
        if (pendingObj != null) return;
        if (!CheckCount(index)) return;
        pendingObj = SpawnItem.Instance.GetObjItem(objects[index], pos, Quaternion.identity);
        UpdateColor(index, -1);
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

    void PlaceObject()
    {
        pendingObj = null;
    }

    public void UpdateUI()
    {
        txtBlue.text = blueCount.ToString();
        txtRed.text = redCount.ToString();
        txtYellow.text = yellowCount.ToString();
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
}
