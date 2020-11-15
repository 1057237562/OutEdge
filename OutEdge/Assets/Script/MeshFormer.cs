using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshFormer : MonoBehaviour
{
    struct content
    {
        public Vector3 vector;
        public int id;

        public content(Vector3 vector3, int i) : this()
        {
            vector = vector3;
            id = i;
        }
    };
    
    public float strength = 0.1f;
    public bool safemode = true;
    public bool on = false;
    Dictionary<GameObject,Vector3[]> map = new Dictionary<GameObject, Vector3[]>();
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (on)
        {
            if (Input.GetMouseButtonDown(0))
            {
                modify(-1);
            }
            if (Input.GetMouseButtonDown(1))
            {
                modify(1);
            }
        }
    }

    public void modify(int multiplier)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo))
        {
            GameObject gameObj = hitInfo.collider.gameObject;
            Vector3 hitPoint = new Vector3(hitInfo.point.x - gameObj.transform.position.x, hitInfo.point.y - gameObj.transform.position.y, hitInfo.point.z - gameObj.transform.position.z);
            MeshFilter filter = gameObj.GetComponent<MeshFilter>();
            //Debug.Log(hitPoint.x+":"+hitPoint.y+":"+hitPoint.z+":"+ gameObj.name);
            if (safemode)
            {
                if (!map.ContainsKey(gameObj))
                {
                    map.Add(gameObj, filter.mesh.vertices);
                }
            }

            List<content> v3 = new List<content>();
            Vector3[] vertices = filter.mesh.vertices;
            Vector3[] ov;
            if (safemode)
            {
                map.TryGetValue(gameObj, out ov);
            }
            else
            {
                ov = filter.mesh.vertices;
            }
            
            for (int i = 0; i < ov.Length; i++)
            {
                if (v3.Count < 3)
                {
                    v3.Add(new content(ov[i], i));
                }
                else
                {
                    for (int j = 0; j < v3.Count; j++)
                    {
                        if ((ov[i] - hitPoint).magnitude < (v3[j].vector - hitPoint).magnitude)
                        {
                            v3[j] = new content(ov[i], i);
                        }
                    }
                }
            }

            Vector3 modifier = (transform.position - hitInfo.point).normalized * strength;
            //Debug.Log(modifier.x + ":" + modifier.y + ":" + modifier.z);
            for (int i = 0; i < v3.Count; i++)
            {
                vertices[v3[i].id] += multiplier * modifier;
            }
            filter.mesh.vertices = vertices;
            try
            {
                gameObj.GetComponent<MeshCollider>().sharedMesh = filter.mesh;
            }
            catch
            {

            }
            
        }
    }
}
