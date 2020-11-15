using SimpleFileBrowser;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExportOBJButton : MonoBehaviour
{

    public void SaveToLocal()
    {
        FileBrowser.SetFilters(true, new FileBrowser.Filter[] { new FileBrowser.Filter("OBJ File(.obj)", ".obj") });
        FileBrowser.SetDefaultFilter(".obj");
        FileBrowser.ShowSaveDialog(OnSuccess, OnCancel, false, null, "Save To:", "Save");
    }

    public void OnSuccess(string path)
    {
        ObjExporter.MeshToFile(ExportToWorld.targetObj.GetComponent<MeshFilter>(), path.Substring(0, path.LastIndexOf("\\")), path.Substring(path.LastIndexOf("\\") + 1, path.LastIndexOf(".") - path.LastIndexOf("\\") - 1));
    }
    public void OnCancel()
    {

    }
}
