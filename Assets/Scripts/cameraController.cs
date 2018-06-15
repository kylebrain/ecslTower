using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraController : MonoBehaviour
{
    //Public
    /// <summary>
    /// Moving the cursor this close to the edge of the screenview will scroll the screen.
    /// </summary>
    [Range(0.0f, 0.5f)]
    public float EdgeScrollTolerance;

    [Header("Pitch Settings")]
    public bool topDown = true;
    [Range(0.0f, 90.0f)]
    public float angledPitch = 60;
    public float angleRange = 10f;
    public float pitchMod = 5f;
    public float anglePitchResetRate = 0.5f;
    public float anglePitchSetRate = 0.05f;
    public float transitionalPitchFudge = 0.001f;
    private float targetPitch;

    //if the passed directional float is 0, move towards reseting the pitch to normal
    //if it is in some direction, move in that direction
    private float TransitionalPitch
    {
        get
        {
            return transitionalPitch;
        }
        set
        {
            if (value == 0)
            {
                if (transitionalPitch > 0 )
                {
                    transitionalPitch -= anglePitchResetRate;
                    if(transitionalPitch < 0)
                    {
                        transitionalPitch = 0;
                    }
                } else
                {
                    transitionalPitch += anglePitchResetRate;
                    if (transitionalPitch > 0)
                    {
                        transitionalPitch = 0;
                    }
                }
            } else
            {
                transitionalPitch += value * anglePitchSetRate;
                if(Mathf.Abs(transitionalPitch) > angleRange)
                {
                    transitionalPitch = angleRange * Mathf.Sign(transitionalPitch);
                }
            }
        }
    }
    private float transitionalPitch;

    //Private
    private Camera cam;
    private float deltaX = 0;
    private float deltaZ = 0;
    private float targetY;

    private float leftBound;
    private float rightBound;

    private float bottomBound;
    private float topBound;

    private float minZoom;
    private float maxZoom;

    private float moveSpeed;
    private float zoomSpeed;


    private WorldGrid worldGrid;


    private void Start()
    {
        cam = GetComponent<Camera>();
        cam.fieldOfView = 60;
        targetY = transform.position.y;
        worldGrid = GameObject.FindWithTag("WorldGrid").GetComponent<WorldGrid>();
        if (worldGrid == null)
        {
            Debug.LogError("Could not find WorldGrid object in the scene. Either the tag was changed or the object is missing.");
        }
        initBounds();
        if (topDown)
        {
            ResetTopDownCameraPitch();
        } else
        {
            ResetAngleCameraPitch();
        }
    }


    private void FixedUpdate()
    {
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

        if (mouseInGameView)
        {
            if (EdgeScrollTolerance != 0f)
            {
                //-------Left, right----------------
                if (mouse.x <= EdgeScrollTolerance)
                {
                    deltaX -= moveSpeed;
                }
                if ((1 - mouse.x) <= EdgeScrollTolerance)
                {
                    deltaX += moveSpeed;
                }

                //---------Forward, backward----------
                if (mouse.y <= EdgeScrollTolerance)
                {
                    deltaZ -= moveSpeed;
                }
                if ((1 - mouse.y) <= EdgeScrollTolerance)
                {
                    deltaZ += moveSpeed;
                }
            }

            //------Zoom in, out----------------
            if (movement.y != 0)
            {
                targetY = Mathf.Clamp(transform.position.y + movement.y, minZoom, maxZoom);
            }
        }

        TransitionalPitch = deltaZ;

        if (!topDown) {

            float cameraIncrease;
            float cameraPitch = transform.localEulerAngles.x;
            if (Input.GetKey(KeyCode.F))
            {
                cameraIncrease = cameraPitch - Input.GetAxisRaw("Mouse Y") * pitchMod;
                cameraIncrease = Mathf.Clamp(cameraIncrease, 0, 90);
                targetPitch = cameraIncrease;
            }
            else
            {
                cameraIncrease = targetPitch - TransitionalPitch;
                cameraIncrease = Mathf.Clamp(cameraIncrease, 0, 90);
            }
            
            transform.localEulerAngles = new Vector3(cameraIncrease, transform.localEulerAngles.y, transform.localEulerAngles.z);
        }

        

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (topDown)
            {
                topDown = false;
                ResetAngleCameraPitch();
                Debug.Log("Angled camera mode");
            }
            else if (Mathf.Abs(transform.localEulerAngles.x - angledPitch) <= angleRange)
            {
                topDown = true;
                ResetTopDownCameraPitch();
                Debug.Log("Top down camera mode");
            }
            else
            {
                ResetAngleCameraPitch();
                Debug.Log("Reset position");
            }
        }
    }

    private void ResetAngleCameraPitch()
    {
        transform.localEulerAngles = new Vector3(angledPitch, transform.localEulerAngles.y, transform.localEulerAngles.z);
        targetPitch = angledPitch;
    }

    private void ResetTopDownCameraPitch()
    {
        transform.localEulerAngles = Vector3.right * 90; //perpendicular to plane
    }

    private void LateUpdate()
    {
        //Edges of the screen
        Vector3 bottomLeft = cam.ScreenToWorldPoint(new Vector3(0, 0, transform.position.y));
        Vector3 topRight = cam.ScreenToWorldPoint(new Vector3(cam.pixelWidth, cam.pixelHeight, transform.position.y));

        float leftEdge = bottomLeft.x;
        float rightEdge = topRight.x;
        float bottomEdge = bottomLeft.z;
        float topEdge = topRight.z;

        //Keep the edges of the screen within the specified area
        deltaX = softBound(leftEdge, rightEdge, leftBound, rightBound, deltaX);
        deltaZ = softBound(bottomEdge, topEdge, bottomBound, topBound, deltaZ);

        //------------Set transform-----------
        Vector3 newPos = Vector3.ClampMagnitude(new Vector3(deltaX, 0f, deltaZ), moveSpeed) * Time.deltaTime;
        if (Input.GetKey(KeyCode.LeftShift))
        {
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
    private float softBound(float minEdge, float maxEdge, float minBound, float maxBound, float deltaMovement)
    {

        //Both are past their limits, so compromise
        if (minEdge < minBound && maxEdge > maxBound)
        {
            deltaMovement = minBound - minEdge + maxBound - maxEdge;

            //Above the limit on the max side, and not already trying to correct it with deltaMovement
        }
        else if ((maxEdge > maxBound) && (deltaMovement > maxBound - maxEdge))
        {
            deltaMovement = maxBound - maxEdge;

            //Below the limit on the min side, and not already trying to correct it with deltaMovement
        }
        else if ((minEdge < minBound) && (deltaMovement < minBound - minEdge))
        {
            deltaMovement = minBound - minEdge;
        }

        return deltaMovement;
    }

    private void initBounds()
    {
        /*
        private float leftBound;
        private float rightBound;

        private float bottomBound;
        private float topBound;

        private float minZoom;
        private float maxZoom;
        */
        float tolerance = 0.25f;

        leftBound = -tolerance * worldGrid.width;
        rightBound = (1 + tolerance) * worldGrid.width;

        bottomBound = -tolerance * worldGrid.height;
        topBound = (1 + tolerance) * worldGrid.height;

        /*We want the horizontal viewing distance at maxZoom to be 1.5 * the width of the worldGrid.
         * We want the horizontal viewing distance at minZoom to be around 5-10 tiles. We chose 7.
         * We can get these by using trig, which simplify to the equations below.
         */
        float sqrt3Over2 = 0.8660254038f;
        maxZoom = sqrt3Over2 * (1.5f * worldGrid.width);
        minZoom = sqrt3Over2 * 7;


        zoomSpeed = Mathf.Sqrt(worldGrid.width * worldGrid.height);
        moveSpeed = 5f + Mathf.Sqrt(zoomSpeed);
    }
}


