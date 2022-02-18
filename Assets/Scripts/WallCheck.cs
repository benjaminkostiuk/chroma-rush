using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallCheck : MonoBehaviour
{
    [SerializeField] private int wallCollisionIndex;
    private void OnTriggerEnter(Collider other)
    {
        UpdateWallCollider(other, true);
    }

    private void OnTriggerStay(Collider other)
    {
        UpdateWallCollider(other, true);
    }

    private void OnTriggerExit(Collider other)
    {
        UpdateWallCollider(other, false);
    }

    private void UpdateWallCollider(Collider other, bool value)
    {
        if (other.CompareTag("LevelBlock") || other.CompareTag("LevelBlockLight"))
        {
            Debug.Log("has wall for " + wallCollisionIndex);
            if (transform.parent.CompareTag("Player"))
            {
                transform.parent.GetComponent<PlayerMovement>().hasWallCollision[wallCollisionIndex] = value;
            }
            else if (transform.parent.CompareTag("Ghost"))
            {
                transform.parent.GetComponent<GhostMovement>().hasWallCollision[wallCollisionIndex] = value;
            }
        }
    }
}
