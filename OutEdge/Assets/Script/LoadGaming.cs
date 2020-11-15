using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadGaming : MonoBehaviour
{

    public Slider slider;
    public Toggle fastLoad;
    private AsyncOperation operation;

    public void Start()
    {
        GetComponent<Button>().onClick.AddListener(delegate
        {
            StartGame.savePath = transform.GetChild(0).GetComponent<Text>().text;
            GetComponent<Button>().interactable = false;
            Time.timeScale = 0;
            OutEdgeNetworkManager.networkManager.StartHost();
            startLoading();
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
                //TerrainManager.tm.OnSyncWithServer();
            }
        }
    }
}
