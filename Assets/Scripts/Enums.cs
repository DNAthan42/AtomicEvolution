using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enums : MonoBehaviour
{
    public enum Motion { None, Linear, Rotational/*, Periodic*/ };

    public enum Shape { Cube, Sphere, Cylinder };

    public enum Direction { None, Port, Starboard, Below, Above, Aft, Fore };

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
            case (Direction.None):
            default:
                break;
        }

        return source + new Vector3(x, y, z);
    }

    public static Direction Reverse(Direction direction)
    {
        switch (direction)
        {
            case Direction.Port:
                return Direction.Starboard;
            case Direction.Starboard:
                return Direction.Port;
            case Direction.Below:
                return Direction.Above;
            case Direction.Above:
                return Direction.Below;
            case Direction.Aft:
                return Direction.Fore;
            case Direction.Fore:
                return Direction.Aft;
            default:
            case Direction.None:
                return Direction.None;
        }
    }
}
