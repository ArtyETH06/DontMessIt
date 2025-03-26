using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GuardianData
{
    public List<Vector3Serializable> points = new List<Vector3Serializable>();
}

[System.Serializable]
public struct Vector3Serializable
{
    public float x, y, z;

    public Vector3Serializable(Vector3 v)
    {
        x = v.x;
        y = v.y;
        z = v.z;
    }

    public Vector3 ToVector3()
    {
        return new Vector3(x, y, z);
    }
}
