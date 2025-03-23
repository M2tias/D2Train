using System.Collections.Generic;
using UnityEngine;

public static class NeighbourHelper
{
    public static readonly Dictionary<Neighbour, Vector2> NeighbourPositions = new() {
        { Neighbour.Left, Vector2.left },
        { Neighbour.Right, Vector2.right },
        { Neighbour.Top, Vector2.up },
        { Neighbour.Bottom, Vector2.down }
    };

    public static readonly Dictionary<Neighbour, Neighbour> ReverseNeighbour = new()
    {
        {Neighbour.Left, Neighbour.Right },
        {Neighbour.Right, Neighbour.Left },
        {Neighbour.Top, Neighbour.Bottom },
        {Neighbour.Bottom, Neighbour.Top },
    };
}

public enum Neighbour
{
    Left,
    Right,
    Top,
    Bottom
}