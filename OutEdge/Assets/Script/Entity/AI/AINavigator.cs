using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AINavigator : MonoBehaviour
{
    public AstarBase astar;

    // Start is called before the first frame update
    void Start()
    {
        node = transform.position;
    }

    public float speed = 5.0f;
    public Vector3 target;
    public GameObject TAR;
    public bool start = false;

    public bool i = false;
    public Vector3 node = Vector3.zero;

    // Update is called once per frame
    void FixedUpdate()
    {
        if (start && astar == null)
        {
            astar = new AstarBase(transform.position, target);
            
            astar.StartSearch();
            node = Vector3.zero;
            //start = false;
        }
        if(astar.path.Count == 0 && (astar.thread == null || !astar.thread.IsAlive) && (transform.position - TAR.transform.position).magnitude > 4f)
        {
            astar.ChangeTarget(transform.position,TAR.transform.position);
            node = Vector3.zero;
        }
        if (astar != null && astar.path.Count != 0 && !astar.locked)
        {
            if ((Mathf.Abs(transform.position.x - node.x) < 1.5f && Mathf.Abs(transform.position.y - node.y) < 2f && Mathf.Abs(transform.position.z - node.z) < 1.5f) || node == Vector3.zero)
            {
                node = astar.path.Pop();
            }
            transform.LookAt(new Vector3(node.x,transform.position.y,node.z));
            transform.position = Vector3.MoveTowards(transform.position, node + new Vector3(0,1f,0.0f), speed * Time.deltaTime);
        }
    }

    [ContextMenu("ForceStop")]
    public void ForceStop()
    {
        astar.thread.Abort();

        astar.StartSearch();
        node = Vector3.zero;
    }
}