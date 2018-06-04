using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class RolodexText : MonoBehaviour {

    private Vector3 targetPos;
    private float speed;
    private float fudge;

    private void Awake()
    {
        targetPos = transform.localPosition;
        RolodexSelection parent = transform.parent.GetComponent<RolodexSelection>();
        if(parent == null)
        {
            Debug.LogError("RolodexText must have a parent of RolodexSelection!");
            return;
        }
        speed = parent.shiftSpeed;
        fudge = parent.fudge;
    }

    void Update () {
        if(Vector3.SqrMagnitude(transform.localPosition - targetPos) < fudge)
        {
            return;
        }
        transform.localPosition = Vector3.Lerp(transform.localPosition, targetPos, speed * Time.deltaTime);
	}

    public void TransitionToVector(Vector3 pos)
    {
        targetPos = pos;
    }
}
