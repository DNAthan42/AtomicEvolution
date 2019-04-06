using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent : MonoBehaviour
{
    private static int GenerationSize = 8;
    private static Agent[] Generation = new Agent[GenerationSize];
    private static int AgentSize = 9;

    private int id;
    private Atom[,,] atoms = new Atom[AgentSize, AgentSize, AgentSize];
    private Vector3 center;

    private Vector3 lastPosition = Vector3.zero;
    private double totalDistance = 0;
    private bool tracking = false;

    public delegate void DistanceCallback(double distance, int id);

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
    
    public void StartTracking(DistanceCallback distanceCallback)
    {
        tracking = true;
        lastPosition = GetHeadPos();
        atoms[(int)center.x, (int)center.y, (int)center.z].AddTrail();
        StartCoroutine(WaitForDistance(distanceCallback));
    }

    IEnumerator WaitForDistance(DistanceCallback distanceCallback)
    {
        //Debug.Log("Tracking agent");
        yield return new WaitForSeconds(15);
        distanceCallback(totalDistance, id);
        tracking = false;
        totalDistance = 0;
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

    public void AddTrail()
    {
        atoms[(int)center.x, (int)center.y, (int)center.z].AddTrail();
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
        return FromDetails(CreateAgentGameObject(), ToDetails(f), new Vector3(4,4,4));
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

    public static Agent FromDetails(Agent agent, AtomDetails[,,] details, Vector3 center)
    {
        agent.center = center;
        agent.FromDetails(details, center);

        return agent;
    }

    public static Agent BasicAgent()
    {
        Agent agent = CreateAgentGameObject();
        agent.CreateBasicAgent();
        return agent;
    }

    private static Agent CreateAgentGameObject()
    {

        int id = 0;

        for (int i = 0; i < GenerationSize; i++)
        {
            if (Generation[i] == null)
            {
                id = i;
                break;
            }
        }
        GameObject go = new GameObject($"Agent {id}");
        go.layer = id + 8; //The first 8 layers have uses in unity, avoid them.

        Agent agent = go.AddComponent<Agent>();
        Generation[id] = agent;
        return agent;
    }

    public static void Kill(Agent agent)
    {
        Generation[agent.id] = null;
        Destroy(agent.gameObject);
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

    private void CreateBasicAgent()
    {
        center = new Vector3(4, 4, 4);
        Atom atom = Atom.Create(center, this);
        atoms[4, 4, 4] = atom;
        atoms[4, 4, 4].AddChild(Enums.Direction.Aft);

        atoms[4, 4, 3] = Atom.CreateRandom(new Vector3(4, 4, 3), this, Enums.Direction.Fore);

    }
}
