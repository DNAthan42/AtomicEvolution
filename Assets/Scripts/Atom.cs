using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Atom : MonoBehaviour
{
    #region statics
    /// <summary>
    /// Create the base atom with no motion
    /// </summary>
    /// <param name="pos">index in the atoms array of where to spawn the atom</param>
    /// <returns>The newly generated atom attached to a gameobject</returns>
    public static Atom Create(Vector3 pos, Agent agent)
    {
        Enums.Shape shape = Enums.Shape.Cube;
        GameObject gameObject = CreateShape(shape, agent);

        Atom atom = gameObject.AddComponent<Atom>();
        Rigidbody rb = gameObject.AddComponent<Rigidbody>();
        rb.useGravity = false;

        AtomDetails details = new AtomDetails(shape, Enums.Motion.None);
        atom.Create(agent, rb, pos, Enums.Direction.None, details);

        return atom;
    }

    /// <summary>
    /// Create a new atom with random properties.
    /// </summary>
    /// <param name="pos">index in the atoms array of where to spawn the atom</param>
    /// <returns>The newly generated atom attached to a gameobject</returns>
    public static Atom CreateRandom(Vector3 pos, Agent agent, Enums.Direction parent)
    {
        Enums.Shape shape = Enums.GetRandomShape();
        GameObject gameObject = CreateShape(shape, agent);    
        
        Enums.Motion motion = Enums.GetRandomMotion();

        Atom atom = gameObject.AddComponent<Atom>();
        Rigidbody rb = gameObject.AddComponent<Rigidbody>();
        //rb.useGravity = false;

        AtomDetails details = new AtomDetails(shape, motion);
        atom.Create(agent, rb, pos, parent, details);

        return atom;
    }

    public static Atom Create(Vector3 pos, Agent agent, Enums.Direction parent, AtomDetails details)
    {
        GameObject gameObject = CreateShape(details.shape, agent);
        Atom atom = gameObject.AddComponent<Atom>();
        Rigidbody rb = gameObject.AddComponent<Rigidbody>();

        atom.Create(agent, rb, pos, parent, details);

        return atom;
    }

    private static GameObject CreateShape(Enums.Shape shape, Agent agent)
    {
        GameObject gameObject;

        switch (shape)
        {
            case Enums.Shape.Cylinder:
                gameObject = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                break;
            case Enums.Shape.Sphere:
                gameObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                break;
            case Enums.Shape.Cube:
            default:
                gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
                break;
        }
        gameObject.transform.parent = agent.transform;
        gameObject.layer = agent.id + 8;
        return gameObject;
    }
    #endregion

    private Agent agent;
    public Rigidbody rb;
    public AtomDetails details;

    private Vector3 pos;
    public Enums.Direction parent;

    private HingeJoint joint;

    private void Create(Agent agent, Rigidbody rb, Vector3 pos, Enums.Direction parent, AtomDetails details)
    {
        this.agent = agent;
        this.rb = rb;
        this.details = details;
        this.pos = pos;
        this.parent = parent;

        transform.localPosition = pos;
        Resize();

        MakeJoint();
    }

    private void Resize()
    {
        Vector3 prev;
        switch (details.shape)
        {
            case Enums.Shape.Cube:
                transform.localScale *= .5f;
                break;
            case Enums.Shape.Cylinder:
                prev = transform.localScale;
                transform.localScale = new Vector3(prev.x * .75f, prev.y * .6f, prev.z * .75f);
                break;
            case Enums.Shape.Sphere:
                prev = transform.localScale;
                transform.localScale = new Vector3(prev.x * .95f, prev.y * .95f, prev.z * .95f);
                break;
            default:
                break;
        }
    }

    private void MakeJoint()
    {
        if (parent == Enums.Direction.None) return;

        joint = gameObject.AddComponent<HingeJoint>();
        switch (details.motion)
        {
            case Enums.Motion.Rotational:
                MakeRotationalJoint();
                break;
            case Enums.Motion.Linear:
                MakeLinearJoint();
                break;
            case Enums.Motion.None:
            default:
                MakeBasicJoint();
                break;
        }
    }

    private void MakeLinearJoint()
    {
        MakeBasicJoint();//TODO: Get linear motion working.
    }

    private void MakeBasicJoint()
    {
        joint.connectedBody = agent.getParent(pos, parent).rb;

        joint.useLimits = true;
        JointLimits limits = joint.limits;
        limits.max = limits.min = limits.bounciness = 0;
        joint.limits = limits;
    }

    private void MakeRotationalJoint()
    {
        joint.connectedBody = agent.getParent(pos, parent).rb;

        joint.useMotor = true;
        joint.anchor = Vector3.zero;
        joint.axis = details.direction;
        JointMotor motor = joint.motor;
        motor.targetVelocity = AtomDetails.MaxTorque * details.force;
        motor.force = 10; //hard coded for now because changing force doesn't have much effect empircally.
        motor.freeSpin = true;
        joint.motor = motor;
    }

    public void Mutate()
    {
        details.Mutate();
    }
    public void AddChild(Enums.Direction direction)
    {
        details.children[(int)direction - 1] = true;
    }

    public string Serialize()
    {
        return details.Serialize();
    }

    public static AtomDetails Deserialize(string json)
    {
        return AtomDetails.Deserialize(json);
    }

    public void AddTrail()
    {
        TrailRenderer trail = gameObject.AddComponent<TrailRenderer>();
        trail.material.shader = Shader.Find("Particles/Standard Surface");
        trail.startWidth = .1f;
        trail.startColor = Color.red;
        trail.endWidth = 0;
        trail.endColor = Color.black;

        trail.time = 2.5f;


    }
}
