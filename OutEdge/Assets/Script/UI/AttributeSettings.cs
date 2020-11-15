using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AttributeSettings : MonoBehaviour
{
    GameObject target;

    public GameObject nametag;

    public GameObject attributeList;
    public GameObject listItem;

    List<GameObject> items = new List<GameObject>();

    public GameObject slider;
    public GameObject inputfield;
    public GameObject keycodefield;
    public GameObject clickbox;

    public void SetTarget(GameObject tar)
    {
        target = tar;
        tar.GetComponent<AttributeContainer>().OnOpen();
        nametag.GetComponent<TextMeshProUGUI>().text = tar.name.Replace("(Clone)",null);

        foreach(AttributeContainer ac in tar.GetComponents<AttributeContainer>())
        {
            foreach(FieldInfo field in ac.GetType().GetFields())
            {
                GameObject n_item = Instantiate(listItem, attributeList.transform);

                AttributeType attributeType = field.GetCustomAttribute<AttributeType>();

                if (attributeType == null)
                    continue;

                switch (attributeType.type)
                {
                    case "String":
                        n_item.transform.GetComponent<TextMeshProUGUI>().text = field.Name + Environment.NewLine + Environment.NewLine;

                        GameObject ipf = Instantiate(inputfield, n_item.transform);

                        ipf.SetActive(true);

                        ipf.transform.localPosition = new Vector3(0, -10);

                        ipf.GetComponent<TextMeshProUGUI>().text = field.GetValue(ac).ToString();
                        break;
                    case "Int":
                        n_item.transform.GetComponent<TextMeshProUGUI>().text = field.Name + Environment.NewLine + Environment.NewLine + Environment.NewLine;

                        GameObject inta = Instantiate(slider, n_item.transform);

                        inta.SetActive(true);

                        inta.transform.localPosition = new Vector3(0, -10);

                        if (attributeType.metadata != "")
                        {
                            string[] vs = attributeType.metadata.Split(':');

                            Slider sli = inta.GetComponent<Slider>();

                            sli.maxValue = float.Parse(vs[1]);
                            sli.minValue = float.Parse(vs[0]);
                        }

                        inta.GetComponent<Slider>().value = float.Parse(field.GetValue(ac).ToString());
                        break;
                    case "KeyCode":
                        n_item.transform.GetComponent<TextMeshProUGUI>().text = field.Name + Environment.NewLine + Environment.NewLine + Environment.NewLine + Environment.NewLine;

                        GameObject kcd = Instantiate(keycodefield, n_item.transform);

                        kcd.SetActive(true);

                        kcd.transform.localPosition = new Vector3(0, -10);
                        kcd.GetComponent<DataContainer>().data = field.GetValue(ac).ToString();

                        kcd.transform.GetChild(0).GetComponent<Text>().text = field.GetValue(ac).ToString();
                        break;
                    case "Boolean":
                        try
                        {
                            bool isOn = bool.Parse(field.GetValue(ac).ToString());
                            n_item.transform.GetComponent<TextMeshProUGUI>().text = field.Name + Environment.NewLine + Environment.NewLine;

                            GameObject cb = Instantiate(clickbox, n_item.transform);

                            cb.SetActive(true);

                            cb.transform.localPosition = new Vector3(0, -10);
                            cb.GetComponent<DataContainer>().data = field.GetValue(ac).ToString();

                            cb.transform.GetComponent<Toggle>().isOn = isOn;
                        }
                        catch { Destroy(n_item); }
                        break;
                }
                n_item.SetActive(true);
                items.Add(n_item);
            }
        }
    }

    public void Apply()
    {

        int index = 0;

        foreach (AttributeContainer ac in target.GetComponents<AttributeContainer>())
        {
            foreach (FieldInfo field in ac.GetType().GetFields())
            {
                if (field.GetCustomAttribute<AttributeType>() == null)
                    continue;

                try
                {
                    field.SetValue(ac, items[index].transform.GetChild(1).GetComponent<DataContainer>().data);
                }
                catch { }

                index++;
            }
            ac.Apply();
        }
    }

    public void Close()
    {
        target.GetComponent<AttributeContainer>().OnClose();
    }
}

public class AttributeType : Attribute
{
    public string type;
    public string metadata;

    public AttributeType(string t,string m)
    {
        type = t;
        metadata = m;
    }
}
