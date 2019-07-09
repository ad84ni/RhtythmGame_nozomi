using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CompleteScript : MonoBehaviour
{
     int rankAScore;
     int rankBScore;
     int rankCScore;

    int score;
    int maxCombo;
    
    int[] notes = new int[5];

    public Text perfectText;
    public Text greatText;
    public Text goodText;
    public Text badText;
    public Text missText;
    public Text maxComboText;
    public Text scoreText;

    // Start is called before the first frame update
    void Start()
    {
        showNoteCounts();
        showMaxComboAndScore();
    }
    private void Awake()
    {
        for (int i = 0; i < 5; i++)
        {
            notes[i] = 0;
        }

        score = 0;
        maxCombo = 0;

        //初始化文本
        perfectText.text = "0";
        greatText.text = "0";
        goodText.text = "0";
        badText.text = "0";
        missText.text = "0";

        scoreText.text = "0";
        maxComboText.text = "0";

    }
    // Update is called once per frame
    void Update()
    {
        
    }

    public void showNoteCounts()
    {
        for (int i = 0; i < 5; i++)
        {
            //获取计数器中的值
            notes[i] = GlobalControl.Instance.notes[i];
        }
        perfectText.text = notes[4].ToString();
        greatText.text = notes[3].ToString();
        goodText.text = notes[2].ToString();
        badText.text = notes[1].ToString();
        missText.text = notes[0].ToString();

    }

    public void showMaxComboAndScore()
    {
        score = GlobalControl.Instance.score;
        maxCombo = GlobalControl.Instance.maxCombo;

        scoreText.text = score.ToString();
        maxComboText.text = maxCombo.ToString();
    }


    
}
