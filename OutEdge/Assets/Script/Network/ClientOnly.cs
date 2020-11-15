using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ClientOnly : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (!transform.parent.GetComponent<NetworkIdentity>().isLocalPlayer) DestroyImmediate(gameObject);
    }
}
