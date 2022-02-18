using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicSpeedup : MonoBehaviour
{
    AudioSource audioSource;
    [SerializeField] private float maxPitch = 1.5f;
    [SerializeField] private float minPitch = 1f;

    private void Start()
    {
        this.audioSource = GetComponent<AudioSource>();
        audioSource.pitch = minPitch;
    }

    // Update is called once per frame
    void Update()
    {
        //if()

        if(audioSource.pitch < maxPitch)
        {
            audioSource.pitch += 0.1f * Time.deltaTime;
        }
    }
}
