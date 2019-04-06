﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AtomDetails
{
    public static float DeltaForce = .1f;
    public static int DeltaAngle = 45;

    public static float MaxForce = 100f;
    public static float MaxTorque = 1000f;

    public static AtomDetails CreateRandom()
    {
        return new AtomDetails(Enums.GetRandomShape(), Enums.GetRandomMotion());
    }

    public Enums.Shape shape;
    public Enums.Motion motion;

    public float force;
    public Vector3 direction;

    public bool[] children;

    public AtomDetails(Enums.Shape shape, Enums.Motion motion, float force, Vector3 direction, bool[] children)
    {
        this.shape = shape;
        this.motion = motion;
        this.force = force;
        this.direction = direction;
        this.children = children;
    }

    public AtomDetails(Enums.Shape shape, Enums.Motion motion)
    {
        this.shape = shape;
        this.motion = motion;

        force = .5f;
        direction = Vector3.forward;
        children = new bool[6];
    }

    public void Mutate()
    {
        int mType = Random.Range(0, 4);
        if (mType == 0) //change shape
        {
            ChangeShape();
        }
        else if (mType == 1)
        {
            ChangeForce();
        }
        else if (mType == 2)
        {
            ChangeDirection();
        }
        else if (mType == 3)
        {
            ChangeMotion();
        }

    }

    private void ChangeShape()
    {
        Enums.Shape newShape;
        do newShape = Enums.GetRandomShape();
        while (newShape == shape); //new shape must be different then the current one.
        shape = newShape; //now that newSahpe != shape, update this objects shape
    }

    private void ChangeForce()
    {
        //5050 chance to increase or decrease the force by DeltaForce
        force += (Random.value < .5) ? -DeltaForce : DeltaForce;
    }

    private void ChangeDirection()
    {
        // choose rotation direction for each axe
        int xdir, ydir, zdir;
        do
        {
            //foreach axe, get either -1, 0, or 1.
            xdir = Random.Range(-1, 2);
            ydir = Random.Range(-1, 2);
            zdir = Random.Range(-1, 2);
        }
        while (xdir == 0 && ydir == 0 && zdir == 0); //Must rotate. if all are 0, no rotation happens

        //apply rotation
        direction = Quaternion.Euler(xdir * DeltaAngle, ydir * DeltaAngle, zdir * DeltaAngle) * direction;
    }

    private void ChangeMotion()
    {
        Enums.Motion newMotion;
        do newMotion = Enums.GetRandomMotion();
        while (newMotion == motion); //new motion must be different then the current one.
        motion = newMotion;
    }

    public string Serialize()
    {
        return JsonUtility.ToJson(this);
    }

    public static AtomDetails Deserialize(string json)
    {
        return JsonUtility.FromJson<AtomDetails>(json);
    }
}
