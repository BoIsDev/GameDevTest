using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnItem : MonoBehaviour
{
    public Dictionary<string, Queue<GameObject>> objPool = new Dictionary<string, Queue<GameObject>>();
    public Transform Holder;
    private static SpawnItem instance;
    public static SpawnItem Instance => instance;
    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    public GameObject CreateNewObj(GameObject obj, Vector3 pos, Quaternion quaternion)
    {
        GameObject newGo = Instantiate(obj, Holder);
        newGo.SetActive(true);
        newGo.transform.position = pos;
        newGo.name = obj.name;
        return newGo;
    }

    public GameObject GetObjItem(GameObject gameObject, Vector3 pos, Quaternion quaternion)
    {
        if (objPool.TryGetValue(gameObject.name, out Queue<GameObject> objList))
        {
            if (objList.Count == 0)
            {
                return CreateNewObj(gameObject, pos,quaternion);
            }
            else
            {
                GameObject newObjList = objList.Dequeue();
                newObjList.SetActive(true);
                newObjList.transform.position = pos;
                return newObjList;
            }
        }
        else
        {
            return CreateNewObj(gameObject, pos,quaternion);
        }
    }

    public void ReturnObjePool(GameObject gameObject)
    {
        if (objPool.TryGetValue(gameObject.name, out Queue<GameObject> objList))
        {
            objList.Enqueue(gameObject);
        }
        else
        {
            Queue<GameObject> newQueue = new Queue<GameObject>();
            newQueue.Enqueue(gameObject);
            objPool.Add(gameObject.name, newQueue);
        }
        gameObject.SetActive(false);

    }
}
