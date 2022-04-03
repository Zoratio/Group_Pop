using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardGrid : MonoBehaviour
{
    public int gridX;
    public int gridY;
    public int offset;

    public GameObject tilePrefab;
    public GameObject[] tiles;
    public GameObject[,] allTiles;

    public List<List<GameObject>> islands;  

    private BackgroundTiles[,] tilesArr;    

    void Start()
    {
        tilesArr = new BackgroundTiles[gridX, gridY];
        allTiles = new GameObject[gridX, gridY];
        islands = new List<List<GameObject>>();
        GridSetup();
        FindIslands(allTiles);
    }

    private void GridSetup()
    {
        for (int tileX = 0; tileX < gridX; tileX++)
        {
            for (int tileY = 0; tileY < gridY; tileY++)
            {
                Vector2 tempPos = new Vector2(tileX, tileY + offset);

                GameObject backgroundTile = Instantiate(tilePrefab, tempPos, Quaternion.identity) as GameObject;
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

    public void DestroyIsland(int index)
    {
        foreach (GameObject tile in islands[index])
        {
            Destroy(tile);
            allTiles[tile.GetComponent<Tile>().column, tile.GetComponent<Tile>().row] = null;
        }
        StartCoroutine(TilesFall());
    }

    public void FindIslands(GameObject[,] grid)
    {
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
                    islandIndex++;
                }
            }
        }
    }

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
        yield return new WaitForSeconds(.4f);
        StartCoroutine(FillGrid());
    }

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
                }
            }
        }
    }

    private IEnumerator FillGrid()
    {
        RefillGrid();

        islands.Clear();
        FindIslands(allTiles);

        yield return new WaitForSeconds(.5f);
        FindObjectOfType<Controls>().SetPlayable(true);
    }
}
