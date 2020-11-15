using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemHolderListener : MonoBehaviour
{
    public Image img;
    public Text count;

    public GameObject damage;

    public void OnChanged(ItemHolder itemHolder)
    {
        img.sprite = itemHolder.holder.GetComponent<Image>().sprite;
        count.text = itemHolder.cdis.text;
        if(itemHolder.GetItem() != null && itemHolder.GetItem().damage > 0)
        {
            damage.SetActive(true);
            damage.GetComponent<Slider>().maxValue = itemHolder.damage.GetComponent<Slider>().maxValue;
            damage.GetComponent<Slider>().value = itemHolder.GetItem().damage;
        }
        else
        {
            damage.SetActive(false);
        }
    }
}
