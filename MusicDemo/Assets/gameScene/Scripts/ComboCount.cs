using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ComboCount : MonoBehaviour
{
    // Start is called before the first frame update
    private int comboNum = 0;
    public Text comboNumText;
    //最大combo数
    public int maxCombo = 0;
    //notes类型数组 从0-4分别是 miss bad good great perfect
    int[] notes = new int[5];
    void Start()
    {
        
    }

    private void Awake()
    {
        
        setMaxCombo(0);
    }


    public int getComboNum
    {
        get
        {
            return comboNum;
        }
    }

    public int[] getNotes
    {
        get
        {
            return notes;
        }
    }

    public void UpdateComboNum(int type)
    {
        //perfect与great计为连击
        if(type == 3 || type == 4)
        {
            comboNum += 1;
            setMaxCombo(comboNum);
        }
        else
        {
            setMaxCombo(comboNum);
            comboNum = 0;
        }
        comboNumText.text = comboNum.ToString();
        notes[type] += 1;
    }
    public void ResetCombo()
    {
        //重置combo数
        comboNum = 0;
        
    }
    //重置当前歌曲    
    public void ResetComboAndNote()
    {
        ResetCombo();
        for(int i=0;i<5;i++)
        {
            notes[i] = 0;
        }
    }

    public int getMaxCombo
    {
        get
        {
            return maxCombo;
        }
    }

    public void setMaxCombo(int combo)
    {
        if(combo > maxCombo)
        {
            maxCombo = combo;

        }
        GlobalControl.Instance.maxCombo = maxCombo;
    }




}
