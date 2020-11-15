using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CloseUI : MonoBehaviour
{
    public GameObject window;
    public UnityEvent closeEvent;
    
    public void Close()
    {
        closeEvent.Invoke();
        Destroy(window);
    }
}
