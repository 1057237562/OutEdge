using SimpleFileBrowser;
using LibMMD.Unity3D;
using System.Collections.Generic;
using UniHumanoid;
using UnityEngine;

public class MMDLoader : MonoBehaviour
{
    public string ModelPath;

    public string MotionPath;

    public RuntimeAnimatorController ctor;

    public GameObject mmdObj;

    public GameObject prefab;

    private void Start()
    {
        if (!string.IsNullOrEmpty(ModelPath))
        {
            Load();
        }
    }

    void Load()
    {
        if(mmdObj != null)
        {
            Destroy(mmdObj);
        }
        if (string.IsNullOrEmpty(ModelPath))
        {
            Debug.LogError("please fill your model file path");
        }
        mmdObj = MmdGameObject.CreateGameObject("MmdGameObject");
        var mmdGameObject = mmdObj.GetComponent<MmdGameObject>();

        mmdGameObject.LoadModel(ModelPath);
        if (!string.IsNullOrEmpty(MotionPath))
        {
            mmdGameObject.LoadMotion(MotionPath);
        }

        mmdObj.transform.rotation = Quaternion.Euler(new Vector3(0, 180, 0));
        mmdObj.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        mmdObj.transform.localPosition = new Vector3(0, -0.01f * mmdObj.GetComponent<SkinnedMeshRenderer>().bounds.extents.y, 0f);

        BoneMapping bm = mmdObj.AddComponent<BoneMapping>();
        bm.Bones[0] = mmdObj;

        bm.GuessBoneMapping();
        bm.EnsureTPose();

        Animator an = mmdObj.AddComponent<Animator>();
        an.avatar = bm.GetAvatar();
        an.runtimeAnimatorController = ctor;
        mmdGameObject.UpdateConfig(new MmdUnityConfig
        {
            EnableDrawSelfShadow = MmdConfigSwitch.ForceFalse,
            EnableCastShadow = MmdConfigSwitch.ForceFalse
        });

        mmdObj.transform.parent = transform.parent;
        mmdObj.transform.localPosition = new Vector3(0, 0, 0);
        mmdObj.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
        mmdObj.layer = 13;

        Accelerate(mmdObj, mmdObj.transform);

        GameControll gc = GetComponent<GameControll>();
        gc.animator = mmdObj;
        //transform.parent.GetComponent<RigidbodyFirstPersonController>().enabled = true;
        transform.parent.GetComponent<CapsuleCollider>().height = mmdObj.GetComponent<SkinnedMeshRenderer>().bounds.extents.y * 2;
    }

    List<DynamicBoneColliderBase> legc = new List<DynamicBoneColliderBase>();
    GameObject gameskirt;

    void Accelerate(GameObject mmdObj, Transform transform)
    {
        foreach (Transform child in transform)
        {
            if (child.name == "右目")
            {
                GetComponent<GameControll>().rightcamera = child.gameObject;
            }
            if (child.name == "左目")
            {
                GetComponent<GameControll>().leftcamera = child.gameObject;
            }
            if(child.name== "首")
            {
                GetComponent<GameControll>().neck = child.gameObject;
            }
            if (child.name.Contains("髪") || child.name.Contains("ツインテ"))
            {
                DynamicBone db = mmdObj.AddComponent<DynamicBone>();
                db.m_Root = child;
                db.m_Inert = 0.65f;
                db.m_Damping = 0.2f;

                continue;
            }
            if (child.name.Contains("スカート") && child.gameObject != gameskirt)
            {
                /*if (gameskirt == null)
                {
                    gameskirt = new GameObject("スカートGROUP");
                    gameskirt.transform.parent = child.parent;
                    gameskirt.transform.localPosition = new Vector3(0, 0, 0);
                    DynamicBone db = mmdObj.AddComponent<DynamicBone>();
                    db.m_Root = gameskirt.transform;
                    db.m_Damping = 0.3f;
                    db.m_Inert = 0.5f;

                    db.m_Radius = 0.5f;
                    db.m_Colliders = legc;
                }
                child.parent = gameskirt.transform;*/
                DynamicBone db = mmdObj.AddComponent<DynamicBone>();
                db.m_Root = child;
                db.m_Damping = 0.3f;
                db.m_Inert = 0.5f;

                db.m_Radius = 0.5f;
                db.m_Colliders = legc;
                continue;
            }
            if (child.name.Contains("襟"))
            {
                DynamicBone db = mmdObj.AddComponent<DynamicBone>();
                db.m_Root = child;
                db.m_Damping = 0.3f;

                db.m_Radius = 0.1f;
                db.m_Gravity = new Vector3(0, -9.8f, 0);
                continue;
            }
            if (child.name.Contains("帯"))
            {
                DynamicBone db = mmdObj.AddComponent<DynamicBone>();
                db.m_Root = child;
                db.m_Damping = 0.3f;

                db.m_Radius = 0.1f;
                continue;
            }
            if (child.name.Contains("ﾏﾝﾄ"))
            {
                DynamicBone db = mmdObj.AddComponent<DynamicBone>();
                db.m_Root = child;
                db.m_Damping = 0.2f;
                db.m_Elasticity = 0.05f;
                db.m_Inert = 0.3f;

                db.m_Radius = 0.1f;
                continue;
            }
            if (child.name.Contains("腕"))
            {
                DynamicBoneCollider db = child.gameObject.AddComponent<DynamicBoneCollider>();
                db.m_Radius = 0.75f;
            }

            if (child.name.Contains("足") || child.name.Contains("ひじ"))
            {
                DynamicBoneCollider db = child.gameObject.AddComponent<DynamicBoneCollider>();
                db.m_Radius = 0.75f;
                db.m_Height = 0.2f;
                legc.Add(db);
            }
            if (child.name.Contains("ひざ"))
            {
                DynamicBoneCollider db = child.gameObject.AddComponent<DynamicBoneCollider>();
                db.m_Radius = 1.6f;
                db.m_Center = new Vector3(0, 0.5f, 1);
                legc.Add(db);
            }
            Accelerate(mmdObj, child);
        }
    }

    public void ChangeModel()
    {
        FileBrowser.SetFilters(true, new FileBrowser.Filter[] { new FileBrowser.Filter("MMD File(.pmx)", ".pmx")});
        FileBrowser.SetDefaultFilter(".pmx");
        FileBrowser.ShowLoadDialog(OnSuccess, OnCancel, false, null, "Load", "Select");
    }

    public void OnSuccess(string path)
    {
        string filepath = path;//选择的文件路径;  
        ModelPath = filepath;
        Load();
    }
    public void OnCancel()
    {

    }
}
