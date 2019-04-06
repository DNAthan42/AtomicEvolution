using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class EvolutionManager : MonoBehaviour
{
    private string best;
    private double bestDist;

    private Agent agent;

    private string allbests = "";

    #region Single HillClimb
    public void SingleHillClimb(string candidate)
    {
        if (agent != null) Agent.Kill(agent); //safety check
        agent = Agent.Deserialize(candidate); //make the agent
        agent.StartTracking(SingleHillClimbEval);
    }

    private void SingleHillClimbEval(double distance)
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
