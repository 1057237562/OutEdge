using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class ItemManager : MonoBehaviour
{
    public static ItemManager im;

    [Serializable]
    public class Item
    {

        public int id; // Normal Item 0~Inf //Custom Item -1~-Inf
        public int sub;
        public string meta;

        public Item(int id, int sub, string meta)
        {
            this.id = id;
            this.sub = sub;
            this.meta = meta;
        }
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            return !(obj is Item p) ? false : id == p.id && sub == p.sub && meta == p.meta;
        }

        public override int GetHashCode()
        {
            return (id+""+sub+""+meta).GetHashCode();
        }

        public static bool operator ==(Item a, Item b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(Item a, Item b)
        {
            return !(a == b);
        }
    }

    [Serializable]
    public class ItemStack
    {
        public Item item;
        public float count;
        public int damage;

        public ItemStack(Item items, float count, int damage)
        {
            item = items;
            this.count = count;
            this.damage = damage;
        }

        public static bool operator >=(ItemStack a, ItemStack b)
        {
            if (a.item.Equals(b.item))
            {
                if(a.count >= b.count)
                {
                    return true;
                }
            }
            return false;
        }

        public static bool operator <=(ItemStack a, ItemStack b)
        {
            if (a.item.Equals(b.item))
            {
                if (a.count <= b.count)
                {
                    return true;
                }
            }
            return false;
        }

        public static bool operator >(ItemStack a, ItemStack b)
        {
            if (a.count > b.count)
            {
                return true;
            }
            return false;
        }

        public static bool operator <(ItemStack a, ItemStack b)
        {
            if (a.count < b.count)
            {
                return true;
            }
            return false;
        }
    }

    public Texture2D[] texture2d;
    public GameObject[] prefabs;

    [Serializable]
    public struct Result
    {
        public List<ItemStack> result;
    }
    public List<Result> algriculture = new List<Result>();

    public Func<ItemStack, bool>[] rightevents;
    public Func<ItemStack, bool>[] leftevents;
    readonly Func<ItemStack, bool> marchingfunc = new Func<ItemStack, bool>((ItemStack itemStack) => {
        GameControll.buildid = itemStack.item.id;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hitInfo, GameControll.reachdis))
        {
            GameObject gameObj = hitInfo.collider.gameObject;
            MarchingObject mc = hitInfo.collider.gameObject.GetComponent<MarchingObject>();
            if (mc != null)
            {
                Vector3 localhit = hitInfo.collider.transform.InverseTransformPoint(hitInfo.point);

                return mc.AddBlock(localhit);
            }
            else
            {
                return false;
            }
        }
        return false;
    });
    readonly Func<ItemStack, bool> stonefunc = new Func<ItemStack, bool>((ItemStack itemStack) =>
    {
        GameControll.buildid = itemStack.item.id;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hitInfo, GameControll.reachdis))
        {
            GameObject gameObj = hitInfo.collider.gameObject;
            MarchingObject mc = hitInfo.collider.GetComponent<MarchingObject>();
            if (mc != null)
            {
                Vector3 localhit2 = hitInfo.collider.transform.InverseTransformPoint(hitInfo.point);
                mc.RemoveBlock(localhit2, 2);
            }
            else
            {
                return false;
            }
        }
        return true;
    });
    readonly Func<ItemStack, bool> ironfunc = new Func<ItemStack, bool>((ItemStack itemStack) =>
    {
        GameControll.buildid = itemStack.item.id;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hitInfo, GameControll.reachdis))
        {
            GameObject gameObj = hitInfo.collider.gameObject;
            MarchingObject mc = hitInfo.collider.GetComponent<MarchingObject>();
            if (mc != null)
            {
                Vector3 localhit2 = hitInfo.collider.transform.InverseTransformPoint(hitInfo.point);
                mc.RemoveBlock(localhit2, 3);
            }
            else
            {
                return false;
            }
        }
        return true;
    });

    public static Dictionary<int,float> fooditem;
    public static Dictionary<int, float> drinkitem;
    public static Dictionary<int, float> healitem;
    public static Dictionary<int, int> igniteitem;
    public List<int> maxdamage;

    [Serializable]
    public class PrefabReflect : SerializeableDictionary<int, GameObject> { }
    [SerializeField]
    public PrefabReflect placeitem;
    readonly Func<ItemStack, bool> foodfunc = new Func<ItemStack, bool>((ItemStack itemStack) => {
        if (PlayerEntity.localPlayer.MaxSatisfaction - PlayerEntity.localPlayer.Satisfaction > 1)
        {
            fooditem.TryGetValue(itemStack.item.id, out float addition);
            PlayerEntity.localPlayer.Satisfaction += addition;
            drinkitem.TryGetValue(itemStack.item.id, out addition);
            PlayerEntity.localPlayer.Thrist += addition;
            return true;
        }
        return false;
    });
    readonly Func<ItemStack, bool> healfunc = new Func<ItemStack, bool>((ItemStack itemStack) => {
        if (PlayerEntity.localPlayer.maxHealth - PlayerEntity.localPlayer.nowHealth > 1)
        {
            healitem.TryGetValue(itemStack.item.id, out float addition);
            PlayerEntity.localPlayer.nowHealth += addition;
            return true;
        }
        return false;
    });
    readonly Func<ItemStack, bool> drinkfunc = new Func<ItemStack, bool>((ItemStack itemStack) => {
        if (PlayerEntity.localPlayer.MaxThrist - PlayerEntity.localPlayer.Thrist > 1)
        {
            drinkitem.TryGetValue(itemStack.item.id, out float addition);
            PlayerEntity.localPlayer.Thrist += addition;
            return true;
        }
        return false;
    });
    readonly Func<ItemStack, bool> ignitefunc = new Func<ItemStack, bool>((ItemStack itemStack) =>
    {
        if (itemStack.count >= 2)
        {
            igniteitem.TryGetValue(itemStack.item.id, out int temperature);
            Ray ray2 = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray2, out RaycastHit hit2, GameControll.reachdis))
            {
                FuelObject fo = hit2.collider.GetComponent<FuelObject>();
                if (fo != null)
                {
                    if (UnityEngine.Random.Range(0, 1) < 0.25f)
                    {
                        ThermalReceiver tr = hit2.collider.GetComponent<ThermalReceiver>();
                        tr.temperature = temperature;
                        fo.FixedUpdate();
                    }
                }
            }
            return true;
        }
        return false;
    });
    readonly Func<ItemStack, bool> placefunc = new Func<ItemStack, bool>((ItemStack itemStack) =>
    {
        Ray ray = GameControll.localControll.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hitInfo, GameControll.reachdis))// Placing Block
        {
            im.placeitem.TryGetValue(itemStack.item.id, out GameObject target);
            GameObject nobj = Instantiate(target, new Vector3(hitInfo.point.x, hitInfo.point.y + target.transform.lossyScale.y / 2 + 0.2f, hitInfo.point.z), RigidbodyFirstPersonController.rfpc.transform.rotation);
            return true;
        }
        return false;
    });

    readonly Func<ItemStack, bool> plantfunc = new Func<ItemStack, bool>((ItemStack itemStack) =>
    {
        Ray ray = GameControll.localControll.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hitInfo, GameControll.reachdis) && Input.GetMouseButtonDown(1) && hitInfo.collider.GetComponent<MarchingObject>() != null)// Placing Block
        {
            im.placeitem.TryGetValue(itemStack.item.id, out GameObject target);
            GameObject nobj = Instantiate(target, new Vector3(hitInfo.point.x, hitInfo.point.y, hitInfo.point.z), RigidbodyFirstPersonController.rfpc.transform.rotation);
            return true;
        }
        return false;
    });

    // Start is called before the first frame update
    void Awake()
    {
        im = this;

        fooditem = new Dictionary<int, float>();
        drinkitem = new Dictionary<int, float>();
        healitem = new Dictionary<int, float>();
        igniteitem = new Dictionary<int, int>();

        rightevents = new Func<ItemStack, bool>[prefabs.Length];
        leftevents = new Func<ItemStack, bool>[prefabs.Length];

        rightevents[0] = marchingfunc;
        rightevents[1] = marchingfunc;
        rightevents[2] = marchingfunc;
        rightevents[3] = marchingfunc;

        rightevents[4] = foodfunc;
        rightevents[5] = foodfunc;

        rightevents[8] = ignitefunc;

        rightevents[10] = foodfunc;
        rightevents[11] = foodfunc;

        rightevents[13] = placefunc;

        rightevents[17] = plantfunc;
        rightevents[18] = plantfunc;
        rightevents[19] = plantfunc;

        leftevents[15] = stonefunc;
        leftevents[16] = ironfunc;

        fooditem.Add(4, 3f);
        fooditem.Add(5, 3.5f);

        fooditem.Add(10, 7.5f);
        fooditem.Add(11, 6f);

        drinkitem.Add(4, 1f);
        drinkitem.Add(5, 1f);

        drinkitem.Add(10, 0.5f);
        drinkitem.Add(11, 0.5f);

        igniteitem.Add(8, 225);
    }

    public static GameObject SummonItem(Vector3 point,Item item,int damage = 0)
    {
        GameObject p = Instantiate(im.prefabs[item.id], point, Quaternion.identity);
        p.GetComponent<ItemBase>().item = item;
        p.GetComponent<ItemBase>().damage = damage;
        return p;
    }
}
