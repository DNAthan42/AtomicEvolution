using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GenViewer : MonoBehaviour
{
    public string filePath;
    private string[] agentlist;


    void Update()
    {
        
    }

    private void Start()
    {
        if (!filePath.EndsWith(".genlog"))
        {
            Debug.Log("Improper file type");
            return;
        }

        StreamReader reader = new StreamReader(File.Open(filePath, FileMode.Open));
        List<string> list = new List<string>();

        string next = "";

        while ((next = reader.ReadLine()) != null)
        {
            list.Add(next);
        }

        agentlist = list.ToArray();

        reader.Close();
    }
}
