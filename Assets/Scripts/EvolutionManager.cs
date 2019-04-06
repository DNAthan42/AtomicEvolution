using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvolutionManager : MonoBehaviour
{
    private string best;
    private double bestDist;

    private Agent agent;

    public void SingleHillClimb(string candidate)
    {
        if (agent != null) Destroy(agent.gameObject); //safety check
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
        }

        SingleHillClimb(new Mutator(best).Mutate(new Vector3(4, 4, 4)));
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            Agent agent = Agent.BasicAgent();
            string basic = agent.Serialize();
            Destroy(agent.gameObject);

            SingleHillClimb(basic);
        }
    }
}
