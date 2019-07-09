using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(AudioSource))]

public class NoteHitSound : MonoBehaviour
{

    public AudioClip perfectClick;
    public AudioClip greatClick;
    public AudioClip goodClick;
    public AudioClip badClick;
    public AudioClip missClick;
    AudioSource audioSource;
    
    // Start is called before the first frame update
    void Start()
    {
        //DontDestroyOnLoad(this.gameObject);
        audioSource = GetComponent<AudioSource>();
        ChangeNoteVolume(GlobalControl.Instance.noteVolume);
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
    public void OnNoteHitted(int type)
    {

    }
    //更改note点击音量
    public void ChangeNoteVolume(float newVolume)
    {
        audioSource.volume = newVolume;
    }

    public float getNoteVolume
    {
        get
        {
            return audioSource.volume;
        }
    }
    
}
