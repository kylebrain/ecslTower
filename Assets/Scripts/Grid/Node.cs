using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour {
    public Material hoverMaterial;
    public Material hoverInvalidMaterial;
    public bool Occupied { get; set; }


    private bool occupied = false;
    private Renderer rend;
    private Material initialMaterial;
    

    private void Start() {
        rend = GetComponent<Renderer>();
        if(rend == null) {
            Debug.Log("Cannot find Node's renderer.\nDid you remove the component?");
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
        rend.material = initialMaterial;
    }

    /// <summary>
    /// Sets the material to hoverMaterial (hoverInvalidMaterial if the node is occupied).
    /// </summary>
    public void setHovered() {
        if(occupied) {
            rend.material = hoverInvalidMaterial;
        } else {
            rend.material = hoverMaterial;
        }
    }
}
