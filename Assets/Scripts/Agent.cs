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
        reset();

    }

    //Vector3 last;
    //Atom lastAtom;
    //void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.W))
    //    {
    //        last += Vector3.forward;
    //        AddAtom(last);
    //    }
    //    if (Input.GetKeyDown(KeyCode.A))
    //    {
    //        last.x -= 1;
    //        AddAtom(last);
    //    }
    //    if (Input.GetKeyDown(KeyCode.S))
    //    {
    //        last.z -= 1;
    //        AddAtom(last);
    //    }
    //    if (Input.GetKeyDown(KeyCode.D))
    //    {
    //        last.x += 1;
    //        AddAtom(last);
    //    }
    //    if (Input.GetKeyDown(KeyCode.R))
    //    {
    //        last.y += 1;
    //        AddAtom(last);
    //    }
    //    if (Input.GetKeyDown(KeyCode.F))
    //    {
    //        last.y -= 1;
    //        AddAtom(last);
    //    }

    //    if (Input.GetKeyDown(KeyCode.Space)) atoms[4, 4, 4].rb.constraints = RigidbodyConstraints.None;
    //}

    //void AddAtom(Vector3 pos)
    //{
    //    if(atoms[(int)pos.x, (int)pos.y, (int)pos.z] == null)
    //        atoms[(int)pos.x, (int)pos.y, (int)pos.z] = lastAtom = Atom.CreateRandom(pos, lastAtom);

        

    //}
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            reset();
            foreach (Atom a in atoms) if (a != null) a.Mutate();
        }
    }

    void reset()
    {
        foreach (Atom a in atoms) if (a != null) Destroy(a.gameObject);
        center = new Vector3(4, 4, 4);
        Atom atom = Atom.Create(center, this);
        atoms[4, 4, 4] = atom;

        //last = center;
        //lastAtom = atom;
        //atom.rb.constraints = RigidbodyConstraints.FreezeAll;

        sample();
    }

    void sample()
    {
        atoms[4, 4, 3] = Atom.CreateRandom(new Vector3(4, 4, 3), this, Enums.Direction.Aft);
        atoms[4, 4, 5] = Atom.CreateRandom(new Vector3(4, 4, 5), this, Enums.Direction.Fore);
        atoms[3, 4, 4] = Atom.CreateRandom(new Vector3(3, 4, 4), this, Enums.Direction.Port);
        atoms[5, 4, 4] = Atom.CreateRandom(new Vector3(5, 4, 4), this, Enums.Direction.Starboard);
    }

    public Atom getParent(Vector3 pos, Enums.Direction direction)
    {
        Vector3 parentLoc = Enums.InDirection(pos, direction);
        return atoms[(int)parentLoc.x, (int)parentLoc.y, (int)parentLoc.z];
    }
}
