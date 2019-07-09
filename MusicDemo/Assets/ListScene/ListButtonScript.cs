using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;
using System.Collections;
using System;
public class ListButtonScript : MonoBehaviour
{
   

    GlobalBGM globalBGM;
    


    // Start is called before the first frame update
    void Start()
    {
        globalBGM = GameObject.Find("GlobalBGM").GetComponent<GlobalBGM>();
        if(globalBGM.getIsPlay == false)
        {
            globalBGM.startPlayBGM(5);
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    

    public void onSongClick(string songName)
    {

        //demo 0 zhuqu 1
        if (songName == "zhuqu")
        {
            GlobalControl.Instance.eventID = "zhuqu2";
            GlobalControl.Instance.chosedNumber = 1;
        }
        else if(songName == "demo")
        {
            GlobalControl.Instance.eventID = "testgipsy2";
            GlobalControl.Instance.chosedNumber = 0;
        }

        Application.LoadLevel("RhythmGame");

    }

    public void OnValueChanged(bool isOn)
    {
        GlobalControl.Instance.NozomiMode = isOn;
        print(isOn);
    }

    public void OnSettingClick(string sceneName)
    {
        Application.LoadLevel(sceneName);
    }
}
