using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enums : MonoBehaviour
{
    public enum Motion { None, Linear, Rotational/*, Periodic*/ };

    public enum Shape { Cube, Sphere, Cylinder };

    public enum Direction { Port, Starboard, Below, Above, Aft, Fore };

    public static Shape GetRandomShape()
    {
        return (Shape)Random.Range(0, 3);
    }

    public static Motion GetRandomMotion()
    {
        return (Motion)Random.Range(0, 3);
    }

    public static Vector3 InDirection(Vector3 source, Direction direction)
    {
        int x, y, z;
        x = y = z = 0;

        switch (direction)
        {
            case (Direction.Port):
                x += -1;
                break;
            case (Direction.Starboard):
                x += 1;
                break;
            case (Direction.Below):
                y += -1;
                break;
            case (Direction.Above):
                y += 1;
                break;
            case (Direction.Aft):
                z += -1;
                break;
            case (Direction.Fore):
                z += 1;
                break;
            default:
                break;
        }

        return source + new Vector3(x, y, z);
    }
}
