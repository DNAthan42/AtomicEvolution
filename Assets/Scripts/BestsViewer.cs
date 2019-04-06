using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class BestsViewer : MonoBehaviour
{

    public string filedir;

    private StreamReader reader;
    private Agent agent;

    void Start()
    {
        reader = new StreamReader(File.Open(filedir, FileMode.Open));
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ShowNext();
        }
    }

    void ShowNext()
    {
        //check if there's an existing agent
        if (agent != null) Destroy(agent.gameObject);

        agent = Agent.Deserialize(reader.ReadLine()); //get the agent from file. 
        //Replays should be formatted such that only one serialized agent exist on one line

        //add the trail to the leading atom, should refactor this to atom creating
        agent.AddTrail();
    }
}
