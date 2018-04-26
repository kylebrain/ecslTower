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

    protected LineRenderer line;

    /// <summary>
    /// Used to keep track of when the tower's position changes so the radius circle can be redrawn
    /// </summary>
    private Vector3 prevPos;


    /// <summary>
    /// Initializes settings for the radius display's LineRenderer component
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
    /// Recalculates the points in the LineRenderer's circle
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

        Vector2 centerPoint = new Vector2((float)(startX + endX) / 2, (float)(startY + endY) / 2);
        transform.position = new Vector3(centerPoint.x, 0f, centerPoint.y);

        worldGrid.setOccupied(loc);
        location = loc;
    }



    /// <summary>
    /// Removes the tower from the map
    /// </summary>
    public void removeFromMap() {
        worldGrid.setUnoccupied(location);
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



    /// <summary>
    /// Action to be performed in each call of FixedUpdate
    /// </summary>
    public abstract void updateAction();




}
