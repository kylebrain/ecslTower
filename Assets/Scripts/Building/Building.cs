using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public abstract class Building : MonoBehaviour{
    public WorldGrid worldGrid;
    public Color radiusColor;
    public int numSegments;

    private float health = 0;
    public float Health { get; set; }

    private GridArea location;
    public GridArea Location { get; set; }

    private float radius = 0;
    public float Radius { get; set; }

    private LineRenderer line;

    private void Start() {
        line = gameObject.GetComponent<LineRenderer>();
        line.positionCount = numSegments;
        line.startColor = radiusColor;
        line.startWidth = 0.1f;
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
        for(int i = loc.topLeft.x; i <= loc.bottomRight.x; ++i) {
            for(int j = loc.topLeft.y; j <= loc.bottomRight.y; ++j) {
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
        //TODO: implement placing the tower
    }
    public void removeFromMap() {
        //TODO: implement removing tower based on private variable "location"
    }

    public void showRadius() {
        float deltaTheta = (float)(2.0 * Mathf.PI) / (numSegments - 1);
        float theta = 0f;

        for(int i = 0; i < numSegments; i++) {
            float x = radius * Mathf.Cos(theta);
            float z = radius * Mathf.Sin(theta);
            Vector3 pos = new Vector3(x, 0.1f, z);
            line.SetPosition(i, pos);
            theta += deltaTheta;
        }
    }

}
