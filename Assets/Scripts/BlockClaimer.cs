using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockClaimer : MonoBehaviour
{
    [SerializeField] Material modifyMaterialDark;
    [SerializeField] Material modifyMaterialLight;

    private void OnTriggerEnter(Collider other)
    {
        bool hasChange = false;
        Material mat = other.GetComponent<Renderer>().material;
        if (other.CompareTag("LevelBlock") && !mat.name.Contains(modifyMaterialDark.name))
        {
            other.GetComponent<Renderer>().material = modifyMaterialDark;
            hasChange = true;
        } else if(other.CompareTag("LevelBlockLight") && !mat.name.Contains(modifyMaterialLight.name))
        {
            other.GetComponent<Renderer>().material = modifyMaterialLight;
            hasChange = true;
        }
        if (hasChange)
        {
            PlayerMovement player = GameObject.FindWithTag("Player").GetComponent<PlayerMovement>();
            if (transform.parent.CompareTag("Player"))
            {
                player.ClaimBlock();
            }
            else if (transform.parent.CompareTag("Ghost"))
            {
                player.UnclaimBlock();
            }
        }
    }
}
