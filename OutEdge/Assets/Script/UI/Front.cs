using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Front : MonoBehaviour
{
    public GameObject center;

    public void SetPreRotate()
    {
        gameObject.SetActive(false);
        if (center)
        {
            center.GetComponent<CenterObject>().prerot = Quaternion.Inverse(Quaternion.Inverse(center.transform.rotation) * transform.rotation);
        }
    }

    public void LocateCenter(GameObject c)
    {
        try
        {
            center = c.GetComponent<CenterObject>() ? c : c.GetComponent<ConnectiveMaterial>().centerparent;
            gameObject.SetActive(true);
            transform.rotation = center.transform.rotation * Quaternion.Inverse(center.GetComponent<CenterObject>().prerot);
            transform.position = center.transform.position;
            transform.localScale = center.transform.localScale/2;
        }
        catch { }
    }
}
