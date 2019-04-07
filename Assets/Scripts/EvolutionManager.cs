using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class EvolutionManager : MonoBehaviour
{
    private string best;
    private double bestDist;

    private Agent agent;

    private string allbests = "";

    private int gen = 0;

    #region Single HillClimb
    public void SingleHillClimb(string candidate)
    {
        if (agent != null) Agent.Kill(agent); //safety check
        agent = Agent.Deserialize(candidate); //make the agent
        agent.StartTracking(SingleHillClimbEval);
    }

    private void SingleHillClimbEval(double distance, int id)
    {
        Debug.Log($"Best: {bestDist}\nCurrent: {distance}");
        if (distance > bestDist)
        {
            bestDist = distance;
            best = agent.Serialize();
            allbests += best + "\n";
        }

        SingleHillClimb(new Mutator(best).Mutate(new Vector3(4, 4, 4)));
    }
    #endregion

    #region MultiHillClimb
    private Agent[] agents;
    private double[] distances;
    int reports;

    public void MultiHillClimb(int genSize)
    {
        agents = new Agent[genSize];

        //create the start point
        Agent agent = Agent.BasicAgent();
        string basic = agent.Serialize();
        Agent.Kill(agent);

        MultiHillClimb(basic);
    }

    private void MultiHillClimb(string parent)
    {
        //reset the completion tracker
        distances = new double[agents.Length];
        reports = 0;

        for (int i = 0; i < agents.Length; i++)
        {
            if (agents[i] != null) Agent.Kill(agents[i]);
            //Create the new agents
            string thisAgent;
            if (i == 0) thisAgent = parent; //Keep the parent between generations
            else thisAgent = new Mutator(parent).Mutate(new Vector3(4, 4, 4));
            //spawn the agent
            agents[i] = Agent.Deserialize(thisAgent);
            agents[i].StartTracking(MultiHillClimbEval);
        }
    }

    private void MultiHillClimbEval(double distance, int id)
    {
        distances[id] = distance;
        reports++;
        if (reports == agents.Length)
        {
            double bestReported = 0;
            int bestId = 0; //if none are better, reuse the parent (this should always happen algorithmically but w/e)

            for (int i = 0; i < distances.Length; i++)
            {
                if (distances[i] > bestReported)
                {
                    bestReported = distances[i];
                    bestId = i;
                }
            }

            if (bestReported > bestDist)
            {
                bestDist = bestReported;
                best = agents[bestId].Serialize();
                allbests += best + "\n";
                Debug.Log($"New Best Distance: {bestDist}");
            }
            MultiHillClimb(best);
        }

    }
    #endregion

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            Agent agent = Agent.BasicAgent();
            string basic = agent.Serialize();
            Agent.Kill(agent);

            SingleHillClimb(basic);
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log($"Creating Record Log at: out/{System.DateTime.Now.ToBinary()}");
            StreamWriter writer = new StreamWriter(File.Create($"out/{System.DateTime.Now.ToBinary()}"));
            writer.Write(allbests);

        }
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            MultiHillClimb(4);
        }
    }

    private void OnDestroy()
    {
        Log();
    }

    IEnumerator HourlyLog(string pathAddition)
    {
        Directory.CreateDirectory($"out/{pathAddition}");

        while (true)
        {
            yield return new WaitForSeconds(3600); //waits an hour before the rest of the code is called.
            Debug.Log($"Creating Record Log at: out/{System.DateTime.Now.ToBinary()}");
            Log();
        }
    }

    private void Log()
    {
        StreamWriter writer = new StreamWriter(File.Create($"out/{System.DateTime.Now.ToBinary()}"));
        writer.Write(allbests);
    }
}
