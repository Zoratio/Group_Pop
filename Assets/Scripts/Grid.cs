using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public int gridX;
    public int gridY;
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
                Vector2 tempPos = new Vector2(tileX, tileY);
                GameObject backgroundTile = Instantiate(tilePrefab, tempPos, Quaternion.identity) as GameObject;
                backgroundTile.transform.parent = this.transform;
                backgroundTile.name = "tilesArr[" + tileX + "][" + tileY + "]";

                int tileToUse = Random.Range(0, tiles.Length);
                GameObject tile = Instantiate(tiles[tileToUse], tempPos, Quaternion.identity);
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
        }
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
}
