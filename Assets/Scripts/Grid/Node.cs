using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour {
    public Material hoverMaterial;
    public Material hoverInvalidMaterial;

    /// <summary>
    /// The possible values the occupied field can take
    /// </summary>
    public enum nodeStates { empty, building, navigation};

    /// <summary>
    /// The occupied state of the current node
    /// </summary>
    public nodeStates Occupied;

    public Vector2Int Coordinate
    {
        get
        {
            return new Vector2Int((int)transform.position.x, (int)transform.position.z);
        }
    }

    private Renderer rend;
    private Material initialMaterial;
    

    private void Start() {
        rend = GetComponent<Renderer>();
        if(rend == null) {
            Debug.LogError("Cannot find Node's renderer. Did you remove the component?");
        }

        initialMaterial = rend.material;
    }

    private void Update() {
        setUnhovered();
    }

    
    /// <summary>
    /// Sets the material back to what it was originally
    /// </summary>
    public void setUnhovered() {
        if(rend.material != initialMaterial) {
            rend.material = initialMaterial;
        }
    }

    /// <summary>
    /// Sets the material to hoverMaterial (hoverInvalidMaterial if the node is occupied).
    /// </summary>
    public void setHovered() {
        if(Occupied == nodeStates.empty) {
            rend.material = hoverMaterial;
        } else {
            rend.material = hoverInvalidMaterial;
        }
    }

    public override string ToString() {
        return base.ToString() + " Occupied: " + Occupied;
    }


}
