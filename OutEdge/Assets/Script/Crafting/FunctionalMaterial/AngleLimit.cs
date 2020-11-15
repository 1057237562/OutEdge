using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AngleLimit : AttributeContainer
{
    [AttributeType("Int", "-90:90")]
    public string maxDegree = "45";
        [AttributeType("Int", "-90:90")]
    public string minDegree = "-45";
    public override void Apply(bool firstload = false)
    {
        JointLimits jl = new JointLimits();
        jl.max = float.Parse(maxDegree);
        jl.min = float.Parse(minDegree);
        GetComponent<HingeJoint>().limits = jl;
        base.Apply(firstload);
    }
}
