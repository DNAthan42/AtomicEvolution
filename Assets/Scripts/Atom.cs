using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Atom : MonoBehaviour
{

    private Rigidbody rb;

    public static Atom Create(Vector3 pos)
    {
        GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
        Atom atom = gameObject.AddComponent<Atom>();
        atom.Create(pos, gameObject.AddComponent<Rigidbody>());

        return atom;
    }

    private void Create(Vector3 pos, Rigidbody rb)
    {
        this.rb = rb;
        transform.localPosition = pos;
        transform.localScale *= .5f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
