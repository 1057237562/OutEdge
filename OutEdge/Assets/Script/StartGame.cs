using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartGame : MonoBehaviour
{

    public static string savePath;
    public static int gameMode = 0;

    public Slider slider;
    private AsyncOperation operation;

    public GameObject settings;
    public GameObject loading;
    public GameObject joining;

    public TMPro.TMP_InputField path;
    public TMPro.TMP_Dropdown gamemode;
    public InputField randomize;
    public Toggle fastLoad;
    // Start is called before the first frame update
    void Start()
    {
        slider.gameObject.SetActive(false);
        GetComponent<Button>().onClick.AddListener(delegate () { if (settings.activeSelf)
            {
                int outputseed = 0;
                if (randomize.text != "")
                {
                    try
                    {
                        outputseed = int.Parse(randomize.text);
                    }
                    catch
                    {

                        outputseed = randomize.text.GetHashCode();
                    }
                }
                else
                {
                    outputseed = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
                }
                GameControll.globalRandomize = new System.Random(outputseed);
                GameControll.randomseed = outputseed;

                savePath = path.text;
                if (Directory.Exists(Environment.CurrentDirectory + "/saves/" + savePath))
                {
                    int apply = 1;
                    while (Directory.Exists(Environment.CurrentDirectory + "/saves/" + savePath+apply.ToString()))
                    {
                        apply++;
                    }
                    savePath = path.text + apply.ToString();
                }
                gameMode = gamemode.value;
                GetComponent<Button>().interactable = false;
                
                Time.timeScale = 0;

                OutEdgeNetworkManager.networkManager.StartHost();
                startLoading();
                loading.SetActive(false);
                joining.SetActive(false);
            }
            else
            {
                loading.SetActive(false);
                settings.SetActive(true);
                joining.SetActive(false);
            }
            });
    }

    public void startLoading()
    {
        slider.gameObject.SetActive(true);
        //StartCoroutine(AsyncLoading());
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
