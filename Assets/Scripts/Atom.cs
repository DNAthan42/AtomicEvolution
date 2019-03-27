using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Atom : MonoBehaviour
{
    #region statics
    public static float MaxForce = 100f;
    public static float MaxTorque = 100f;

    /// <summary>
    /// Create the base atom with no motion
    /// </summary>
    /// <param name="pos">index in the atoms array of where to spawn the atom</param>
    /// <returns>The newly generated atom attached to a gameobject</returns>
    public static Atom Create(Vector3 pos)
    {
        Enums.Shape shape = Enums.Shape.Cube;
        GameObject gameObject = CreateShape(shape);

        Atom atom = gameObject.AddComponent<Atom>();
        Rigidbody rb = gameObject.AddComponent<Rigidbody>();
        rb.useGravity = false;

        atom.Create(pos, rb, shape, Enums.Motion.None, null);

        return atom;
    }

    /// <summary>
    /// Create a new atom with random properties.
    /// </summary>
    /// <param name="pos">index in the atoms array of where to spawn the atom</param>
    /// <returns>The newly generated atom attached to a gameobject</returns>
    public static Atom Create(Vector3 pos, Atom parent)
    {
        Enums.Shape shape = Enums.GetRandomShape();
        GameObject gameObject = CreateShape(shape);    
        
        Enums.Motion motion = Enums.GetRandomMotion();

        Atom atom = gameObject.AddComponent<Atom>();
        Rigidbody rb = gameObject.AddComponent<Rigidbody>();
        //rb.useGravity = false;

        atom.Create(pos, rb, shape, motion, parent);

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

    public Rigidbody rb;
    private Enums.Shape shape;
    private Enums.Motion motion;

    private float force;
    private Vector3 direction;

    private Atom parent;

    private HingeJoint joint;

    private void Create(Vector3 pos, Rigidbody rb, Enums.Shape shape, Enums.Motion motion, Atom parent)
    {
        this.rb = rb;
        this.shape = shape;
        this.motion = motion;
        this.parent = parent;
        transform.localPosition = pos;
        Resize();

        force = .5f;
        direction = Vector3.forward;

        MakeJoint();
    }

    private void Resize()
    {
        Vector3 prev;
        switch (shape)
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
        if (parent == null) return;

        joint = gameObject.AddComponent<HingeJoint>();
        switch (motion)
        {
            case Enums.Motion.Rotational:
                MakeRotationalJoint();
                break;
            case Enums.Motion.Linear:
                MakeLinearJoint();
                break;
            case Enums.Motion.None:
                MakeBasicJoint();
                break;
            default:
                Destroy(joint);
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
        joint.connectedBody = parent.rb;

        joint.useLimits = true;
        JointLimits limits = joint.limits;
        limits.max = limits.min = limits.bounciness = 0;
        joint.limits = limits;
    }

    private void MakeRotationalJoint()
    {
        joint.connectedBody = parent.rb;

        joint.useMotor = true;
        joint.anchor = Vector3.zero;
        joint.axis = direction;
        JointMotor motor = joint.motor;
        motor.targetVelocity = MaxTorque * force;
        motor.force = 10; //hard coded for now because changing force doesn't have much effect empircally.
        motor.freeSpin = true;
        joint.motor = motor;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
