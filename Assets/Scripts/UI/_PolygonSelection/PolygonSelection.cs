using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class PolygonSelection : MonoBehaviour {

    [HideInInspector]
    [Tooltip("Circumradius of the polygon")]
    public float radius = 100;
    [HideInInspector]
    [Tooltip("Number of sides of the polygon")]
    public int sideCount = 3;
    [Tooltip("Numbers are rounded is they are 10^-x close to the nearest integer")]
    public int cutoffExponent = 5;
    [Tooltip("Percentage from the original position out from the center")]
    public float dotBuffer = 50;

    [SerializeField]
    public string[] selections;

    public RectTransform side;
    public RectTransform dot;
    public PolygonButton transferButton;

    public PolygonTraveler traveler;

    public UnityEvent OnSelectionChanged;

    private List<PolygonVertex> vertices = new List<PolygonVertex>();
    private List<PolygonButton> buttons = new List<PolygonButton>();

    public int Selection
    {
        get
        {
            if(traveler != null)
            {
                return traveler.Selection;
            } else
            {
                return -1;
            }
        }
    }

    private List<Vector3> sideCoords = new List<Vector3>();

    private void Awake()
    {
        if(selections != null && selections.Length > 0)
        {
            Initialize(selections);
        }
    }

    public void Initialize(string [] labels)
    {
        if(vertices.Count > 0)
        {
            Debug.LogError("PolygonSelection has already been initialized!");
            return;
        }

        if (labels == null || labels.Length == 0)
        {
            Debug.Log("PolygonSelection must be initialized with a valid list!");
            return;
        }

        sideCount = labels.Length;
        Vector2 size = GetComponent<RectTransform>().sizeDelta;
        radius = (size.x + size.y) / 4f;

        Graphic graphic = GetComponent<Graphic>();
        if(graphic != null)
        {
            Destroy(graphic);
        }

        Polygon polygon = new Polygon(sideCount, radius, cutoffExponent);
        InstantiatePolygon(polygon);

        for (int i = 0; i < sideCoords.Count; i++)
        {
            Vector3 positionAndAngle = sideCoords[i];
            PolygonButton sideButton = Instantiate(transferButton, transform);

            Vector2 position = positionAndAngle;
            sideButton.transform.localPosition = position * (1 + dotBuffer / 100f);
            sideButton.transform.localEulerAngles = new Vector3(0f, 0f, positionAndAngle.z);

            sideButton.left = vertices[i];
            sideButton.right = vertices[Bounded(i + 1, vertices.Count)];

            sideButton.name = "PolyButton " + i;

            buttons.Add(sideButton);
        }

        for (int i = 0; i < vertices.Count; i++)
        {
            RectTransform vertexDot = Instantiate(dot, transform);
            vertexDot.localPosition = vertices[i].position * (1 + dotBuffer / 100f);
            vertices[i].right = buttons[i];
            //Debug.Log((i - 1) % buttons.Count);
            vertices[i].left = buttons[Bounded(i - 1, buttons.Count)];

            Text currentText = vertexDot.transform.GetChild(0).GetComponent<Text>();
            if(currentText != null && i < labels.Length)
            {
                currentText.text = labels[i];
            }
        }

        traveler = Instantiate(traveler, transform);
        int initialPosition = Random.Range(0, vertices.Count);
        traveler.SetCurrentVertex(vertices[initialPosition]);
        traveler.OnValueChanged += _SelectionChanged;
    }

    private void _SelectionChanged()
    {
        OnSelectionChanged.Invoke();
    }

    private int Bounded(int index, int bound)
    {
        if(index < 0)
        {
            return bound - 1;
        } else if(index >= bound)
        {
            return 0;
        } else
        {
            return index;
        }
    }

    private void InstantiatePolygon(Polygon polygon)
    {
        //List<float> angleList = new List<float>();
        for (int i = 0; i < sideCount; i++)
        {
            float angle = polygon.initialAngle + (polygon.deltaTheta * i);
            RectTransform currentTransform = Instantiate(side, transform);
            currentTransform.sizeDelta = new Vector2(currentTransform.sizeDelta.x, polygon.sideLength + polygon.lengthBuffer(currentTransform.sizeDelta.x));
            currentTransform.localPosition = polygon.Coord(angle);
            sideCoords.Add(new Vector3(currentTransform.localPosition.x, currentTransform.localPosition.y, 90f + polygon.zAngle(angle)));

            vertices.Add(new PolygonVertex(i, polygon.Vertex(angle - polygon.initialAngle)));

            currentTransform.localEulerAngles = new Vector3(0f, 0f, polygon.zAngle(angle));
        }
    }

    public bool MoveTraveler(PolygonButton polygonButton)
    {
        if(traveler.Moving)
        {
            return false;
        }

        PolygonVertex vertex = (polygonButton.left == traveler.currentVertex ? polygonButton.right : polygonButton.left);
        traveler.currentVertex.Enable(false);
        traveler.destinationVertex = vertex;
        return true;
    }
}
