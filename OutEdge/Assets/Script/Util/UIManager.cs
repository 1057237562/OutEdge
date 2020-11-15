using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityStandardAssets.Characters.FirstPerson.RigidbodyFirstPersonController;

public class UIManager : MonoBehaviour
{
    public GuiObject interactor;
    public bool interacting = false;

    public Canvas normal;
    public GameObject menu;
    public Canvas inventory;
    public GameObject container;
    public GameObject settings;

    public GameObject mainhd;
    public GameObject sechd;
    public GameObject mischd;

    public Canvas crafting;

    public GameObject progression;

    public GameObject health;
    public GameObject thrist;
    public GameObject satisfaction;

    public Light sunLight;

    public static UIManager ui;

    private void Awake()
    {
        ui = this;
    }

    private void Update()
    {
        if (interacting && (InputManager.GetKeyDown("OpenInventory") || Input.GetKeyDown(KeyCode.Escape)) && !menu.activeSelf && !settings.activeSelf && (interactor != null ? interactor.CanDestroy : true))
        {
            interacting = false;
            normal.enabled = true;
            rfpc.releaseControll = true;
            if (inventory.GetComponent<Canvas>().enabled)
            {
                inventory.GetComponent<Canvas>().enabled = false;
            }

            if (interactor != null)
            {
                interactor.interacting = false;
                rfpc.cam.GetComponent<Camera>().enabled = true;
                if (interactor.gui != null)
                {
                    interactor.gui.GetComponent<Camera>().enabled = false;
                    rfpc.cam.GetComponent<AudioListener>().enabled = true;
                    interactor.gui.GetComponent<AudioListener>().enabled = false;
                }
                interactor.LostFocus();
                interactor = null;
            }

            return;
        }

        if (!interacting)
        {
            if (InputManager.GetKeyDown("OpenInventory"))
            {
                interact();
            }
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            if (Input.GetKeyDown(KeyCode.Escape) && !settings.activeSelf && !inventory.enabled)
            {
                Time.timeScale = 0.0f;
                interacting = true;
                menu.SetActive(true);
                rfpc.releaseControll = false;
                normal.enabled = false;
            }
        }
        else
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }

    public void interact()
    {
        inventory.enabled = true;
        interacting = true;
        rfpc.releaseControll = false;
        normal.enabled = false;
    }

    public void deinteract()
    {
        Time.timeScale = 1.0f;
        menu.SetActive(false);
        interacting = false;
        normal.enabled = true;
        rfpc.releaseControll = true;
        Cursor.visible = false;
        if (inventory.GetComponent<Canvas>().enabled)
        {
            inventory.GetComponent<Canvas>().enabled = false;
        }

        if (interactor != null)
        {
            interactor.interacting = false;
            rfpc.cam.GetComponent<Camera>().enabled = true;
            interactor.gui.GetComponent<Camera>().enabled = false;
            rfpc.cam.GetComponent<AudioListener>().enabled = true;
            interactor.gui.GetComponent<AudioListener>().enabled = false;
            interactor.LostFocus();
            interactor = null;
        }
    }
}
