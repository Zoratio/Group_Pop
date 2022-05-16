using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    move,
    wait
}

public class BoardGrid : MonoBehaviour
{
    public int gridX;
    public int gridY;
    private int offset = 20;

    [HideInInspector] public GameState currentState;

    [SerializeField] GameObject tilePrefab;
    [SerializeField] GameObject[] tiles;
    [SerializeField] GameObject popParticle;
    public GameObject[,] allTiles;
    private float particleDestroyDelay = .2f;
    private float moveWait = .8f;
    private float fillWait = .2f;

    public List<List<GameObject>> islands;

    private Goal goal;
    [SerializeField] int pointsPerPop;


    void OnEnable()
    {
        goal = FindObjectOfType<Goal>();
        allTiles = new GameObject[gridX, gridY];
        islands = new List<List<GameObject>>();
        GridSetup();
        FindIslands(allTiles);
        if (goal.movesRemaining > 0)
            currentState = GameState.move;
    }

    //Creates the grid with all the tiles and background tiles
    private void GridSetup()
    {
        for (int tileX = 0; tileX < gridX; tileX++)
        {
            for (int tileY = 0; tileY < gridY; tileY++)
            {
                Vector2 tempPos = new Vector2(tileX, tileY + offset);
                Vector2 tilePos = new Vector2(tileX, tileY);
                GameObject backgroundTile = Instantiate(tilePrefab, tilePos, Quaternion.identity) as GameObject;
                backgroundTile.transform.parent = this.transform;
                backgroundTile.name = "tilesArr[" + tileX + "][" + tileY + "]";

                int tileToUse = Random.Range(0, tiles.Length);
                GameObject tile = Instantiate(tiles[tileToUse], tempPos, Quaternion.identity) as GameObject;

                tile.GetComponent<Tile>().row = tileY;
                tile.GetComponent<Tile>().column = tileX;

                tile.transform.parent = this.transform;
                tile.name = "tilesArr[" + tileX + "][" + tileY + "]";
                allTiles[tileX, tileY] = tile;
            }
        }
    }

    //Called from Tile script when the player taps on a tile provided the game is currently in move state and the island is creater than 1 - if condition is met, game state is also changed to wait.
    //Loops through all the elements in that 2D list index, destroying them and calculating the score multiplier.
    //Once destroyed, the TilesFall() function is called to reposition the tiles after gaps have occurred.
    public void DestroyIsland(int index)
    {
        currentState = GameState.wait;
        float scoreMultiplier = 1f;
        int tiles = 0;
        foreach (GameObject tile in islands[index])
        {
            GameObject particle = Instantiate(popParticle, tile.transform.position, Quaternion.identity);
            Destroy(particle, particleDestroyDelay);
            Destroy(tile);      
            allTiles[tile.GetComponent<Tile>().column, tile.GetComponent<Tile>().row] = null;

            tiles++;
            if (tiles > 2)
            {
                scoreMultiplier += 0.25f;
            }
        }
        float tempPoints = pointsPerPop * tiles;
        tempPoints *= scoreMultiplier;
        goal.IncreaseScore(tempPoints);

        StartCoroutine(TilesFall());
    }

    //Creates a 2D list of all islands. 
    //Every touching area of tiles is an island. 
    //Calculates the islands by using DFS method while keeping track of which tiles have been visited yet (aka part of a list/island already)
    //Each tile has reference to its island index. 
    //If no island is greater than 1 tile, the board is not solvable so Unsolvable() is called.
    public void FindIslands(GameObject[,] grid)
    {
        bool solvable = false;
        bool[,] visited = new bool[grid.GetLength(0), grid.GetLength(1)];
        int islandIndex = 0;
        for (int i = 0; i < grid.GetLength(0); i++)
        {
            for (int j = 0; j < grid.GetLength(1); j++)
            {
                if (!visited[i, j])
                {
                    List<GameObject> island = new List<GameObject>();
                    var tag = grid[i, j].tag;
                    DFS(grid, visited, i, j, tag, islandIndex, island);
                    islands.Add(island);
                    if (islands[islandIndex].Count > 1)
                    {
                        solvable = true;
                    }
                    islandIndex++;                    
                }
            }
        }
        if (!solvable) Unsolvable();
    }

    //DFS to locate and add all tiles to the correct island list.
    public void DFS(GameObject[,] grid, bool[,] visited, int i, int j, string tag, int islandIndex, List<GameObject> island)
    {
        if (i < 0 || i >= grid.GetLength(0)) return;

        if (j < 0 || j >= grid.GetLength(1)) return;

        if (grid[i, j].gameObject.tag != tag || visited[i, j]) return;

        visited[i, j] = true;
        grid[i, j].GetComponent<Tile>().islandListIndex = islandIndex;
        island.Add(grid[i, j]);

        DFS(grid, visited, i + 1, j, tag, islandIndex, island);
        DFS(grid, visited, i - 1, j, tag, islandIndex, island);
        DFS(grid, visited, i, j + 1, tag, islandIndex, island);
        DFS(grid, visited, i, j - 1, tag, islandIndex, island);
    }

    //If unsolvable, the grid needs to restart.
    public void Unsolvable()
    {
        Debug.Log("Unsolvable, resetting grid tiles");
        for (int i = 0; i < gridX; i++)
        {
            for (int j = 0; j < gridY; j++)
            {
                Destroy(allTiles[i, j]);
                allTiles[i, j] = null;
            }
        }
        if (goal.movesRemaining > 0)
            currentState = GameState.move;

        this.GetComponent<BoardGrid>().enabled = false;
        this.GetComponent<BoardGrid>().enabled = true;
    }

    //Calculates how much of a gap below the tiles need to fall to be in the correct position.
    //Once the tiles are in the correct positions, the gaps at the top need to be filled using FillGrid().
    private IEnumerator TilesFall()
    {
        int emptyCount = 0;
        for (int i = 0; i < gridX; i++)
        {
            for (int j = 0; j < gridY; j++)
            {
                if (allTiles[i,j] == null)
                {
                    emptyCount++;
                }
                else if (emptyCount > 0)
                {
                    allTiles[i, j].GetComponent<Tile>().row -= emptyCount;
                    allTiles[i, j] = null;
                }
            }
            emptyCount = 0;
        }
        yield return new WaitForSeconds(fillWait);
        StartCoroutine(FillGrid());
    }

    //Spawns the tiles in the correct position when taking the offset due to the gap size into consideration.
    private void RefillGrid()
    {
        for (int i = 0; i < gridX; i++)
        {
            for (int j = 0; j < gridY; j++)
            {
                if(allTiles[i,j] == null)
                {
                    Vector2 tempPos = new Vector2(i, j + offset);
                    int tileToUse = Random.Range(0, tiles.Length);
                    GameObject tile = Instantiate(tiles[tileToUse], tempPos, Quaternion.identity);
                    allTiles[i, j] = tile;
                    tile.GetComponent<Tile>().row = j;
                    tile.GetComponent<Tile>().column = i;
                    tile.transform.parent = this.transform;
                    tile.name = "tilesArr[" + i + "][" + j + "]";
                }
            }
        }
    }

    //Calls RefillGrid() to make the tiles spawn and falls into their correct position from within the Tile script.
    //Clears all of the islands and recreates the list of islands
    //Game is back into move state so the player can tap on tiles again.
    private IEnumerator FillGrid()
    {
        RefillGrid();

        islands.Clear();
        FindIslands(allTiles);

        yield return new WaitForSeconds(moveWait);
        if (goal.movesRemaining > 0)
            currentState = GameState.move;        
    }
}
