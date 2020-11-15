using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;
using static ItemManager;

public class RecipeUI : MonoBehaviour
{
    public GameObject result;
    public ItemStack itemresult;
    public GameObject materialsList;
    public GameObject materialItem;

    public GameObject target;
    public GameObject text;
    GameObject preview;
    public static bool blocked = false;
    bool active = false;

    public Material material;
    public Material denymaterial;
    bool wait = false;

    public List<ItemStack> mater = new List<ItemStack>();

    public static float lastTime = 0;
    bool occpied = false;

    void Awake()
    {
        /*foreach(ItemStack item in mater)
        {
            GameObject n = Instantiate(materialItem, materialsList.transform);
            n.GetComponent<ItemHolder>().SetItem(new ItemStack(item.item,item.count,item.damage));
            n.SetActive(true);
        }*/
    }

    public void Clicked()
    {
        if (!blocked)
        {
            if (target == null)
            {
                if (Inventory.m.removeItems(mater))
                {
                    Inventory.m.attemptAddItem(itemresult.item, itemresult.count,itemresult.damage);
                    return;
                }
            }
            else
            {
                if (!Inventory.m.removeItems(mater))
                {
                    MessageBox.ShowMessage("You don't have enough materials for that.");
                    return;
                }

                UIManager.ui.deinteract();
                if (preview != null)
                {
                    Destroy(preview);
                }
                preview = Instantiate(target);
                preview.AddComponent<Trigger>();

                active = true;
                blocked = true;
                UIManager.ui.interacting = true;
            }
        }
    }

    private void Update()
    {
        if (active)
        {
            if (Input.GetMouseButtonDown(1) || InputManager.GetKeyDown("OpenInventory"))
            {
                active = false;
                blocked = false;
                UIManager.ui.interacting = false;
                Inventory.m.attemptAddItems(mater);
                Destroy(preview);
                return;
            }
            if (Input.GetMouseButtonDown(0))
            {
                if (!occpied)
                {
                    Ray ray = GameControll.localControll.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
                    RaycastHit hitInfo;
                    if (Physics.Raycast(ray, out hitInfo, Mathf.Infinity, 1 << LayerMask.NameToLayer("Default")))// Placing Block
                    {
                        if (Time.time - lastTime > 0.2f)
                        {
                            lastTime = Time.time;
                            GameObject nobj;
                            if (hitInfo.collider.GetComponent<StructureEntity>() == null || target.GetComponent<StructureEntity>() == null)
                            {
                                nobj = Instantiate(target, new Vector3(hitInfo.point.x, hitInfo.point.y + target.transform.lossyScale.y / 2 + 0.2f, hitInfo.point.z), RigidbodyFirstPersonController.rfpc.transform.rotation);

                            }
                            else
                            {
                                Transform trans = hitInfo.collider.transform;
                                StructureEntity se = target.GetComponent<StructureEntity>();
                                Vector3 multiplier = trans.position + se.AutoAlign(hitInfo.point, trans, target.transform);
                                nobj = Instantiate(target, multiplier, se.AutoRotate(hitInfo.point, trans, target.transform));
                                if (hitInfo.collider.GetComponent<StructureEntity>() != null)
                                {
                                    nobj.transform.parent = trans;
                                    Destroy(nobj.GetComponent<Rigidbody>());
                                }
                            }

                            if (nobj.GetComponent<StructureEntity>() != null)
                            {
                                nobj.GetComponent<StructureEntity>().mater = mater;
                            }

                            Collider[] co = nobj.GetComponents<Collider>();
                            foreach (Collider c in co)
                            {
                                c.isTrigger = false;
                            }

                            foreach (Transform child in nobj.transform)
                            {
                                Collider c = child.GetComponent<Collider>();

                                if (c)
                                {
                                    c.enabled = true; c.isTrigger = false;
                                }
                            }

                            nobj.layer = 0;
                            foreach (Transform child in nobj.transform)
                            {
                                if(child.gameObject.layer == 8) child.gameObject.layer = 0;
                            }
                            try
                            {
                                nobj.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
                            }
                            catch { }
                            try
                            {
                                nobj.GetComponent<EntityBase>().enabled = true;
                            }
                            catch { }
                            active = false;
                            blocked = false;
                            UIManager.ui.interacting = false;
                            wait = true;
                            Clicked();
                        }
                    }
                }
                else
                {
                    MessageBox.ShowMessage("Space have already been occupied.");
                }
            }

            if (wait && Input.GetMouseButtonUp(0))
            {
                wait = false;
            }
        }
    }

    void FixedUpdate()
    {
        if (active)
        {
            try
            {
                occpied = preview.GetComponent<Trigger>().occpied;
                Destroy(preview);
            }
            catch { }
            Ray ray = GameControll.localControll.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo,Mathf.Infinity,1<<LayerMask.NameToLayer("Default")))// Placing Block
            {
                if (!Input.GetMouseButtonDown(0))
                {
                    if (hitInfo.collider.GetComponent<StructureEntity>() == null || target.GetComponent<StructureEntity>() == null)
                    {
                        preview = Instantiate(target, new Vector3(hitInfo.point.x, hitInfo.point.y + target.transform.lossyScale.y / 2 + 0.2f, hitInfo.point.z), RigidbodyFirstPersonController.rfpc.transform.rotation);
                    }
                    else
                    {
                        Transform trans = hitInfo.collider.transform;
                        Vector3 multiplier = trans.position + target.GetComponent<StructureEntity>().AutoAlign(hitInfo.point, trans, target.transform);
                        preview = Instantiate(target, multiplier, target.GetComponent<StructureEntity>().AutoRotate(hitInfo.point, trans, target.transform));
                    }
                    preview.AddComponent<Trigger>();
                    //Draw Rendering
                    if (!occpied)
                    {
                        try
                        {
                            Material[] mat = preview.GetComponent<Renderer>().materials;
                            for (int j = 0; j < preview.GetComponent<Renderer>().materials.Length; j++)
                            {
                                mat[j] = material;
                            }
                            preview.GetComponent<Renderer>().materials = mat;
                        }
                        catch
                        {

                        }

                        for (int i = 0; i < preview.transform.childCount; i++)
                        {
                            try
                            {
                                Material[] mat = preview.transform.GetChild(i).GetComponent<Renderer>().materials;
                                for (int j = 0; j < preview.transform.GetChild(i).GetComponent<Renderer>().materials.Length; j++)
                                {
                                    mat[j] = material;
                                }
                                preview.transform.GetChild(i).GetComponent<Renderer>().materials = mat;
                            }
                            catch
                            {

                            }
                        }
                    }
                    else
                    {
                        try
                        {
                            Material[] mat = preview.GetComponent<Renderer>().materials;
                            for (int j = 0; j < preview.GetComponent<Renderer>().materials.Length; j++)
                            {
                                mat[j] = denymaterial;
                            }
                            preview.GetComponent<Renderer>().materials = mat;
                        }
                        catch
                        {

                        }

                        for (int i = 0; i < preview.transform.childCount; i++)
                        {
                            try
                            {
                                Material[] mat = preview.transform.GetChild(i).GetComponent<Renderer>().materials;
                                for (int j = 0; j < preview.transform.GetChild(i).GetComponent<Renderer>().materials.Length; j++)
                                {
                                    mat[j] = denymaterial;
                                }
                                preview.transform.GetChild(i).GetComponent<Renderer>().materials = mat;
                            }
                            catch
                            {

                            }
                        }
                    }

                }
            }
        }
    }
}
