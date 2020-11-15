using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class Seat : MonoBehaviour
{

    GameObject p;

    public void AttachSeat(GameObject player)
    {
        //player.transform.parent.GetComponent<FirstPersonController>().enabled = false;
        //player.transform.parent.GetComponent<CapsuleCollider>().enabled = false;
        player.transform.parent.GetComponent<RigidbodyFirstPersonController>().lockPosition = true;
        player.transform.parent.GetComponent<Rigidbody>().useGravity = false;
        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Default"), LayerMask.NameToLayer("Player"),true);

        enabled = true;

        player.transform.parent.position = transform.position;
        player.transform.parent.parent = transform;

        player.transform.parent.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        p = player;

        //Active KeyListener

        //Collider[] suspectGroup = Physics.OverlapBox(transform.position, transform.localScale / 2, transform.rotation, 1 << (LayerMask.NameToLayer("Item")), QueryTriggerInteraction.UseGlobal);

        //Default choose the first one

        //Debug.Log(suspectGroup.Length);

        GameObject center = GetComponent<ConnectiveMaterial>().centerparent;

        GameObject[] subObject = center.GetComponent<CenterObject>().objlist.ToArray();

        foreach (GameObject obj in subObject)
        {
            KeyListener[] kls = obj.GetComponents<KeyListener>();
            foreach(KeyListener kl in kls)
                kl.enabled = true;
        }
    }

    public void PopPlayer()
    {
        p.transform.parent.Translate(new Vector3(0, 2, 0), Space.World);
        //p.transform.parent.GetComponent<FirstPersonController>().enabled = true;
        //p.transform.parent.GetComponent<CapsuleCollider>().enabled = true;
        p.transform.parent.GetComponent<RigidbodyFirstPersonController>().lockPosition = false;
        p.transform.parent.GetComponent<Rigidbody>().useGravity = true;
        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Default"), LayerMask.NameToLayer("Player"), false);

        p.transform.parent.parent = null;
        p.transform.parent.rotation = Quaternion.identity;

        p.transform.parent.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        enabled = false;

        //Deactive KeyListener
        //Default choose the first one

        //Debug.Log(suspectGroup.Length);

        GameObject center = (GetComponent<ConnectiveMaterial>().centerparent ?? gameObject);

        GameObject[] subObject = center.GetComponent<CenterObject>().objlist.ToArray();

        foreach (GameObject obj in subObject)
        {
            KeyListener[] kls = obj.GetComponents<KeyListener>();
            foreach (KeyListener kl in kls)
                kl.enabled = false;
        }
    }

    void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
        {
            PopPlayer();
        }
    }
}
