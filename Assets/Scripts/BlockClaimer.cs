using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockClaimer : MonoBehaviour
{
    [SerializeField] Material modifyMaterial;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("LevelBlock"))
        {
            other.GetComponent<Renderer>().material = modifyMaterial;
        }
    }
}
