using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartGame : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    /*
    IEnumerator test()
    {
        print("Before " + Time.time);
        yield return new WaitForSeconds(3);    //注意等待时间的写法
        print("After " + Time.time);
    }
    */

    public void OnStartGame(string sceneName)
    {
        Application.LoadLevel(sceneName);
        
    }

}
