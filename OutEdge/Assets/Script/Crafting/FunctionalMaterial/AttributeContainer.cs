using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;
using UnityEngine;

public class AttributeContainer : MonoBehaviour
{

    bool opened = false;

    public XmlElement Serialize(XmlDocument doc)
    {
        XmlElement ele = doc.CreateElement("Attributes");
        FieldInfo[] fields = GetType().GetFields();  //Only retrun public field
        foreach (FieldInfo item in fields)
        {
            if (item.GetCustomAttribute<AttributeType>() == null)
                continue;
            XmlElement a = doc.CreateElement("Attribute");
            object obj = item.GetValue(this);
            a.SetAttribute(item.Name, obj.ToString());
            ele.AppendChild(a);
        }
        return ele;
    }

    public void Deserialize(XmlElement attributes)
    {
        XmlNodeList attributeList = attributes.GetElementsByTagName("Attribute");
        foreach(XmlElement xmlElement in attributeList)
        {
            FieldInfo field = GetType().GetField(xmlElement.Attributes[0].Name);
            field.SetValue(this, xmlElement.Attributes[0].Value);
        }
    }

    public virtual void Apply(bool firstload = false) { }

    public bool CanOpen()
    {
        return !opened;
    }

    public void OnClose()
    {
        opened = false;
    }

    public void OnOpen()
    {
        opened = true;
    }
}
