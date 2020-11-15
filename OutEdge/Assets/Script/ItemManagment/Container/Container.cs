using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ItemManager;
using static GameControll;
using static UIManager;
using System;
using Newtonsoft.Json;

[RequireComponent(typeof(GuiObject))]
public class Container : DataEntity
{
    public ItemStack[] stacks;
    public int slotcount;
    GuiObject go;

    public Container()
    {
        save = new Func<string>(SaveData);
        load = new Action<string>(LoadData);
    }

    public override void Start()
    {
        base.Start();
        go = GetComponent<GuiObject>();
        go.interact.Add(delegate { OpenContainer(); });
        go.lostfocus.Add(delegate { CloseContainer(); });

        if(stacks.Length != slotcount)
            stacks = new ItemStack[slotcount];
    }

    public string SaveData()
    {
        return JsonConvert.SerializeObject(stacks);
    }

    public void LoadData(string data)
    {
        stacks = JsonConvert.DeserializeObject<ItemStack[]>(data);
    }

    public void OpenContainer()
    {
        ui.container.GetComponent<ContainerUI>().container = this;
        ui.container.SetActive(true);
    }

    public void CloseContainer()
    {
        ui.container.SetActive(false);
    }
}
