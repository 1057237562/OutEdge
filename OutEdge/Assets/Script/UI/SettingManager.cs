using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Xml;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SettingManager : MonoBehaviour
{
    public List<Toggle> toggles;
    public List<InputField> inputs;
    public List<Dropdown> drops;

    // Start is called before the first frame update
    void Start()
    {
        LoadFile();
    }

    // Update is called once per frame
    public void UpdateFile()
    {
        XmlDocument content = new XmlDocument();
        XmlDeclaration dec = content.CreateXmlDeclaration("1.0", "UTF-8", null);
        content.AppendChild(dec);
        XmlElement objs = content.CreateElement("Settings");

        foreach (Toggle toggle in toggles)
        {
            XmlElement e = content.CreateElement(toggle.name);
            e.SetAttribute("Active", toggle.isOn.ToString());
            objs.AppendChild(e);
        }

        foreach (InputField text in inputs)
        {
            XmlElement e = content.CreateElement(text.name);
            e.SetAttribute("Text", text.text);
            objs.AppendChild(e);
        }

        foreach (Dropdown drop in drops)
        {
            XmlElement e = content.CreateElement(drop.name);
            e.SetAttribute("Select", drop.value.ToString());
            objs.AppendChild(e);
        }

        content.AppendChild(objs);
        content.Save(Environment.CurrentDirectory + "/settings.cfg");
    }

    public void LoadFile()
    {
        if (File.Exists(Environment.CurrentDirectory + "/settings.cfg"))
        {
            XmlDocument content = new XmlDocument();
            content.Load(Environment.CurrentDirectory + "/settings.cfg");
            int pointer = 0;
            XmlNodeList list = content.GetElementsByTagName("Settings")[0].ChildNodes;
            foreach (Toggle toggle in toggles)
            {
                try
                {
                    XmlElement e = (XmlElement)list[pointer];
                    toggle.isOn = bool.Parse(e.GetAttribute("Active"));
                    pointer++;
                }
                catch { }
            }

            foreach (InputField text in inputs)
            {
                try
                {
                    XmlElement e = (XmlElement)list[pointer];
                    text.text = e.GetAttribute("Text");
                    pointer++;
                }
                catch { }
            }

            foreach (Dropdown drop in drops)
            {
                try
                {
                    XmlElement e = (XmlElement)list[pointer];
                    drop.value = int.Parse(e.GetAttribute("Select"));
                    pointer++;
                }
                catch { }
            }
        }
    }
}
