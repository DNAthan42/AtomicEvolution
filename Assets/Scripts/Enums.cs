using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enums : MonoBehaviour
{
    public enum Motion { Linear, Rotational/*, Periodic*/ };

    public enum Shape { Cube, Sphere, Cylinder };

    public static Shape GetRandomShape()
    {
        return (Shape)Random.Range(0, 3);
    }

    public static Motion GetRandomMotion()
    {
        return (Motion)Random.Range(0, 2);
    }
}
