using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallCheck : MonoBehaviour
{
    [SerializeField] private int wallCollisionIndex;
    private void OnTriggerEnter(Collider other)
    {
       //Debug.Log("has wall for " + wallCollisionIndex);
        transform.parent.GetComponent<PlayerMovement>().hasWallCollision[wallCollisionIndex] = true;
    }

    private void OnTriggerExit(Collider other)
    {
        //Debug.Log("no wall for " + wallCollisionIndex);
        transform.parent.GetComponent<PlayerMovement>().hasWallCollision[wallCollisionIndex] = false;
    }
}
