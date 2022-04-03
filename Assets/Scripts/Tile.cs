using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField] public int islandListIndex;

    private void OnMouseDown()
    {
        GameObject Grid = GameObject.Find("Grid");
        if (Grid.GetComponent<Grid>().islands[islandListIndex].Count > 1)
        {
            Grid.GetComponent<Grid>().DestroyIsland(islandListIndex);
        }
    }
}
