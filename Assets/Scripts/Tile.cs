using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField] public int islandListIndex;
    public int row;
    public int column;
    public int targetRow;
    public float fallSpeed = .5f;

    private GameObject tileBelow;
    private BoardGrid grid;
    private Controls controls;
    private Vector2 tempPos;

    void Start()
    {
        grid = FindObjectOfType<BoardGrid>();
        controls = FindObjectOfType<Controls>();
    }

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

    private void OnMouseDown()
    {
        if (controls.GetPlayable() && grid.islands[islandListIndex].Count > 1)
        {
            controls.SetPlayable(false);
            grid.DestroyIsland(islandListIndex);
        }
    }

    void MoveTile()
    {
        if (row > 0)
        {
            tileBelow = grid.allTiles[column, row - 1];
            tileBelow.GetComponent<Tile>().row += 1;
            row -= 1;
        }
    }
}
