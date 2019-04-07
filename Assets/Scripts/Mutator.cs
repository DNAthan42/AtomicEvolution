using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Mutator
{
    public static float ExpansionRate = .25f;
    public static float PointMutationRate = .25f;

    private AtomDetails[,,] details;

    public Mutator(string f)
    {
        details = Agent.ToDetails(f);
    }

    public string Mutate(Vector3 root)
    {
        MutateAtom(root);

        return Agent.Serialize(details);
    }

    private void MutateAtom(Vector3 p)
    {
        if (Random.value < PointMutationRate)
        {
            details[(int)p.x, (int)p.y, (int)p.z].Mutate();
        }

        if (Random.value < ExpansionRate)
        {
            Enums.Direction direction = (Enums.Direction) Random.Range(0, 6) + 1; //add one to skip direction.none

            Vector3 childLoc = Enums.InDirection(p, direction);
            if (details[(int)childLoc.x, (int)childLoc.y, (int)childLoc.z] == null) //only create the child if there's space
            {
                details[(int)childLoc.x, (int)childLoc.y, (int)childLoc.z] = AtomDetails.CreateRandom();
                details[(int)p.x, (int)p.y, (int)p.z].children[((int)direction - 1)] = true;
            }
        }

        //Mutate by traversing over children depth first.
        for (int i = 0; i < details[(int)p.x, (int)p.y, (int)p.z].children.Length; i++)
        {
            Enums.Direction childDir = (Enums.Direction)(i + 1);
            if (details[(int)p.x, (int)p.y, (int)p.z].children[i])
                MutateAtom(Enums.InDirection(p, childDir));
        }
    }
}
