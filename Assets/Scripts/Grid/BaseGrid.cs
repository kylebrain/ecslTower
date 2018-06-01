using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseGrid : MonoBehaviour {

    public float gridOffset = 0.1f;

    public void Resize(WorldGrid grid)
    {
        transform.position = new Vector3(grid.transform.position.x + (grid.width - 1) / 2f, -gridOffset, grid.transform.position.z + (grid.height - 1) / 2f);
        transform.localScale = new Vector3(grid.width, grid.height, 1f);
    }

}
