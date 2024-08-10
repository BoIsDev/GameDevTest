using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockColor : MonoBehaviour
{
    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Blocks"))
        {
            Debug.Log("Placed");
            GameManager.Instance.isPlaced = true;
        }
    }
    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Blocks"))
        {
            Debug.Log("Not Placed");
            GameManager.Instance.isPlaced = false;
        }
    }
    
}
