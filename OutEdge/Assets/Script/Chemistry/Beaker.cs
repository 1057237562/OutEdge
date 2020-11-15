using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using static ItemManager;
using static ReactionManager;

[RequireComponent(typeof(ClickableObject),typeof(ThermalReceiver))]
public class Beaker : DataEntity
{
    public const int volumn = 10;
    [SerializeField]
    public SerializeableDictionary<Item,ItemStack> contain = new SerializeableDictionary<Item, ItemStack>();
    int currentvolumn;
    [SerializeField]
    public List<Item> output = new List<Item>();
    [SerializeField]
    public Queue<Process> processing = new Queue<Process>();
    public ThermalReceiver tr;
    int lt = 0;

    [Serializable]
    public struct Process
    {
        public Recipe recipe;
        public int starttime;

        public Process(Recipe r,int s)
        {
            recipe = r;
            starttime = s;
        }
    }

    public Beaker()
    {
        save = new Func<string>(SaveData);
        load = new Action<string>(LoadData);
    }

    public string SaveData()
    {
        return JsonUtility.ToJson(contain);
    }

    public void LoadData(string data)
    {
        contain = JsonUtility.FromJson<SerializeableDictionary<Item, ItemStack>>(data);
    }

    public float GetCount()
    {
        float sum = 0;
        foreach(ItemStack stack in contain.Values)
        {
            sum += stack.count;
        }
        return output.Count + sum; 
    }

    public override void Start()
    {
        base.Start();
        currentvolumn = (int) (volumn * transform.lossyScale.x*transform.lossyScale.y*transform.lossyScale.z);
        GetComponent<ClickableObject>().action = new Func<ItemStack,bool>(x => {
            if (x == null)
            {
                if (output.Count > 0)
                {
                    Inventory.m.attemptAddItem(output[0], 1);
                    output.RemoveAt(0);
                }
                else if (contain.Count > 0)
                {
                    IEnumerator<Item> e = contain.Keys.GetEnumerator();
                    e.MoveNext();
                    Item item = e.Current;
                    Inventory.m.attemptAddItem(item, 1);
                    ItemStack stack;
                    contain.TryGetValue(item, out stack);
                    stack.count--;
                    if (stack.count == 0)
                    {
                        contain.Remove(item);
                    }
                }
            }
            else
            {
                if (GetCount() < currentvolumn)
                {
                    ItemStack stack;
                    if (!contain.TryGetValue(x.item, out stack))
                    {
                        stack = new ItemStack(x.item, 1, 0);
                        contain.Add(x.item, stack);
                    }
                    else
                    {
                        stack.count++;
                    }
                    x.count--;

                    UpdateProcess();
                }
            }
            return true;
        });
    }

    private void Update()
    {
        if(contain.Count != 0 && tr.temperature != lt)
        {
            UpdateProcess();
        }
        lt = tr.temperature;
        if(processing.Count != 0)
        {
            Process p = processing.Peek();
            if(p.recipe.minTemperature > tr.temperature)
            {
                processing.Dequeue();
                foreach (ItemStack itemstack in p.recipe.input)
                {
                    ItemStack stack;
                    if (!contain.TryGetValue(itemstack.item, out stack))
                    {
                        stack = new ItemStack(itemstack.item, itemstack.count, 0);
                        contain.Add(itemstack.item, stack);
                    }
                    else
                    {
                        stack.count += itemstack.count;
                    }
                }
            }
            if(Time.time * 1000 -  p.starttime >= p.recipe.time)
            {
                processing.Dequeue();
                foreach(ItemStack itemstack in p.recipe.output)
                {
                    for(int i = 0; i < itemstack.count; i++)
                    {
                        output.Add(itemstack.item);
                        UpdateProcess();
                    }
                }
                
            }
        }
    }

    /*private void OnCollisionEnter(Collision collision)
    {
        if (used < currentvolumn)
        {
            GameObject drop = collision.collider.gameObject;
            ItemBase itembase = drop.GetComponent<ItemBase>();
            if (itembase == null) return;
            ItemStack stack;
            if(!contain.TryGetValue(itembase.item, out stack))
            {
                stack = new ItemStack(itembase.item, 1, 0);
                contain.Add(itembase.item, stack);
            }
            else
            {
                stack.count++;
            }
            used++;
            Destroy(drop);

            UpdateProcess();
        }
    }*/

    void UpdateProcess()
    {
        foreach(Recipe recipe in rm.b_recipes)
        {
            if(tr.temperature < recipe.minTemperature)
            {
                continue;
            }
            bool contained = true;
            foreach(ItemStack itemStack in recipe.input)
            {
                int count = output.FindAll(x => { return itemStack.item == x; }).Count;
                if (itemStack.count > count)
                {
                    itemStack.count -= count;
                }
                else
                {
                    continue;
                }

                ItemStack storage;
                if(!contain.TryGetValue(itemStack.item, out storage) || storage < itemStack)
                {
                    contained = false;
                }
                itemStack.count += count;
            }
            if (contained)
            {
                foreach(ItemStack itemStack in recipe.input)
                {
                    int required = 0;
                    for(int i = 0; i < itemStack.count; i++)
                    {
                        if (output.Remove(itemStack.item))
                        {
                            required++;
                        }
                        else
                        {
                            break;
                        }
                    }
                    if(required == itemStack.count)
                    {
                        continue;
                    }
                    else
                    {
                        ItemStack storage;
                        contain.TryGetValue(itemStack.item, out storage);
                        storage.count -= itemStack.count - required;
                        if(storage.count == 0)
                        {
                            contain.Remove(itemStack.item);
                        }
                    }
                }
                processing.Enqueue(new Process(recipe, (int)(Time.time * 1000)));
            }
        }
    }
}
