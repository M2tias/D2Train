using System;
using UnityEngine;

[Serializable]
public class Pair<T> : Pair<T, GameObject> { }

[Serializable]
public class Pair<T, U>
{
    public T Key;
    public U Value;
}