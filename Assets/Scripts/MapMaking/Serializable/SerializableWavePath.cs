using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Basic X/Y coordinate that can be Serialized
/// </summary>
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

/// <summary>
/// List of Coordinates that represents a WavePath but can be Serialized
/// </summary>
[System.Serializable]
public class SerializableWavePath
{
    /// <summary>
    /// The List of Coordinates that holds the data
    /// </summary>
    public List<Coordinate> list = new List<Coordinate>();

    /// <summary>
    /// Constructs based on a WavePath to allow Serialization
    /// </summary>
    /// <param name="path">WavePath to be create into an object that can be Serialized</param>
    public SerializableWavePath(WavePath path)
    {
        List<Node> tempNodeList = new List<Node>(path.NodeQueue);

        foreach (Node n in tempNodeList)
        {
            list.Add(new Coordinate(n.Coordinate.x, n.Coordinate.y));
        }
    }

}
