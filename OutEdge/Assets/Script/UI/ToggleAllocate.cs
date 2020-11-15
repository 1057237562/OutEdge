using UnityEngine;
using UnityEngine.UI;
using static UnityStandardAssets.Characters.FirstPerson.RigidbodyFirstPersonController;

public class ToggleAllocate : MonoBehaviour
{
    public string componentName;

    private void Start()
    {
        GetComponent<Toggle>().isOn = ((Behaviour)rfpc.cam.GetComponent(componentName)).enabled;
    }

    public void Changed(bool on)
    {
        ((Behaviour)rfpc.cam.GetComponent(componentName)).enabled = on;
    }
}
