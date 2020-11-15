using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MouseImage : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
{
    public static ItemManager.ItemStack holding;
    public List<GameObject> btnDock;
    public ThrowItem ti;
    public Text cdis;
    public GameObject damage;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public bool setTexture(Texture2D texture, ItemManager.ItemStack stack)
    {
        if (holding == null)
        {
            cdis.text = stack.count + "";
            holding = stack;
            if(stack.item.id >= 0 && ItemManager.im.maxdamage[stack.item.id] > 0)
            {
                damage.SetActive(true);
                damage.GetComponent<Slider>().maxValue = ItemManager.im.maxdamage[stack.item.id];
                damage.GetComponent<Slider>().value = stack.damage;
            }
            else
            {
                damage.SetActive(false);
            }
            GetComponent<Image>().sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0, 0));
            gameObject.SetActive(true);
            transform.position = Input.mousePosition;
            return true;
        }
        return false;
    }

    public void dismiss()
    {
        cdis.text = "";
        holding = null;
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Input.mousePosition;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        PassEvent(eventData, ExecuteEvents.pointerDownHandler);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        PassEvent(eventData, ExecuteEvents.pointerUpHandler);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        PassEvent(eventData, ExecuteEvents.submitHandler);
        PassEvent(eventData, ExecuteEvents.pointerClickHandler);
        if(holding != null)
            cdis.text = holding.count + "";
    }

    public void PassEvent<T>(PointerEventData data, ExecuteEvents.EventFunction<T> function)
        where T : IEventSystemHandler
    {
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(data, results);
        //GameObject current = data.pointerCurrentRaycast.gameObject;
        for (int i = results.IndexOf(data.pointerCurrentRaycast) + 1; i < results.Count; i++)
        {
            if (btnDock.Contains(results[i].gameObject))
            {
                ExecuteEvents.Execute(results[i].gameObject, data, function);
                return;
            }
        }
        ExecuteEvents.Execute(ti.gameObject, data, function);
    }
}
