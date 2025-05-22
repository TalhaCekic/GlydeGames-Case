using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class dayManager : NetworkBehaviour
{
    public static dayManager instance;
    [SerializeField] private Light sun;

    [SerializeField] [SyncVar]  private float sunRotationSpeed;
    
    [SerializeField]
    private Gradient equatorColor;

    [SerializeField] private Gradient sunColor;

    [SyncVar]  private float newRotation;
    [SyncVar] private float currentRotation;
    private Quaternion startRotation;
    private Quaternion endRotation;
    [SyncVar] private bool hasRotated;
    [SyncVar] private float rotationTime;
    [SyncVar] private bool changeTime;

    //public float Time;
    [SyncVar] public bool isDayOn;
    [SyncVar] public bool isNightDay;
    [SyncVar] public bool isHourOn;
    [SyncVar] public bool isdayFinished;
    [SyncVar] public int day;
    [SyncVar] [SerializeField, Range(0, 24)] public int hour;
    [SyncVar] [SerializeField, Range(0, 24)] public float timeOfDay;
    [SyncVar] public int minute;
    [SyncVar] public float minuteF;

    public TMP_Text HourText;
    public TMP_Text MinuteText;

    //sleep
    public Image SleepingBg;
    private Color NewColor;

    public GameObject endDayCanvas;

    void Start()
    {
        if (isServer)
        {
            ServerStart();
        }
    }

    [Server]
    private void ServerStart()
    {
        instance = this;
        isNightDay = false;
        hour = 08;
        timeOfDay = 8;
        minute = 00;
        sunRotationSpeed = 1;
        RpcStart();
    }  
    [ClientRpc]
    private void RpcStart()
    {
       // SleepingBg.gameObject.SetActive(false);
        ///endDayCanvas.SetActive(false);
    }
    private void OnValidate()
    {
        UpdateSunRotation();
        UpdateLighting();
    }
    [Server]
    private void UpdateSunRotation()
    {
        float sunRotation = Mathf.Lerp(-90, 270, timeOfDay / 20);
        RpcSunRotation(sunRotation);
    }

    [ClientRpc]
    private void RpcSunRotation(float sunRotation)
    {
        sun.transform.rotation = Quaternion.Euler(sunRotation, sun.transform.rotation.y, sun.transform.rotation.z);
    }
    [Server]
    private void UpdateLighting()
    {
        float timeFraction = timeOfDay / 20;
        RpcLighting(timeFraction);
    }
    [ClientRpc]
    private void RpcLighting(float timeFraction)
    {
        RenderSettings.ambientEquatorColor = equatorColor.Evaluate(timeFraction);
        //RenderSettings.ambientSkyColor = skyColor.Evaluate(timeFraction);
        sun.color = sunColor.Evaluate(timeFraction);
    }

    void Update()
    {
        if (isServer)
        {
            ServerUpdate();
        }
    }
    [Server]
    private void ServerUpdate()
    {
        TimeSettings();
        HourPrint();
        DayOfCheck();
        UpdateSunRotation();
        UpdateLighting();
        //EndTheDay();
        if (GameManager.instance.isDayOn)
        {
            timeOfDay += Time.deltaTime / 30 * sunRotationSpeed;

            minuteF += Time.deltaTime * 2 * sunRotationSpeed;
            minute = Mathf.RoundToInt(minuteF);

            if (minuteF > 59)
            {
                isHourOn = true;
            }

            if (isHourOn)
            {
                minuteF = 0;
                hour++;
                if (hour == 20)
                {
                    GameManager.instance.isDayOn = false;
                    hasRotated = false;
                    isHourOn = false;
                    minute = 0;
                    minuteF = 0;
                }
                hasRotated = false;
                isHourOn = false;
            }
        }
        else
        {
            if (hour == 20)
            {
                CustomerManager.instance.ServerListQueueState();
            }
        }
    }

    [Server]
    private void DayOfCheck()
    {
        if (hour >= 24)
        {
            hour = 0;
            timeOfDay = 0;
            minuteF = 0;
            minute = 0;
            sunRotationSpeed = 0;
        }

        if (hour >= 17 || hour < 7)
        {
            isNightDay = true;
            //RenderSettings.skybox = skybox2;
            //RenderSettings.skybox.Lerp(skybox2, skybox1, 100);
        }
        else
        {
            isNightDay = false;
            //RenderSettings.skybox = skybox1.tintColor;
            //RenderSettings.skybox.Lerp(skybox2, skybox1, 100000);
        }
    }
    
    // zaman yazdırmak için
    [ClientRpc]
    private void HourPrint()
    {
        switch (minute)
        {
            case < 10:
                MinuteText.text = "0" + minute.ToString();
                break;
            case >= 10:
                MinuteText.text = minute.ToString();
                break;
        }

        switch (hour)
        {
            case < 10:
                HourText.text = "0" + hour.ToString();
                break;
            case >= 10:
                HourText.text = hour.ToString();
                break;
        }
    }

    [Server]
    private void TimeSettings()
    {
        if (Input.GetKey(KeyCode.Alpha0))
        {
            sunRotationSpeed = 0;
        }

        if (Input.GetKey(KeyCode.Alpha1))
        {
            sunRotationSpeed = 1;
            //isdayFinished = false;
            //isDayOn = true;
        }

        if (Input.GetKey(KeyCode.Alpha2))
        {
            sunRotationSpeed = 2;
            //isdayFinished = false;
            //isDayOn = true;
        }

        if (Input.GetKey(KeyCode.Alpha3))
        {
            sunRotationSpeed = 3;
            //isdayFinished = false;
            //isDayOn = true;
        }

        if (Input.GetKey(KeyCode.Alpha4))
        {
            sunRotationSpeed = 6;
            //isdayFinished = false;
            //isDayOn = true;
        }
    }

    // gün sonu
    // private void EndTheDay()
    // {
    //     if (isdayFinished && !isDayOn)
    //     {
    //         endDayCanvas.SetActive(true);
    //     }
    //     else
    //     {
    //         endDayCanvas.SetActive(false);
    //     }
    // }

    //butona verlen devam et etkisi
    public void notEndTheDay()
    {
        isdayFinished = false;
        isDayOn = true;
    }
}