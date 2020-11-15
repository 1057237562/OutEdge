using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateMotion : AttributeContainer
{
    [AttributeType("Int","0:30")]
    public string speed = "0";
    [AttributeType("Int", "-10:10")]
    public string acceleration = "0";
    [AttributeType("Boolean", "")]
    public string autobrake;

    public bool brake = false;
    Quaternion targetrot;
    Vector3 origin;

    // Start is called before the first frame update
    void Start()
    {
        origin = transform.localPosition;
    }

    public void FixedUpdate()
    {
        //GetComponent<Rigidbody>().maxAngularVelocity = Mathf.Abs(float.Parse(speed));
        //Rotate();
    }

    public void StopRotate()
    {
        try
        {
            if (bool.Parse(autobrake))
            {
                ConfigurableJoint joint = GetComponent<ConfigurableJoint>();
                Rigidbody temp = joint.connectedBody;
                joint.connectedBody = null;
                joint.connectedBody = temp;
                joint.angularYMotion = ConfigurableJointMotion.Locked;
            }
        }
        catch { }
    }

    // Update is called once per frame
    public void Rotate()
    {
        if (bool.Parse(autobrake))
        {
            ConfigurableJoint joint = GetComponent <ConfigurableJoint>();
            joint.angularYMotion = ConfigurableJointMotion.Free;
            GetComponent<Rigidbody>().AddRelativeTorque(new Vector3(0, float.Parse(acceleration) * 1000, 0), ForceMode.Acceleration);
        }
        else
        {
            GetComponent<Rigidbody>().AddRelativeTorque(new Vector3(0, float.Parse(acceleration) * 1000, 0), ForceMode.Acceleration);
        }
    }

    public void RotateInverse()
    {
        if (bool.Parse(autobrake))
        {
            ConfigurableJoint joint = GetComponent<ConfigurableJoint>();
            joint.angularYMotion = ConfigurableJointMotion.Free;
            GetComponent<Rigidbody>().AddRelativeTorque(new Vector3(0, -float.Parse(acceleration) * 1000, 0), ForceMode.Acceleration);
        }
        else
        {
            GetComponent<Rigidbody>().AddRelativeTorque(new Vector3(0, -float.Parse(acceleration) * 1000, 0), ForceMode.Acceleration);
        }
    }

    public void RotateAround()
    {
        if (brake)
        {
            ConfigurableJoint joint = GetComponent<ConfigurableJoint>();
            joint.angularZMotion = ConfigurableJointMotion.Free;
            GetComponent<Rigidbody>().AddRelativeTorque(new Vector3(0, 0, float.Parse(acceleration) * 1000), ForceMode.VelocityChange);
            transform.localPosition = origin;
            joint.angularZMotion = ConfigurableJointMotion.Locked;
        }
        else
        {
            HingeJoint hj = GetComponent<HingeJoint>();
            JointSpring js = new JointSpring();
            js.spring = Mathf.Abs(float.Parse(acceleration)) * 100;
            js.targetPosition = Mathf.Clamp(hj.spring.targetPosition + float.Parse(acceleration)*float.Parse(speed) / 50,hj.limits.min,hj.limits.max);
            hj.spring = js;
        }
    }

    public void RotateAroundInverse()
    {
        if (brake)
        {
            ConfigurableJoint joint = GetComponent<ConfigurableJoint>();
            joint.angularZMotion = ConfigurableJointMotion.Free;
            GetComponent<Rigidbody>().AddRelativeTorque(new Vector3(0, 0, -float.Parse(acceleration) * 1000), ForceMode.VelocityChange);
            transform.localPosition = origin;
            joint.angularZMotion = ConfigurableJointMotion.Locked;
        }
        else
        {
            HingeJoint hj = GetComponent<HingeJoint>();
            JointSpring js = new JointSpring();
            js.spring = Mathf.Abs(float.Parse(acceleration)) * 100;
            js.targetPosition = Mathf.Clamp(hj.spring.targetPosition - float.Parse(acceleration) * float.Parse(speed) / 50,hj.limits.min,hj.limits.max);
            hj.spring = js;
        }
    }

    public override void Apply(bool firstload = false)
    {
        GetComponent<Rigidbody>().maxAngularVelocity = Mathf.Abs(float.Parse(speed))*Mathf.PI;
        StopRotate();
        base.Apply(firstload);
    }
}
