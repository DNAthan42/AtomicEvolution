using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent : MonoBehaviour
{
    private static int AgentSize = 9;
    private Atom[,,] atoms = new Atom[AgentSize, AgentSize, AgentSize];
    private Vector3 center;

    private Vector3 lastPosition = Vector3.zero;
    private double totalDistance = 0;
    private bool tracking = false;

    // Start is called before the first frame update
    void Start()
    {

    }

    void Update()
    { 
        if (Input.GetKeyDown(KeyCode.S))
        {
            Debug.Log(Serialize());
            //foreach (Atom atom in atoms)
            //{
            //    if (atom != null) Debug.Log(atom.Serialize());
            //}
        }

        if (tracking)
        {
            Vector3 thisPosition = GetHeadPos();
            totalDistance += Vector3.Distance(lastPosition, thisPosition);
            lastPosition = thisPosition;
        }
    }
    
    public void StartTracking()
    {
        tracking = true;
        lastPosition = GetHeadPos();
        atoms[(int)center.x, (int)center.y, (int)center.z].AddTrail();
        StartCoroutine(WaitForDistance());
    }

    IEnumerator WaitForDistance()
    {
        Debug.Log("Tracking agent");
        yield return new WaitForSeconds(15);
        Debug.Log($"Distance: {totalDistance}");
        tracking = false;
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
        Agent.FromDetails(new GameObject("agent"), details, center);
        Destroy(this.gameObject);
    }

    public void sample()
    {
        center = new Vector3(4, 4, 4);
        Atom atom = Atom.Create(center, this);
        atoms[4, 4, 4] = atom;

        atoms[4, 4, 3] = Atom.CreateRandom(new Vector3(4, 4, 3), this, Enums.Direction.Fore);
        atoms[4, 4, 5] = Atom.CreateRandom(new Vector3(4, 4, 5), this, Enums.Direction.Aft);
        atoms[3, 4, 4] = Atom.CreateRandom(new Vector3(3, 4, 4), this, Enums.Direction.Starboard);
        atoms[5, 4, 4] = Atom.CreateRandom(new Vector3(5, 4, 4), this, Enums.Direction.Port);
        Atom centerAtom = atoms[4, 4, 4];
        centerAtom.AddChild(Enums.Direction.Aft);
        centerAtom.AddChild(Enums.Direction.Fore);
        centerAtom.AddChild(Enums.Direction.Port);
        centerAtom.AddChild(Enums.Direction.Starboard);
    }

    public Atom getParent(Vector3 pos, Enums.Direction direction)
    {
        Vector3 parentLoc = Enums.InDirection(pos, direction);
        return atoms[(int)parentLoc.x, (int)parentLoc.y, (int)parentLoc.z];
    }

    public string Serialize()
    {
        return Serialize(atoms);
    }

    public Vector3 GetHeadPos()
    {
        return atoms[(int)center.x, (int)center.y, (int)center.z].transform.position;
    }

    #region Statics


    public static string Serialize(Atom[,,] atoms)
    {
        string agent = "";
        bool first = true;
        for (int i = 0; i < AgentSize; i++)
            for (int k = 0; k < AgentSize; k++)
                for (int j = 0; j < AgentSize; j++)
                {
                    if (first) first = false;
                    else agent += ";";

                    if (atoms[i, j, k] != null) agent += atoms[i, j, k].Serialize();
                }
        return agent;
    }

    public static string Serialize(AtomDetails[,,] atoms)
    {
        string agent = "";
        bool first = true;
        for (int i = 0; i < AgentSize; i++)
            for (int k = 0; k < AgentSize; k++)
                for (int j = 0; j < AgentSize; j++)
                {
                    if (first) first = false;
                    else agent += ";";

                    if (atoms[i, j, k] != null) agent += atoms[i, j, k].Serialize();
                }
        return agent;
    }

    public static Agent Deserialize(string f)
    {
        return FromDetails(new GameObject("Agent"), ToDetails(f), new Vector3(4,4,4));
    }

    public static AtomDetails[,,] ToDetails(string f)
    {
        string[] pieces = f.Split(';');
        AtomDetails[,,] details = new AtomDetails[AgentSize, AgentSize, AgentSize];
        int total = 0;
        for (int i = 0; i < AgentSize; i++)
            for (int k = 0; k < AgentSize; k++)
                for (int j = 0; j < AgentSize; j++)
                {
                    if (pieces[total] != "")
                    {
                        details[i, j, k] = Atom.Deserialize(pieces[total]);
                    }
                    total++;
                }
        return details;
    }

    public static Agent FromDetails(GameObject gameObject, AtomDetails[,,] details, Vector3 center)
    {
        Agent agent = gameObject.AddComponent<Agent>();
        agent.center = center;
        agent.FromDetails(details, center);

        return agent;
    }

    #endregion

    private void FromDetails(AtomDetails[,,] details, Vector3 here, Atom atom = null)
    {
        //Used for the initial case, creates the default starter atom
        if (atom == null) atom = Atom.Create(here, this);

        atom.details = details[(int)here.x, (int)here.y, (int)here.z];
        atoms[(int)here.x, (int)here.y, (int)here.z] = atom;
        for (int i = 0; i < atom.details.children.Length; i++) //create the children
        {
            if (atom.details.children[i])
            {
                Enums.Direction direction = (Enums.Direction)(i + 1);
                Vector3 pos = Enums.InDirection(here, direction);
                Atom child = Atom.Create(pos, this, Enums.Reverse(direction), details[(int)pos.x, (int)pos.y, (int)pos.z]);
                atoms[(int)pos.x, (int)pos.y, (int)pos.z] = child;
                FromDetails(details, pos, child);
            }
        }
    }
}
