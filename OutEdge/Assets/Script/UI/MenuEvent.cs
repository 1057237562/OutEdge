using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityStandardAssets.Characters.FirstPerson.RigidbodyFirstPersonController;

public class MenuEvent : MonoBehaviour
{

    public void resume()
    { 
        Time.timeScale = 1.0f;
        transform.parent.gameObject.SetActive(false);
        rfpc.cam.GetComponent<GameControll>().enabled = true;
        UIManager.ui.deinteract();
    }

    public void save()
    {
        try
        {
            if(OutEdgeNetworkManager.networkManager.mode == Mirror.NetworkManagerMode.Host)
            {
                Debug.Log("Saved to:" + Environment.CurrentDirectory + "/saves/" + StartGame.savePath + "/players.dat");
                TerrainManager.tm.SaveChunks();

                FileSystem.SaveCharacter(rfpc.gameObject, rfpc.cam.gameObject, Environment.CurrentDirectory + "/saves/" + StartGame.savePath + "/players.dat");
                FileSystem.SaveWorld(Environment.CurrentDirectory + "/saves/" + StartGame.savePath + "/world.dat");
            }
        }
        catch { }
    }

    public void saveAndQuit()
    {
        try
        {
            if (OutEdgeNetworkManager.networkManager.mode == Mirror.NetworkManagerMode.Host)
            {
                OutEdgeNetworkManager.networkManager.StopHost();
                Debug.Log("Saved to:" + Environment.CurrentDirectory + "/saves/" + StartGame.savePath + "/players.dat");
                TerrainManager.tm.SaveChunks();

                FileSystem.SaveCharacter(rfpc.gameObject, rfpc.cam.gameObject, Environment.CurrentDirectory + "/saves/" + StartGame.savePath + "/players.dat");
                FileSystem.SaveWorld(Environment.CurrentDirectory + "/saves/" + StartGame.savePath + "/world.dat");
                SceneManager.LoadScene("StartScene");
                Time.timeScale = 1f;
            }
            else
            {
                OutEdgeNetworkManager.networkManager.StopClient();
                SceneManager.LoadScene("StartScene");
                Time.timeScale = 1f;
            }
        }
        catch { }
        // Return to MainMenu
    }

    public void autoSave(bool value)
    {
        if (value)
        {

        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Time.timeScale = 1.0f;
            transform.parent.gameObject.SetActive(false);
            rfpc.cam.GetComponent<GameControll>().enabled = true;
            UIManager.ui.deinteract();
        }
    }
}
