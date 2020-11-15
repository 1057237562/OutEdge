using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityStandardAssets.Characters.FirstPerson;

public class ThrowItem : MonoBehaviour, IPointerClickHandler
{
    public MouseImage mouse;
    bool executed = false;
    public void PassEvent<T>(PointerEventData data, ExecuteEvents.EventFunction<T> function)
        where T : IEventSystemHandler
    {
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(data, results);
        for (int i = 0; i < results.Count; i++)
        {
            if (results[i].gameObject.GetComponent<ItemHolder>() != null)
            {
                ExecuteEvents.Execute(results[i].gameObject, data, function);
                executed = true;
            }
        }
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        executed = false;
        PassEvent(eventData, ExecuteEvents.pointerClickHandler);
        if (MouseImage.holding == null) return;
        if (executed) return;
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (MouseImage.holding.item.id >= 0)
            {
                for (int i = 0; i < MouseImage.holding.count; i++)
                    ItemManager.SummonItem(RigidbodyFirstPersonController.rfpc.transform.position + RigidbodyFirstPersonController.rfpc.transform.rotation * new Vector3(0, 1, 2) + new Vector3(0, (i+1)*ItemManager.im.prefabs[MouseImage.holding.item.id].transform.localScale.y,0), MouseImage.holding.item,MouseImage.holding.damage);
                mouse.dismiss();
            }
        }
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (MouseImage.holding.item.id >= 0)
            {
                ItemManager.SummonItem(RigidbodyFirstPersonController.rfpc.transform.position + RigidbodyFirstPersonController.rfpc.transform.rotation * new Vector3(0, 1, 2), MouseImage.holding.item);
                MouseImage.holding.count--;
                if(MouseImage.holding.count == 0)
                    mouse.dismiss();
            }
        }
    }
}
