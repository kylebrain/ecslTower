using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingRingDisplayAgent : MonoBehaviour {

    public float transitionSpeed;
    public float fudge = 0.001f;

    [HideInInspector]
    public RingDisplayAgent displayAgent;
    public Vector2 DesiredPos
    {
        get
        {
            return desiredPos;
        }
    }
    public Vector2 desiredPos;

    public RectTransform rectTransform
    {
        get
        {
            return GetComponent<RectTransform>();
        }
    }

    public bool IsAtDestination
    {
        get
        {
            return Vector3.SqrMagnitude(rectTransform.anchoredPosition - desiredPos) < fudge;
        }
    }

    private void Awake()
    {
        displayAgent = GetComponentInChildren<RingDisplayAgent>();
        if(displayAgent == null)
        {
            Debug.Log("Could not find Agent!");
        }
    }

    public void InitDisplayAgent()
    {
        displayAgent = GetComponentInChildren<RingDisplayAgent>();
        if (displayAgent == null)
        {
            Debug.Log("Could not find Agent!");
        }
    }

    private void Update()
    {
        if (IsAtDestination)
        {
            return;
        }
        rectTransform.anchoredPosition = Vector3.Lerp(rectTransform.anchoredPosition, desiredPos, transitionSpeed * Time.deltaTime);
    }

    public void SetPosition(Vector2 _position)
    {
        rectTransform.anchoredPosition = _position;
        desiredPos = _position;
    }

    public void SetDesiredPos(Vector2 _desiredPos)
    {
        desiredPos = _desiredPos;
    }

}
