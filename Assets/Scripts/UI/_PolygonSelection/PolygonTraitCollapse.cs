using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class PolygonTraitCollapse : MonoBehaviour {

    public RectTransform hideBar;

    public bool left = true;

    public RectTransform rectTransform
    {
        get
        {
            return GetComponent<RectTransform>();
        }
    }

    float speed = 5;

    float collapsed;
    float expanded;

    bool isCollapsed = true;



    private void Awake()
    {
        if(hideBar == null)
        {
            Debug.LogError("Please attach a hide bar gameObject!");
            return;
        }
        collapsed = hideBar.sizeDelta.x / 2f;
        expanded = rectTransform.sizeDelta.x / 2f;
    }

    private void Update()
    {
        int direction = left ? 1 : -1;
        rectTransform.anchoredPosition = Vector2.Lerp(rectTransform.anchoredPosition, new Vector2( direction *  (isCollapsed ? collapsed : expanded), rectTransform.anchoredPosition.y), speed * Time.deltaTime);
    }

    public void Collapse(bool _collapsed)
    {
        isCollapsed = _collapsed;
    }
}
