using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Atom : MonoBehaviour
{
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    private BoxCollider boxCollider;

    public static Atom Create(Vector3 pos)
    {
        GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
        Atom atom = gameObject.AddComponent<Atom>();
        atom.Create(gameObject.GetComponent<MeshFilter>(), 
            gameObject.GetComponent<MeshRenderer>(), 
            gameObject.GetComponent<BoxCollider>(), 
            pos);

        return atom;
    }

    private void Create(MeshFilter filter, MeshRenderer renderer, BoxCollider collider, Vector3 pos)
    {
        meshFilter = filter;
        meshRenderer = renderer;
        boxCollider = collider;

        transform.localPosition = pos;
        transform.localScale *= .5f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
