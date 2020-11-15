using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemovingSelect : MonoBehaviour
{
    public Color rectColor = Color.green;

    private Vector2 start = Vector2.zero;
    public bool drawRectangle = false;
    public Shader shader;
    public Texture mask;
    public GameObject ct;
    Collider[] objs;

    public Dictionary<GameObject,Material> selected = new Dictionary<GameObject, Material> ();
    public Dictionary<GameObject, Material> lastselected = new Dictionary<GameObject, Material>();

    public float modifyDistance = 15.0f;

    // Use this for initialization
    void Start()
    {
    }


    public void Disable()
    {
        foreach (GameObject @keys in selected.Keys)
        {
            Material @o_mat; selected.TryGetValue(keys, out o_mat);
            keys.GetComponent<Renderer>().material = o_mat;
        }
        drawRectangle = false;
        lastselected = new Dictionary<GameObject, Material>();
        enabled = false;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            drawRectangle = true;
            start = Input.mousePosition;
            objs = Physics.OverlapBox(transform.position,new Vector3(modifyDistance,modifyDistance,modifyDistance),Quaternion.identity,1<<0 | 1<<17,QueryTriggerInteraction.UseGlobal);
            if (selected != null)
            {
                if (!Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.RightShift))
                {
                    foreach (GameObject @keys in selected.Keys)
                    {
                        Material @o_mat; selected.TryGetValue(keys, out o_mat);
                        keys.GetComponent<Renderer>().material = o_mat;
                    }
                    selected = new Dictionary<GameObject, Material>();
                }
            }
            
        }
        else if (Input.GetMouseButtonUp(0))
        {
            drawRectangle = false;
            lastselected = selected;
        }
    }

    void OnGUI()
    {
        if (drawRectangle)
        {
            Vector2 size = new Vector2(Input.mousePosition.x, Input.mousePosition.y) - start;
            GUI.DrawTexture(new Rect(start.x, Screen.height - start.y,size.x, -size.y),mask);
            checkSelection(start,Input.mousePosition);
        }
    }

    void checkSelection(Vector3 start, Vector3 end)
    {
        Camera @camera = GetComponent<Camera>();

        //Collider[] objs = Physics.OverlapBox((endpos + startpos) / 2, new Vector3(-size.x/2,size.y/2,(camera.farClipPlane - camera.nearClipPlane)/2), transform.rotation, 1 << (LayerMask.NameToLayer("Default")), QueryTriggerInteraction.UseGlobal);
        foreach(Collider collider in objs)
        {
            try
            {
                GameObject @obj = collider.gameObject;
                if (obj.tag == "Material" && !obj.GetComponent<Lift>())
                {
                    Vector3 @m_pos = camera.WorldToScreenPoint(obj.transform.position);
                    if (Mathf.Min(start.x, end.x) <= m_pos.x && Mathf.Max(start.x, end.x) >= m_pos.x && Mathf.Max(start.y, end.y) >= m_pos.y && Mathf.Min(start.y, end.y) <= m_pos.y)
                    {

                        //if (obj.GetComponent<Rigidbody>() != null)
                        //{
                        Material org_mat = obj.GetComponent<Renderer>().material;
                        Material mat = new Material(shader);
                        mat.mainTexture = org_mat.mainTexture;
                        mat.color = org_mat.color;
                        obj.GetComponent<Renderer>().material = mat;
                        if (!selected.ContainsKey(obj))
                        { 
                            selected.Add(obj, org_mat);
                        }
                        /*}
                        else
                        {
                            foreach (Transform child in obj.transform)
                            {
                                Material org_matc = child.GetComponent<Renderer>().material;
                                Material matc = new Material(shader);
                                matc.mainTexture = org_matc.mainTexture;
                                matc.color = org_matc.color;
                                child.GetComponent<Renderer>().material = matc;
                                if (!selected.ContainsKey(child.gameObject))
                                {
                                    selected.Add(child.gameObject, org_matc);
                                }
                            }
                        }*/

                    }
                    else if (!Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.RightShift))
                    {

                        if (selected.ContainsKey(obj))
                        {
                            Material @o_mat; selected.TryGetValue(obj, out o_mat);
                            obj.GetComponent<Renderer>().material = o_mat;
                            selected.Remove(obj);
                        }
                    }

                }
            }
            catch { }
            
            
        }
    }
}
