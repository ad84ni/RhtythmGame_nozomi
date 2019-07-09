using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class OnTextChange : MonoBehaviour
{
     Text gameVolumeNum;
     Text noteVolumeNum;
     Text noteSpeedNum;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void Awake()
    {
        gameVolumeNum = GameObject.Find("GameVolumeNum").GetComponent<Text>();
        noteVolumeNum = GameObject.Find("NoteVolumeNum").GetComponent<Text>();
        noteSpeedNum = GameObject.Find("NoteSpeedNum").GetComponent<Text>();
    }
    //BGM滑条
    public void OnBGMSliderChange(float value)
    {
        gameVolumeNum.text = (10 * value).ToString("0.0");
    }
    //note音量滑条
    public void OnNoteVolumeSliderChange(float value)
    {
        noteVolumeNum.text = (10 * value).ToString("0.0");
    }

    //note speed滑条
    public void OnNoteSpeedSliderCHange(float value)
    {
        noteSpeedNum.text = (16 * value).ToString("0.0");
    }
}
