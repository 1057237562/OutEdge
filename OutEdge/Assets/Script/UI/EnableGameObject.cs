using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableGameObject : MonoBehaviour
{
    public GameObject gameObj;

    public void Enable()
    {
        gameObj.SetActive(true);
    }
}
