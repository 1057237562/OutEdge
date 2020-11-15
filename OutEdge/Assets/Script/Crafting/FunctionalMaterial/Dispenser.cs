using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dispenser : AttributeContainer
{

    public GameObject bullet;

    [AttributeType("Int","0:50")]
    public string force = "0";

    public GameObject genPos;

    public float delay = 0.15f;
    public float lastFire = 0;

    public void Fire()
    {
        GameControll.localControll.archor.GetComponent<ConfigurableJoint>().angularXMotion = ConfigurableJointMotion.Free;
        if (Time.fixedTime - lastFire > delay)
        {
            GameObject nb = Instantiate(bullet, genPos.transform.position, transform.rotation);
            Rigidbody rb = nb.GetComponent<Rigidbody>();
            rb.useGravity = true;
            rb.AddForce((genPos.transform.position - transform.position).normalized * float.Parse(force));
            GetComponent<Rigidbody>().AddForce(-(genPos.transform.position - transform.position).normalized * float.Parse(force) * GetComponent<Rigidbody>().mass * 10);
            lastFire = Time.fixedTime;
        }
    }

    public void StopFire()
    {
        GameControll.localControll.archor.GetComponent<ConfigurableJoint>().angularXMotion = ConfigurableJointMotion.Limited;
    }
}
