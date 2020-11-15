using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static ItemManager;
using static Inventory;
using UnityEngine.EventSystems;

public class Crafting : MonoBehaviour
{
    [Serializable]
    public class MaterialCost
    {
        public List<ItemStack> material;
    }

    public GameObject desk;
    public static int sid = 0;
    public static int tid = 0;

    public GameObject preview;
    public Material material;
    public Material denymaterial;
    public Toggle autoc;
    public Toggle autoa;
    public float roolingspeed = 0.05f;
    public GameObject coordinate;
    public GameObject quadant;
    public GameObject scale;
    public float scaleLimit = 0.5f;
    //GameObject nowGroup;

    public static Dictionary<int, GameObject> dict;
    public static Dictionary<string, int> reflect;

    public List<MaterialCost> materialCosts = new List<MaterialCost>();

    GameObject selected;
    Material original;
    Vector2 last = new Vector2();

    public static Crafting m;

    public GameObject attributeSetting;

    public List<GameObject> acs = new List<GameObject>();

    public GameObject locklast;

    public float buildsize = 0.5f;
    float connectScale;
    public bool doubleConnect = false;
    public bool pickable = true;
    bool occupied = false;

    public static bool autoLock = false; // May cause Bug When true

    public bool global = false;
    public void SetGlobal(bool g)
    {
        global = g;
    }
    public GameObject front;
    public TMPro.TMP_InputField bs;

    public static Vector3 align(RaycastHit info, GameObject tar)
    {
        Vector3 direction = info.collider.transform.InverseTransformPoint(info.point);
        if (Math.Abs(direction.x) > Math.Abs(direction.y) && Math.Abs(direction.x) > Math.Abs(direction.z))
        {
            return new Vector3(direction.normalized.x, 0, 0).normalized;
        }
        if (Math.Abs(direction.y) > Math.Abs(direction.x) && Math.Abs(direction.y) > Math.Abs(direction.z))
        {
            return new Vector3(0, direction.normalized.y, 0).normalized;
        }
        if (Math.Abs(direction.z) > Math.Abs(direction.x) && Math.Abs(direction.z) > Math.Abs(direction.y))
        {
            return new Vector3(0, 0, direction.normalized.z).normalized;
        }
        return new Vector3(0, 0, 0);
    }

    public static Vector3 Cos(Vector3 a)
    {
        return new Vector3(Mathf.Cos(a.x), Mathf.Cos(a.y), Mathf.Cos(a.z));
    }

    public static Vector3 multiplyeach(Vector3 a, Vector3 b)
    {
        return new Vector3(a.x * b.x, a.y * b.y, a.z * b.z);
    }
    public static float dot(Vector3 a, Vector3 b)
    {
        return a.x * b.x + a.y * b.y + a.z * b.z;
    }
    public static Vector3 divideeach(Vector3 a, Vector3 b)
    {
        return new Vector3(a.x / b.x, a.y / b.y, a.z / b.z);
    }

    public static Vector3 denegative(Vector3 a)
    {
        return new Vector3(a.x > 0 ? a.x : 0, a.y > 0 ? a.y : 0, a.z > 0 ? a.z : 0);
    }

    public static Vector3 absolute(Vector3 a)
    {
        return new Vector3(Mathf.Abs(a.x), Mathf.Abs(a.y), Mathf.Abs(a.z));
    }

    public static Vector3 OnlyOne(Vector3 a)
    {
        return new Vector3(a.x == 0 ? 0 : a.x > 0 ? 1 : -1, a.y == 0 ? 0 : a.y > 0 ? 1 : -1, a.z == 0 ? 0 : a.z > 0 ? 1 : -1);
    }

    public static Vector3 PowEach(Vector3 a, Vector3 b)
    {
        return new Vector3(Mathf.Pow(a.x, b.x), Mathf.Pow(a.y, b.y), Mathf.Pow(a.z, b.z));
    }

    void Awake()
    {
        dict = new Dictionary<int, GameObject>();
        reflect = new Dictionary<string, int>();
    }

    // Start is called before the first frame update
    void Start()
    {
        m = this;
    }

    float scalation = 0.005f;

    public void SetBuildSize(string text)
    {
        buildsize = float.Parse(text);
    }

    public void UpdateModifierScale()
    {
        float scaler = -desk.GetComponent<Crafter>().tc.transform.localPosition.z * scalation;
        scaleLimit = desk.GetComponent<Crafter>().maxscale;
        connectScale = desk.GetComponent<Crafter>().connectScale;
        doubleConnect = desk.GetComponent<Crafter>().doubleConnect;
        pickable = desk.GetComponent<Crafter>().pickable;

        coordinate.transform.localScale = new Vector3(scaler, scaler, scaler);
        quadant.transform.localScale = quadant.transform.parent ? divideeach(new Vector3(scaler, scaler, scaler), quadant.transform.parent.localScale) : new Vector3(scaler, scaler, scaler);
        scale.transform.localScale = new Vector3(scaler * 1.2f, scaler * 1.2f, scaler * 1.2f); // Temp
    }

    public void ReleaseCurrent()
    {
        Destroy(preview);
        //block = false;
        coordinate.SetActive(false); // HiddenLayer
        coordinate.GetComponent<CoordinateModifier>().Disable();
        quadant.SetActive(false);
        quadant.GetComponent<CoordinateModifier>().Disable();
        scale.SetActive(false);
        scale.GetComponent<CoordinateModifier>().Disable();
        RemovingSelect rs = desk.GetComponent<Crafter>().tc.GetComponent<RemovingSelect>();
        rs.Disable();
        if (selected != null)
        {
            selected.GetComponent<Renderer>().sharedMaterial = original;
            selected = null;
            original = null;
        }
    }

    public void Update()
    {
        if(desk != null) {
            if (!Input.GetKey(KeyCode.LeftControl))
            {
                if (!EventSystem.current.IsPointerOverGameObject())
                {
                    desk.GetComponent<Crafter>().tc.transform.Translate(new Vector3(0, 0, Input.GetAxis("Mouse ScrollWheel") * roolingspeed), Space.Self);
                }
            }
            else
            {
                if (buildsize + Input.GetAxis("Mouse ScrollWheel") * roolingspeed > 0 && buildsize + Input.GetAxis("Mouse ScrollWheel") * roolingspeed <= scaleLimit)
                {
                    buildsize += Input.GetAxis("Mouse ScrollWheel") * roolingspeed;
                    bs.text = buildsize + "";
                }
            }

            UpdateModifierScale();

            if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Escape))
            {
                ReleaseCurrent();
                if (selected == null)
                {
                    sid = 0;
                }
                front.GetComponent<Front>().SetPreRotate();
                desk.GetComponent<GuiObject>().CanDestroy = true;
            }
            if (sid != 0)
            {
                desk.GetComponent<GuiObject>().CanDestroy = false;
            }
            if (Input.GetMouseButton(1))
            {
                if (last.x == 0 && last.y == 0)
                {
                    last = Input.mousePosition;
                }
                GameObject center = desk.GetComponent<Crafter>().ct;
                center.transform.Rotate(-Input.mousePosition.y + last.y, 0, 0, Space.Self);
                center.transform.Rotate(0, Input.mousePosition.x - last.x, 0, Space.World);
                last = Input.mousePosition;
            }
            else
            {
                last = new Vector2();
            }
            if (Input.GetMouseButtonDown(2))
            {
                Ray ray = desk.GetComponent<Crafter>().tc.ScreenPointToRay(Input.mousePosition);
                RaycastHit hitInfo;
                if (!EventSystem.current.IsPointerOverGameObject() && Physics.Raycast(ray, out hitInfo))
                {
                    GameObject gameObject = hitInfo.collider.gameObject;
                    if (gameObject.GetComponent<ConnectiveMaterial>())
                    {
                        desk.GetComponent<Crafter>().ct.transform.position = gameObject.transform.position;
                    }
                }
            }

            if (sid != -6)
            {
                RemovingSelect rs = desk.GetComponent<Crafter>().tc.GetComponent<RemovingSelect>();
                rs.Disable();
            }

            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = desk.GetComponent<Crafter>().tc.ScreenPointToRay(Input.mousePosition);
                RaycastHit hitInfo;
                if (Physics.Raycast(ray, out hitInfo, 5f, 1 << (LayerMask.NameToLayer("UILayer"))))
                {
                    return;
                }
            }
            if (sid > 0)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    if (!occupied)
                    {
                        Ray ray = desk.GetComponent<Crafter>().tc.ScreenPointToRay(Input.mousePosition);
                        RaycastHit hitInfo;
                        if (!EventSystem.current.IsPointerOverGameObject() && Physics.Raycast(ray, out hitInfo, Mathf.Infinity, 1 << 0 | 1 << 18))// Placing Block
                        {
                            if (hitInfo.collider.GetComponent<MarchingObject>()) return;
                            GameObject selectedObject; dict.TryGetValue(sid, out selectedObject);
                            if (!Inventory.m.removeItems(materialCosts[sid - 1].material))
                            {
                                MessageBox.ShowMessage("You don't have enough materials for that.");
                                return;
                            }

                            //Placing
                            GameObject newObj = Instantiate(selectedObject, (Input.GetKey(KeyCode.LeftShift) ? new Quaternion(0, 0, 0, 0) : desk.transform.rotation) * selectedObject.transform.lossyScale * buildsize / 2 + new Vector3(hitInfo.point.x, hitInfo.point.y, hitInfo.point.z), Input.GetKey(KeyCode.LeftShift) ? new Quaternion(0, 0, 0, 0) : desk.transform.rotation);

                            //Scaling
                            newObj.transform.localScale = new Vector3(newObj.transform.localScale.x * buildsize, newObj.transform.localScale.y * buildsize, newObj.transform.localScale.z * buildsize);
                            try
                            {
                                newObj.GetComponent<Rigidbody>().mass *= newObj.transform.localScale.x * newObj.transform.localScale.y * newObj.transform.localScale.z;
                            }
                            catch
                            {

                                GameObject inObj = newObj.transform.GetChild(0).gameObject;
                                inObj.transform.parent = null;
                                Destroy(newObj);
                                newObj = inObj;
                                newObj.GetComponent<Rigidbody>().mass *= newObj.transform.localScale.x * newObj.transform.localScale.y * newObj.transform.localScale.z;
                            }

                            SimpleWing sw = newObj.GetComponent<SimpleWing>();
                            if (sw != null)
                            {
                                sw.dimensions = new Vector2(newObj.transform.localScale.x, newObj.transform.localScale.z);
                                sw.liftMultiplier = newObj.transform.localScale.x * newObj.transform.localScale.y * newObj.transform.localScale.z;
                            }

                            if (!hitInfo.collider.gameObject.GetComponent<Crafter>() && (!hitInfo.collider.transform.parent || !hitInfo.collider.transform.parent.GetComponent<Crafter>()) && autoa.isOn) // Auto Align
                            {
                                newObj.transform.position = hitInfo.collider.transform.position + hitInfo.collider.transform.rotation * multiplyeach(align(hitInfo, newObj), new Vector3((newObj.transform.lossyScale.y + (hitInfo.collider.transform.parent ? hitInfo.collider.transform.parent.lossyScale.y : hitInfo.collider.transform.lossyScale.y)) / 2, (newObj.transform.lossyScale.y + hitInfo.collider.transform.lossyScale.y) / 2, (newObj.transform.lossyScale.y + (hitInfo.collider.transform.parent ? hitInfo.collider.transform.parent.lossyScale.y : hitInfo.collider.transform.lossyScale.y)) / 2));
                                newObj.transform.rotation = hitInfo.collider.transform.rotation * Quaternion.FromToRotation(Vector3.up, hitInfo.collider.transform.InverseTransformPoint(newObj.transform.position).normalized);
                            }

                            foreach (Transform child in newObj.transform)
                            {
                                child.gameObject.SetActive(true);//EnableChild

                                child.gameObject.layer = 0;

                                Collider collider = child.GetComponent<Collider>();
                                if (collider != null)
                                {
                                    collider.enabled = true;
                                }

                                foreach (MonoBehaviour component in child.GetComponents<MonoBehaviour>())
                                {
                                    component.enabled = true;
                                }

                                Rigidbody r = child.GetComponent<Rigidbody>();
                                if (r != null)
                                {
                                    r.useGravity = true;
                                    r.constraints = new RigidbodyConstraints();
                                    r.mass *= newObj.transform.localScale.x * newObj.transform.localScale.y * newObj.transform.localScale.z;
                                }
                            }

                            /*foreach (AttributeContainer component in newObj.GetComponents<AttributeContainer>())
                            {
                                component.enabled = true;
                            }*/
                            //Group Item
                            try
                            {
                                if (sid == 10)
                                {
                                    newObj.layer = 17;
                                }
                                else
                                {
                                    newObj.layer = 0;
                                }
                                newObj.GetComponent<Collider>().isTrigger = false;

                                Rigidbody r = newObj.GetComponent<Rigidbody>();
                                r.useGravity = true;

                                if (autoLock)
                                {
                                    if (locklast != null)
                                    {
                                        locklast.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
                                    }
                                    locklast = newObj;
                                    newObj.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
                                }
                                if((hitInfo.collider.gameObject == desk || hitInfo.collider.transform.parent == desk.transform) && !autoLock)
                                {
                                    if (locklast != null)
                                    {
                                        locklast.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
                                    }
                                    locklast = newObj;
                                    newObj.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
                                }

                                if (!hitInfo.collider.gameObject.GetComponent<Crafter>() && (!hitInfo.collider.transform.parent || !hitInfo.collider.transform.parent.GetComponent<Crafter>()) && autoc.isOn) // Auto Connect
                                {
                                    ConfigurableJoint joint = newObj.AddComponent<ConfigurableJoint>();

                                    joint.connectedMassScale = connectScale;
                                    //joint.enableCollision = true;

                                    joint.xMotion = ConfigurableJointMotion.Locked;
                                    joint.yMotion = ConfigurableJointMotion.Locked;
                                    joint.zMotion = ConfigurableJointMotion.Locked;
                                    joint.angularXMotion = ConfigurableJointMotion.Locked;
                                    joint.angularYMotion = ConfigurableJointMotion.Locked;
                                    joint.angularZMotion = ConfigurableJointMotion.Locked;

                                    SoftJointLimit sj = new SoftJointLimit();
                                    sj.contactDistance = float.PositiveInfinity;
                                    joint.linearLimit = sj;
                                    joint.angularYLimit = sj;
                                    joint.angularZLimit = sj;
                                    joint.highAngularXLimit = sj;
                                    joint.lowAngularXLimit = sj;

                                    joint.projectionMode = JointProjectionMode.PositionAndRotation;

                                    //joint.enablePreprocessing = false;

                                    ConnectiveMaterial m = newObj.GetComponent<ConnectiveMaterial>();
                                    joint.breakForce = m.breakForce;
                                    joint.breakTorque = m.breakTorque;
                                    joint.connectedBody = hitInfo.collider.gameObject.GetComponent<Rigidbody>();
                                    m.LinkTarget();

                                    if (doubleConnect)
                                    {
                                        joint = newObj.AddComponent<ConfigurableJoint>();

                                        joint.connectedMassScale = connectScale;
                                        //joint.enableCollision = true;

                                        joint.xMotion = ConfigurableJointMotion.Locked;
                                        joint.yMotion = ConfigurableJointMotion.Locked;
                                        joint.zMotion = ConfigurableJointMotion.Locked;
                                        joint.angularXMotion = ConfigurableJointMotion.Locked;
                                        joint.angularYMotion = ConfigurableJointMotion.Locked;
                                        joint.angularZMotion = ConfigurableJointMotion.Locked;

                                        sj.contactDistance = float.PositiveInfinity;
                                        joint.linearLimit = sj;
                                        joint.angularYLimit = sj;
                                        joint.angularZLimit = sj;
                                        joint.highAngularXLimit = sj;
                                        joint.lowAngularXLimit = sj;

                                        joint.projectionMode = JointProjectionMode.PositionAndRotation;

                                       //joint.enablePreprocessing = false;

                                        m = newObj.GetComponent<ConnectiveMaterial>();
                                        joint.breakForce = m.breakForce;
                                        joint.breakTorque = m.breakTorque;
                                        joint.connectedBody = hitInfo.collider.GetComponent<Rigidbody>();
                                        m.LinkTarget();
                                    }
                                }
                            }
                            catch
                            {
                                if (!hitInfo.collider.gameObject.GetComponent<Crafter>() && (!hitInfo.collider.transform.parent || !hitInfo.collider.transform.parent.GetComponent<Crafter>()) && autoc.isOn) // Auto Connect
                                {
                                    ConfigurableJoint joint = newObj.transform.GetChild(0).gameObject.AddComponent<ConfigurableJoint>();

                                    joint.connectedMassScale = connectScale;
                                    //joint.enableCollision = true;

                                    joint.xMotion = ConfigurableJointMotion.Locked;
                                    joint.yMotion = ConfigurableJointMotion.Locked;
                                    joint.zMotion = ConfigurableJointMotion.Locked;
                                    joint.angularXMotion = ConfigurableJointMotion.Locked;
                                    joint.angularYMotion = ConfigurableJointMotion.Locked;
                                    joint.angularZMotion = ConfigurableJointMotion.Locked;

                                    SoftJointLimit sj = new SoftJointLimit();
                                    sj.contactDistance = float.PositiveInfinity;
                                    joint.linearLimit = sj;
                                    joint.angularYLimit = sj;
                                    joint.angularZLimit = sj;
                                    joint.highAngularXLimit = sj;
                                    joint.lowAngularXLimit = sj;

                                    joint.projectionMode = JointProjectionMode.PositionAndRotation;

                                   // joint.enablePreprocessing = false;

                                    ConnectiveMaterial m = newObj.GetComponent<ConnectiveMaterial>();
                                    joint.breakForce = m.breakForce;
                                    joint.breakTorque = m.breakTorque;
                                    joint.connectedBody = hitInfo.collider.gameObject.GetComponent<Rigidbody>();
                                    m.LinkTarget();

                                    if (doubleConnect)
                                    {
                                        joint = newObj.transform.GetChild(0).gameObject.AddComponent<ConfigurableJoint>();

                                        joint.connectedMassScale = connectScale;
                                        //joint.enableCollision = true;

                                        joint.xMotion = ConfigurableJointMotion.Locked;
                                        joint.yMotion = ConfigurableJointMotion.Locked;
                                        joint.zMotion = ConfigurableJointMotion.Locked;
                                        joint.angularXMotion = ConfigurableJointMotion.Locked;
                                        joint.angularYMotion = ConfigurableJointMotion.Locked;
                                        joint.angularZMotion = ConfigurableJointMotion.Locked;

                                        joint.linearLimit = sj;
                                        joint.angularYLimit = sj;
                                        joint.angularZLimit = sj;
                                        joint.highAngularXLimit = sj;
                                        joint.lowAngularXLimit = sj;

                                        joint.projectionMode = JointProjectionMode.PositionAndRotation;

                                       // joint.enablePreprocessing = false;

                                        m = newObj.transform.GetChild(0).GetComponent<ConnectiveMaterial>();
                                        joint.breakForce = m.breakForce;
                                        joint.breakTorque = m.breakTorque;
                                        joint.connectedBody = hitInfo.collider.GetComponent<Rigidbody>();
                                        m.LinkTarget();
                                    }
                                }

                            }

                            if (hitInfo.collider.gameObject.GetComponent<Crafter>() || (hitInfo.collider.transform.parent && hitInfo.collider.transform.parent.GetComponent<Crafter>()))
                            { // Create Item Group
                                
                                newObj.AddComponent<CenterObject>();
                                newObj.GetComponent<CenterObject>().Regist();
                                newObj.GetComponent<CenterObject>().pickable = pickable;
                                newObj.GetComponent<CenterObject>().objlist.Add(newObj);
                                Inventory.allocateItem.Add(newObj.GetInstanceID());
                                newObj.GetComponent<CenterObject>().id = Inventory.GetCustomId(newObj);
                            }
                            else if (hitInfo.collider.GetComponent<CenterObject>() != null) // Reset Item Group // Doesn't detect the heady material
                            {
                                hitInfo.collider.GetComponent<CenterObject>().objlist.Add(newObj);
                                hitInfo.collider.GetComponent<ConnectiveMaterial>().sons.Add(newObj);
                                newObj.GetComponent<ConnectiveMaterial>().centerparent = hitInfo.collider.gameObject;
                                foreach (ConnectiveMaterial child in newObj.GetComponentsInChildren<ConnectiveMaterial>())
                                {
                                    child.centerparent = hitInfo.collider.gameObject;
                                }

                            }
                            else if (hitInfo.collider.transform.parent != null)
                            {
                                if (hitInfo.collider.transform.parent.GetComponent<CenterObject>() != null) // Reset Item Group // Doesn't detect the heady material
                                {
                                    hitInfo.collider.transform.parent.GetComponent<CenterObject>().objlist.Add(newObj);
                                    hitInfo.collider.transform.parent.GetComponent<ConnectiveMaterial>().sons.Add(newObj);
                                    newObj.GetComponent<ConnectiveMaterial>().centerparent = hitInfo.collider.transform.parent.gameObject;
                                    foreach (ConnectiveMaterial child in newObj.GetComponentsInChildren<ConnectiveMaterial>())
                                    {
                                        child.centerparent = hitInfo.collider.transform.parent.gameObject;
                                    }

                                }
                                else if (hitInfo.collider.transform.parent.GetComponent<ConnectiveMaterial>() != null)
                                {
                                    hitInfo.collider.transform.parent.GetComponent<ConnectiveMaterial>().centerparent.GetComponent<CenterObject>().objlist.Add(newObj);
                                    hitInfo.collider.transform.parent.GetComponent<ConnectiveMaterial>().sons.Add(newObj);
                                    newObj.GetComponent<ConnectiveMaterial>().centerparent = hitInfo.collider.transform.parent.GetComponent<ConnectiveMaterial>().centerparent;
                                    foreach (ConnectiveMaterial child in newObj.GetComponentsInChildren<ConnectiveMaterial>())
                                    {
                                        child.centerparent = hitInfo.collider.transform.parent.GetComponent<ConnectiveMaterial>().centerparent;
                                    }
                                }
                            }
                            else if (hitInfo.collider.GetComponent<ConnectiveMaterial>() != null)
                            {
                                hitInfo.collider.GetComponent<ConnectiveMaterial>().centerparent.GetComponent<CenterObject>().objlist.Add(newObj);
                                hitInfo.collider.GetComponent<ConnectiveMaterial>().sons.Add(newObj);
                                newObj.GetComponent<ConnectiveMaterial>().centerparent = hitInfo.collider.GetComponent<ConnectiveMaterial>().centerparent;
                                foreach (ConnectiveMaterial child in newObj.GetComponentsInChildren<ConnectiveMaterial>())
                                {
                                    child.centerparent = hitInfo.collider.GetComponent<ConnectiveMaterial>().centerparent;
                                }
                            }
                            front.GetComponent<Front>().LocateCenter(newObj);
                            newObj.GetComponent<ConnectiveMaterial>().RefreshJoint();
                        }
                    }
                    else
                    {
                        MessageBox.ShowMessage("Space have already been occupied.");
                    }
                }
            }
            else if (sid != 0)
            {
                Ray ray = desk.GetComponent<Crafter>().tc.ScreenPointToRay(Input.mousePosition);
                RaycastHit hitInfo;
                switch (sid)
                {
                    case -1:
                        if (Input.GetMouseButtonDown(0))
                            if (!EventSystem.current.IsPointerOverGameObject() && Physics.Raycast(ray, out hitInfo))
                            {
                                if (Input.GetMouseButtonDown(0))
                                {
                                    if (selected == null && selected.tag == "Material" && hitInfo.collider.gameObject.layer != 18 && hitInfo.collider.gameObject.layer != 5 && !hitInfo.collider.gameObject.GetComponent<Crafter>())
                                    {
                                        selected = hitInfo.collider.gameObject;
                                        front.GetComponent<Front>().LocateCenter(selected);
                                        original = selected.GetComponent<Renderer>().sharedMaterial;
                                        selected.GetComponent<Renderer>().sharedMaterial = material;
                                    }
                                    else
                                    {
                                        ConfigurableJoint joint = selected.AddComponent<ConfigurableJoint>();

                                        joint.connectedMassScale = connectScale;
                                        //joint.enableCollision = true;

                                        joint.xMotion = ConfigurableJointMotion.Locked;
                                        joint.yMotion = ConfigurableJointMotion.Locked;
                                        joint.zMotion = ConfigurableJointMotion.Locked;
                                        joint.angularXMotion = ConfigurableJointMotion.Locked;
                                        joint.angularYMotion = ConfigurableJointMotion.Locked;
                                        joint.angularZMotion = ConfigurableJointMotion.Locked;

                                        SoftJointLimit sj = new SoftJointLimit();
                                        sj.contactDistance = float.PositiveInfinity;
                                        joint.linearLimit = sj;
                                        joint.angularYLimit = sj;
                                        joint.angularZLimit = sj;
                                        joint.highAngularXLimit = sj;
                                        joint.lowAngularXLimit = sj;

                                        joint.projectionMode = JointProjectionMode.PositionAndRotation;

                                       // joint.enablePreprocessing = false;

                                        ConnectiveMaterial m = selected.GetComponent<ConnectiveMaterial>();
                                        joint.breakForce = m.breakForce;
                                        joint.breakTorque = m.breakTorque;
                                        joint.connectedBody = hitInfo.collider.gameObject.GetComponent<Rigidbody>();
                                        m.LinkTarget();

                                        joint = selected.AddComponent<ConfigurableJoint>();

                                        joint.connectedMassScale = connectScale;
                                        //joint.enableCollision = true;

                                        joint.xMotion = ConfigurableJointMotion.Locked;
                                        joint.yMotion = ConfigurableJointMotion.Locked;
                                        joint.zMotion = ConfigurableJointMotion.Locked;
                                        joint.angularXMotion = ConfigurableJointMotion.Locked;
                                        joint.angularYMotion = ConfigurableJointMotion.Locked;
                                        joint.angularZMotion = ConfigurableJointMotion.Locked;

                                        joint.linearLimit = sj;
                                        joint.angularYLimit = sj;
                                        joint.angularZLimit = sj;
                                        joint.highAngularXLimit = sj;
                                        joint.lowAngularXLimit = sj;

                                        joint.projectionMode = JointProjectionMode.PositionAndRotation;

                                        //joint.enablePreprocessing = false;

                                        m = selected.GetComponent<ConnectiveMaterial>();
                                        joint.breakForce = m.breakForce;
                                        joint.breakTorque = m.breakTorque;
                                        joint.connectedBody = hitInfo.collider.gameObject.GetComponent<Rigidbody>();
                                        m.LinkTarget();

                                        selected.GetComponent<Renderer>().sharedMaterial = original;
                                        selected = null;
                                        original = null;
                                    }
                                }
                                else if (selected != null)
                                {
                                    selected.transform.position = new Vector3(hitInfo.point.x + selected.transform.lossyScale.x, hitInfo.point.y + selected.transform.lossyScale.y, hitInfo.point.z + selected.transform.lossyScale.z);
                                }
                            }
                        break;
                    case -2:
                        if (Input.GetMouseButtonDown(0))
                        {
                            if (!EventSystem.current.IsPointerOverGameObject() && Physics.Raycast(ray, out hitInfo))
                            {
                                GameObject hit = hitInfo.collider.gameObject;
                                if (hit.tag == "Material" && hit.layer != 18 && hit.layer != 5 && !hit.GetComponent<Crafter>())
                                {
                                    if (selected != null)
                                    {
                                        ReleaseCurrent();
                                    }
                                    selected = hit.transform.parent ? hit.transform.parent.gameObject : hit;
                                    front.GetComponent<Front>().LocateCenter(selected);

                                    LinkNewObject(selected);

                                    coordinate.transform.position = selected.transform.position;
                                    if (!global)
                                    {
                                        coordinate.transform.rotation = selected.transform.rotation;
                                    }
                                    coordinate.GetComponent<CoordinateModifier>().selected = hit;
                                    coordinate.GetComponent<CoordinateModifier>().SetDesk(desk);
                                    coordinate.GetComponent<CoordinateModifier>().Enable();
                                    coordinate.SetActive(true);

                                    original = selected.GetComponent<Renderer>().sharedMaterial;
                                    selected.GetComponent<Renderer>().sharedMaterial = material;
                                }
                            }
                        }
                            
                        break;
                    case -3:
                        if (Input.GetMouseButtonDown(0))
                        {
                            if (!EventSystem.current.IsPointerOverGameObject() && Physics.Raycast(ray, out hitInfo))
                            {
                                GameObject hit = hitInfo.collider.gameObject;
                                if (hit.tag == "Material" && hit.layer != 18 && !hit.GetComponent<Crafter>())
                                {
                                    if (selected != null)
                                    {
                                        ReleaseCurrent();
                                    }
                                    selected = hit.transform.parent ? hit.transform.parent.gameObject : hit;
                                    front.GetComponent<Front>().LocateCenter(selected);

                                    LinkNewObject(selected);

                                    quadant.transform.position = selected.transform.position;
                                    quadant.GetComponent<CoordinateModifier>().selected = hit;
                                    quadant.GetComponent<CoordinateModifier>().SetDesk(desk);
                                    quadant.GetComponent<CoordinateModifier>().Enable();
                                    if (!global)
                                    {
                                        quadant.transform.parent = hit.transform;
                                        quadant.transform.localRotation = Quaternion.identity;
                                    }
                                    quadant.SetActive(true);

                                    original = selected.GetComponent<Renderer>().sharedMaterial;
                                    selected.GetComponent<Renderer>().sharedMaterial = material;
                                }
                            }
                        }
                        break;
                    case -4:
                        if (Input.GetMouseButtonDown(0))
                        {
                            if (!EventSystem.current.IsPointerOverGameObject() && Physics.Raycast(ray, out hitInfo))
                            {
                                GameObject hit = hitInfo.collider.gameObject;
                                if (hit.tag == "Material" && hit.layer != 18 && hit.layer != 5 && !hit.GetComponent<Crafter>())
                                {
                                    if (selected != null)
                                    {
                                        ReleaseCurrent();
                                    }
                                    selected = hit.transform.parent ? hit.transform.parent.gameObject : hit;
                                    front.GetComponent<Front>().LocateCenter(selected);

                                    LinkNewObject(selected);

                                    scale.transform.position = selected.transform.position;
                                    scale.transform.rotation = selected.transform.rotation;
                                    scale.GetComponent<CoordinateModifier>().selected = hit;
                                    scale.GetComponent<CoordinateModifier>().SetDesk(desk);
                                    scale.GetComponent<CoordinateModifier>().Enable();
                                    scale.SetActive(true);

                                    original = selected.GetComponent<Renderer>().sharedMaterial;
                                    selected.GetComponent<Renderer>().sharedMaterial = material;
                                }
                            }
                        }   
                        break;
                    case -5:
                        if (Input.GetMouseButtonDown(0))
                            if (selected == null)
                            {
                                if (!EventSystem.current.IsPointerOverGameObject() && Physics.Raycast(ray, out hitInfo))
                                {
                                    GameObject hit = hitInfo.collider.gameObject;
                                    if (hit.layer != 18 && hit.layer != 5 && !hit.GetComponent<Crafter>())
                                    {
                                        try
                                        {
                                            (hit.GetComponent<CenterObject>() != null ? hit.GetComponent<CenterObject>() : hit.GetComponent<ConnectiveMaterial>().centerparent.GetComponent<CenterObject>()).objlist.Remove(hit);
                                            List<GameObject> rep = hit.GetComponent<ConnectiveMaterial>().sons;
                                            foreach (GameObject re in rep)
                                            {
                                                foreach(Joint joint in re.GetComponents<Joint>())
                                                {
                                                    if (!joint.connectedBody)
                                                    {
                                                        Destroy(joint);
                                                    }
                                                }
                                                CenterObject.CreateCenter(re, pickable);
                                            }
                                        }
                                        catch (Exception e) { Debug.LogError(e); }
                                        if (hit.tag == "Material")
                                        {
                                            Destroy(hit.transform.parent ? hit.transform.parent.gameObject : hit);
                                        }
                                        if(hit == front.GetComponent<Front>().center)
                                        {
                                            front.SetActive(false);
                                        }
                                    }
                                }
                            }
                        break;
                    case -6:
                        if (Input.GetMouseButtonDown(0))
                            if (selected != null)
                            {
                                Destroy(selected);
                                selected = null;
                                coordinate.SetActive(false); // HiddenLayer
                                coordinate.GetComponent<CoordinateModifier>().Disable();
                                quadant.SetActive(false);
                                quadant.GetComponent<CoordinateModifier>().Disable();
                                scale.SetActive(false);
                                scale.GetComponent<CoordinateModifier>().Disable();
                            }
                        if (Input.GetMouseButtonUp(0) && tid == -6)
                        {
                            RemovingSelect rs = desk.GetComponent<Crafter>().tc.GetComponent<RemovingSelect>();
                            if (rs.enabled)
                            {
                                if (rs.lastselected.Count > 0)
                                {
                                    List<GameObject> rep = new List<GameObject>();
                                    foreach (GameObject @obj in rs.lastselected.Keys)
                                    {
                                        if (obj.GetComponent<ConnectiveMaterial>() != null)
                                        {
                                            rep.AddRange(obj.GetComponent<ConnectiveMaterial>().sons);
                                        }
                                        try
                                        {
                                            (obj.GetComponent<CenterObject>() != null ? obj.GetComponent<CenterObject>() : obj.GetComponent<ConnectiveMaterial>().centerparent.GetComponent<CenterObject>()).objlist.Remove(obj);
                                            if (obj.transform.parent != null)
                                            {
                                                Destroy(obj.transform.parent.gameObject);
                                            }
                                            else
                                            {
                                                Destroy(obj);
                                            }
                                        }
                                        catch
                                        {

                                        }
                                    }

                                    foreach (GameObject re in rep)
                                    {
                                        if (re != null)
                                        {
                                            CenterObject.CreateCenter(re, pickable);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                rs.enabled = true;
                            }
                            break;

                        }
                        break;
                    case -7:
                        if (Input.GetMouseButtonDown(0))
                            if (selected == null)
                            {
                                if (!EventSystem.current.IsPointerOverGameObject() && Physics.Raycast(ray, out hitInfo))
                                {
                                    GameObject hit = hitInfo.collider.gameObject;
                                    if (hit.tag == "Material" && locklast != null)
                                    {
                                        locklast.GetComponent<Rigidbody>().constraints = new RigidbodyConstraints();
                                    }
                                    AttributeContainer attributeContainer = hit.GetComponent<AttributeContainer>();
                                    if (attributeContainer != null && attributeContainer.CanOpen())
                                    {
                                        GameObject ac = Instantiate(attributeSetting, new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0), Quaternion.identity, transform);
                                        ac.SetActive(true);

                                        ac.GetComponent<AttributeSettings>().SetTarget(hit);
                                        acs.Add(ac);

                                        //sid = 0;
                                        tid = 0;
                                    }
                                }
                            }
                        break;
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                tid = 0;
            }
        }
    }

    void FixedUpdate()
    {
        if (desk != null)
        {
            if (sid > 0)
            {
                //occupied = true;
                try
                {
                    occupied = preview.GetComponent<Trigger>().occpied;
                    Destroy(preview);
                }
                catch { }
                Ray ray = desk.GetComponent<Crafter>().tc.ScreenPointToRay(Input.mousePosition);
                RaycastHit hitInfo;
                if (!EventSystem.current.IsPointerOverGameObject() && Physics.Raycast(ray, out hitInfo, Mathf.Infinity, 1 << 0 | 1 << 18))// Placing Block
                {
                    GameObject selectedObject; dict.TryGetValue(sid, out selectedObject);
                    if (hitInfo.collider.gameObject.tag == "Material")
                    {
                        if (!Input.GetMouseButtonDown(0))
                        {
                            preview = Instantiate(selectedObject, (Input.GetKey(KeyCode.LeftShift) ? new Quaternion(0, 0, 0, 0) : desk.transform.rotation)*selectedObject.transform.lossyScale*buildsize/2+new Vector3(hitInfo.point.x, hitInfo.point.y, hitInfo.point.z), Input.GetKey(KeyCode.LeftShift) ? new Quaternion(0, 0, 0, 0) : desk.transform.rotation);
                            preview.AddComponent<Trigger>();
                            //Draw Rendering
                            if (!occupied)
                            {
                                try
                                {
                                    Material[] mat = preview.GetComponent<Renderer>().materials;
                                    for (int i = 0; i < preview.GetComponent<Renderer>().materials.Length; i++)
                                    {
                                        mat[i] = material;
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

                            preview.transform.localScale = new Vector3(preview.transform.localScale.x * buildsize, preview.transform.localScale.y * buildsize, preview.transform.localScale.z * buildsize);
                            if (!hitInfo.collider.gameObject.GetComponent<Crafter>() && (!hitInfo.collider.transform.parent || !hitInfo.collider.transform.parent.GetComponent<Crafter>()) && autoa.isOn)
                            {
                                preview.transform.position = hitInfo.collider.transform.position + hitInfo.collider.transform.rotation * multiplyeach(align(hitInfo, preview), new Vector3((preview.transform.lossyScale.y + (hitInfo.collider.transform.parent ? hitInfo.collider.transform.parent.lossyScale.y: hitInfo.collider.transform.lossyScale.y)) / 2, (preview.transform.lossyScale.y + hitInfo.collider.transform.lossyScale.y) / 2, (preview.transform.lossyScale.y + (hitInfo.collider.transform.parent ? hitInfo.collider.transform.parent.lossyScale.y : hitInfo.collider.transform.lossyScale.y)) / 2));
                                preview.transform.rotation = hitInfo.collider.transform.rotation * Quaternion.FromToRotation(Vector3.up, hitInfo.collider.transform.InverseTransformPoint(preview.transform.position).normalized);
                            }
                        }
                    }
                }
            }
        }
    }

    public void LinkNewObject(GameObject obj)
    {
        if (obj.GetComponent<Joint>() != null && autoLock)
        {
            if (locklast != null)
            {
                locklast.GetComponent<Rigidbody>().constraints = new RigidbodyConstraints();
            }

            locklast = obj.GetComponent<Joint>().connectedBody.gameObject;
            locklast.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        }
    }
}
