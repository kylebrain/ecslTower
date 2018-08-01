using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Polygon {

    private float radius;
    private int sideCount;

    private int cutoffPower;

    public Polygon(int _sideCount, float _radius, int _cutoffPower = -1)
    {
        radius = _radius;
        sideCount = _sideCount;
        cutoffPower = _cutoffPower;
    }

    public float deltaTheta
    {
        get
        {
            return 360f / sideCount;
        }
    }

    public float initialAngle
    {
        get
        {
            return deltaTheta / 2f;
        }
    }

    public float apothem
    {
        get
        {
            return radius * Mathf.Cos(initialAngle * Mathf.Deg2Rad);
        }
    }

    public float sideLength
    {
        get
        {
            return 2 * radius * Mathf.Sin(initialAngle * Mathf.Deg2Rad);
        }
    }

    public Vector2 Coord(float theta)
    {
        float radAngle = (90 + theta) * Mathf.Deg2Rad;
        Vector2 direction = new Vector2(Mathf.Cos(radAngle), Mathf.Sin(radAngle));
        return cutOff(apothem * direction);
    }

    public Vector2 Vertex(float theta)
    {
        return Coord(theta) / apothem * radius;
    }

    public float zAngle(float theta)
    {
        return theta - 90f;
    }

    public float lengthBuffer(float width)
    {
        return width * Mathf.Tan(initialAngle * Mathf.Deg2Rad);
    }

    private float cutOff(float value)
    {
        if(cutoffPower > 0 && Mathf.Abs(value - Mathf.RoundToInt(value)) < Mathf.Pow(10, -cutoffPower))
        {
            return Mathf.RoundToInt(value);
        } else
        {
            return value;
        }
    }

    private Vector2 cutOff(Vector2 value)
    {
        return new Vector2(cutOff(value.x), cutOff(value.y));
    }

}
