using CommandTerminal;
using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;
using static ItemManager;
using static UnityStandardAssets.Characters.FirstPerson.RigidbodyFirstPersonController;
using static UIManager;
using AuraAPI;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

public class GameControll : MonoBehaviour
{
    public bool usingitem = false;
    public GameObject archor;

    public static GameControll localControll;

    public static EventQueue physicQueue = new EventQueue();

    public static System.Random globalRandomize;
    public static int randomseed;

    public static int buildid = 1;

    public RenderTexture Itemrender;

    public static bool triggered = false;

    public GameObject animator;
    public GameObject leftcamera;
    public GameObject rightcamera;
    public GameObject neck;

    public float lastAttack;
    public float attackDelay = 0.65f;
    public float force = 100f;
    GameObject grabobj;
    float distance = 2;
    float lastdrag = 0;

    public static float reachdis = 6f;

    // Start is called before the first frame update
    void Start()
    {
        localControll = this;
        TimeManager.startTime = Time.fixedTime;

        if (QualitySettings.GetQualityLevel() == 0)
        {
            GetComponent<UnityEngine.Rendering.PostProcessing.PostProcessLayer>().enabled = false;
        }else{
            GetComponent<UnityEngine.Rendering.PostProcessing.PostProcessLayer>().enabled = true;
        }
        if(QualitySettings.GetQualityLevel() < 5)
        {
            GetComponent<CloudScript>().enabled = false;
        }
        if (QualitySettings.GetQualityLevel() < 4)
        {
            GetComponent<SEGI>().enabled = false;
        }
        if (QualitySettings.GetQualityLevel() < 4)
        {
            GetComponent<Aura>().enabled = false;
        }

        ui.settings.GetComponent<SettingManager>().LoadFile();

        SnapShot.renderTexture = Itemrender;
    }

    public static void loadGame()
    {
        if (Directory.Exists(Environment.CurrentDirectory + "/saves/" + StartGame.savePath))
        {
            //mg.GetComponent<CharacterController>().enabled = false;
            try
            {
                FileSystem.ReadCharacter(rfpc.gameObject, rfpc.cam.gameObject, Environment.CurrentDirectory + "/saves/" + StartGame.savePath + "/players.dat");
            }
            catch(Exception e)
            {
                Debug.LogError(e);
            }
            //mg.GetComponent<CharacterController>().enabled = true;
        }
        else
        {
            Inventory.m.Reload(null, null);
        }
    }

    void LateUpdate()
    {
        if (!ui.interacting &&Input.GetMouseButton(0) && usingitem && !triggered)
        {
            //TODO : Attack
            archor.transform.localRotation = Quaternion.RotateTowards(archor.transform.localRotation, Quaternion.Euler(0, -90, 0), 30);// (Vector3.Lerp(archor.transform.localRotation.eulerAngles,new Vector3(0, -90, 0),Time.deltaTime),Space.Self);
        }
        else
        {
            archor.transform.localRotation = Quaternion.RotateTowards(archor.transform.localRotation, Quaternion.identity, 30);
        }
        triggered = false;
    }

    [RegisterCommand(Help = "Get current map's seed")]
    public static void seed(CommandArg[] args)
    {
        Terminal.Log(TerminalLogType.input, "{0}", randomseed);
    }

    float destTime;

    // Update is called once per frame
    void Update()
    {

        if (!ui.interacting) {
            // remove block .. with mouse
            /*if (Input.GetMouseButtonDown(0) && !usingitem)
            {
                animator.GetComponent<Animator>().SetBool("Attack", true);
                Ray ray2 = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit2;
                if (Physics.Raycast(ray2, out hit2))
                {
                    if (attackDelay < Time.fixedTime - lastAttack)
                    {
                        LivingEntity eb = hit2.collider.GetComponent<LivingEntity>();
                        if (eb != null)
                        {
                            Debug.Log(force);
                            eb.ForceAppendCollision(force, hit2.collider.transform.position);
                        }
                        lastAttack = Time.fixedTime;
                    }
                }
            }*/
            if (Input.GetMouseButton(0) && !usingitem) // button is held down
            {
                animator.GetComponent<Animator>().SetBool("Attack", true);
                Ray ray2 = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit2;
                if (Physics.Raycast(ray2, out hit2,reachdis))
                {
                    MarchingObject mc = hit2.collider.GetComponent<MarchingObject>();
                    if (mc != null)
                    {
                        Vector3 localhit2 = hit2.collider.transform.InverseTransformPoint(hit2.point);
                        /*if (mc.show)
                        {
                            Instantiate(mc.pres, hit2.point, Quaternion.identity).SetActive(true);
                            Debug.Log(hit2.triangleIndex + ":" + hit2.point);
                            for(int i = 0; i < mc.rangeList.GetRangeCount() - 1; i++)
                            {
                                if(mc.rangeList.GetRange(i).start < hit2.triangleIndex * 3 && mc.rangeList.GetRange(i+1).start > hit2.triangleIndex * 3)
                                {
                                    Debug.Log("In z:" + i + "Vertice index : "+ hit2.triangleIndex * 3);
                                    for (int j = 0; j < mc.rangeList.GetRangeCount() - 1; j++)
                                    {
                                        if (mc.rangeList.GetRange(i).start + ((RangeList)mc.rangeList.GetRange(i)).GetRange(j).start < hit2.triangleIndex * 3 && mc.rangeList.GetRange(i).start + ((RangeList)mc.rangeList.GetRange(i)).GetRange(j + 1).start > hit2.triangleIndex * 3)
                                        {
                                            Debug.Log("In x:" + j + " bt x  start in:" + (mc.rangeList.GetRange(i).start + ((RangeList)mc.rangeList.GetRange(i)).GetRange(j).start) + " and its count" + ((RangeList)mc.rangeList.GetRange(i)).GetRange(j).count);
                                            int start = mc.rangeList.GetRange(i).start + ((RangeList)mc.rangeList.GetRange(i)).GetRange(j).start;
                                            for (int k = 0; k < ((RangeList)((RangeList)mc.rangeList.GetRange(i)).GetRange(j)).GetRangeCount() - 1; k++)
                                            {
                                                Range r = ((RangeList)((RangeList)mc.rangeList.GetRange(i)).GetRange(j)).GetRange(k);
                                                Range rp = ((RangeList)((RangeList)mc.rangeList.GetRange(i)).GetRange(j)).GetRange(k+1);
                                                Debug.Log((start + r.start) + "to" + (start +r.start+ r.count)+":"+(start + rp.start));
                                                if (start + r.start <= hit2.triangleIndex * 3 && start + rp.start > hit2.triangleIndex * 3)
                                                {
                                                    Debug.Log("In y:" + k);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }*/
                        mc.RemoveBlock(localhit2, 0);
                    }
                    if (attackDelay < Time.fixedTime - lastAttack)
                    {
                        LivingEntity eb = hit2.collider.GetComponent<LivingEntity>();
                        if (eb != null)
                        {
                            eb.ForceAppendCollision(force, hit2.collider.transform.position);
                        }
                        lastAttack = Time.fixedTime;
                    }
                }
            }
            else
            {
                animator.GetComponent<Animator>().SetBool("Attack", false);
            }

            if (Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1))
            {
                physicQueue.RunQueue();
            }

            if (Input.GetMouseButton(1) && !usingitem && !ui.interacting)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hitInfo;
                if (Physics.Raycast(ray, out hitInfo, reachdis,1<<LayerMask.NameToLayer("Default")))
                {
                    GameObject gameObj = hitInfo.collider.gameObject;

                    if (Input.GetMouseButtonDown(1))
                    {
                        Seat seat = gameObj.GetComponent<Seat>();
                        if (seat != null)
                        {
                            seat.AttachSeat(gameObject);
                        }

                        ClickableObject co = gameObj.GetComponent<ClickableObject>();
                        if (co != null)
                        {
                            ItemStack its = null;
                            if (ui.mainhd.GetComponent<InterfaceHolder>().enabled)
                            {
                                its = ui.mainhd.GetComponent<ItemHolder>().GetItem();
                            }
                            if (ui.sechd.GetComponent<InterfaceHolder>().enabled)
                            {
                                its = ui.sechd.GetComponent<ItemHolder>().GetItem();
                            }
                            if (ui.mischd.GetComponent<InterfaceHolder>().enabled)
                            {
                                its = ui.mischd.GetComponent<ItemHolder>().GetItem();
                            }
                            co.action(its);

                            if (its != null)
                            {
                                if (ui.mainhd.GetComponent<InterfaceHolder>().enabled)
                                {
                                    ui.mainhd.GetComponent<ItemHolder>().UpdateUI();
                                }
                                if (ui.sechd.GetComponent<InterfaceHolder>().enabled)
                                {
                                    ui.sechd.GetComponent<ItemHolder>().UpdateUI();
                                }
                                if (ui.mischd.GetComponent<InterfaceHolder>().enabled)
                                {
                                    ui.mischd.GetComponent<ItemHolder>().UpdateUI();
                                }
                            }
                        }
                        else
                        {
                            try
                            {
                                GuiObject gui = gameObj.GetComponent<GuiObject>();
                                if (gui != null)
                                {
                                    ui.interactor = gui;
                                    gui.interacting = true;
                                    ui.interacting = true;
                                    if (gui.gui != null)
                                    {
                                        GetComponent<Camera>().enabled = false;
                                        gui.gui.GetComponent<Camera>().enabled = true;
                                        GetComponent<AudioListener>().enabled = false;
                                        gui.gui.GetComponent<AudioListener>().enabled = true;
                                    }
                                    rfpc.releaseControll = false;
                                    ui.interactor.Interact();
                                    ui.normal.enabled = false;
                                }
                            }
                            catch
                            {

                            }
                        }
                    }

                }
            }


            if (InputManager.GetKey("PickItem"))
            {
                //iterate Entities First
                Unity.Physics.RaycastHit hit;
                Entity entity = PhysicUtil.Raycast(PhysicUtil.Convert(transform.position), PhysicUtil.Convert(transform.position + transform.rotation * new Vector3(0, 0, reachdis)), out hit);
                //Debug.Log(transform.position + " to " + (transform.position + transform.rotation * new Vector3(0, 0, reachdis)));
                if (entity != Entity.Null)
                {
                    IGrowable data = World.DefaultGameObjectInjectionWorld.EntityManager.GetComponentData<IGrowable>(entity);
                    if (InputManager.GetKeyDown("PickItem"))
                    {
                        if (data.seperated)
                        {
                            DynamicBuffer<Child> childs = World.DefaultGameObjectInjectionWorld.EntityManager.GetBuffer<Child>(entity);
                            Entity nearest = childs[0].Value;
                            float3 hitpos = hit.Position - World.DefaultGameObjectInjectionWorld.EntityManager.GetComponentData<Translation>(entity).Value;
                            float dis = math.distance(World.DefaultGameObjectInjectionWorld.EntityManager.GetComponentData<Translation>(nearest).Value, hitpos);
                            for (int i = 1; i < childs.Length; i++)
                            {
                                IGrowable e_data = World.DefaultGameObjectInjectionWorld.EntityManager.GetComponentData<IGrowable>(childs[i].Value);
                                float m_dis = math.distance(World.DefaultGameObjectInjectionWorld.EntityManager.GetComponentData<Translation>(childs[i].Value).Value, hitpos);
                                if (dis >= m_dis && (TimeManager.GetCurrentPlayTime() - e_data.startTime) / (e_data.matureTime - e_data.startTime) >= 1f)
                                {
                                    nearest = childs[i].Value;
                                    dis = m_dis;
                                }
                            }
                            IGrowable m_data = World.DefaultGameObjectInjectionWorld.EntityManager.GetComponentData<IGrowable>(nearest);
                            float rate = Mathf.Clamp((TimeManager.GetCurrentPlayTime() - m_data.startTime) / (m_data.matureTime - m_data.startTime), 0, 1);
                            foreach (ItemStack stack in im.algriculture[m_data.ResultType].result)
                            {
                                float currentheight = 0;
                                for (int i = 0; i < Mathf.FloorToInt(stack.count * rate); i++)
                                {
                                    SummonItem((Vector3)hit.Position + new Vector3(0, currentheight, 0), stack.item);
                                    currentheight += im.prefabs[stack.item.id].transform.lossyScale.y;
                                }
                            }
                            IGrowable t_data = new IGrowable { startTime = TimeManager.GetCurrentPlayTime(), matureTime = TimeManager.GetCurrentPlayTime() + m_data.grownTime + UnityEngine.Random.Range(-m_data.randomBiasRange, m_data.randomBiasRange), curve = m_data.curve, ResultType = m_data.ResultType, seperated = m_data.seperated, grownTime = m_data.grownTime, randomBiasRange = m_data.randomBiasRange };
                            World.DefaultGameObjectInjectionWorld.EntityManager.SetComponentData(nearest, t_data);
                        }
                        else
                        {
                            float rate = Mathf.Clamp((TimeManager.GetCurrentPlayTime() - data.startTime) / (data.matureTime - data.startTime), 0, 1);
                            foreach (ItemStack stack in im.algriculture[data.ResultType].result)
                            {
                                float currentheight = 0;
                                for (int i = 0; i < Mathf.FloorToInt(stack.count * rate); i++)
                                {
                                    SummonItem((Vector3)hit.Position + new Vector3(0, currentheight, 0), stack.item);
                                    currentheight += im.prefabs[stack.item.id].transform.lossyScale.y;
                                }
                            }
                            IGrowable t_data = new IGrowable { startTime = TimeManager.GetCurrentPlayTime(), matureTime = TimeManager.GetCurrentPlayTime() + data.grownTime + UnityEngine.Random.Range(-data.randomBiasRange, data.randomBiasRange), curve = data.curve, ResultType = data.ResultType, seperated = data.seperated, grownTime = data.grownTime, randomBiasRange = data.randomBiasRange };
                            World.DefaultGameObjectInjectionWorld.EntityManager.SetComponentData(entity, t_data);
                        }
                        destTime = Time.time;
                    }
                    ui.progression.transform.parent.gameObject.SetActive(true);
                    ui.progression.GetComponent<Slider>().value = (Time.time - destTime) / 4f;
                    if (Time.time - destTime > 4)
                    {
                        DynamicBuffer<Child> childs = World.DefaultGameObjectInjectionWorld.EntityManager.GetBuffer<Child>(entity);

                        foreach (Child c in childs)
                        {
                            World.DefaultGameObjectInjectionWorld.EntityManager.DestroyEntity(c.Value);
                        }
                        World.DefaultGameObjectInjectionWorld.EntityManager.DestroyEntity(entity);
                    }
                }
                else
                {
                    Ray i_ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    RaycastHit i_hitInfo;
                    if (Physics.Raycast(i_ray, out i_hitInfo, reachdis))
                    {
                        GameObject gameObj = i_hitInfo.collider.gameObject;
                        if (InputManager.GetKeyDown("PickItem"))
                        {
                            destTime = Time.time;
                        }
                        if (gameObj.GetComponent<StructureEntity>() != null)
                        {
                            StructureEntity se = gameObj.GetComponent<StructureEntity>();
                            ui.progression.transform.parent.gameObject.SetActive(true);
                            ui.progression.GetComponent<Slider>().value = (Time.time - destTime) / 4f;
                            if (Time.time - destTime > 4)
                            {
                                foreach (Transform child in gameObj.transform)
                                {
                                    if (child.GetComponent<StructureEntity>() != null)
                                    {
                                        child.parent = null;
                                        child.GetComponent<StructureEntity>().Start();
                                        Rigidbody rb = child.gameObject.AddComponent<Rigidbody>();
                                        rb.mass = im.prefabs[child.GetComponent<StructureEntity>().id].GetComponent<Rigidbody>().mass;
                                    }
                                    else
                                    {
                                        Destroy(child.gameObject);
                                    }
                                }
                                Inventory.m.attemptAddItems(se.mater);
                                Destroy(gameObj);
                            }
                        }
                        if (gameObj.GetComponent<Crafter>() != null)
                        {
                            Crafter se = gameObj.GetComponent<Crafter>();
                            ui.progression.transform.parent.gameObject.SetActive(true);
                            ui.progression.GetComponent<Slider>().value = (Time.time - destTime) / 4f;
                            if (Time.time - destTime > 4)
                            {
                                Inventory.m.attemptAddItems(se.mater);
                                Destroy(gameObj);
                            }
                        }
                        if (InputManager.GetKeyDown("PickItem"))
                        {
                            if ((gameObj.GetComponent<ConnectiveMaterial>() != null || gameObj.GetComponent<CenterObject>() != null) && (gameObj.GetComponent<CenterObject>() == null ? gameObj.GetComponent<ConnectiveMaterial>().centerparent.GetComponent<CenterObject>() : gameObj.GetComponent<CenterObject>()).pickable)
                            {
                                try
                                {
                                    GameObject[] subObject = gameObj.GetComponent<CenterObject>() == null ? gameObj.GetComponent<ConnectiveMaterial>().centerparent.GetComponent<CenterObject>().objlist.ToArray() : gameObj.GetComponent<CenterObject>().objlist.ToArray();

                                    foreach (GameObject obj in subObject)
                                    {
                                        if (obj.tag == "Material")
                                        {
                                            obj.layer = LayerMask.NameToLayer("SnapLayer");
                                        }
                                    }

                                    GameObject centerobject = gameObj.GetComponent<CenterObject>() == null ? gameObj.GetComponent<ConnectiveMaterial>().centerparent : gameObj;

                                    FileSystem.SerializeGameObjects(subObject, centerobject, Environment.CurrentDirectory + "/saves/" + StartGame.savePath + "/items/" + Math.Abs(centerobject.GetComponent<CenterObject>().id) + ".oeg", new FileSystem.Content(Vector3.zero, centerobject.GetComponent<CenterObject>().prerot), "Material");
                                    centerobject.AddComponent<SnapShot>();
                                    centerobject.GetComponent<SnapShot>().TakeSnapShot(centerobject, Environment.CurrentDirectory + "/saves/" + StartGame.savePath + "/items/" + Math.Abs(centerobject.GetComponent<CenterObject>().id) + ".oep", delegate
                                    {
                                        foreach (GameObject obj in subObject)
                                        {
                                            if (obj.tag == "Material")
                                            {
                                                if (obj.transform.parent != null)
                                                {
                                                    Destroy(obj.transform.parent.gameObject);
                                                }
                                                else
                                                {
                                                    Destroy(obj.gameObject);
                                                }
                                            }
                                        }

                                        Inventory.m.attemptAddItem(new Item(centerobject.GetComponent<CenterObject>().id, 0, null), 1);
                                    });
                                }
                                catch (Exception e)
                                {
                                    Debug.LogWarning(e);
                                }
                            }
                            else if (gameObj.GetComponent<ItemBase>() != null)
                            {
                                if (gameObj.GetComponent<FuelObject>() == null || !gameObj.GetComponent<FuelObject>().ignited)
                                {
                                    ItemBase ib = gameObj.GetComponent<ItemBase>();
                                    Inventory.m.attemptAddItem(ib.item, 1, ib.damage);
                                    Destroy(gameObj);
                                }
                            }
                        }
                    }
                }
            }
            if (InputManager.GetKeyUp("PickItem"))
            {
                ui.progression.transform.parent.gameObject.SetActive(false);
            }
            if (InputManager.GetKeyDown("GrabItem"))
            {
                distance = 2;
                Ray i_ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit i_hitInfo;
                if (Physics.Raycast(i_ray, out i_hitInfo, reachdis))
                {
                    grabobj = i_hitInfo.collider.gameObject;
                    if (grabobj.GetComponent<ItemBase>() == null) { grabobj = null; }
                    else
                    {
                        grabobj.layer = 15;
                        Rigidbody rigidbody = grabobj.GetComponent<Rigidbody>();
                        lastdrag = rigidbody.angularDrag;
                        rigidbody.angularDrag = 1;
                        if (rigidbody != null)
                        {
                            Vector3 tar = rfpc.transform.position + c.transform.rotation * new Vector3(0, 1, 2) - grabobj.transform.position;
                            rigidbody.velocity = tar * 5;
                        }
                    }
                }
            }
            if (InputManager.GetKey("GrabItem"))
            {
                try
                {
                    float axis = Input.GetAxis("Mouse ScrollWheel");
                    if (distance < 4 && axis > 0)
                        distance += axis;
                    if (distance > 0 && axis < 0)
                        distance += axis;
                    Rigidbody rigidbody = grabobj.GetComponent<Rigidbody>();
                    if (rigidbody != null)
                    {
                        Vector3 tar = rfpc.transform.position + c.transform.rotation * new Vector3(0, 1, distance) - grabobj.transform.position;
                        rigidbody.velocity = tar*5;
                    }
                }
                catch { }
            }
            else
            {
                if(grabobj != null)
                {
                    grabobj.GetComponent<Rigidbody>().angularDrag = lastdrag;
                    grabobj.layer = 0;
                    grabobj = null;
                }
            }
        }

        if (InputManager.GetKeyDown("DropItem"))
        {
            if (ui.mainhd.GetComponent<InterfaceHolder>().enabled)
            {
                ui.mainhd.GetComponent<InterfaceHolder>().DropItem();
            }
            if (ui.sechd.GetComponent<InterfaceHolder>().enabled)
            {
                ui.sechd.GetComponent<InterfaceHolder>().DropItem();
            }
            if (ui.mischd.GetComponent<InterfaceHolder>().enabled)
            {
                ui.mischd.GetComponent<InterfaceHolder>().DropItem();
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ui.sechd.GetComponent<InterfaceHolder>().Deactive();
            ui.sechd.GetComponent<InterfaceHolder>().enabled = false;
            ui.mischd.GetComponent<InterfaceHolder>().Deactive();
            ui.mischd.GetComponent<InterfaceHolder>().enabled = false;
            if (ui.mainhd.GetComponent<InterfaceHolder>().enabled)
            {
                ui.mainhd.GetComponent<InterfaceHolder>().Deactive();
                ui.mainhd.GetComponent<InterfaceHolder>().enabled = false;
            }
            else
            {
                ui.mainhd.GetComponent<InterfaceHolder>().VisuallizeItem();
                ui.mainhd.GetComponent<InterfaceHolder>().enabled = true;
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            ui.mainhd.GetComponent<InterfaceHolder>().Deactive();
            ui.mainhd.GetComponent<InterfaceHolder>().enabled = false;
            ui.mischd.GetComponent<InterfaceHolder>().Deactive();
            ui.mischd.GetComponent<InterfaceHolder>().enabled = false;
            if (ui.sechd.GetComponent<InterfaceHolder>().enabled)
            {
                ui.sechd.GetComponent<InterfaceHolder>().Deactive();
                ui.sechd.GetComponent<InterfaceHolder>().enabled = false;
            }
            else
            {
                ui.sechd.GetComponent<InterfaceHolder>().VisuallizeItem();
                ui.sechd.GetComponent<InterfaceHolder>().enabled = true;
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            ui.mainhd.GetComponent<InterfaceHolder>().Deactive();
            ui.mainhd.GetComponent<InterfaceHolder>().enabled = false;
            ui.sechd.GetComponent<InterfaceHolder>().Deactive();
            ui.sechd.GetComponent<InterfaceHolder>().enabled = false;
            if (ui.mischd.GetComponent<InterfaceHolder>().enabled)
            {
                ui.mischd.GetComponent<InterfaceHolder>().Deactive();
                ui.mischd.GetComponent<InterfaceHolder>().enabled = false;
            }
            else
            {
                ui.mischd.GetComponent<InterfaceHolder>().VisuallizeItem();
                ui.mischd.GetComponent<InterfaceHolder>().enabled = true;
            }
        }
    }
}
