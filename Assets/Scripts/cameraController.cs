using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class cameraController : MonoBehaviour{
    //Public
    public float leftBound;
    public float rightBound;

    public float bottomBound;
    public float topBound;

    public float minZoom;
    public float maxZoom;

    public float moveSpeed;
    public float zoomSpeed;

    public float cursorScrollTolerance;

    //Private
    private Camera cam;
    float deltaX = 0;
    float deltaZ = 0;
    float targetY;
    

    private void Start()
    {
        cam = GetComponent<Camera>();
        targetY = transform.position.y;
    }


    void FixedUpdate () {
        //Desired movement vector
        Vector3 movement = Vector3.Normalize(new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical")));
        movement += new Vector3(0f, -Input.GetAxisRaw("Mouse ScrollWheel"), 0f);
        movement.Scale(new Vector3(moveSpeed, zoomSpeed, moveSpeed));

        //Check if the cursor is in the game view
        Vector3 mouse = cam.ScreenToViewportPoint(Input.mousePosition);
        bool mouseInGameView = (mouse.x >= 0) && (mouse.x <= 1) && (mouse.y >= 0) && (mouse.y <= 1);

        //Assign keyboard movement
        deltaX = movement.x;        
        deltaZ = movement.z;

        if(mouseInGameView) {
            //-------Left, right----------------
            if(Mathf.Abs(mouse.x) <= cursorScrollTolerance) {
                deltaX -= moveSpeed;
            }
            if(Mathf.Abs(mouse.x - 1) <= cursorScrollTolerance) {
                deltaX += moveSpeed;
            }

            //---------Forward, backward----------
            if(Mathf.Abs(mouse.y) <= cursorScrollTolerance) {
                deltaZ -= moveSpeed;
            }
            if(Mathf.Abs(mouse.y - 1) <= cursorScrollTolerance) {
                deltaZ += moveSpeed;
            }

            //------Zoom in, out----------------
            if(movement.y != 0) {
                targetY = Mathf.Clamp(transform.position.y + movement.y, minZoom, maxZoom);
            }
        }




    }

    private void LateUpdate(){
        //Edges of the screen
        Vector3 bottomLeft = cam.ScreenToWorldPoint(new Vector3(0, 0, transform.position.y));
        Vector3 topRight = cam.ScreenToWorldPoint(new Vector3(cam.pixelWidth, cam.pixelHeight, transform.position.y));

        float leftEdge = bottomLeft.x;
        float rightEdge = topRight.x;
        float bottomEdge = bottomLeft.z;
        float topEdge = topRight.z;

        //Keep the edges of the creen within the specified area
        deltaX = softBound(leftEdge, rightEdge, leftBound, rightBound, deltaX);
        deltaZ = softBound(bottomEdge, topEdge, bottomBound, topBound, deltaZ);

        //------------Set transform-----------
        Vector3 newPos = Vector3.ClampMagnitude(new Vector3(deltaX, 0f, deltaZ), moveSpeed) * Time.deltaTime;
        if(Input.GetKey(KeyCode.LeftShift)) {
            newPos.Scale(new Vector3(2f, 1f, 2f));
        }
        transform.position += newPos;
        transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, targetY, transform.position.z), Time.deltaTime * 5f);
    }




    /* Keeps minEdge and maxEdge to the range [minBound, maxBound].
     * If deltaMovement's current value would cause minEdge to go
     * further below minBOund, or maxEdge to go further above maxBound,
     * it corrects it by adjusting deltaMovement to
     * move back within target range.
     */
    float softBound(float minEdge, float maxEdge, float minBound, float maxBound, float deltaMovement){

        //Both are past their limits, so compromise
        if (minEdge < minBound && maxEdge > maxBound){
            deltaMovement = minBound - minEdge + maxBound - maxEdge;

        //Above the limit on the max side, and not already trying to correct it with deltaMovement
        } else if ( (maxEdge > maxBound) && (deltaMovement > maxBound - maxEdge) ){
            deltaMovement = maxBound - maxEdge;

        //Below the limit on the min side, and not already trying to correct it with deltaMovement
        } else if ( (minEdge < minBound) && (deltaMovement < minBound - minEdge) ){
            deltaMovement = minBound- minEdge;
        }

        return deltaMovement;
    }



}
