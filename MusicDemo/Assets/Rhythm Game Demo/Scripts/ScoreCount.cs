using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreCount : MonoBehaviour
{
    // Start is called before the first frame update
    public int score=0;
    public Text scoreCountText;

    void Start()
    {
        
    }

    private void Awake()
    {
        scoreCountText.text = score.ToString();
    }
    public int CurrentScore() { return score;  }
    // Update is called once per frame
    void Update()
    {
        
    }

    public void  UpdateScore(int tempscore)
    {
        scoreCountText.text = tempscore.ToString();
        score = tempscore;
    }
    
}
