using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using  SonicBloom.Koreo.Demos;

public class SettingScript : MonoBehaviour
{
    public Slider GameVolume;
    public Slider NoteVolume;
    public Slider NoteSpeed;
    GlobalBGM globalBGM;


    // Start is called before the first frame update
    void Start()
    {
        
        //获取BGM,note音量
        float gameVolume = globalBGM.getBGMVolume;
        float noteVolume = GlobalControl.Instance.noteVolume;
        float noteSpeed = GlobalControl.Instance.noteSpeed;
    
        //加载时BGM音量初始化
        GameVolume.value = gameVolume;
        NoteVolume.value = noteVolume;
        NoteSpeed.value = noteSpeed / 16;

    }
    void Awake()
    {
        globalBGM = GameObject.Find("GlobalBGM").GetComponent<GlobalBGM>();

        //noteCtl = GameObject.Find("NoteSpeed").GetComponent<NoteSpeed>();
    }
    // Update is called once per frame
    void Update()
    {
        //每帧检测bgm，note音量，note速度变化
        OnBGMVolumeSliderClick();
        
        OnNoteSpeedSliderClick();
        OnNoteVolumeSliderClick();
    }
    //BGM音量更改的滑条
    public void OnBGMVolumeSliderClick()
    {
        
        float newVolume = GameVolume.value;
        globalBGM.setVolume(newVolume);
        


    }
    //Note音量更改的滑条
    public void OnNoteVolumeSliderClick()
    {
        float newVolume = NoteVolume.value;
        GlobalControl.Instance.noteVolume = newVolume;

    }
    //NoteSpeed更改的滑条
    public void OnNoteSpeedSliderClick()
    {
        //设置notespeed在1-16之间
        float newSpeed = NoteSpeed.value * 16.0f;
        GlobalControl.Instance.noteSpeed = newSpeed;

    }

    //返回按钮
    public void OnBackClick(string sceneName)
    {
        Application.LoadLevel(sceneName);
    }

    //按下note volume复位button
    public void onNoteVolumeResetClick()
    {
        float reset = 0.5f;
        NoteVolume.value = reset;
        OnNoteSpeedSliderClick();

    }


    //按下game volume复位button
    public void OnVolumeResetClick()
    {
        float reset = 0.7f;
        GameVolume.value = reset;
        OnBGMVolumeSliderClick();
    }

    //按下notespeed复位butoon
    public void OnNoteSpeedResetClick()
    {
        
        float reset = 0.5f;
        NoteSpeed.value = reset;
        OnNoteSpeedSliderClick();

    }
  

}
