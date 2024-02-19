using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Score : MonoBehaviour
{
    private PlayerHUD playerHUD;
    private int score;

    private void Start()
    {
        Invoke("InitializeScore", 0.1f);

    }

    public void AddToScore(int scoreToAdd)
    {
        score += scoreToAdd;
        playerHUD.UpdateScoreUI(score);
    }

    public int GetScore()
    {
        return score;
    }

    private void InitializeScore()
    {
        playerHUD = FindObjectOfType<PlayerHUD>();
        score = 0;
        playerHUD.UpdateScoreUI(score);
    }

    
}
