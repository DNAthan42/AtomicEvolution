using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Atom : MonoBehaviour
{
    #region statics
    public static float MaxForce = 100f;
    public static float MaxTorque = 100f;

    public static Atom Create(Vector3 pos)
    {
        Enums.Shape shape = Enums.GetRandomShape();
        GameObject gameObject = CreateShape(shape);    
        
        Enums.Motion motion = Enums.GetRandomMotion();

        Atom atom = gameObject.AddComponent<Atom>();
        Rigidbody rb = gameObject.AddComponent<Rigidbody>();
        rb.useGravity = false;

        atom.Create(pos, rb, shape, motion);

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

    private Rigidbody rb;
    private Enums.Shape shape;
    private Enums.Motion motion;
    private float force;
    private Vector3 direction;

    private HingeJoint joint;

    private void Create(Vector3 pos, Rigidbody rb, Enums.Shape shape, Enums.Motion motion)
    {
        this.rb = rb;
        this.shape = shape;
        this.motion = motion;
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
        joint = gameObject.AddComponent<HingeJoint>();
        switch (motion)
        {
            case Enums.Motion.Rotational:
                MakeRotationalJoint();
                break;
            case Enums.Motion.Linear:
            default:
                MakeLinearJoint();
                break;
        }
    }

    private void MakeLinearJoint()
    {
        joint.useLimits = true;
        JointLimits limits = joint.limits;
        limits.max = limits.min = limits.bounciness = 0;
        joint.limits = limits;
    }

    private void MakeRotationalJoint()
    {
        joint.useMotor = true;
        joint.anchor = Vector3.zero;
        joint.axis = direction;
        JointMotor motor = joint.motor;
        motor.targetVelocity = MaxTorque * force;
        motor.force = 10; //hard coded for now because changing force doesn't have much effect empircally.
        joint.motor = motor;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
