using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ItemManager;

public class ContainerUI : MonoBehaviour
{
    public Container container;
    public List<GameObject> holder;
    public GameObject itemholder_i;
    // Start is called before the first frame update
    void OnEnable ()
    {
        if(holder != null)
        {
            foreach(GameObject obj in holder)
            {
                Destroy(obj);
            }
            holder.Clear();
        }
        holder = new List<GameObject>();

        for (int i = 0; i < container.slotcount; i++)
        {
            holder.Add(AddItemHolder(null));
            ItemStack item = container.stacks[i];
            if (item != null)
            {
                holder[i].GetComponent<ItemHolder>().SetItem(item);
            }
        }
    }

    public GameObject AddItemHolder(Transform parent)
    {
        if (parent == null)
        {
            parent = itemholder_i.transform.parent;
        }
        GameObject itemholder_n = Instantiate(itemholder_i, parent);
        itemholder_n.SetActive(true);

        parent.gameObject.SetActive(false);//Restart
        parent.gameObject.SetActive(true);
        return itemholder_n;
    }

    public void ItemChanged(ItemHolder e)
    {
        int index = holder.IndexOf(e.gameObject);
        container.stacks[index] = e.GetComponent<ItemHolder>().GetItem();
    }
}
