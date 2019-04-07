using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class EvolutionManager : MonoBehaviour
{
    private static string DatePattern = "MMddHHmmssff";

    private string best;
    private double bestDist;

    private Agent agent;

    private string allbests = "";

    private List<string> genDetails;
    private string logPath = "";

    private int gen = 0;

    #region Single HillClimb
    public void SingleHillClimb(string candidate)
    {
        if (agent != null) Agent.Kill(agent); //safety check
        agent = Agent.Deserialize(candidate); //make the agent

        StartCoroutine(RegularLog(1800, false));

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
        genDetails = new List<string>();

        //create the log folder
        logPath = System.DateTime.Now.ToString(DatePattern);
        StartCoroutine(RegularLog(1800));

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
            double average = 0;
            int bestId = 0; //if none are better, reuse the parent (this should always happen algorithmically but w/e)

            //find the best reported and average distance this generation
            for (int i = 0; i < distances.Length; i++)
            {
                average += distances[i];
                if (distances[i] > bestReported)
                {
                    bestReported = distances[i];
                    bestId = i;
                }
            }
            average /= reports;
            
            if (bestReported > bestDist)
            {
                bestDist = bestReported;
                best = agents[bestId].Serialize();
                allbests += best + "\n";
                Debug.Log($"New Best Distance: {bestDist}");

                LogGen(bestReported, average, best);
            }
            else
            {
                LogGen(bestReported, average);
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
        LogBests();
        LogAllGens();
    }

    IEnumerator RegularLog(int seconds, bool useGen = true)
    {
        if (!Directory.Exists($"out/{logPath}"))
            Directory.CreateDirectory($"out/{logPath}");

        while (true)
        {
            yield return new WaitForSeconds(seconds); //waits an hour before the rest of the code is called.
            if (useGen)
            {
                LogAllGens();
            }
            else
            {
                Debug.Log($"Creating Record Log at: out/{System.DateTime.Now.ToBinary()}");
                LogBests();
            }
        }
    }

    private void LogBests()
    {
        if (!Directory.Exists($"out/{logPath}"))
            Directory.CreateDirectory($"out/{logPath}");

        StreamWriter writer = new StreamWriter(File.Create($"out/{logPath}/{System.DateTime.Now.ToBinary()}"));
        writer.Write(allbests);
        writer.Close();
    }

    private void LogAllGens()
    {
        if (!Directory.Exists($"out/{logPath}"))
            Directory.CreateDirectory($"out/{logPath}");

        string logFile = System.DateTime.Now.ToString(DatePattern) + ".genlog";
        Debug.Log($"Dumping logs to out/{logPath}/{logFile}");

        //create and write the file
        StreamWriter writer = new StreamWriter(File.Create($"out/{logPath}/{logFile}"));
        foreach (string line in genDetails)
        {
            writer.WriteLine(line);
        }
        writer.Close();
    }

    private void LogGen(double distance, double average = -1, string newBest = "")
    {
        //using # as a delimiter since commas are used in the serializer
        string line = $"{gen++}#{distance}#{average}#{newBest}";
        genDetails.Add(line);
    }
}
