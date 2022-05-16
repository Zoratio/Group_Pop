using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPos : MonoBehaviour
{
    private BoardGrid grid;
    private float aspectRatio = 0.5625f;
    private float padding = 2;

    // Start is called before the first frame update
    void Start()
    {        
        grid = FindObjectOfType<BoardGrid>();   
        if (grid != null)
        {
            AdjustCameraPos(grid.gridX - 1, grid.gridY + 1);
        }
    }

    //Calculates the correct camera position for the size of grid being generated.
    void AdjustCameraPos(float x, float y)
    {
        Vector3 tempPos = new Vector3(x / 2, y / 2, transform.position.z);
        transform.position = tempPos;
        if (grid.gridX >= grid.gridY)
        {
            Camera.main.orthographicSize = (grid.gridX / 2 + padding) / aspectRatio;
        }
        else
        {
            Camera.main.orthographicSize = grid.gridY / 2 + padding;
        }
    }
}
