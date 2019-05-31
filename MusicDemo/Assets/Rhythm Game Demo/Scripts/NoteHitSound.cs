using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(AudioSource))]

public class NoteHitSound : MonoBehaviour
{

    public AudioClip perfectClick;
    public AudioClip missClick;
    AudioSource audioSource;
    
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame

    public void OnNoteHitted(bool isHit)
    {
        if(isHit == true)
        {
            audioSource.Stop();
            audioSource.PlayOneShot(perfectClick);
        }
        else
        {
            audioSource.Stop();
            audioSource.PlayOneShot(missClick);
        }

    }
    
}
