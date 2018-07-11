using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingRingDisplayAgent : RingDisplayAgent {

    public Vector2 desiredPos;
    public float transitionSpeed;
    public float fudge = 0.001f;
    public bool IsAtDestination
    {
        get
        {
            return Vector3.SqrMagnitude(center - desiredPos) < fudge;
        }
    }

    protected override void UpdateAction()
    {
        if (IsAtDestination)
        {
            return;
        }
        center = Vector3.Lerp(center, desiredPos, transitionSpeed * Time.deltaTime);
    }

    public void SetCenter(Vector2 _center)
    {
        center = _center;
        desiredPos = _center;
        UpdatePosition();
    }

    public void SetDesiredPos(Vector2 _desiredPos)
    {
        desiredPos = _desiredPos;
    }

}
