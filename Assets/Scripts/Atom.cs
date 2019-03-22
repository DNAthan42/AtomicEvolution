using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Atom : MonoBehaviour
{
    #region statics
    public static Atom Create(Vector3 pos)
    {
        Enums.Shape shape = Enums.GetRandomShape();
        GameObject gameObject = CreateRandomShape(shape);            

        Atom atom = gameObject.AddComponent<Atom>();
        Rigidbody rb = gameObject.AddComponent<Rigidbody>();
        rb.useGravity = false;
        atom.Create(pos, rb, shape);

        return atom;
    }

    private static GameObject CreateRandomShape(Enums.Shape shape)
    {
        GameObject gameObject;

        switch (shape)
        {
            case Enums.Shape.Cylinder:
                gameObject = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                break;
            case Enums.Shape.Sphere:
                gameObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                break;
            case Enums.Shape.Cube:
            default:
                gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
                break;
        }
        return gameObject;
    }
    #endregion

    private Rigidbody rb;
    private Enums.Shape shape;

    private void Create(Vector3 pos, Rigidbody rb, Enums.Shape shape)
    {
        this.rb = rb;
        this.shape = shape;
        transform.localPosition = pos;
        Resize();
    }

    private void Resize()
    {
        Vector3 prev;
        switch (shape)
        {
            case Enums.Shape.Cube:
                transform.localScale *= .5f;
                break;
            case Enums.Shape.Cylinder:
                prev = transform.localScale;
                transform.localScale = new Vector3(prev.x * .75f, prev.y * .3f, prev.z * .75f);
                break;
            case Enums.Shape.Sphere:
                prev = transform.localScale;
                transform.localScale = new Vector3(prev.x * .95f, prev.y * .95f, prev.z * .95f);
                break;
            default:
                break;
        }
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
