using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public int islandListIndex;
    public int row;
    public int column;
    public int targetRow;
    public float fallSpeed;

    private GameObject tileBelow;
    private BoardGrid grid;
    private Vector2 tempPos;

    void Start()
    {
        grid = FindObjectOfType<BoardGrid>();
        fallSpeed = 0.1f;
    }

    //Ensures the tile is in the right position after gaps have been made
    void Update()
    {
        targetRow = row;
        
        if (Mathf.Abs(targetRow - transform.position.y) > .1)
        {
            tempPos = new Vector2(transform.position.x, targetRow);
            transform.position = Vector2.Lerp(transform.position, tempPos, fallSpeed);
        }
        else
        {
            tempPos = new Vector2(transform.position.x, targetRow);
            transform.position = tempPos;
            grid.allTiles[column, row] = this.gameObject;
        }
    }

    //If the player touches the tile and the game is in move state and the island is bigger than 1, it will call the BoardGrids DestroyIsland() function
    private void OnMouseDown()
    {
        if (grid.currentState == GameState.move && grid.islands[islandListIndex].Count > 1)
        {
            grid.currentState = GameState.wait;
            grid.DestroyIsland(islandListIndex);
        }
    }
}
