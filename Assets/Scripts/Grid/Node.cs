using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Node : MonoBehaviour {

    //change to visual pref if really necessary
    public Color emptyColor;
    public Color navColor;
    public Color buildingColor;
    public Color invalidColor;

    /// <summary>
    /// The possible values the occupied field can take
    /// </summary>
    public enum nodeStates { empty, building, navigation};

    /// <summary>
    /// The occupied state of the current node
    /// </summary>
    public nodeStates Occupied;

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
    //private Material initialMaterial;
    

    private void Start() {
        rend = GetComponent<Renderer>();
        if(rend == null) {
            Debug.LogError("Cannot find Node's renderer. Did you remove the component?");
        }

        //initialMaterial = rend.material;
    }

    private void Update() {
        setUnhovered();
    }

    public bool IsBetween(Arrow arrow)
    {
        return IsBetween(arrow.Origin, arrow.Destination);
    }

    public bool IsBetween(Node origin, Node destination)
    {
        float distBetween = (destination.Coordinate - origin.Coordinate).magnitude;
        float distSegmented = (destination.Coordinate - Coordinate).magnitude + (Coordinate - origin.Coordinate).magnitude;
        return distBetween == distSegmented;
    }

    /// <summary>
    /// Sets the material back to what it was originally
    /// </summary>
    public void setUnhovered() {
        if(rend.enabled == true) {
            rend.enabled = false;
        }
    }

    /// <summary>
    /// Sets the material to hoverMaterial (hoverInvalidMaterial if the node is occupied).
    /// </summary>
    public void setHovered() {
        rend.enabled = true;

        switch (Occupied)
        {
            case nodeStates.empty:
                rend.material.SetColor("_Color", emptyColor);
                break;
            case nodeStates.navigation:
                rend.material.SetColor("_Color", navColor);
                break;
            case nodeStates.building:
                rend.material.SetColor("_Color", buildingColor);
                break;
            default:
                rend.material.SetColor("_Color", invalidColor);
                break;
        }
    }

    public override string ToString() {
        return base.ToString() + " Occupied: " + Occupied;
    }


}
