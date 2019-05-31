using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    //返回按钮
    public void OnBackClick(string sceneName)
    {
        Application.LoadLevel(sceneName);
    }
    //按下speed设定button
    public void OnSpeedSetClick()
    {

    }
    //按下speed复位button
    public void onSpeedResetClick()
    {

    }

    //按下volume设定button
    public void OnVolumeSetClick()
    {

    }
    //按下volume复位button
    public void OnVolumeResetClick()
    {

    }

}
