using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserScript : MonoBehaviour {

    private VolumetricLines.VolumetricLineBehavior laser;
    private float buffer = 0.0f;

    private void Start()
    {
        laser = transform.parent.Find("Laser").GetComponent<VolumetricLines.VolumetricLineBehavior>();
        if (laser == null)
        {
            Debug.LogError("Could not find laser child!");
            return;
        }
        laser.StartPos = transform.localPosition + Vector3.left * buffer;
    }

    private void FixedUpdate()
    {
        Ray ray = new Ray(transform.position, transform.parent.TransformDirection(Vector3.left));
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit))
        {
            if(hit.transform.tag == "Pole")
            {
                laser.EndPos = hit.transform.localPosition;
            }
            if(hit.transform.tag == "Agent")
            {
                laser.EndPos = transform.parent.InverseTransformPoint(hit.transform.position);
            }
            laser.EndPos = new Vector3(laser.EndPos.x, laser.StartPos.y, laser.StartPos.z);
        }
    }
}
