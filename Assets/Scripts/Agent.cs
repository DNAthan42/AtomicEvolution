using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent : MonoBehaviour
{
    private static int AgentSize = 9;
    private Atom[,,] atoms;
    private Vector3 center;

    // Start is called before the first frame update
    void Start()
    {
        atoms = new Atom[AgentSize, AgentSize, AgentSize];
        center = new Vector3(4, 4, 4);
        Atom atom = Atom.Create(center);
        atoms[4, 4, 4] = atom;

        last = center;
    }

    Vector3 last;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            last += Vector3.forward;
            AddAtom(last);
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            last.x -= 1;
            AddAtom(last);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            last.z -= 1;
            AddAtom(last);
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            last.x += 1;
            AddAtom(last);
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            last.y += 1;
            AddAtom(last);
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            last.y -= 1;
            AddAtom(last);
        }
    }

    void AddAtom(Vector3 pos)
    {
        if(atoms[(int)pos.x, (int)pos.y, (int)pos.z] == null)
            atoms[(int)pos.x, (int)pos.y, (int)pos.z] = Atom.Create(pos);

        

    }
}
