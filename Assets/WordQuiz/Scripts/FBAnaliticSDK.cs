using System.Collections;
using Facebook.Unity;
using System.Collections.Generic;
using UnityEngine;

public class FBAnaliticSDK : MonoBehaviour
{
    [Header("FaceBook Analytic ID")]
    // public string FBAnalyticID;
    private SDKController SDKController;

    void Start()
    {
        SDKController = GetComponent<SDKController>();
        Facebook_init();
    }

    /// <summary>
    /// метод вызывается на старте данного класса
    /// </summary>
    private void Facebook_init()
    {
        if (!FB.IsInitialized)
        {
            // Initialize the Facebook SDK
            FB.Init(InitCallback, OnHideUnity);
        }
        else
        {
            // Already initialized, signal an app activation App Event
            FB.ActivateApp();
        }

        void InitCallback()
        {
            if (FB.IsInitialized)
            {
                // Signal an app activation App Event
                FB.ActivateApp();
                // Continue with Facebook SDK
                // ...
                SDKController.Message = "Facebook Analytic init Succesfull";
            }
            else
            {
                Debug.Log("Failed to Initialize the Facebook SDK");
                SDKController.Message = "Failed to Initialize the Facebook Analytic";
            }
        }
        void OnHideUnity(bool isGameShown)
        {
            if (!isGameShown)
            {
                // Pause the game - we will need to hide
                Time.timeScale = 0;
            }
            else
            {
                // Resume the game - we're getting focus again
                Time.timeScale = 1;
            }
        }
    }

    /// <summary>
    /// Вызывать на финише когда уровень пройден
    /// </summary>
    /// <param name="lvl">Номер пройденного уровня</param>
    public void OnlvlEnded(int lvl)
    {
        var tutParams = new Dictionary<string, object>();
        tutParams["Level"] = lvl.ToString(); ;

        FB.LogAppEvent(
            "Пройденных уровней",
            parameters: tutParams
        );
    }

    /// <summary>
    /// Вызвать когда была нажата подсказка за рубины
    /// </summary>
    public void OnAskForHelp()
    {
        var tutParams = new Dictionary<string, object>();
        FB.LogAppEvent(
            "Нажатий подсказки",
            parameters: tutParams
        );
    }

    /// <summary>
    /// Вызвать когда пользователь согласился смотреть рекламу за вознагрождение
    /// </summary>
    public void OnClickToWatchRewardedAd()
    {
        var tutParams = new Dictionary<string, object>();

        FB.LogAppEvent(
            "Согласных на рекламу за вознагрождение",
            parameters: tutParams
        );
    }

    /// <summary>
    /// Вызвать когда пользователь выбрал ячейку с покупкой монет
    /// </summary>
    /// <param name="golds">Количество монет на которые согласился пользователь</param>
    public void OnClickToPayGolds(int golds)
    {
        var tutParams = new Dictionary<string, object>();
        tutParams["Купленных монет"] = golds.ToString(); ;

        FB.LogAppEvent(
            "Клик купить монеты",
            parameters: tutParams
        );
    }
}
