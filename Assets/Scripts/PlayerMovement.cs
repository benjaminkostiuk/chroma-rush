using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private Transform movePoint;
    [SerializeField] private GhostMovement ghost;

    [SerializeField] private Transform avatar;
    [SerializeField] private Animator avatarAnimator;
    private AvatarState avatarState = AvatarState.IDLE;

    [SerializeField] private LayerMask levelMask;

    private float horizontal;
    private float vertical;

    // front (z = 1), back (z = -1), right (x = 1), left (x = -1)
    public bool[] hasWallCollision = { false, false, false, false };

    /**
     * Animation state for the player avatar
     */
    private enum AvatarState
    {
        IDLE, 
        RUNNING,
        CELEBRATING
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if(Vector3.Distance(transform.position, movePoint.position) == 0)
        {
            avatarState = AvatarState.IDLE;
        }

        transform.position = Vector3.MoveTowards(transform.position, movePoint.position, moveSpeed * Time.deltaTime);
        if(IsOffEdge() && Vector3.Distance(transform.position, movePoint.position) <= .05f)
        {
            if(horizontal > 0)
            {
                transform.Rotate(0, 0, -90);
                
                movePoint.position += transform.right;
            } else if(horizontal < 0)
            {
                transform.Rotate(0, 0, 90);
                movePoint.position -= transform.right;
            } else if (vertical > 0)
            {
                transform.Rotate(90, 0, 0);
                movePoint.position += transform.forward;
            }
            else if (vertical < 0)
            {
                transform.Rotate(-90, 0, 0);
                movePoint.position -= transform.forward;
            }
        }
        else if (Vector3.Distance(transform.position, movePoint.position) <= .01f)
        {   
            horizontal = Input.GetAxisRaw("Horizontal");
            vertical = Input.GetAxisRaw("Vertical");
            // Add input snapshot for ghost
            ghost.addPlayerSnapshot(horizontal, vertical);

            if (Mathf.Abs(horizontal) > 0 || Mathf.Abs(vertical) > 0)
            {
                avatarState = AvatarState.RUNNING;
            }

            if (Mathf.Abs(horizontal) == 1f)
            {
                vertical = 0;
                if (hasWallCollision[2] && horizontal > 0)
                {
                    transform.Rotate(0, 0, 90);
                }
                if (hasWallCollision[3] && horizontal < 0)
                {
                    transform.Rotate(0, 0, -90);
                }
                movePoint.position += transform.right * horizontal;     // X Axis

            }
            else if (Mathf.Abs(vertical) == 1f)
            {
                horizontal = 0;

                if (hasWallCollision[0] && vertical > 0)
                {
                    transform.Rotate(-90, 0, 0);
                }
                if (hasWallCollision[1] && vertical < 0)
                {
                    transform.Rotate(90, 0, 0);
                }
                movePoint.position += transform.forward * vertical;     // Z axis
            }


            if (horizontal > 0) // Right
            {
                avatar.localRotation = Quaternion.AngleAxis(90, Vector3.up);
            }
            else if (horizontal < 0)   // Left
            {
                avatar.localRotation = Quaternion.AngleAxis(-90, Vector3.up);

            }
            else if (vertical > 0)     // Forward
            {
                avatar.localRotation = Quaternion.AngleAxis(0, Vector3.up);
            }
            else if (vertical < 0)     // Backwards
            {
                avatar.localRotation = Quaternion.AngleAxis(-180, Vector3.up);
            }
        }
        UpdateAvatarAnimationState();
    }

    private void UpdateAvatarAnimationState()
    {
        avatarAnimator.SetInteger("state", (int)avatarState);
    }

    private void OnDrawGizmos()
    {
        float maxDistance = 0.6f;
        RaycastHit hit;

        bool isHit = Physics.BoxCast(transform.position, new Vector3(0.2f, 0.2f, 0.2f), transform.up * (-1), out hit, transform.rotation, maxDistance);
        if(isHit)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(transform.position + transform.up * (-1) * hit.distance, new Vector3(0.4f, 0.4f, 0.4f));
        } else
        {
            Gizmos.color = Color.green;
            Gizmos.DrawRay(transform.position, transform.up * (-1) * maxDistance);
        }
    }

    private bool IsOffEdge()
    {
        return !Physics.BoxCast(transform.position, new Vector3(0.2f, 0.2f, 0.2f), transform.up * (-1), transform.rotation, 0.6f, levelMask);
    }
}
