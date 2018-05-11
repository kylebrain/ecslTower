using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Node : MonoBehaviour {
    public Material hoverMaterial;
    public Material hoverInvalidMaterial;

    private bool occupied = false;
    public bool Occupied {
        get { return occupied; }
        set { occupied = value; }
    }

    [SerializeField]
    public Vector2Int Coordinate
    {
        get
        {
            return new Vector2Int((int)transform.position.x, (int)transform.position.z);
        }
        set
        {
            transform.position = new Vector3(value.x, transform.position.y, value.y);
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
        if(occupied) {
            rend.material = hoverInvalidMaterial;
        } else {
            rend.material = hoverMaterial;
        }
    }

    public override string ToString() {
        return base.ToString() + " Occupied: " + occupied;
    }


}
