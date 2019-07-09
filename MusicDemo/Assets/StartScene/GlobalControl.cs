using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SonicBloom.Koreo;
public class GlobalControl : MonoBehaviour
{
    //单例的object
    public static GlobalControl Instance;

    //notes计数器 0 miss, 1 bad, 2 good 3 great 4 perfect
    public int[] notes = new int[5];
    public int score =0;
    public int maxCombo=0;

    public float noteVolume = 8.0f;
    public float noteSpeed = 8.0f;

    //游戏初始化时装载的歌曲
    public Koreography[] songs;
    public AudioClip[] audios;
    //选择的song数字，1是demo，2是竹取
    public int chosedNumber = 0;
    public string eventID;

    //选择的模式 false代表普通模式，true代表nozomi模式
    public bool NozomiMode=false;


    

    // Start is called before the first frame update
    void Start()
    {
        
    }



    //初始化
    private void Awake()
    {
        if (Instance == null)
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
        }
        else if (Instance != null)
        {
            Destroy(gameObject);
        }
    }

    

}


