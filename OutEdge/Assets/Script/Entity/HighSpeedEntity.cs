using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighSpeedEntity : MonoBehaviour
{
    Vector3 lastPos;
    Vector3 previousVelocity;

    public const float minSpeed = 5;

    // Start is called before the first frame update
    void Start()
    {
        lastPos = transform.position;
        previousVelocity = GetComponent<Rigidbody>().velocity;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(transform.position.y < -5f)
        {
            Destroy(gameObject);
        }
        if (GetComponent<Rigidbody>().velocity.magnitude > minSpeed)
        {
            //Debug.LogWarning("Detecting");
            Vector3 direction = transform.position - lastPos;
            try
            {
                Collider[] colliders = Physics.OverlapBox((transform.position + lastPos) / 2, new Vector3(direction.magnitude / 2, transform.localScale.y, transform.localScale.z), Quaternion.LookRotation(direction));

                /*Ray ray = new Ray(lastPos, direction.normalized);

                RaycastHit hitInfo;
                if (Physics.Raycast(ray, out hitInfo ,direction.magnitude))
                {*/
                float distance = Mathf.Infinity;
                Collider f_col = null;

                foreach (Collider col in colliders)
                {
                    if ((col.transform.position - lastPos).magnitude < distance)
                    {
                        distance = (col.transform.position - lastPos).magnitude;
                        f_col = col;
                    }
                }

                if (f_col != null)
                {
                    //Debug.DrawLine(ray.origin, hitInfo.point);

                    //Collider f_col = hitInfo.collider;
                    if (f_col.gameObject != gameObject)
                    {
                        Debug.LogWarning(f_col.gameObject.name);

                        LivingEntity entity = f_col.GetComponent<LivingEntity>();
                        if (entity != null)
                        {
                            float ratio = (lastPos - entity.transform.position).magnitude / (lastPos - transform.position).magnitude;

                            Vector3 dv = (1 - ratio) * GetComponent<Rigidbody>().velocity + ratio * previousVelocity + entity.GetComponent<Rigidbody>().velocity;

                            entity.ForceAppendCollision(dv.magnitude / Time.fixedDeltaTime / ratio, (1 - ratio) * transform.position + ratio * lastPos);
                        }

                        Destroy(gameObject);
                    }
                }
            }
            catch
            {

            }
        }

        lastPos = transform.position;
        previousVelocity = GetComponent<Rigidbody>().velocity;

    }
}
