using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildHistory : MonoBehaviour
{
    class SerialTransform
    {
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 scale;
        public Component[] components;

        public SerialTransform(Vector3 pos,Quaternion quaternion,Vector3 sca,Component[] cp)
        {
            position = pos;
            rotation = quaternion;
            scale = sca;
            components = cp;
        }
    }

    class BuildCommand
    {
        public string command_name = "";
        SerialTransform former;
        SerialTransform after;
        GameObject target_f;
        GameObject target_a;

        public void ListenBegin(GameObject @obj)
        {
            if(obj == null)
            {
                target_f = null;
                return;
            }
            target_f = obj;
            Component[] @copy = new Component[0]; obj.GetComponents<Component>().CopyTo(copy, 0);
            former = new SerialTransform(obj.transform.position, obj.transform.rotation, obj.transform.localScale, copy);
        }

        public void ListenEnd(GameObject @obj)
        {
            if(obj == null)
            {
                target_a = null;
                return;
            }
            target_a = obj;
            after = new SerialTransform(obj.transform.position, obj.transform.rotation, obj.transform.localScale, obj.GetComponents<Component>()); // Because Undo is stackful. Reverse will bring back the Components
        }

        public void Undo()
        {
            if (target_f == null)
            {
                Destroy(target_a);
            }
            else
            {
                target_f.transform.position = former.position;
                target_f.transform.rotation = former.rotation;
                target_f.transform.localScale = former.scale;
                Component[] coms = target_f.GetComponents<Component>();
                Component[] f_coms = former.components;
                if (!coms.Equals(f_coms))
                {
                    foreach (Component @com in coms)
                    {
                        Destroy(com);
                    }
                    foreach (Component @com in f_coms)
                    {
                        CopyComponent(com, target_f);
                    }
                }
            }
        }
        
        public void Excute()
        {
            if (target_a == null)
            {
                Destroy(target_a);
            }
            else
            {
                target_a.transform.position = after.position;
                target_a.transform.rotation = after.rotation;
                target_a.transform.localScale = after.scale;
                Component[] coms = target_a.GetComponents<Component>();
                Component[] a_coms = after.components;
                if (!coms.Equals(a_coms))
                {
                    foreach (Component @com in coms)
                    {
                        Destroy(com);
                    }
                    foreach (Component @com in a_coms)
                    {
                        CopyComponent(com, target_a);
                    }
                }
            }
        }
    }

    public static T CopyComponent<T>(T original, GameObject destination) where T : Component
    {
        System.Type type = original.GetType();
        Component copy = destination.AddComponent(type);
        System.Reflection.FieldInfo[] fields = type.GetFields();
        foreach (System.Reflection.FieldInfo field in fields)
        {
            field.SetValue(copy, field.GetValue(original));
        }
        return copy as T;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
