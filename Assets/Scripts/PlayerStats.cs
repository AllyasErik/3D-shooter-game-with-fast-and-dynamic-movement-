using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [SerializeField] private int health;
    private GameManager gameManager;
    private Score score;

    private PlayerHUD playerHUD;

    private void Awake()
    {
        playerHUD = GetComponent<PlayerHUD>();
        gameManager = FindObjectOfType<GameManager>();
        score = FindObjectOfType<Score>();
    }

    private void Start()
    {
        health = 100;
        playerHUD.UpdateHealthUI(health);

    }

    private void CheckHealth()
    {
        playerHUD.UpdateHealthUI(health);
        if (health <= 0)
        {
            gameManager.GameOver();
        }
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        int scoreToLose = damage * 10;
        score.AddToScore(-scoreToLose);
        CheckHealth();
    }
}
