using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Wandering : AIBase
{
    public int MinDelay = 5;
    public int MaxDelay = 10;

    public float MinRange = 4f;
    public float MaxRange = 9f;

    public float speed = 3.5f;

    int delay;
    float start;
    float range;

    bool fired = false;

    Vector2 direction;

    ParameterizedThreadStart movement;

    private void Start()
    {
        delay = MinDelay;
        start = Time.time;
        if (movement == null)
        {
            movement = new ParameterizedThreadStart(OnEntityMove);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (range > 0)
        {
            transform.LookAt(new Vector3(transform.position.x + direction.x, transform.position.y, transform.position.z + direction.y));
            if(TerrainManager.tm.isAreaLoaded(Vector3.MoveTowards(transform.position, new Vector3(transform.position.x + direction.x * range, transform.position.y, transform.position.z + direction.y * range), speed * Time.deltaTime))){
                transform.position = Vector3.MoveTowards(transform.position, new Vector3(transform.position.x + direction.x * range, transform.position.y, transform.position.z + direction.y * range), speed * Time.deltaTime);
            }else{
                range = 0;
            }
            
            range -= speed * Time.deltaTime;
        }
        else if (!fired)
        {
            fired = true;
            Thread thread = new Thread(movement);
            thread.Start(transform.position);
        }

        if (Time.time - start >= delay)
        {
            direction = new Vector2(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f));
            //Debug.LogWarning(direction);
            range = UnityEngine.Random.Range(MinRange, MaxRange);

            start = Time.time;
            delay = UnityEngine.Random.Range(MinDelay, MaxDelay);
            fired = false;
        }
    }

    public virtual void OnEntityMove(object position)
    {

    }

    public void RegisterEvent(ParameterizedThreadStart action)
    {
        movement = action;
    }
}
