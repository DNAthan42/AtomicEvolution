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
    }
}
