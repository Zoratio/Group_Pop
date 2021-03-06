using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.SceneManagement;

public class Goal : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] TextMeshProUGUI finalScoreText;
    [SerializeField] TextMeshProUGUI moves;

    [SerializeField] GameObject GameOverCanvas;
    private int score;
    public int movesRemaining;


    // Start is called before the first frame update
    void Start()
    {
        score = 0;
        scoreText.text = "Score: " + score;
        moves.text = "Moves: " + movesRemaining;
    }


    //Called from the BoardGrid script when an island is destroyed. 
    //For every tile that was part of the island, the multiplier is increased. 
    //The bigger the island, the bigger the score.
    public void IncreaseScore(float multiplier)
    {
        score += (int)Mathf.Round(multiplier);
        scoreText.text = "Score: " + score;
        DecreaseMoves();
    }

    //When a player makes a valid pop, the number of moves remaining decreases.
    //Game changes to wait state if the game is over.
    public void DecreaseMoves()
    {
        if (movesRemaining <= 1)
        {
            GameOver();
        }
        movesRemaining--;
        moves.text = "Moves: " + movesRemaining;
    }

    //Canvas that displays the players final score and provides a restart button if they wish to play again.
    private void GameOver()
    {
        GameOverCanvas.SetActive(true);
        finalScoreText.text = score.ToString();
    }
    
    //Reloads the scene again.
    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
