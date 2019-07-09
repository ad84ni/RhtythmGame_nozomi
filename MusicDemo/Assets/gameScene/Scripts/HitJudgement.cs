using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HitJudgement : MonoBehaviour
{
    public Text hitJudgement;
    string[] hitType = new string[5];
    // Start is called before the first frame update
    void Start()
    {
        hitType[0] = "MISS";
        hitType[1] = "BAD";
        hitType[2] = "GOOD";
        hitType[3] = "GREAT";
        hitType[4] = "PERFECT";

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeHitJudgementText(int type)
    {
        hitJudgement.text = hitType[type].ToString();
    }


}
