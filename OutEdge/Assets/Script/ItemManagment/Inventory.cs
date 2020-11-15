using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using static ItemManager;
using CommandTerminal;

public class Inventory : MonoBehaviour
{

    public static List<int> allocateItem = new List<int>();

    public static List<GameObject> holders;

    public GameObject ui;

    public static Inventory m;

    public GameObject itemholder_i;

    public int holder_count = 20;

    public List<GameObject> hotbar;

    // Start is called before the first frame update
    void Awake()
    {
        m = this;
    }

    public void Reload(XmlNodeList input,XmlNodeList hot)
    {
        holders = new List<GameObject>();

        for (int i = 0; i < holder_count; i++)
        {
            holders.Add(AddItemHolder(null));
            //holders[i].GetComponent<ItemHolder>().SetItem(new ItemStack(new Item(0, 0, null), 1, 0));
            if (input != null && input.Count > i)
            {
                XmlElement item = (XmlElement)input[i];
                holders[i].GetComponent<ItemHolder>().ReadXml(item);
            }
        }

        if (hot != null && hot.Count > 0)
            for (int i = 0; i < hot.Count; i++)
            {
                XmlElement item = (XmlElement)hot[i];
                hotbar[i].GetComponent<ItemHolder>().ReadXml(item);
            }

    }

    public static int GetCustomId(GameObject @obj)
    {
        return -allocateItem.IndexOf(obj.GetInstanceID()) - 1;
    }

    public GameObject AddItemHolder(Transform parent)
    {
        if(parent == null)
        {
            parent = itemholder_i.transform.parent;
        }
        GameObject itemholder_n = Instantiate(itemholder_i,parent);
        itemholder_n.SetActive(true);

        parent.gameObject.SetActive(false);//Restart
        parent.gameObject.SetActive(true);
        return itemholder_n;
    }

    public int findEmptySpace(Item item)
    {
        foreach(GameObject holder in holders)
        {
            if (holder.GetComponent<ItemHolder>().isEmpty() || holder.GetComponent<ItemHolder>().GetItem().item.id == item.id)
            {
                return holders.IndexOf(holder);
            }
        }
        return -1;
    }

    [RegisterCommand(Help = "Give Player Items. Usage: give id count [sub] [meta]", MinArgCount = 2, MaxArgCount = 4)]
    public static void give(CommandArg[] args)
    {
        Item item = new Item(args[0].Int,args.Length > 2 ? args[2].Int:0,args.Length > 3 ? args[3].String:"");
        m.attemptAddItem(item, args[1].Int,im.maxdamage[args[0].Int]);
    }

    public bool attemptAddItem(Item item,float count,int damage = 0)
    {
        int @result = findEmptySpace(item);
        if (result != -1)
        {
            ItemHolder itemholder = holders[result].GetComponent<ItemHolder>();
            if (itemholder.isEmpty())
            {
                itemholder.SetItem(new ItemStack(item, count, damage));
            }
            else
            {
                ItemStack @itemstack = itemholder.GetItem();
                itemstack.count += count;
                itemholder.UpdateUI();
            }
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool findItem(Item item)
    {
        foreach (GameObject holder in holders)
        {
            if (!holder.GetComponent<ItemHolder>().isEmpty() && holder.GetComponent<ItemHolder>().GetItem().item.id == item.id)
            {
                return true;
            }
        }
        return false;
    }

    public bool findItem(Item item,float count)
    {
        if(count == 0)
        {
            return true;
        }
        float foundCount = 0;
        foreach (GameObject holder in holders)
        {
            if (!holder.GetComponent<ItemHolder>().isEmpty() && holder.GetComponent<ItemHolder>().GetItem().item.id == item.id)
            {
                foundCount += holder.GetComponent<ItemHolder>().GetItem().count;
            }
        }
        if(foundCount >= count)
        {
            return true;
        }
        return false;
    }

    public bool removeItem(Item item,float count)
    {
        float remainCount = count;
        foreach (GameObject holder in holders)
        {
            if (!holder.GetComponent<ItemHolder>().isEmpty() && holder.GetComponent<ItemHolder>().GetItem().item.id == item.id)
            {
                float hc = holder.GetComponent<ItemHolder>().GetItem().count;
                if (hc >= remainCount)
                {
                    holder.GetComponent<ItemHolder>().RemoveItem(remainCount);
                    return true;
                }
                else
                {
                    if (holder.GetComponent<ItemHolder>().RemoveItem(hc))
                    {
                        remainCount -= hc;
                    }
                }
            }
        }
        return false;
    }

    public bool removeItems(List<ItemStack> items)
    {
        foreach(ItemStack item in items)
        {
            if (!findItem(item.item, item.count))
            {
                return false;
            }
        }
        foreach (ItemStack item in items)
        {
            removeItem(item.item, item.count);
        }
        return true;
    }

    public bool removeItems(List<ItemStack> items,float multiplier)
    {
        foreach (ItemStack item in items)
        {
            if (!findItem(item.item, item.count * multiplier))
            {
                return false;
            }
        }
        foreach (ItemStack item in items)
        {
            removeItem(item.item, item.count * multiplier);
        }
        return true;
    }

    public bool attemptAddItems(List<ItemStack> items)
    {
        foreach(ItemStack item in items)
        {
            if(!attemptAddItem(item.item, item.count,item.damage))
            {
                return false;
            }
        }
        return true;
    }

    public bool attemptAddItems(List<ItemStack> items,float multiplier)
    {
        foreach (ItemStack item in items)
        {
            if (!attemptAddItem(item.item, item.count * multiplier,item.damage))
            {
                return false;
            }
        }
        return true;
    }
}
