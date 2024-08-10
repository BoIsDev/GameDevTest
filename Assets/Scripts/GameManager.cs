using UnityEngine;
using UnityEngine.UI;

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
        if (pendingObj != null)
        {
            Destroy(pendingObj);
        }
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
}
