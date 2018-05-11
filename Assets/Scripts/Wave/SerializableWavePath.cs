using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Coordinate
{
    public int x;
    public int y;

    public Coordinate(int X, int Y)
    {
        x = X;
        y = Y;
    }
}

[System.Serializable]
public class SerializableWavePath
{
    public List<Coordinate> list = new List<Coordinate>();

    public SerializableWavePath(WavePath path)
    {
        List<Node> tempNodeList = new List<Node>(path.NodeQueue);

        foreach (Node n in tempNodeList)
        {
            list.Add(new Coordinate(n.Coordinate.x, n.Coordinate.y));
        }
    }

}
