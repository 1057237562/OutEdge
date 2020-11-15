using AuraAPI;
using CommandTerminal;
using DigitalRuby.RainMaker;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class DayNight : MonoBehaviour
{

    float last;

    public static bool night = false;

    public static float rot = 0;

    public GameObject nightlight;
    public bool enableAura;

    public GameObject rains;
    public GameObject raineffect;

    public static DayNight dn;

    public float startTime;
    public int duration;

    public Material terrain;

    void Awake()
    {
        dn = this;
    }

    private void Start()
    {
        last = Time.fixedTime;
        startTime = -10f;
        dn.terrain.SetFloat("_Glossiness", 0);
        if (QualitySettings.GetQualityLevel() >= 4)
        {
            enableAura = true;
        }
    }

    [RegisterCommand(Help ="Change Weather State",MaxArgCount = 3,MinArgCount = 1)]
    public static void weather(CommandArg[] args)
    {
        switch (args[0].String){
            case "rain":
                if (args.Length >= 2)
                {
                    StartRain(args[1].Float);
                    if(args.Length == 3)
                    {
                        dn.duration = args[2].Int;
                        dn.startTime = Time.fixedTime;
                    }
                }
                else
                {
                    StartRain(0.2f);
                }
                break;
            case "clear":
                StopRain();
                break;
        }
    }

    public static bool israin = false;

    //public CloudScript cs;

    public static void StartRain(float strength)
    {
        if (!israin)
        {
            dn.rains.GetComponent<RainScript>().RainIntensity = strength;
            //dn.raineffect.GetComponent<RainCameraController>().distance = strength * 10;
            dn.GetComponent<Light>().intensity = 0.5f;
            dn.terrain.SetFloat("_Glossiness",0.5f);
            israin = true;
        }
    }

    public void StartRainEffect()
    {
        dn.raineffect.GetComponent<RainCameraController>().Play();
    }
    public void StopRainEffect()
    {
        dn.raineffect.GetComponent<RainCameraController>().Stop();
    }

    public static void StopRain()
    {
        if (israin)
        {
            dn.rains.GetComponent<RainScript>().RainIntensity = 0;
            dn.raineffect.GetComponent<RainCameraController>().Stop();
            dn.GetComponent<Light>().intensity = 1.4f;
            dn.terrain.SetFloat("_Glossiness", 0);
            israin = false;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        rot += 0.006f;
        transform.rotation = Quaternion.Euler(rot, -90, 0);
        if (Time.fixedTime - last >= 0.1)
        {
            if (israin)
            {
                Camera.main.GetComponent<CloudScript>().coverage = 0.95f + Mathf.Clamp(1.05f * 0.1f * (Time.fixedTime - startTime), 0, 1.05f);
                Camera.main.GetComponent<CloudScript>().sunLightFactor = 0.8f - Mathf.Clamp(0.3f * 0.1f * (Time.fixedTime - startTime), 0, 0.3f);

                Ray ray = new Ray(RigidbodyFirstPersonController.rfpc.transform.position, RigidbodyFirstPersonController.rfpc.transform.up);
                if (Physics.Raycast(ray,out _, 20,1<<0))
                {
                    StopRainEffect();
                }
                else
                {
                    StartRainEffect();
                }
            }
            else
            {
                RigidbodyFirstPersonController.rfpc.cam.GetComponent<CloudScript>().coverage = 2 - Mathf.Clamp(1.05f * 0.1f * (Time.fixedTime - startTime), 0, 1.05f);
                RigidbodyFirstPersonController.rfpc.cam.GetComponent<CloudScript>().sunLightFactor = 0.5f + Mathf.Clamp(0.3f * 0.1f * (Time.fixedTime - startTime), 0, 0.3f);
            }
            if (Random.Range(0, 10000) == 1)
            {
                StartRain(Random.Range(0f,1f));

                duration = Random.Range(0, 200);
                startTime = Time.fixedTime;
            }
            
            if(Time.fixedTime - startTime > startTime + duration && israin)
            {
                StopRain();
                startTime = Time.fixedTime;
            }
            //RenderSettings.skybox.mainTextureOffset = new Vector2((6 + rot / 360f * 24f) % 24/24f, 0);
            float d = (Mathf.Sin((rot-45)/180 * 2*Mathf.PI)+1)*4;
            if (night)
            {
                d *= 5f;
            }
            float e = Mathf.Sin((rot+20)/360*2*Mathf.PI);
            RenderSettings.skybox.SetFloat("_Exposure", Mathf.Clamp(1.3f+e*2,0f,2f));

            RenderSettings.skybox.SetColor("_SkyTint", new Color(Mathf.Clamp(1-d,0,1),Mathf.Clamp(1-d,0,1),Mathf.Clamp(1-d,0,1),1));
            last = Time.fixedTime;
            if((6 + rot / 360f * 24)%24 > 18 || (6 + rot / 360f * 24) % 24 < 6)
            {
                night = true;
                GetComponent<Light>().enabled = false;
                if (enableAura)
                {
                    GetComponent<AuraLight>().enabled = false;
                    nightlight.GetComponent<AuraLight>().enabled = true;
                }
                nightlight.GetComponent<Light>().enabled = true;
                
            }
            else
            {
                night = false;
                nightlight.GetComponent<Light>().enabled = false;
                if (enableAura)
                {
                    nightlight.GetComponent<AuraLight>().enabled = false;
                    GetComponent<AuraLight>().enabled = true;
                }
                GetComponent<Light>().enabled = true;
            }
        }
    }

    [RegisterCommand(Help = "Set the global time. Usage: setTime time", MinArgCount = 1, MaxArgCount = 1)]
    public static void setTime(CommandArg[] args)
    {
        rot = args[0].Float;
    }
}
