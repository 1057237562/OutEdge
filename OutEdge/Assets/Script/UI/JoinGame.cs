using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class JoinGame : MonoBehaviour
{

    public GameObject joining;
    public TMPro.TMP_InputField ip;
    public TMPro.TMP_InputField port;

    public Slider slider;
    private AsyncOperation operation;
    // Start is called before the first frame update

    public GameObject settings;
    public GameObject loading;

    void Start()
    {
        slider.gameObject.SetActive(false);
        GetComponent<Button>().onClick.AddListener(delegate () {
            if (joining.activeSelf)
            {
                Time.timeScale = 0;
                OutEdgeNetworkManager.networkManager.networkAddress = ip.text;
                OutEdgeNetworkManager.networkManager.gameObject.GetComponent<TelepathyTransport>().port = ushort.Parse(port.text);
                OutEdgeNetworkManager.networkManager.StartClient();
                startLoading();

                loading.SetActive(false);
                settings.SetActive(false);
            }
            else
            {
                loading.SetActive(false);
                settings.SetActive(false);
                joining.SetActive(true);
            }
        });
    }

    public void startLoading()
    {
        slider.gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (NetworkManager.loadingSceneAsync != null)
        {
            slider.value = NetworkManager.loadingSceneAsync.progress;
            if (NetworkManager.loadingSceneAsync.progress >= 0.9f)
            {
                slider.value = 1;
            }
        }
    }

}
