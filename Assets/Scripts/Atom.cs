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
        GameObject gameObject = CreateShape(shape);

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
        GameObject gameObject = CreateShape(shape);    
        
        Enums.Motion motion = Enums.GetRandomMotion();

        Atom atom = gameObject.AddComponent<Atom>();
        Rigidbody rb = gameObject.AddComponent<Rigidbody>();
        //rb.useGravity = false;

        AtomDetails details = new AtomDetails(shape, motion);
        atom.Create(agent, rb, pos, parent, details);

        return atom;
    }

    private static GameObject CreateShape(Enums.Shape shape)
    {
        GameObject gameObject;

        switch (shape)
        {
            case Enums.Shape.Cylinder:
                gameObject = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                break;
            case Enums.Shape.Sphere:
                gameObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                break;
            case Enums.Shape.Cube:
            default:
                gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
                break;
        }
        return gameObject;
    }
    #endregion

    private Agent agent;
    public Rigidbody rb;
    private AtomDetails details;

    private Vector3 pos;
    private Enums.Direction parent;

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
                transform.localScale = new Vector3(prev.x * .75f, prev.y * .3f, prev.z * .75f);
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
}
