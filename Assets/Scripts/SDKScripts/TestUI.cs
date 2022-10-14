using NotificationSamples;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestUI : MonoBehaviour
{
    public static TestUI instance;

    [SerializeField] private Text consoleText;
    [SerializeField] private InputField timerInput;
    [SerializeField] private InputField rewardBonusesInput;
    [SerializeField] private GameObject SDKOptionsPanel;
    private SDKController SDKController;
    private QuizManager quizManager;

    void Awake()
    {
        if (!instance)
        {
            instance = this;
            DontDestroyOnLoad(transform.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        SDKController = GameObject.Find("[SDKController]").GetComponent<SDKController>();
        InitQuizManager();
        timerInput.gameObject.transform.GetChild(0).GetComponent<Text>().text = SDKController.Timer.ToString();
        rewardBonusesInput.gameObject.transform.GetChild(0).GetComponent<Text>().text = SDKController.rewardBonuses.ToString();
        SDKController.eventMessage += outConsoleMessage;
    }


    /// <summary>
    /// Событие кнопки
    /// </summary>
    public void clickSDKOptions()
    {
        if (SDKOptionsPanel.activeSelf)
        {
            SDKOptionsPanel.SetActive(false);
        }
        else
        {
            SDKOptionsPanel.SetActive(true);
        }
    }

    /// <summary>
    /// Событие InputField
    /// </summary>
    public void setRewardBonuses()
    {
        SDKController.rewardBonuses = Convert.ToInt32(rewardBonusesInput.text);
    }
    /// <summary>
    /// Событие InputField
    /// </summary>
    public void setSecondsForTimer()
    {
        SDKController.setTimer(Convert.ToInt32(timerInput.text));

        var timeFormat = TimeSpan.FromSeconds(SDKController.Timer);
        SDKController.Message = string.Format("Рекламный таймер обновлен до {0} мин и {1} сек", timeFormat.Minutes, timeFormat.Seconds);
    }

    public void clickSendPushMessage()
    {
        SDKController.Message = "Пуш уведомление придет через 5 сек";
        Notyfier.sendMessageToPush("Пуш заголовок прибыл", "Бро ты крут!", 0);
    }
    public void clickShowRewarded()
    {
        SDKController.ShowRewarded();
    }

    private void outConsoleMessage(string message)
    {
        consoleText.text = SDKController.Message;
    }
    public void clickAddGolds()
    {
        if (quizManager != null)
        {
            quizManager.Golds += 1000;
            quizManager.SaveGolds();
            SDKController.Message = "читы +1000 монет";
        }
        else
        {
            SDKController.Message = "Работает только в игре";
        }
    }
    public void clickRemoveGolds()
    {
        if(quizManager != null)
        {
            quizManager.Golds = 0;
            quizManager.SaveGolds();
            SDKController.Message = "Обнуление";
        }
        else
        {
            SDKController.Message = "Работает только в игре";
        }
    }
    public void LevelToZero()
    {
        PlayerPrefs.SetInt("PlayerLvl", 0);
        SDKController.Message = "Перезайдите в сцену для сброса уровней";
    }

    private void OnLevelWasLoaded(int level)
    {
        InitQuizManager();
    }
    private bool InitQuizManager()
    {
        GameObject GO = GameObject.Find("QuizManager");
        if (GO != null)
        {
            quizManager = GO.GetComponent<QuizManager>();
            return true;
        }
        else
        {
            return false;
        }
    }
}
