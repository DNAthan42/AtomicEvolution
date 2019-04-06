using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManager : MonoBehaviour
{

    public Agent agent;
    // Update is called once per frame


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            agent.sample();
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            string thisAgent = agent.Serialize();
            Destroy(agent.gameObject);
            agent = Agent.Deserialize(thisAgent);
        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            string thisAgent = agent.Serialize();
            Destroy(agent.gameObject);
            agent = Agent.Deserialize(new Mutator(thisAgent).Mutate(new Vector3(4, 4, 4)));
            agent.StartTracking(DisplayDistance);
        }
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            prev = 0;
            SimpleRunner();
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            agent = Agent.Deserialize(prevAgent);
        }
    }

    private void DisplayDistance(double distance)
    {
        Debug.Log($"Distance: {distance}");
    }

    double prev = 0;
    string prevAgent = "";

    void SimpleRunnerMeasurer(double distance)
    {
        Debug.Log($"Distance: {distance}");
        if (distance > prev)
        {
            prev = distance;
            prevAgent = agent.Serialize();
            SimpleRunner(); //recursively call a new generation
        }
        else
        {
            Debug.Log($"Performed worse. Best Distance: {prev}");
        }


    }

    void SimpleRunner()
    {
        string thisAgent = agent.Serialize();
        Destroy(agent.gameObject);
        agent = Agent.Deserialize(new Mutator(thisAgent).Mutate(new Vector3(4, 4, 4)));
        agent.StartTracking(SimpleRunnerMeasurer);
    }
}
