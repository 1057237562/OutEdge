using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CommandTerminal;
using Mirror;
using static UIManager; 

public class PlayerEntity : LivingEntity
{
    public float MaxSatisfaction = 20;
    public float Satisfaction = 20;

    public float MaxThrist = 20;
    public float Thrist = 20;

    public float Starvespeed = 0.0001f;
    public float Thristspeed = 0.0005f;

    public float harmspeed = 0.01f;

    public static PlayerEntity localPlayer;

    // Start is called before the first frame update
    void Start()
    {
        if (!GetComponent<NetworkIdentity>().isLocalPlayer) { 
            Destroy(this);
            return;
        }
        localPlayer = this;
    }

    // Update is called once per frame
    public void FixedUpdate()
    {
        //base.FixedUpdate();
        if(Satisfaction <= 0 || Thrist <= 0)
        {
            nowHealth-=harmspeed;
        }

        if(nowHealth >= maxHealth && Satisfaction >= maxHealth - 1 && Thrist >= MaxThrist - 1)
        {
            if (nowHealth + 0.5f <= maxHealth)
            {
                nowHealth += 0.5f;
            }
            else
            {
                nowHealth = maxHealth;
            }
        }

        Satisfaction -= Starvespeed;
        Thrist -= Thristspeed;

        ui.health.GetComponent<Image>().fillAmount = nowHealth / maxHealth;
        ui.thrist.GetComponent<Image>().fillAmount = Thrist / MaxThrist;
        ui.satisfaction.GetComponent<Image>().fillAmount = Satisfaction / MaxSatisfaction;
    }

    [RegisterCommand(Help = "Fill the satisfaction")]
    public static void feed(CommandArg[] args)
    {
        localPlayer.Satisfaction = localPlayer.MaxSatisfaction;
    }

    [RegisterCommand(Help = "Fill the thrist")]
    public static void drink(CommandArg[] args)
    {
        localPlayer.Thrist = localPlayer.MaxThrist;
    }

    [RegisterCommand(Help = "Restore to healthy")]
    public static void regenerate(CommandArg[] args)
    {
        localPlayer.nowHealth = localPlayer.maxHealth;
    }

    public override void Death()
    {
        // Show Gui
    }
}
