using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using AudienceNetwork;

/// <summary>
///  Если игрок хочет получить подсказку, но ему не хватает средств
///  ему нужно предложить купить монеты или посмотреть рекламу за вознаграждение.
///  Если игрок хочет подсказку его нужно попросить подтвердить.
///  Вы желаете получить подсказку?
///  Да - Нет
///  Игроку нужно предлагать отключить рекламу купив премиум
///  Отключите рекламу и получите 800 кристаллов
///  всего за 149р.
///  (начинаем показывать после 15лвл (startSecondMode)
///  всплывает в 2х случаях
///  - После показа рекламы
///  - после победы в сессиях в которых не показывали рекламу
/// </summary>
public class SDKController : MonoBehaviour
{
    public static SDKController instance;

    public QuizEvent OnFinishInterAd;
    public QuizEvent OnFinishRewardAd;
   

    [Header("Таймер до показа Рекламы (в секундах)")]
    public int Timer;
    [Header("Бонусы за просмотр специальной рекламы")]
    public int rewardBonuses;
    [Header("С какого левела предлагать купить премиум")]
    public int vipPanelShowAdCount = 3;
    [Header("Счетчик раундов для запуска обычной рекламы после ревард рекламы")]
    public int commonRoundCounterForInterAds;
    public AdsRoundManager adsManager;
    public int intAdsWatched;
    private ApplovinSDK applovinSDK;

    public event Action<string> eventMessage;

    private string message = "";
    public string Message
    {
        get
        {
            return message;
        }
        set
        {
            string newMessage = string.Format("[{0}] {1}\n", DateTime.Now.ToString("HH:mm:ss"), value);
            message = message.Insert(0, newMessage);
            eventMessage?.Invoke(value);
        }
    }

    private QuizManager quizManager;
    [HideInInspector] public GameUIController gameUIController;

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
        OnFinishInterAd = new QuizEvent();
        OnFinishRewardAd = new QuizEvent();
    }

    private void Start()
    {
        if (PlayerPrefs.HasKey("Timer"))
        {
            Timer = 99999;
        }
        adsManager = new AdsRoundManager(commonRoundCounterForInterAds, Timer);
        applovinSDK = GetComponent<ApplovinSDK>();

        OnFinishInterAd.AddListener(() =>
        {
            intAdsWatched++;
        });
        AdSettings.SetDataProcessingOptions(new string[] { }); //Чтобы явно не включать режим ограниченного использования данных (LDU) , используйте:
        AdSettings.SetDataProcessingOptions(new string[] { "LDU" }, 1, 1000);//Чтобы включить LDU для пользователей и указать географию пользователя , используйте:
    }

    public void ShowInterstitial()
    {
        applovinSDK.ShowInterstitial();
    }

    public void ShowRewarded()
    {
        applovinSDK.ShowRewarded();
    }
    

    /// <summary>
    /// Для TestUI.cs
    /// </summary>
    public void getRewardBonuses()
    {
        quizManager.Golds += rewardBonuses;
        quizManager.SaveGolds();
    }

    /// <summary>
    /// запихнуть в метод 
    /// </summary>
    /// <param name="newTimer"></param>
    public void setTimer(int newTimer)
    {
        Timer = newTimer;
        adsManager.setTimer(Timer);
    }


    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnLevelGameScene;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnLevelGameScene;
    }

    public bool AdWillBeShown()
    {
        if (adsManager.isTimeUp() && !adsManager.hasNextRound)
            return true;
        return false;
    }

    private void TEST_ProvideAdsToEvent()
    {
        if (adsManager.isTimeUp() && !adsManager.hasNextRound) // Если сработал таймер и не смотрели рекламу за вознаграждение то можно запускать обычную рекламу
        {
            gameUIController.TestBanner_Panel.SetActive(true);
            var timeFormat = TimeSpan.FromSeconds(Timer);
            Message = string.Format("Рекламный таймер обновлен до {0} мин и {1} сек", timeFormat.Minutes, timeFormat.Seconds);
            if (intAdsWatched % vipPanelShowAdCount == 0)
            {
                gameUIController.mayShowVipPanel = true; //В следующий раз предложить купить вип акк
            }
            adsManager.setTimer();
        }
        else // Если не сработал таймер на финише
        {
            Message = "Реклама будет через " + (Timer - adsManager.getRestTime());
            Message = "И осталось раундов: " + adsManager.getCurrentRound();
        }
        gameUIController.ADSButton.SetActive(true);
        adsManager.NextRound();
    }
    private void ProvideAdsToEvent()
    {
        if (adsManager.isTimeUp() && !adsManager.hasNextRound) // Если сработал таймер и не смотрели рекламу за вознаграждение то можно запускать обычную рекламу
        {
            ShowInterstitial();
            if (intAdsWatched % vipPanelShowAdCount == 0)
            {
                gameUIController.mayShowVipPanel = true; //В следующий раз предложить купить вип акк
            }
            adsManager.setTimer();
        }
        else // Если не сработал таймер на финише
        {
        }
        gameUIController.ADSButton.SetActive(true);
        adsManager.NextRound();
    }


    private void OnLevelGameScene(Scene scene, LoadSceneMode sceneMode)
    {
        if (scene.name == "Game")
        {
            GameObject GO = GameObject.Find("QuizManager");
            if (GO != null)
            {
                quizManager = GO.GetComponent<QuizManager>();
                gameUIController = GO.GetComponent<GameUIController>();
                gameUIController.sdkController = instance;
                gameUIController.callAds += ProvideAdsToEvent;
            }
        }
    }
}

/// <summary>
/// для слежения за раундами и таймером в целях вычисления условия когда можно показать рекламу
/// </summary>
public struct AdsRoundManager
{
    private int commonRounds;
    private int currentRound;
    public bool hasNextRound
    {
        get
        {
            return (currentRound > 0) ? true : false;
        }
    }
    private int timer;
    private DateTime startTimer;


    public AdsRoundManager(int commonRounds, int timer)
    {
        this.commonRounds = commonRounds;
        currentRound = 0;
        startTimer = DateTime.Now;
        this.timer = timer;
    }

    public void NextRound()
    {
        if (currentRound > 0)
        {
            currentRound--;
        }
    }
    public int getCurrentRound()
    {
        return currentRound;
    }
    public void newSession()
    {
        currentRound = commonRounds;
        startTimer = DateTime.Now;
    }
    public bool isTimeUp()
    {
        double restTime = ((TimeSpan)(DateTime.Now - startTimer)).TotalSeconds;
        if (restTime > timer)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public void setTimer(int newTimer)
    {
        timer = newTimer;
        startTimer = DateTime.Now;
    }
    public void setTimer()
    {
        startTimer = DateTime.Now;
    }
    public void setRound()
    {
        currentRound = commonRounds;
    }
    public void setRound(int newRound)
    {
        commonRounds = newRound;
        currentRound = newRound;
    }
    public int getRestTime()
    {
        double restTime = ((TimeSpan)(DateTime.Now - startTimer)).TotalSeconds;
        return (int)restTime;
    }
}

[Serializable]
public class QuizEvent : UnityEvent { }


