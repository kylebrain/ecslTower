using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Building {
    public WorldGrid worldGrid;

    private float health = 0;
    public float Health { get; set; }

    private GridArea location;
    public GridArea Location { get; set; }

    private float radius = 0;
    public float Radius { get; set; }

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
        for(int i = loc.topLeft.x; i < loc.bottomRight.x; ++i) {
            for(int j = loc.topLeft.y; j < loc.bottomRight.y; ++j) {
                if(worldGrid.getAt(i, j).Occupied) {
                    available = false;
                }
            }
        }
        if(!available) {
            return;
        }
        //TODO:implement placing the tower
    }
    public void removeFromMap(GridArea loc) {

    }

    public void showRadius() {

    }

}
