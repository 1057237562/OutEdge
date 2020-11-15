using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MaterialItem : MonoBehaviour
{
    public Canvas parent;
    public int id;
    public GameObject assignObject;

    // Start is called before the first frame update
    void Start()
    {
        Crafting.dict.Add(id, assignObject);
        Crafting.reflect.Add(assignObject.name, id);
        GetComponent<Button>().onClick.AddListener(delegate { replace();  });
        enabled = false;
    }

    public void replace()
    {
        Crafting.sid = id;
        enabled = true;
        GetComponent<RawImage>().color = Color.gray;
        Crafting.m.ReleaseCurrent();
    }

    private void Update()
    {
        if(Crafting.sid != id)
        {
            Debug.Log(Crafting.sid);
            enabled = false;
            GetComponent<RawImage>().color = Color.white;
        }
    }

}
