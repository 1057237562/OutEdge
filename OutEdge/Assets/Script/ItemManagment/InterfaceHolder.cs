using System;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;
using static GameControll;
using static UIManager;
public class InterfaceHolder : MonoBehaviour
{
    public bool visualizing = false;
    private ItemManager.ItemStack itemstack;

    private GameObject centerObject;
    // Start is called before the first frame update
    void Awake()
    {
        GetComponent<ItemHolder>().additem.AddListener(delegate {
            if (enabled)
            {
                VisuallizeItem();
            }
            
        });
        GetComponent<ItemHolder>().removeitem.AddListener(delegate {
            if (enabled)
            {
                Deactive();
            }
        });
    }

    private void Update()
    {
        if (visualizing && !ui.interacting)
        {
            if(Input.GetMouseButton(1)){
                if (itemstack.item.id >= 0)
                {
                    if (ItemManager.im.rightevents[itemstack.item.id](itemstack))
                    {
                        itemstack.count--;
                        GetComponent<ItemHolder>().UpdateUI();
                        /*if (itemstack.count == 0)
                        {
                            GetComponent<ItemHolder>().RemoveItem();
                        }*/
                    }
                }
                else
                {
                    //Fall Back
                }
            }
            if (Input.GetMouseButton(0))
            {
                if (itemstack.item.id >= 0)
                {
                    if (ItemManager.im.leftevents[itemstack.item.id] != null && ItemManager.im.leftevents[itemstack.item.id](itemstack))
                    {
                        itemstack.damage--;
                        if (itemstack.damage == 0)
                        {
                            GetComponent<ItemHolder>().RemoveItem();
                        }
                        else
                        {
                            GetComponent<ItemHolder>().UpdateUI();
                        }
                    }
                    else
                    {
                        localControll.animator.GetComponent<Animator>().SetBool("Attack", true);
                        Ray ray2 = Camera.main.ScreenPointToRay(Input.mousePosition);
                        RaycastHit hit2;
                        if (Physics.Raycast(ray2, out hit2, reachdis))
                        {
                            if (localControll.attackDelay < Time.fixedTime - localControll.lastAttack)
                            {
                                LivingEntity eb = hit2.collider.GetComponent<LivingEntity>();
                                if (eb != null)
                                {
                                    eb.ForceAppendCollision(localControll.force, hit2.collider.transform.position);
                                }
                                localControll.lastAttack = Time.fixedTime;
                            }
                        }
                    }
                }
            }
        }
    }

    public void VisuallizeItem()
    {
        if (!visualizing)
        {
            ItemHolder item = GetComponent<ItemHolder>();
            itemstack = item.GetItem();
            if (itemstack != null)
            {
                visualizing = true;
                if (itemstack.item.id < 0)
                {
                    localControll.usingitem = true;
                    localControll.archor.transform.localRotation = Quaternion.identity;
                    centerObject = FileSystem.GenerateGameObject(Environment.CurrentDirectory + "/saves/" + StartGame.savePath + "/items/" + Math.Abs(itemstack.item.id) + ".oeg", localControll.archor.transform.TransformPoint(new Vector3(0,1.3f,0)), localControll.archor.transform.parent.rotation,false, LayerMask.NameToLayer("HandyLayer"));
                    localControll.archor.GetComponent<Joint>().connectedBody = centerObject.GetComponent<Rigidbody>();
                    centerObject.GetComponent<CenterObject>().DeRegist();
                    //Active KeyListener
                    GameObject[] subObject = centerObject.GetComponent<CenterObject>().objlist.ToArray();

                    foreach (GameObject collider in subObject)
                    {
                        KeyListener[] kls = collider.GetComponents<KeyListener>();
                        foreach (KeyListener kl in kls)
                            kl.enabled = true;
                    }
                }
                else
                {
                    localControll.usingitem = true;
                    visualizing = true;
                    localControll.archor.transform.localRotation = Quaternion.identity;
                    centerObject = ItemManager.SummonItem(localControll.archor.transform.TransformPoint(new Vector3(0, 1.3f, 0)), itemstack.item,itemstack.damage);
                    centerObject.transform.rotation = localControll.archor.transform.parent.rotation;
                    centerObject.layer = LayerMask.NameToLayer("HandyLayer");
                    localControll.archor.GetComponent<Joint>().connectedBody = centerObject.GetComponent<Rigidbody>();
                    string[] attributes = itemstack.item.meta.Split(';');
                    foreach(string attribute in attributes)
                    {
                        string[] args = attribute.Split(' ');
                        switch (args[0])
                        {
                            case "AttackDamage":
                                localControll.force += int.Parse(args[1]);
                                break;
                        }
                    }
                }
            }
        }
    }

    public void Deactive()
    {
        if (visualizing)
        {
            localControll.usingitem = false;

            if (itemstack != null)
            {

                visualizing = false;
                if (itemstack.item.id < 0)
                {
                    localControll.archor.GetComponent<Joint>().connectedBody = null;
                    if (centerObject != null)
                    {
                        try
                        {
                            GameObject[] subObject = centerObject.GetComponent<CenterObject>().objlist.ToArray();

                            foreach (GameObject obj in subObject)
                            {
                                if (obj.gameObject.tag == "Material")
                                {
                                    obj.gameObject.layer = LayerMask.NameToLayer("SnapLayer");
                                }
                            }

                            centerObject.AddComponent<SnapShot>();
                            centerObject.GetComponent<SnapShot>().TakeSnapShot(centerObject,Environment.CurrentDirectory + "/saves/" + StartGame.savePath + "/items/" + Math.Abs(centerObject.GetComponent<CenterObject>().id) + ".oep", delegate
                            {
                                foreach (GameObject obj in subObject)
                                {
                                    if (obj.gameObject.tag == "Material")
                                    {
                                        Destroy(obj.gameObject);
                                    }
                                }
                            });
                            FileSystem.SerializeGameObjects(subObject, centerObject, Environment.CurrentDirectory + "/saves/" + StartGame.savePath + "/items/" + Math.Abs(centerObject.GetComponent<CenterObject>().id) + ".oeg",new FileSystem.Content(Vector3.zero,centerObject.GetComponent<CenterObject>().prerot), "Material");
                        }
                        catch
                        {

                        }
                    }
                }
                else
                {
                    if(centerObject != null)
                    {
                        //localControll.archor.GetComponent<Joint>().connectedBody = null;
                        Destroy(centerObject);
                    }
                    localControll.force = 100f;
                }
            }
            
        }
    }

    public void DropItem()
    {
        if (visualizing)
        {
            if(itemstack.item.id >= 0)
            {
                Destroy(centerObject);
                for(int i = 0; i < itemstack.count; i++)
                    ItemManager.SummonItem(RigidbodyFirstPersonController.rfpc.transform.position + RigidbodyFirstPersonController.rfpc.transform.rotation * new Vector3(0, 1, 2), itemstack.item,itemstack.damage);
                localControll.usingitem = false;
                visualizing = false;
                GetComponent<ItemHolder>().RemoveItem();
            }
            else if(centerObject != null)
            {
                localControll.usingitem = false;

                localControll.archor.GetComponent<Joint>().connectedBody = null;

                //Deactive KeyListener
                GameObject[] subObject = centerObject.GetComponent<CenterObject>().objlist.ToArray();
                centerObject.GetComponent<CenterObject>().Regist();

                foreach (GameObject obj in subObject)
                {
                    obj.layer = 0;

                    KeyListener[] kls = obj.GetComponents<KeyListener>();
                    foreach (KeyListener kl in kls)
                        kl.enabled = false;
                }

                visualizing = false;
                GetComponent<ItemHolder>().RemoveItem();
            }
        }
    }
}
