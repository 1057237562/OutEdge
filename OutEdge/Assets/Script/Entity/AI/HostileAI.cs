using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LibMMD;

public class HostileAI : AINavigator
{
    public float TrackDistance = 20f;
    public float AttackRange = 2f;
    public float LocateRange = 7.5f;
    public int AttackDuration = 40;
    int Freeze = 0;

    public float Force = 50f;

    public float saturation = 100f;
    public float maxSaturation = 500f;

    // Update is called once per frame
    void FixedUpdate()
    {
        saturation-= 0.03f;
        if(saturation > 90)
        {
            LivingEntity eb = GetComponent<LivingEntity>();
            if (eb.nowHealth + 0.25f <= eb.maxHealth)
            {
                eb.nowHealth += 0.25f;
            }
            else
            {
                eb.nowHealth = eb.maxHealth;
            }
        }
        if (saturation < 0)
        {
            LivingEntity eb = GetComponent<LivingEntity>();
            eb.nowHealth -= 1f;
            if(eb.nowHealth < 0)
            {
                eb.Death();
            }
        }
        if (saturation < 50)
        {
            if (Freeze < AttackDuration)
            {
                Freeze++;
            }

            if (start && astar == null)
            {
                Collider[] colliders = Physics.OverlapSphere(transform.position, TrackDistance);
                foreach (Collider co in colliders)
                {
                    if (co.GetComponent<PlayerEntity>() != null || co.GetComponent<AnimalAI>() != null)
                    {
                        astar = new AstarBase(transform.position, co.transform.position);

                        TAR = co.gameObject;

                        astar.StartSearch();
                        node = Vector3.zero;

                        GetComponent<Wandering>().enabled = false;
                        //start = false;
                        return;
                    }
                }
                GetComponent<Wandering>().enabled = true;
            }

            if (astar != null && !astar.locked)
            {
                if (astar.path.Count == 0)
                {
                    if (TAR != null && (TAR.transform.position - transform.position).magnitude < TrackDistance)
                    {
                        if ((TAR.transform.position - transform.position).magnitude < AttackRange)
                        {
                            if (Freeze >= AttackDuration)
                            {
                                LivingEntity eb = TAR.GetComponent<LivingEntity>();
                                eb.ForceAppendCollision(Force * 10, new Vector3(0, transform.position.y - TAR.transform.position.y, 0));
                                if(eb.nowHealth < 0)
                                {
                                    if (TAR.transform != GameControll.localControll.transform.root)
                                    {
                                        Destroy(TAR);
                                    }
                                    else
                                    {
                                        //TODO : Player Death
                                    }
                                    TAR = null;
                                    saturation = 500;
                                    return;
                                }

                                Freeze = 0;
                            }
                        }
                        else
                        {
                            transform.position = Vector3.MoveTowards(transform.position, TAR.transform.position + new Vector3(0, 0, 0), speed * Time.deltaTime);
                        }
                        transform.LookAt(new Vector3(TAR.transform.position.x, transform.position.y, TAR.transform.position.z));
                    }
                    else
                    {
                        Collider[] colliders = Physics.OverlapSphere(transform.position, TrackDistance);
                        foreach (Collider co in colliders)
                        {
                            if (co.GetComponent<PlayerEntity>() != null || co.GetComponent<AnimalAI>() != null)
                            {
                                TAR = co.gameObject;

                                GetComponent<Wandering>().enabled = false;
                                astar.ChangeTarget(transform.position, co.transform.position);
                                node = Vector3.zero;
                                //start = false;
                                return;
                            }
                        }
                    }
                    GetComponent<Wandering>().enabled = true;
                }
                if (astar.path.Count != 0)
                {
                    if ((Mathf.Abs(transform.position.x - node.x) < 1.5f && Mathf.Abs(transform.position.y - node.y) < 2f && Mathf.Abs(transform.position.z - node.z) < 1.5f) || node == Vector3.zero)
                    {
                        node = astar.path.Pop();
                    }
                    transform.LookAt(new Vector3(node.x, transform.position.y, node.z));
                    transform.position = Vector3.MoveTowards(transform.position, node + new Vector3(0, 1f, 0.0f), speed * Time.deltaTime);
                }
            }
        }
    }
}
