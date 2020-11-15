using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class KeyListener : AttributeContainer
{
    [AttributeType("KeyCode", "")]
    public string key;

    public UnityEvent KeyDownEvent;
    public UnityEvent KeyUpEvent;

    private void Update()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            if (key == "Left Click")
            {
                if (Input.GetMouseButtonUp(0))
                {
                    KeyUpEvent.Invoke();
                }
                return;
            }
            if (key == "Right Click")
            {
                if (Input.GetMouseButtonUp(1))
                {
                    KeyUpEvent.Invoke();
                }
                return;
            }
            if (key == null || Input.GetKeyUp(key))
            {
                KeyUpEvent.Invoke();
            }
        }
    }

    void FixedUpdate()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            GameControll.triggered = false;
            if (key == "Left Click")
            {
                if (Input.GetMouseButton(0))
                {
                    KeyDownEvent.Invoke();
                    GameControll.triggered = true;
                }
                return;
            }
            if (key == "Right Click")
            {
                if (Input.GetMouseButton(1))
                {
                    KeyDownEvent.Invoke();
                    GameControll.triggered = true;
                }
                return;
            }
            if (key == null || Input.GetKey(key))
            {
                KeyDownEvent.Invoke();
                GameControll.triggered = true;
            }
        }
    }
}
