using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [HideInInspector] public int islandListIndex;
    [HideInInspector] public int row;
    [HideInInspector] public int column;
    [HideInInspector] public int targetRow;
    [HideInInspector] public float fallSpeed;

    private GameObject tileBelow;
    private BoardGrid grid;
    private Vector2 tempPos;


    void Start()
    {
        grid = FindObjectOfType<BoardGrid>();
        fallSpeed = 0.13f;
    }

    //Ensures the tile is in the right position after gaps have been made
    void FixedUpdate()
    {
        targetRow = row;
        
        if (Mathf.Abs(targetRow - transform.position.y) > .01)
        {
            tempPos = new Vector2(transform.position.x, targetRow);
            transform.position = Vector2.Lerp(transform.position, tempPos, fallSpeed);
            grid.allTiles[column, row] = this.gameObject;

            if (Mathf.Abs(targetRow - transform.position.y) <= .01)
            {
                tempPos = new Vector2(transform.position.x, targetRow);
                transform.position = tempPos;
            }
        }
    }

    //If the player touches the tile and the game is in move state and the island is bigger than 1, it will call the BoardGrids DestroyIsland() function
    private void OnMouseDown()
    {
        if (grid.currentState == GameState.move && grid.islands[islandListIndex].Count > 1)
        {
            grid.DestroyIsland(islandListIndex);
        }
    }
}
