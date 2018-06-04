using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateText : MonoBehaviour {

    private Vector3 targetPos;
    public float speed = 1f;
    public float fudge = 0.001f;

    private void Start()
    {
        targetPos = transform.localPosition;
    }

    // Update is called once per frame
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
