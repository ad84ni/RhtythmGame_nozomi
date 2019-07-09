using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteSpeed : MonoBehaviour
{

    public float NoteSpeedValue = 8.0f;
    // Start is called before the first frame update
    void Start()
    {
        //DontDestroyOnLoad(this.gameObject);
        setNoteSpeed(GlobalControl.Instance.noteSpeed);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setNoteSpeed(float newSpeed)
    {
        NoteSpeedValue = newSpeed;
    }

    public float getNoteSpeed
    {
        get
        {
            return NoteSpeedValue;
        }
    }
    
    

}
