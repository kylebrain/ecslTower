using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGrid: MonoBehaviour {
    public int width = 1;
    public int height = 1;
    public Node nodePrefab;
    public BaseGrid baseGrid;

    private Node[,] m_grid;


    private void Awake() {
        m_grid = new Node[width, height];

        for(int i = 0; i < width; ++i) {
            for(int j = 0; j < height; ++j) {
                m_grid[i, j] = Instantiate(nodePrefab, transform);
                m_grid[i, j].transform.position = new Vector3(i, 0, j);
                m_grid[i, j].name = "Node (" + i + ", " + j + ")";
            }
        }
        baseGrid.Resize(this);
    }

    public Node getAt(int x, int y) {
        if(x >= width || y >= height || x < 0 || y < 0) {
            return null;
        }
        return m_grid[x, y];
    }

    /// <summary>
    /// Raycasts the mouse location to the WorldGrid.
    /// Returns the node it hit, or null if no collision occured.
    /// </summary>
    /// <returns>The hit node if valid, otherwise null</returns>
    public Node getRaycastNode() {
        Ray ray;
        RaycastHit hit;
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if(!Physics.Raycast(ray, out hit)) {
            return null;
        }

        Node ret = hit.transform.gameObject.GetComponent<Node>();
        if(ret != null) {
            return ret;
        } else {
            return null;
        }
    }

    private void LateUpdate() {
        Node hit = getRaycastNode();
        if(hit != null) {
            //make all adjacent Nodes hovered
            hit.setHovered();
            Node current;
            if((current = getAt(hit.Coordinate.x + 1, hit.Coordinate.y)) != null) {
                current.setHovered();
            }
            if ((current = getAt(hit.Coordinate.x, hit.Coordinate.y + 1)) != null)
            {
                current.setHovered();
            }
            if ((current = getAt(hit.Coordinate.x - 1, hit.Coordinate.y)) != null)
            {
                current.setHovered();
            }
            if ((current = getAt(hit.Coordinate.x, hit.Coordinate.y - 1)) != null)
            {
                current.setHovered();
            }
        }
    }

    /// <summary>
    /// Sets the location's Nodes to occupied
    /// </summary>
    /// <param name="loc">The location to set occupied</param>
    public void setOccupied(GridArea loc, Node.nodeStates state) {

        int startX = loc.bottomLeft.x;
        int endX = startX + loc.width - 1;

        int startY = loc.bottomLeft.y;
        int endY = startY + loc.height - 1;

        for(int i = startX; i <= endX; ++i) {
            for(int j = startY; j <= endY; ++j) {
                if(m_grid[i, j].Occupied == Node.nodeStates.empty) {
                    m_grid[i, j].Occupied = state;
                }
            }
        }
    }

    /// <summary>
    /// Sets the location's Nodes to unoccupied
    /// </summary>
    /// <param name="loc">The location to set unoccupied</param>
    public void setUnoccupied(GridArea loc, Node.nodeStates state) {

        int startX = loc.bottomLeft.x;
        int endX = startX + loc.width - 1;

        int startY = loc.bottomLeft.y;
        int endY = startY + loc.height - 1;

        for(int i = startX; i <= endX; ++i) {
            for(int j = startY; j <= endY; ++j) {
                if(m_grid[i, j].Occupied == state) {
                    m_grid[i, j].Occupied = Node.nodeStates.empty;
                }
            }
        }
    }



}