using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class CameraShut : MonoBehaviour
{

    private void OnPostRender()
    {
        gameObject.SetActive(false);
    }

    [ContextMenu("active")]
    // Start is called before the first frame update
    void Active()
    {
        SaveRenderToPng(GetComponent<Camera>().targetTexture, "D:/Unity-project/OutEdge/Assets/PreRender", gameObject.name);
    }
    static public Texture2D SaveRenderToPng(RenderTexture renderT, string folderName, string name)
    {
        int width = renderT.width;
        int height = renderT.height;
        Texture2D tex2d = new Texture2D(width, height, TextureFormat.ARGB32, false);
        RenderTexture.active = renderT;
        tex2d.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        tex2d.Apply();

        byte[] b = tex2d.EncodeToPNG();
        string sysPath = folderName;
        if (!Directory.Exists(sysPath))
            Directory.CreateDirectory(sysPath);
        FileStream file = File.Open(sysPath + "/" + name + ".png", FileMode.Create);
        BinaryWriter writer = new BinaryWriter(file);
        writer.Write(b);
        file.Close();

        return tex2d;
    }

}
