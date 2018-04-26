using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public abstract class Building : MonoBehaviour{
    public WorldGrid worldGrid;
    public Color radiusColor = new Color(10, 10, 120);
    public int numSegments = 100;
    public float radiusLineWidth = 0.1f;

    protected float health = 0;
    public float Health {
        get { return health; }
        set { health = value; }
    }

    protected GridArea location;
    public GridArea Location {
        get { return location; }
        set { location = value; }
    }

    protected float radius = 1f;
    public float Radius {
        get { return radius; }
        set { radius = value; }
    }

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
    /// LineRenderer component of the gameobject
    /// </summary>
    protected LineRenderer line;

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
                if(cur.Occupied) {
                    available = false;
                }
            }
        }
        if(!available) {
            return;
        }

        location = loc;
        updatePosition();
        worldGrid.setOccupied(loc);
        placed = true;
    }

    /// <summary>
    /// Removes the tower from the map
    /// </summary>
    public void removeFromMap() {
        worldGrid.setUnoccupied(location);
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
            float x = radius * Mathf.Cos(theta);
            float z = radius * Mathf.Sin(theta);
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
        int bottomLeftX = (int)(pos.x - 0.5f * (location.width - 1));
        if(bottomLeftX < 0) {
            bottomLeftX = 0;
        } else if(bottomLeftX > worldGrid.width - location.width) {
            bottomLeftX = worldGrid.width - location.width;
        }

        int bottomLeftY = (int)(pos.z - 0.5f * (location.height - 1));
        if(bottomLeftY < 0) {
            bottomLeftY = 0;
        } else if(bottomLeftY > worldGrid.height - location.height) {
            bottomLeftY = worldGrid.height - location.height;
        }

        location.bottomLeft = new Vector2Int(bottomLeftX, bottomLeftY);
        updatePosition();
    }

    /// <summary>
    /// Recalculates and moves the object's transform based on its Location attribute.
    /// </summary>
    protected void updatePosition() {
        int startX = location.bottomLeft.x;
        int endX = startX + location.width - 1;

        int startY = location.bottomLeft.y;
        int endY = startY + location.height - 1;

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
                placeOnMap(location);
            }
        }

        

    }

    /// <summary>
    /// Action to be performed in each call of FixedUpdate. MUST BE OVERRIDEN BY CHILDREN.
    /// </summary>
    protected abstract void updateAction();



}
