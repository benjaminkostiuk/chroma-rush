using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSnapshot
{
    public float horizontal;
    public float vertical;
    public float time;

    public PlayerSnapshot(float horizontal, float vertical, float time)
    {
        this.horizontal = horizontal;
        this.vertical = vertical;
        this.time = time;
    }
}
