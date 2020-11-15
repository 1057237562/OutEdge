using System;
using System.IO;
using System.Xml;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static ItemManager;

[System.Serializable]
public class UpdateEvent : UnityEvent<ItemHolder>
{
}

public class ItemHolder : MonoBehaviour, IPointerClickHandler
{
    public Sprite sp;
    private ItemStack holding = null;
    public MouseImage mouse;

    public GameObject holder;
    public Text cdis;
    public GameObject damage;

    public UpdateEvent additem;
    public UpdateEvent removeitem;

    private void Start()
    {
        UpdateUI();
    }
    public bool SetItem(ItemStack item)
    {
        if(holding == null)
        {
            cdis.text = item.count + "";
            holding = item;
            if(holding.item.id < 0)
            {
                Texture2D texture = GetTexture2DFromPath(Environment.CurrentDirectory + "/saves/" + StartGame.savePath + "/items/" + Math.Abs(item.item.id) + ".oep");
                holder.GetComponent<Image>().sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0, 0));
            }
            else
            {
                if(holding.item.id >= 0 && im.maxdamage[holding.item.id] > 0)
                {
                    damage.SetActive(true);
                    damage.GetComponent<Slider>().maxValue = im.maxdamage[holding.item.id];
                    damage.GetComponent<Slider>().value = holding.damage;
                }
                Texture2D @texture = im.texture2d[holding.item.id];
                holder.GetComponent<Image>().sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0, 0));
            }
            additem.Invoke(this);
            return true;
        }
        else if(item.item.id == holding.item.id)
        {
            holding.count += item.count;
            cdis.text = holding.count + "";
            additem.Invoke(this);
            return true;
        }
        return false;
    }

    public void SetItemStack(ItemStack itemstack)
    {
        if (itemstack != null)
        {
            holding = itemstack;
            cdis.text = itemstack.count + "";
            if (holding.item.id < 0)
            {
                Texture2D texture = GetTexture2DFromPath(Environment.CurrentDirectory + "/saves/" + StartGame.savePath + "/items/" + Math.Abs(itemstack.item.id) + ".oep");
                holder.GetComponent<Image>().sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0, 0));
            }
            else
            {
                Texture2D @texture = im.texture2d[holding.item.id];
                holder.GetComponent<Image>().sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0, 0));
            }
        }
        else
        {
            RemoveItem();
        }
        additem.Invoke(this);
    }

    public XmlElement AddXml(XmlDocument xml)
    {
        XmlElement ele = xml.CreateElement("ItemHolder");

        ele.SetAttribute("Content", JsonUtility.ToJson(holding));

        return ele;
    }

    public void ReadXml(XmlElement ele)
    {
        SetItemStack(JsonUtility.FromJson<ItemStack>(ele.GetAttribute("Content").Replace("&quot;","\"")));
    }

    public static Texture2D GetTexture2DFromPath(string imgPath)
    {
        FileStream fs = new FileStream(imgPath, FileMode.Open, FileAccess.Read);
        int byteLength = (int)fs.Length;
        byte[] imgBytes = new byte[byteLength];
        fs.Read(imgBytes, 0, byteLength);
        fs.Close();
        fs.Dispose();
        Texture2D t2d = new Texture2D(256, 256);
        t2d.LoadImage(imgBytes);
        t2d.Apply();
        return t2d;
    }

    public ItemStack GetItem()
    {
        return holding;
    }

    public void RemoveItem()
    {
        cdis.text = "";
        damage.SetActive(false);
        holder.GetComponent<Image>().sprite = sp;
        holding = null;
        removeitem.Invoke(this);
    }

    public void UpdateUI()
    {
        if (holding == null)
        {
            cdis.text = "";
            holder.GetComponent<Image>().sprite = sp;
        }
        else if (holding.count == 0)
        {
            cdis.text = "";
            holder.GetComponent<Image>().sprite = sp;
            holding = null;
            removeitem.Invoke(this);
        }
        else
        {
            cdis.text = holding.count + "";
            damage.GetComponent<Slider>().value = holding.damage;
            additem.Invoke(this);
        }
    }

    public bool RemoveItem(float count)
    {
        if(holding.count < count)
        {
            return false;
        }
        holding.count -= count;
        cdis.text = holding.count + "";
        if (holding.count == 0)
        {
            RemoveItem();
        }
        return true;
    }

    public bool isEmpty()
    {
        if(holding == null)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (holding != null)
            {
                if (MouseImage.holding != null)
                {
                    if (SetItem(MouseImage.holding))
                    {
                        mouse.dismiss();
                    }
                }
                else
                {
                    if (holding.item.id >= 0) { if (mouse.setTexture(im.texture2d[holding.item.id], holding)) { RemoveItem(); } }
                    else
                    { //Custom Items
                        Texture2D texture = GetTexture2DFromPath(Environment.CurrentDirectory + "/saves/" + StartGame.savePath + "/items/" + Math.Abs(holding.item.id) + ".oep"); if (mouse.setTexture(texture, holding)) { RemoveItem(); }
                    }
                }
            }
            else if (MouseImage.holding != null)
            {
                if (SetItem(MouseImage.holding))
                {
                    mouse.dismiss();
                }
            }
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (MouseImage.holding != null)
            {
                if (SetItem(new ItemStack(MouseImage.holding.item,1,MouseImage.holding.damage)))
                {
                    MouseImage.holding.count--;
                    if(MouseImage.holding.count == 0)
                        mouse.dismiss();
                }
            }
        }
    }
}
