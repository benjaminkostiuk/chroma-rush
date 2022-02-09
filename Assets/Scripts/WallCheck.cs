using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallCheck : MonoBehaviour
{
    [SerializeField] private int wallCollisionIndex;
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("LevelBlock"))
        {
            //Debug.Log("has wall for " + wallCollisionIndex);
            if(transform.parent.CompareTag("Player")) {
                transform.parent.GetComponent<PlayerMovement>().hasWallCollision[wallCollisionIndex] = true;
            } else if(transform.parent.CompareTag("Ghost"))
            {
                transform.parent.GetComponent<GhostMovement>().hasWallCollision[wallCollisionIndex] = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("LevelBlock"))
        {
            //Debug.Log("no wall for " + wallCollisionIndex);
            if (transform.parent.CompareTag("Player"))
            {
                transform.parent.GetComponent<PlayerMovement>().hasWallCollision[wallCollisionIndex] = false;
            }
            else if (transform.parent.CompareTag("Ghost"))
            {
                transform.parent.GetComponent<GhostMovement>().hasWallCollision[wallCollisionIndex] = false;
            }
        }
    }
}
