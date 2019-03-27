using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Atom : MonoBehaviour
{
    #region statics
    public static float MaxForce = 100f;
    public static float MaxTorque = 100f;
    public static float DeltaForce = .1f;
    public static int DeltaAngle = 45;

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

    #region Mutation
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
        else
        {
            //SpawnChild();
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
        //5050 chance to increase or decrease the force by 1/10th of max
        force += (Random.value < .5) ? -.1f : .1f;
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
    #endregion
}
