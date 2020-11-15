using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static InputManager;

public class LoadControls : MonoBehaviour
{
    public static LoadControls loadControls;

    public List<GameObject> tar = new List<GameObject>();
    public List<string> keypair = new List<string>();

    public List<GameObject> axistar = new List<GameObject>();
    public List<string> axispair = new List<string>();

    // Start is called before the first frame update
    void OnEnable()
    {
        loadControls = this;
        for(int i = 0; i < tar.Count; i++)
        {
            tar[i].GetComponent<KeyCodeInput>().SetKeyCode(m.keypair[m.keyrefer.IndexOf(keypair[i])]);
        }
        for (int i = 0; i < axispair.Count; i++)
        {
            axistar[i * 2].GetComponent<KeyCodeInput>().SetKeyCode(m.axispair[m.axisrefer.IndexOf(axispair[i])].positive);
            axistar[i * 2 + 1].GetComponent<KeyCodeInput>().SetKeyCode(m.axispair[m.axisrefer.IndexOf(axispair[i])].negative);
        }
    }
}
