using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.SceneManagement;

public class Goal : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI finalScoreText;
    public TextMeshProUGUI moves;

    public GameObject GameOverCanvas;
    public int score;
    public int movesRemaining;
    private BoardGrid grid;


    // Start is called before the first frame update
    void Start()
    {
        score = 0;
        grid = FindObjectOfType<BoardGrid>();
    }

    // Update is called once per frame
    void Update()
    {
        scoreText.text = "Score: " + score;
        moves.text = "Moves: " + movesRemaining;
    }

    //Called from the BoardGrid script when an island is destroyed. 
    //For every tile that was part of the island, the multiplier is increased. 
    //The bigger the island, the bigger the score.
    public void IncreaseScore(float multiplier)
    {
        score += (int)Mathf.Round(multiplier);
        DecreaseMoves();
    }

    //When a player makes a valid pop, the number of moves remaining decreases.
    //Game changes to wait state if the game is over.
    public void DecreaseMoves()
    {
        if (movesRemaining <= 1)
        {
            grid.currentState = GameState.wait;
            GameOver();
        }
        movesRemaining--;
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
