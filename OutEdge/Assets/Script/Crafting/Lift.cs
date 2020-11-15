using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lift : AttributeContainer
{
    [AttributeType("Int", "0:200")]
    public string height = "0";

    public GameObject head;
    float speed = 0.05f;
    Vector3 target = new Vector3(1, - 137 * 0.01f, -0.04f);
    Vector3 start;

    private void Start()
    {
        head.layer = 18;
    }

    private void FixedUpdate()
    {
        head.transform.localPosition += (target - start)*speed;
        if(head.transform.localPosition == target)
        {
            enabled = false;
        }
    }

    public override void Apply(bool firstload)
    {
        if (firstload)
        {
            head.transform.localPosition = new Vector3(1, (int.Parse(height) - 137) * 0.01f, -0.04f);
        }
        else
        {
            start = head.transform.localPosition;
            target = new Vector3(1, (int.Parse(height) - 137) * 0.01f, -0.04f);
            enabled = true;
        }
    }
}
