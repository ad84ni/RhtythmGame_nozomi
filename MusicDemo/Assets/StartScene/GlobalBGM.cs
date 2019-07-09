using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalBGM : MonoBehaviour
{
    public AudioSource globalBGM;
    float bgmvolume;
    bool isPlay;


    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
        globalBGM.volume = 0.7f;
        bgmvolume = globalBGM.volume;
        isPlay = true;
    }

    // Update is called once per frame

        //返回全局BGM音量
    public float getBGMVolume
    {
        get
        {
            return globalBGM.volume;
        }
    }
    public bool getIsPlay
    {
        get
        {
            return isPlay;
        }
    }


    void Update()
    {
        
    }
    public void setVolume(float newVolume)
    {
        globalBGM.volume = newVolume;
    }

    //停止播放BGM
    public void stopPlayBGM()
    {
        if (isPlay == false) return;

        globalBGM.Stop();
        isPlay = false;
    }

    //播放bgm
    public void startPlayBGM(ulong delay)
    {
        if (isPlay == true) return;

        globalBGM.Play(delay);
        isPlay = true;
    }

}
