using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(LineRenderer))]
public abstract class Building : NetworkBehaviour
{

    //----------VARIABLES--------------

    public float Radius = 1f;
    public Color radiusColor = new Color(10, 10, 120);
    public float radiusLineWidth = 0.1f;

    public static bool currentlyPlacing = false;

    //[HideInInspector]
    [SyncVar]
    public bool Placed = false;

    public int price;
    public int sellPrice;

    //public float startingHealth = 0f;
    public GridArea Location;

    public GameObject UIOverlay;

    public AudioSource placeAudio;

    protected float health = 0f;

    
    protected bool selected = false;

    static Building selectedBuilding = null;

    /// <summary>
    /// Used to keep track of when the tower's position changes so the radius circle can be redrawn
    /// </summary>
    private Vector3 prevPos;

    /// <summary>
    /// Holds the array of nodes that make the play area
    /// </summary>
    protected WorldGrid worldGrid;

    /// <summary>
    /// LineRenderer component of the gameobject
    /// </summary>
    protected LineRenderer radiusLine;

    /// <summary>
    /// The number of segments in the radius line.
    /// </summary>
    private int numSegments = 100;

    private static RaycastHit raycastHitBuilding;


    //------------PUBLIC---------------
    #region PUBLIC

    /*
    /// <summary>
    /// Add to the tower's health
    /// </summary>
    /// <param name="toAdd">The amount of health to add (or subtract if negative)</param>
    public void updateHealth(float toAdd)
    {
        health += toAdd;
    }

    */

    /// <summary>
    /// Checks to see if the requested location is available, and if so places the tower there
    /// </summary>
    /// <param name=""></param>
    public bool placeOnMap(GridArea loc)
    {
        bool invalidLocation = false;
        bool hasNavigation = false;


        int startX = loc.bottomLeft.x;
        int endX = startX + loc.width - 1;
        int startY = loc.bottomLeft.y;
        int endY = startY + loc.height - 1;

        //All nodes within loc must be empty or navigation, and at least 1 must be navigation
        for (int i = startX; i <= endX; ++i)
        {
            for (int j = startY; j <= endY; ++j)
            {
                Node cur = worldGrid.getAt(i, j);
                if (cur == null)
                {
                    return false;
                }

                if (cur.Occupied == Node.nodeStates.navigation)
                {
                    hasNavigation = true;
                }
                else if (cur.Occupied != Node.nodeStates.empty)
                {
                    invalidLocation = true;
                }
            }
        }

        //If an invalid location, or if none of the nodes were navigation nodes, exit
        if (invalidLocation || !hasNavigation)
        {
            return false;
        }

        Location = loc;
        updatePosition();
        worldGrid.setOccupied(loc, Node.nodeStates.building);
        currentlyPlacing = false;
        Placed = true;
        CmdPlace(true);

        placeAudio.Play();
        Tutorial.CallFunction(1);
        return true;
    }

    [Command]
    void CmdPlace(bool _placed)
    {
        Placed = _placed;
    }

    public void RemoveFromMap()
    {
        if (Placed)
        {
            //probably need a clientRpc to update placement on all systems
            worldGrid.setOccupied(Location, Node.nodeStates.navigation);
        }
        //Placed = false;
        CmdRemoveFromMap();
    }

    /// <summary>
    /// Removes the tower from the map
    /// </summary>
    [Command]
    void CmdRemoveFromMap()
    {
        
        Destroy(transform.root.gameObject);
    }

    /// <summary>
    /// Shows the tower's radius as a cirlce on the map
    /// </summary>
    public void showRadius()
    {
        if (prevPos != transform.position)
        {
            drawRadius();
        }
        prevPos = transform.position;
        radiusLine.enabled = true;
    }

    /// <summary>
    /// Hides the tower's radius
    /// </summary>
    public void hideRadius()
    {
        radiusLine.enabled = false;
    }
    #endregion

    //-----------PROTECTED-------------
    #region PROTECTED
    /// <summary>
    /// Recalculates the points in the LineRenderer's circle. Only called when the object's position changes.
    /// </summary>
    protected void drawRadius()
    {
        float deltaTheta = (float)(2.0 * Mathf.PI) / numSegments;
        float theta = 0f;
        for (int i = 0; i < numSegments; i++)
        {
            float x = Radius * Mathf.Cos(theta);
            float z = Radius * Mathf.Sin(theta);
            Vector3 pos = new Vector3(x, 0.1f, z);
            radiusLine.SetPosition(i, pos);
            theta += deltaTheta;
        }
    }

    protected List<Agent> GetAgentsInRadius()
    {
        GameObject[] agentArray = GameObject.FindGameObjectsWithTag("Agent");
        List<Agent> agentList = new List<Agent>();
        foreach (GameObject obj in agentArray)
        {
            if (Vector3.SqrMagnitude(transform.position - obj.transform.position) <= Mathf.Sqrt(Radius))
            {
                Agent currentAgent = obj.GetComponent<Agent>();
                if (currentAgent == null)
                {
                    Debug.LogError("Cannot find Agent script of object tagged Agent!");
                    continue;
                }
                agentList.Add(currentAgent);

            }
        }

        return agentList;
    }

    /// <summary>
    /// Initializes settings for the LineRenderer component which is used to display the radius.
    /// </summary>
    protected void initLineRenderer()
    {
        radiusLine = gameObject.GetComponent<LineRenderer>();
        radiusLine.positionCount = numSegments;
        radiusLine.material.color = radiusColor;
        radiusLine.startWidth = radiusLineWidth;
        radiusLine.alignment = LineAlignment.View;
        radiusLine.loop = true;
        radiusLine.useWorldSpace = false;

        prevPos = transform.position;

        drawRadius();
        radiusLine.enabled = false;
    }

    /// <summary>
    /// Calculates a new Location value based on pos, snapped to the nearest integer coordinate. Then calls updatePosition.
    /// </summary>
    /// <param name="pos">The new transform. Note: the actual transform after the call might be slightly different
    /// because Location will round bottomLeft to integer values.</param>
    protected void setCenterPosition(Vector3 pos)
    {
        int bottomLeftX = (int)(pos.x - 0.5f * (Location.width - 1));
        int bottomLeftY = (int)(pos.z - 0.5f * (Location.height - 1));

        Location.bottomLeft = new Vector2Int(bottomLeftX, bottomLeftY);
        updatePosition();
    }

    /// <summary>
    /// Recalculates and moves the object's transform based on its Location attribute.
    /// </summary>
    protected void updatePosition()
    {
        int startX = Location.bottomLeft.x;
        int endX = startX + Location.width - 1;

        int startY = Location.bottomLeft.y;
        int endY = startY + Location.height - 1;

        transform.position = new Vector3((float)(startX + endX) / 2, transform.position.y, (float)(startY + endY) / 2);
    }

    protected Building GetRayCastBuildingHit()
    {
        //statics should increase efficiency
        if (raycastHitBuilding.transform == null)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Physics.Raycast(ray, out raycastHitBuilding, Mathf.Infinity, 1 << 11);
        }
        if (raycastHitBuilding.transform == null)
        {
            return null;
        }
        Building buildingHit = raycastHitBuilding.transform.root.GetComponent<Building>();
        if(buildingHit == null)
        {
            buildingHit = raycastHitBuilding.transform.root.GetComponentInChildren<Building>();
        }
        return buildingHit;

        //lastUpdate resets raycastHit to be recalculated each frame
    }

    private void LateUpdate()
    {
        raycastHitBuilding = new RaycastHit();
    }

    /// <summary>
    /// Sets the object's transform to the mouse if the tower is not placed. Places the tower if the mouse button is released.
    /// </summary>
    protected void handleMouse()
    {

        //GameObject canvas = transform.Find("Canvas").gameObject;

        if (currentlyPlacing && Placed)
        {
            HideUI(UIOverlay);
        }

        #region Node unrequired
        if (!Placed)
        {
            currentlyPlacing = true;
            radiusLine.enabled = true;
            HighlightBuilding(false); //change to a non-placeable color/highlight
        }

        if (Input.GetMouseButtonDown(1))
        {
            if (Placed)
            {
                HideUI(UIOverlay);
            }
            else
            {
                Score.Money += price;
                currentlyPlacing = false;
                Destroy(gameObject);
            }
        }

        #endregion

        Node selectedNode = worldGrid.getRaycastNode();
        if (selectedNode == null)
        {
            if (Placed && Input.GetMouseButtonDown(0))
            {
                HideUI(UIOverlay); //mouseWithinBuidling must be false because there is no valid Node
            }
            if (!Placed) //draws the router over no-grid area, might need to be fixed to act like Nodes, because currently the rounding is off
            {
                Vector3 screenMousPos = Input.mousePosition;
                screenMousPos.z = Camera.main.transform.position.y;
                Vector3 worldMousePos = Camera.main.ScreenToWorldPoint(screenMousPos);

                setCenterPosition(worldMousePos);
            }
            return;
        }

        Vector2Int selectedNodePos = selectedNode.Coordinate;
        Vector3 desiredPos = new Vector3(selectedNodePos.x, 0f, selectedNodePos.y);

        bool mouseWithinBuilding;
        Building hitBuilding = GetRayCastBuildingHit();
        //if there is no building hovered over, select the Building based on Node
        if (hitBuilding == null)
        {
            mouseWithinBuilding = Location.Contains(selectedNodePos);
        }
        else
        {
            //if it hits a building, select the Building hovered over
            mouseWithinBuilding = hitBuilding == this;
        }

        //if we find the raycast too performance intensive use the old solution below
        //just delete the current variables and replace the ones that error with the old ones

        /*Vector3 screenMousPos = Input.mousePosition;
        screenMousPos.z = Camera.main.transform.position.y;
        Vector3 worldMousePos = Camera.main.ScreenToWorldPoint(screenMousPos);

        Vector2Int gridMousePos = new Vector2Int((int)Mathf.Round(worldMousePos.x), (int)Mathf.Round(worldMousePos.z));
        bool mouseWithinBuilding = Location.Contains(gridMousePos);*/


        //Handle mouse location

        #region Node required

        if (Placed)
        {
            if (mouseWithinBuilding)
            {
                HighlightBuilding(true);
            }
            else if (!selected)
            {
                HighlightBuilding(false);
            }
            if (Input.GetMouseButtonUp(0))
            {
                if (mouseWithinBuilding && !currentlyPlacing)
                {
                    ShowUI(UIOverlay);
                }
                else
                {
                    HideUI(UIOverlay);
                }
            }
        }
        else
        {
            if (selectedNode.Occupied != Node.nodeStates.building) //does not move the Router to an Occupied Node
            {
                setCenterPosition(desiredPos);
                UpdateRotation(selectedNode);
            }
            if (selectedNode.Occupied == Node.nodeStates.navigation)
            {
                HighlightBuilding(true); //ultimately change to change a placeable color/highlight
            }
            if (Input.GetMouseButtonUp(0) && placeOnMap(Location))
            {
                ShowUI(UIOverlay);
            }
        }

        #endregion
    }

    protected virtual void UpdateRotation(Node node) {
        if (node.Occupied != Node.nodeStates.navigation)
        {
            //router cannot be placed or rotated here
            //set to default value?
            //have to also handle what to do if no node is found
            //possibly with a null check but calling from building would need to be changed
            return;
        }
        List<Arrow> intersectingArrows = new List<Arrow>();
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Arrow"))
        {
            Arrow arrow = obj.GetComponent<Arrow>();
            if (node.IsBetween(arrow))
            {
                intersectingArrows.Add(arrow);
            }
        }
        SetRotation(intersectingArrows);
    }

    private void SetRotation(List<Arrow> arrowList)
    {
        if (arrowList.Count == 0)
        {
            return;
        }
        Vector3 pointVector;
        if (arrowList.Count == 1)
        {
            pointVector = arrowList[0].GetCardinality();
        }
        else
        {
            pointVector = arrowList[1].GetCardinality() + arrowList[0].GetCardinality(); //not the most eligant solution but it works to make sure that any thing over 1 is diagonal
        }
        transform.localRotation = Quaternion.LookRotation(new Vector3(pointVector.x, 0f, pointVector.y));

        DerivedSetRotation();
    }

    protected virtual void DerivedSetRotation() { }

    protected virtual void HighlightBuilding(bool highlight) { }

    /// <summary>
    /// Shows the Sell option inherent to all buildings
    /// </summary>
    /// <param name="canvas">The canvas on which it is displayed</param>
    protected void HideUI(GameObject canvas)
    {
        if (CanvasHover.Over)
        {
            return;
        }
        if(selectedBuilding == this)
        {
            selectedBuilding = null;
        }
        HighlightBuilding(false);
        radiusLine.enabled = false;
        selected = false;
        canvas.SetActive(false);

    }

    /// <summary>
    /// Hides the Sell option inherent to all buildings
    /// </summary>
    /// <param name="canvas">The canvas on which it is displayed</param>
    protected void ShowUI(GameObject canvas)
    {
        if(selectedBuilding != null && selectedBuilding != this)
        {
            bool temp = CanvasHover.Over;
            CanvasHover.Over = false;
            selectedBuilding.HideUI(selectedBuilding.UIOverlay);
            CanvasHover.Over = temp;
        }
        if(selectedBuilding == null)
        {
            selectedBuilding = this;
        }
        HighlightBuilding(true);
        radiusLine.enabled = true;
        selected = true;
        canvas.SetActive(true);
    }

    #endregion

    //------------PRIVATE--------------
    #region PRIVATE
    private void Start()
    {
        worldGrid = GameObject.FindWithTag("WorldGrid").GetComponent<WorldGrid>();
        if (worldGrid == null)
        {
            Debug.LogError("Could not find WorldGrid object in the scene. Either the tag was changed or the object is missing.");
        }

        initLineRenderer();
        derivedStart();
        HideUI(UIOverlay);
    }

    private void Update()
    {
        if (hasAuthority)
        {
            handleMouse();
        }

        updateAction();

    }
    #endregion

    //------------ABSTRACT-------------
    #region ABSTRACT

    /// <summary>
    /// Actions to be performed by the derived class in Start.
    /// </summary>
    protected abstract void derivedStart();

    /// <summary>
    /// Actions to be performed by the derived class in each call of Update.
    /// </summary>
    protected abstract void updateAction();
    #endregion
}
