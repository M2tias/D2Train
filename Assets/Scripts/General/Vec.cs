using UnityEngine;

public static class Vec
{
    public static Vector2 V3to2(Vector3 v)
    {
        return new Vector2(v.x, v.z);
    }

    public static Vector2 CellPos(Vector3 v)
    {
        float x = Mathf.Floor(v.x) + 0.5f;
        float z = Mathf.Floor(v.z) + 0.5f;

        return new Vector2(x, z);
    }

    public static bool isNotSameCell(Vector2 cellPos, Vector3 pos)
    {
        return Mathf.Abs(cellPos.x - pos.x) > 0.5f || Mathf.Abs(cellPos.y - pos.z) > 0.5f;
    }
    public static bool isNotSameCell(Vector2 cellPos, Vector2 pos)
    {
        return Mathf.Abs(cellPos.x - pos.x) > 0.5f || Mathf.Abs(cellPos.y - pos.y) > 0.5f;
    }
}