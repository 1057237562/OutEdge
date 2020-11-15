using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityStandardAssets.Characters.FirstPerson;

public class ExportToWorld : MonoBehaviour
{
    public GameObject target;
    public GameObject cameraObj;

    public static GameObject targetObj;

    private void Start()
    {
        targetObj = target;
        SceneManager.SetActiveScene(SceneManager.GetSceneByName("MeshCreator"));

        try
        {
            targetObj.transform.position = MeshCreatorLoad.meshcreator.position + new Vector3(0, 2f, 0);
            
        }
        catch { }

    }

    public void Export()
    {
        DontDestroyOnLoad(targetObj);
        targetObj.GetComponent<MeshCollider>().convex = true;
        targetObj.AddComponent<Rigidbody>();
        //targetObj.transform.position = 
        SceneManager.UnloadSceneAsync("MeshCreator");
        SceneManager.SetActiveScene(SceneManager.GetSceneByName("GameScene"));

        RigidbodyFirstPersonController.rfpc.enabled = true;
        RigidbodyFirstPersonController.c.enabled = true;
        RigidbodyFirstPersonController.al.enabled = true;
        RigidbodyFirstPersonController.rfpc.goverlay.enabled = true;

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.UnloadSceneAsync("MeshCreator");
            SceneManager.SetActiveScene(SceneManager.GetSceneByName("GameScene"));
        }
    }
}
