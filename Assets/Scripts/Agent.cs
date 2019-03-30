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
        Atom atom = Atom.Create(center, this);
        atoms[4, 4, 4] = atom;

        sample();

    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            reset();
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            Debug.Log(Serialize());
            //foreach (Atom atom in atoms)
            //{
            //    if (atom != null) Debug.Log(atom.Serialize());
            //}
        }
    }

    void reset()
    {
        AtomDetails[,,] details = new AtomDetails[AgentSize, AgentSize, AgentSize];
        Vector3 center = Vector3.zero;

        //retrieve the atom details and clear the atoms
        for (int i = 0; i < AgentSize; i++)
        {
            for (int j = 0; j < AgentSize; j++)
            {
                for (int k = 0; k < AgentSize; k++)
                {
                    if (atoms[i, j, k] == null) continue;

                    if (atoms[i, j, k].parent == Enums.Direction.None) center = new Vector3(i, j, k);
                    else atoms[i, j, k].Mutate();
                    details[i, j, k] = atoms[i, j, k].details;

                    Destroy(atoms[i, j, k].gameObject); //clear the array
                }
            }
        }

        //recreate the base atom
        Atom atom = Atom.Create(center, this);
        atom.details = details[(int)center.x, (int)center.y, (int)center.z];
        atoms[(int)center.x, (int)center.y, (int)center.z] = atom;
        for (int i = 0; i < atom.details.children.Length; i++) //create the children
        {
            if (atom.details.children[i])
            {
                Enums.Direction direction = (Enums.Direction)(i + 1);
                Vector3 pos = Enums.InDirection(center, direction);
                Atom child = Atom.Create(pos, this, Enums.Reverse(direction), details[(int)pos.x, (int)pos.y, (int)pos.z]);
                atoms[(int)pos.x, (int)pos.y, (int)pos.z] = child;
            }
        }
    }

    void sample()
    {
        atoms[4, 4, 3] = Atom.CreateRandom(new Vector3(4, 4, 3), this, Enums.Direction.Fore);
        atoms[4, 4, 5] = Atom.CreateRandom(new Vector3(4, 4, 5), this, Enums.Direction.Aft);
        atoms[3, 4, 4] = Atom.CreateRandom(new Vector3(3, 4, 4), this, Enums.Direction.Starboard);
        atoms[5, 4, 4] = Atom.CreateRandom(new Vector3(5, 4, 4), this, Enums.Direction.Port);
        Atom center = atoms[4, 4, 4];
        center.AddChild(Enums.Direction.Aft);
        center.AddChild(Enums.Direction.Fore);
        center.AddChild(Enums.Direction.Port);
        center.AddChild(Enums.Direction.Starboard);
    }

    public Atom getParent(Vector3 pos, Enums.Direction direction)
    {
        Vector3 parentLoc = Enums.InDirection(pos, direction);
        return atoms[(int)parentLoc.x, (int)parentLoc.y, (int)parentLoc.z];
    }

    public string Serialize()
    {
        string agent = "";
        bool first = true;
        for (int i = 0; i < AgentSize; i++)
            for (int k = 0; k < AgentSize; k++)
                for (int j = 0; j < AgentSize; j++)
                {
                    if (first) first = false;
                    else agent += ",";

                    if (atoms[i, j, k] != null) agent += atoms[i, j, k].Serialize();
                }
        return agent;
    }
}
