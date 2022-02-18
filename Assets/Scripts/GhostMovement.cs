using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private Transform movePoint;
    [SerializeField] private Transform playerPosition;
    [SerializeField] private GameObject bottomTrigger;

    [SerializeField] private Transform avatar;
    [SerializeField] private Animator avatarAnimator;
    [SerializeField] private Material ghostAvatarMaterial;
    private AvatarState avatarState = AvatarState.IDLE;

    [SerializeField] private LayerMask levelMask;

    private float horizontal;
    private float vertical;

    [SerializeField] private float fadeSpeed = 0.15f;
    [SerializeField] private float maxFadeIn = 0.30588f;    // 78

    // front (z = 1), back (z = -1), right (x = 1), left (x = -1)
    public bool[] hasWallCollision = { false, false, false, false };

    private Queue<PlayerSnapshot> playerInputs = new Queue<PlayerSnapshot>();
    [SerializeField] private float delay = 5;
    private float timeElapsedSinceReset = 0;
    private float minPitch = 1f;
    private float maxPitch = 1.7f;
    private float pitchIncreaseLevel;
    [SerializeField] private float pitchIncreaseTimeMargin = 2;
    private float previousPitchTimeLevel = 0;

    [SerializeField] private AudioSource musicAudio;

    public bool victory = false;

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
        Color ghostColor = ghostAvatarMaterial.color;
        ghostColor.a = 0;
        ghostAvatarMaterial.color = ghostColor;
        // Diable bottom trigger
        bottomTrigger.SetActive(false);

        // Compute increase level
        pitchIncreaseLevel = (maxPitch - minPitch) / (delay / pitchIncreaseTimeMargin);
        timeElapsedSinceReset = 0;

        musicAudio.pitch = minPitch;
    }

    // Update is called once per frame
    void Update()
    {
        updateMusicPitch();

        // Wait for X seconds
        if (Time.timeSinceLevelLoad < delay)
        {
            return;
        }

        // Do nothing but fade out on victory
        if (victory)
        {
            bottomTrigger.SetActive(false);
            FadeOut();
            return;
        }

        bottomTrigger.SetActive(true);
        
        // Fade out if too close to player
        if(Vector3.Distance(transform.position, playerPosition.position) < 1f)
        {
            FadeOut();
        } else
        {
            FadeIn();          
        }

        if (Vector3.Distance(transform.position, movePoint.position) == 0)
        {
            avatarState = AvatarState.IDLE;
        }

        transform.position = Vector3.MoveTowards(transform.position, movePoint.position, moveSpeed * Time.deltaTime);
        if (IsOffEdge() && Vector3.Distance(transform.position, movePoint.position) <= .05f)
        {
            if (horizontal > 0)
            {
                transform.Rotate(0, 0, -90);
                movePoint.position += transform.right;
            }
            else if (horizontal < 0)
            {
                transform.Rotate(0, 0, 90);
                movePoint.position -= transform.right;
            }
            else if (vertical > 0)
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
            // Only dequeue if behind X seconds.
            if(playerInputs.Count > 0 && playerInputs.Peek().time <= Time.fixedTime - delay)
            {
                PlayerSnapshot ps = playerInputs.Dequeue();
                horizontal = ps.horizontal;
                vertical = ps.vertical;
            } else
            {
                horizontal = 0;
                vertical = 0;
            }
           
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
        if (isHit)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(transform.position + transform.up * (-1) * hit.distance, new Vector3(0.4f, 0.4f, 0.4f));
        }
        else
        {
            Gizmos.color = Color.green;
            Gizmos.DrawRay(transform.position, transform.up * (-1) * maxDistance);
        }
    }

    private bool IsOffEdge()
    {
        return !Physics.BoxCast(transform.position, new Vector3(0.2f, 0.2f, 0.2f), transform.up * (-1), transform.rotation, 0.6f, levelMask);
    }

    private void FadeIn()
    {
        if(ghostAvatarMaterial.color.a >= maxFadeIn)
        {
            return;
        }

        Color old = ghostAvatarMaterial.color;
        old.a += fadeSpeed * Time.deltaTime;
        ghostAvatarMaterial.color = old;
    }
    
    private void FadeOut()
    {
        if (ghostAvatarMaterial.color.a <= 0)
        {
            return;
        }
        Color old = ghostAvatarMaterial.color;
        old.a -= fadeSpeed * Time.deltaTime;
        old.a = Mathf.Max(old.a, 0);
        ghostAvatarMaterial.color = old;
    }
    
    public void addPlayerSnapshot(float horizontal, float vertical, float time)
    {
        playerInputs.Enqueue(new PlayerSnapshot(horizontal, vertical, time));
    }

    private void updateMusicPitch()
    {
        if(victory)
        {
            musicAudio.pitch = minPitch;
            return;
        }

        if (playerInputs.Count == 0)
        {
            timeElapsedSinceReset = 0;  // Reset timer count
            musicAudio.pitch = minPitch;
            previousPitchTimeLevel = 0;
        }

        timeElapsedSinceReset += Time.deltaTime;

        // Do nothing if already going too fast
        if (musicAudio.pitch >= maxPitch)
        {
            return;
        }

        if(timeElapsedSinceReset >= previousPitchTimeLevel + pitchIncreaseTimeMargin)
        {
            musicAudio.pitch += pitchIncreaseLevel;
            previousPitchTimeLevel = previousPitchTimeLevel + pitchIncreaseTimeMargin;
        }
    }
}
