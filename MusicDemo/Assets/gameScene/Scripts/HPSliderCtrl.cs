using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class HPSliderCtrl : MonoBehaviour
{
    public Slider HPslider;
    bool gameOver = false;
    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void MissNote()
    {

        HPslider.value -= 0.05f;
        if(HPslider.value <= 0)
        {
            gameOver = true;
        }

    }

    public void NozomiModeInit()
    {
        //这个模式下初始血量为0
        HPslider.value = 0;
        gameOver = false;
    }
    //回血note点击回血，miss加倍
    public void HealNoteHitted(int type)
    {
        if(type == 0)
        {
            HPslider.value -= 0.05f;
        }
        else
        {
           
            HPslider.value += 0.05f;
        }
    }
}
