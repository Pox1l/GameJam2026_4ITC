using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
    public TextMeshProUGUI TotalScore;
    public TextMeshProUGUI timerText;
    public double multiplier = 1.8;
    private double score = 0;

    public float timeInterval = 10f;
    public int scoreToAdd = 10;
    public float scoreTimer;

    float elapsedTime;

    // Update is called once per frame
    void Update()
    {
        elapsedTime += Time.deltaTime;
        int minutes = Mathf.FloorToInt(elapsedTime / 60);
        int seconds = Mathf.FloorToInt(elapsedTime % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);

        scoreTimer += Time.deltaTime;

        if (scoreTimer >= timeInterval)
        {
            score += (scoreToAdd * multiplier) ;
            scoreTimer -= timeInterval;

            Debug.Log("momentální scóre:" + score);
        }
    }
}
