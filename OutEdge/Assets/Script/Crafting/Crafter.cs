using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine;
using UnityTemplateProjects;
using static GameControll;
using static TerrainManager;
using static UIManager;

public class Crafter : DataEntity
{

    public Camera tc;
    public GameObject ct;
    public float maxscale;
    public float connectScale = 0.2f;
    float builtsize = 0.5f;
    public bool doubleConnect = false;
    public bool pickable = true;
    // Start is called before the first frame update

    public Crafter()
    {
        save = new Func<string>(SaveData);
        load = new Action<string>(LoadData);
    }

    public string SaveData()
    {
        StringBuilder sb = new StringBuilder();
        foreach (AttributeContainer ac in GetComponents<AttributeContainer>())
        {
            foreach (FieldInfo field in ac.GetType().GetFields())
            {
                if (field.GetCustomAttribute<AttributeType>() == null)
                    continue;

                sb.Append(field.GetValue(ac)+":");

            }
        }
        return sb.ToString();
    }

    public void LoadData(string input)
    {
        int index = 0;
        string[] args = input.Split(':');
        foreach (AttributeContainer ac in GetComponents<AttributeContainer>())
        {
            foreach (FieldInfo field in ac.GetType().GetFields())
            {
                if (field.GetCustomAttribute<AttributeType>() == null)
                    continue;

                field.SetValue(ac, args[index]);

                index++;
            }
            ac.Apply(true);
        }
    }

    public override void Start()
    {
        GetComponent<GuiObject>().interact.Add(delegate { openCrafter(); });
        GetComponent<GuiObject>().lostfocus.Add(delegate { destroyCrafter(); });

        base.Start();
    }

    public override void LoadEntity()
    {
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        GetComponent<Collider>().isTrigger = false;

        gameObject.layer = 0;
        base.LoadEntity();
    }

    public void openCrafter() {
        //Crafting.block = true;

        //ct.transform.localPosition = new Vector3(0, 94, 0);
        //ct.transform.rotation = Quaternion.Euler(30, 0, 0);
        //tc.transform.localPosition = new Vector3(0, 0.7f, -1.5f);
        Crafting.m.bs.text = builtsize + "";
        Crafting.m.buildsize = builtsize;
        ui.crafting.enabled = true;
        ui.crafting.GetComponent<Crafting>().desk = gameObject;
        ct.GetComponent<SimpleCameraController>().enabled = true;
        if(Crafting.m.buildsize > maxscale)
        {
            Crafting.m.buildsize = maxscale;
        }
    }

    public void destroyCrafter()
    {
        builtsize = Crafting.m.buildsize;
        foreach(GameObject ac in ui.crafting.GetComponent<Crafting>().acs)
        {
            if (ac)
            {
                ac.GetComponent<AttributeSettings>().Close();
                Destroy(ac);
            }
        }
        ui.crafting.GetComponent<Crafting>().acs.Clear();

        GameObject gobj = ui.crafting.GetComponent<Crafting>().locklast;
        if(gobj != null)
        {
            gobj.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        }
        Destroy(ui.crafting.GetComponent<Crafting>().preview);
        ui.crafting.enabled = false;
        ct.GetComponent<SimpleCameraController>().enabled = false;
    }
}
