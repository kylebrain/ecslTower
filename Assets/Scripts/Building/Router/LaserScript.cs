using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserScript : MonoBehaviour {

    private VolumetricLines.VolumetricLineBehavior laser;
    private float buffer = 0.3f;

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

    private void Update()
    {
        Ray ray = new Ray(transform.position, Vector3.left);
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit))
        {
            //Debug.Log(hit.transform.name);
            if(hit.transform.tag == "Pole")
            {
                //Debug.Log("Hit!");
                laser.EndPos = hit.transform.localPosition;
            }
        }
    }
}
