using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class BestsViewer : MonoBehaviour
{

    public string filedir;
    private string[] agentlist;

    private Agent agent;

    private int i;

    void Start()
    {
        StreamReader reader = new StreamReader(File.Open(filedir, FileMode.Open));
        List<string> list = new List<string>();

        string next = "";

        while ((next = reader.ReadLine()) != null)
        {
            if (filedir.EndsWith(".genlog"))
            {
                string[] pieces = next.Split('#');
                if (pieces[pieces.Length - 1] == "") continue;
                next = pieces[pieces.Length - 1];
            }
            list.Add(next);
        }

        agentlist = list.ToArray();

        reader.Close();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            ShowNext();
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            ShowPrev();
        }
    }

    void ShowNext()
    {

        if (++i >= agentlist.Length)
        {
            Debug.Log("END OF LIST");
            i--;
            return;
        }

        ShowCurrent();
    }

    void ShowPrev()
    {
        if (--i <= 0)
        {
            Debug.Log("START OF LIST");
            i++;
            return;
        }

        ShowCurrent();
    }

    void ShowCurrent()
    {
        //check if there's an existing agent
        if (agent != null) Agent.Kill(agent);

        agent = Agent.Deserialize(agentlist[i]); //get the agent from file. 
        //Replays should be formatted such that only one serialized agent exist on one line

        //add the trail to the leading atom, should refactor this to atom creating
        agent.AddTrail();

        Debug.Log($"Generation {i} of {agentlist.Length}");

    }
}
