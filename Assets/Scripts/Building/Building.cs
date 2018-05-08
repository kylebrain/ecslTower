using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public abstract class Building : MonoBehaviour{

    //----------VARIABLES--------------

    public float Radius = 1f;
    public Color radiusColor = new Color(10, 10, 120);
    public float radiusLineWidth = 0.1f;

    public float price;
    public float sellPrice;

    public float startingHealth = 0f;
    public GridArea Location;

    protected float health = 0f;
    protected bool placed = false;
    public bool Placed {
        get { return placed; }
        set { placed = value; }
    }

    /// <summary>
    /// Used to keep track of when the tower's position changes so the radius circle can be redrawn
    /// </summary>
    private Vector3 prevPos;

    /// <summary>
    /// Holds the array of nodes that make the play area
    /// </summary>
    private WorldGrid worldGrid;

    /// <summary>
    /// LineRenderer component of the gameobject
    /// </summary>
    protected LineRenderer line;

    /// <summary>
    /// The number of segments in the radius line.
    /// </summary>
    private int numSegments = 100;

    //-------------------FUNCTIONS-------------------
    
    /// <summary>
    /// Add to the tower's health
    /// </summary>
    /// <param name="toAdd">The amount of health to add (or subtract if negative)</param>
    public void updateHealth(float toAdd) {
        health += toAdd;
    }

    /// <summary>
    /// Checks to see if the requested location is available, and if so places the tower there
    /// </summary>
    /// <param name=""></param>
    public void placeOnMap(GridArea loc) {
        bool available = true;

        //Loop through the area occupied by loc
        int startX = loc.bottomLeft.x;
        int endX = startX + loc.width - 1;

        int startY = loc.bottomLeft.y;
        int endY = startY + loc.height - 1;

        for(int i = startX; i <= endX; ++i) {
            for(int j = startY; j <= endY; ++j) {
                Node cur = worldGrid.getAt(i, j);
                if(cur == null) {
                    return;
                }
                if(cur.Occupied != Node.nodeStates.empty) {
                    available = false;
                }
            }
        }
        if(!available) {
            return;
        }

        Location = loc;
        updatePosition();
        worldGrid.setOccupied(loc, Node.nodeStates.building);
        placed = true;
    }

    /// <summary>
    /// Removes the tower from the map
    /// </summary>
    public void removeFromMap() {
        worldGrid.setUnoccupied(Location);
        placed = false;
        Destroy(this);
    }

    /// <summary>
    /// Shows the tower's radius as a cirlce on the map
    /// </summary>
    public void showRadius() {
        if(prevPos != transform.position) {
            drawRadius();
        }
        prevPos = transform.position;
        line.enabled = true;
    }

    /// <summary>
    /// Hides the tower's radius
    /// </summary>
    public void hideRadius() {
        line.enabled = false;
    }

    //-----------PROTECTED-------------

    /// <summary>
    /// Recalculates the points in the LineRenderer's circle. Only called when the object's position changes.
    /// </summary>
    protected void drawRadius() {
        float deltaTheta = (float)(2.0 * Mathf.PI) / numSegments;
        float theta = 0f;
        for(int i = 0; i < numSegments; i++) {
            float x = Radius * Mathf.Cos(theta);
            float z = Radius * Mathf.Sin(theta);
            Vector3 pos = new Vector3(x, 0.1f, z);
            line.SetPosition(i, pos);
            theta += deltaTheta;
        }
    }

    /// <summary>
    /// Initializes settings for the LineRenderer component which is used to display the radius.
    /// </summary>
    protected void initLineRenderer() {
        line = gameObject.GetComponent<LineRenderer>();
        line.positionCount = numSegments;
        line.material.color = radiusColor;
        line.startWidth = radiusLineWidth;
        line.alignment = LineAlignment.View;
        line.loop = true;
        line.useWorldSpace = false;

        prevPos = transform.position;

        drawRadius();
        line.enabled = false;
    }

    /// <summary>
    /// Calculates a new Location value based on pos, snapped to the nearest integer coordinate. Then calls updatePosition.
    /// </summary>
    /// <param name="pos">The new transform. Note: the actual transform after the call might be slightly different
    /// because Location will round bottomLeft to integer values.</param>
    protected void setCenterPosition(Vector3 pos) {
        int bottomLeftX = (int)(pos.x - 0.5f * (Location.width - 1));
        if(bottomLeftX < 0) {
            bottomLeftX = 0;
        } else if(bottomLeftX > worldGrid.width - Location.width) {
            bottomLeftX = worldGrid.width - Location.width;
        }

        int bottomLeftY = (int)(pos.z - 0.5f * (Location.height - 1));
        if(bottomLeftY < 0) {
            bottomLeftY = 0;
        } else if(bottomLeftY > worldGrid.height - Location.height) {
            bottomLeftY = worldGrid.height - Location.height;
        }

        Location.bottomLeft = new Vector2Int(bottomLeftX, bottomLeftY);
        updatePosition();
    }

    /// <summary>
    /// Recalculates and moves the object's transform based on its Location attribute.
    /// </summary>
    protected void updatePosition() {
        int startX = Location.bottomLeft.x;
        int endX = startX + Location.width - 1;

        int startY = Location.bottomLeft.y;
        int endY = startY + Location.height - 1;

        transform.position = new Vector3((float)(startX + endX) / 2, 0f, (float)(startY + endY) / 2);
    }

    /// <summary>
    /// Sets the object's transform to the mouse if the tower is not placed. Places the tower if the mouse button is released.
    /// </summary>
    protected void handleMouse() {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = Camera.main.transform.position.y;
        Vector3 newPos = Camera.main.ScreenToWorldPoint(mousePos);

        if(!placed) {
            setCenterPosition(newPos);

            if(Input.GetMouseButtonUp(0)) {
                //Make sure the mouse is over a valid tile
                if((newPos.x < 0 || newPos.x > worldGrid.width - 1) || (newPos.z < 0 || newPos.z > worldGrid.height - 1)) {
                    return;
                }

                //Place
                placeOnMap(Location);
            }
        }
    }

    //------------PRIVATE-------------

    private void Start() {
        worldGrid = GameObject.FindWithTag("WorldGrid").GetComponent<WorldGrid>();
        if(worldGrid == null) {
            Debug.LogError("Could not find WorldGrid object in the scene. Either the tag was changed or the object is missing.");
        }
        initLineRenderer();
        derivedStart();
    }

    private void Update() {
        handleMouse();
        updateAction();
    }

    //------------ABSTRACT-------------

    /// <summary>
    /// Actions to be performed by the derived class in Start.
    /// </summary>
    protected abstract void derivedStart();

    /// <summary>
    /// Actions to be performed by the derived class in each call of Update.
    /// </summary>
    protected abstract void updateAction();

    

}
