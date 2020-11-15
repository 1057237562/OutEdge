using Dummiesman;
using SimpleFileBrowser;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenOBJ : MonoBehaviour
{
    public Shader shader;

    public void OpenOBJFile()
    {
        FileBrowser.SetFilters(true, new FileBrowser.Filter[] { new FileBrowser.Filter("OBJ File(.obj)", ".obj") });
        FileBrowser.SetDefaultFilter(".obj");
        FileBrowser.ShowLoadDialog(OnSuccess, OnCancel, false, null, "Open from:", "Open");
    }

    public void OnSuccess(string filename)
    {
        GameObject loadedObject = new OBJLoader(shader).Load(filename);
        loadedObject.transform.position = ExportToWorld.targetObj.transform.position;

        Destroy(ExportToWorld.targetObj);

        ExportToWorld.targetObj = loadedObject;
    }

    public void OnCancel()
    {

    }
}
