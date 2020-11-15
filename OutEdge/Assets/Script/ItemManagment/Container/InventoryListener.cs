using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using static Inventory;
using static ItemManager;
public class InventoryListener : MonoBehaviour
{
    public List<GameObject> holder;
    public GameObject itemholder_i;
    // Start is called before the first frame update
    void OnEnable()
    {
        if (holder.Count != m.holder_count)
        {
            holder = new List<GameObject>();

            for (int i = 0; i < m.holder_count; i++)
            {
                holder.Add(AddItemHolder(null));
                ItemStack item = holders[i].GetComponent<ItemHolder>().GetItem();
                if (item != null)
                {
                    holder[i].GetComponent<ItemHolder>().SetItemStack(item);
                }
            }
        }
        else
        {
            for (int i = 0; i < m.holder_count; i++)
            {
                ItemStack item = holders[i].GetComponent<ItemHolder>().GetItem();
                if (item != null)
                {
                    holder[i].GetComponent<ItemHolder>().SetItemStack(item);
                }
            }
        }
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

    public void ItemChanged(ItemHolder e)
    {
        int index = holder.IndexOf(e.gameObject);
        holders[index].GetComponent<ItemHolder>().SetItemStack(e.GetComponent<ItemHolder>().GetItem());
    }
}
