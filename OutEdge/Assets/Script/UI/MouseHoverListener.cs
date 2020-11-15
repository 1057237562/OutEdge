using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class EventSender:UnityEvent<GameObject>{}

public class MouseHoverListener : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    public EventSender enter = new EventSender();
    public EventSender exit = new EventSender();

    public void OnPointerEnter(PointerEventData eventData)
    {
        enter.Invoke(gameObject);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        exit.Invoke(gameObject);
    }
}
