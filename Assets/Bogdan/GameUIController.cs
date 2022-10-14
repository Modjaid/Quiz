using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Класс хранит весь функционал под дополнительные UI окна, также является наблюдателем за раундами
/// startSecondMode левел с которого нужно активировать режим SecondMode который начинает предлагать купить премиум акаунт
/// </summary>
public class GameUIController : MonoBehaviour
{
    public event Action callAds;
    public static GameUIController Instance;

    [Header("Дополнительные окна")]
    public GameObject TestBanner_Panel;
    public GameObject TestRewardBanner_Panel;
    public GameObject BuyVip_Panel;
    public GameObject BuyWord_Panel;
    public GameObject NoMoney_Panel;
    public GameObject ShowRewardVideo_Panel;
    public GameObject Market_PanelGame;
    public GameObject Reward_Panel;
    public GameObject Share_Panel;
    public GameObject Rate_Panel;

    [Header("Кнопки в FinishPanel")]
    public GameObject ADSButton;
    public GameObject NextButton;
    public RectTransform finishNextButton;

    [HideInInspector] public SDKController sdkController;
    private QuizManager quizManager;

    private bool mayInvokeBuyWordPanel; // Вызвать окно убеждения у пользователя уверен ли он что хочет потратить 50 руб за подсказку
    private int clickBuyWordCount; // считать сколько раз игрок потратил подсказок (если на 3 подсказку не хватает предложить посмотреть рекламу за вознаграждение)

    [Header("С какого левела начинать предлагать покупку премиума?")]
    [SerializeField] private int startSecondMode;// Режим запускающий предложения купить премиум
    private bool SecondMode// Режим запускающий предложения купить премиум
    {
        get
        {
            if (PlayerPrefs.GetInt("PlayerLvl") > startSecondMode)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
    [HideInInspector] public bool mayShowVipPanel = false;

    public void Awake()
    {
        Instance = this;
    }
    public void Start()
    {
        sdkController.Message = "Предложения покупки премиума действуют с " + startSecondMode + " левела";
        quizManager = GetComponent<QuizManager>();
        mayInvokeBuyWordPanel = true;
        clickBuyWordCount = 0;
        sdkController.OnFinishRewardAd.AddListener(() => Invoke("Open_rewardPanel", 1f));     
    }
    #region Open Windows
    public void Open_BuyVipPanel()
    {
        if (PlayerPrefs.HasKey("Timer")) return; // чтоб не открывалась если вип купили
        if (SecondMode && mayShowVipPanel)
        {
            foreach (int a in QuizManager.instance.levelsRatePanelIsShownOn)
            {
                if (QuizManager.instance.currentQuestionIndex == a)
                {
                    return;
                }
            }

            foreach (int a in QuizManager.instance.levelsSharePanelIsShownOn)
            {
                if (QuizManager.instance.currentQuestionIndex == a)
                {
                    return;
                }
            }

            BuyVip_Panel.SetActive(true);
            mayShowVipPanel = false;
        }
    }
    public void Open_BuyWordPanel()
    {
        if (quizManager.Golds >= 50 && mayInvokeBuyWordPanel)
        {
            BuyWord_Panel.SetActive(true);
            mayInvokeBuyWordPanel = false;
        }
        else
        {
            Open_NoMoneyPanel();
            quizManager.openWord();
        }
    }
    public void Open_rewardPanel()
    {
        Reward_Panel.SetActive(true);
        sdkController.getRewardBonuses();
    }
    public void Open_ShowRewardVideoPanel()
    {
        if (clickBuyWordCount > 3 && quizManager.Golds < 50)
        {
            ShowRewardVideo_Panel.SetActive(true);
        }
    }
    public void Open_SharePanel()
    {// закрыли пока не знаем как решить эту проблему Саргис так просил 
       // Share_Panel.SetActive(true);
    }
    public void Open_RatePanel()
    {
        Rate_Panel.SetActive(true);
    }
    private void Open_NoMoneyPanel()
    {
        if (clickBuyWordCount <= 3 && quizManager.Golds < 50)
        {
            NoMoney_Panel.SetActive(true);
        }
    }
    public void Open_MarketPanelGame()
    {
        Market_PanelGame.SetActive(true);
        Market_PanelGame.GetComponent<Animation>().Play("OpenMarket");
    }
    #endregion
    #region Close Windows
   
    
    
    public void Close_BuyVipPanel()
    {
        BuyVip_Panel.GetComponent<Animation>().Play("CloseQuestionPanel");
    }
    public void Close_BuyWordPanel()
    {
        BuyWord_Panel.GetComponent<Animation>().Play("CloseQuestionPanel");
    }
    public void Close_NoMoneyPanel()
    {
        NoMoney_Panel.GetComponent<Animation>().Play("CloseQuestionPanel");
    }
    public void Close_ShowRewardVideoPanel()
    {
        ShowRewardVideo_Panel.GetComponent<Animation>().Play("CloseQuestionPanel");
    }
    public void Close_MarketPanelGame()
    {
        Market_PanelGame.GetComponent<Animation>().Play("CloseMarket");
    }
    public void Close_TestBanerPanel()
    {
        sdkController.OnFinishInterAd?.Invoke();
        TestBanner_Panel.SetActive(false);
    }
    public void Close_SharePanel()
    {
        Share_Panel.GetComponent<Animation>().Play("CloseQuestionPanel");
    }
    public void Close_RatePanel()
    {
        Application.OpenURL("https://play.google.com/store/apps/details?id=com.DeltaVisionGames.Quiz4photo");
        Rate_Panel.GetComponent<Animation>().Play("CloseQuestionPanel");        
    }
    public void Close_TestRewardBanerPanel()
    {
        // IMPORTANT
        finishNextButton.anchoredPosition = new Vector2(0, -249);
        TestRewardBanner_Panel.SetActive(false);
      //  Reward_Panel.SetActive(true); // закоментил
    }
    public void Close_RewardPanel()
    {
        NextButton.GetComponent<Animation>().Play("indicateButton");
        Reward_Panel.GetComponent<Animation>().Play("CloseQuestionPanel");
        Close_ADSButton();
    }
    public void Close_ADSButton()
    {
        ADSButton.SetActive(false);
    }
    #endregion

    public void Thanks()
    {
        QuizManager.instance.ShowFinishTextCounterEffect(10);
    }
    public void ClickBuyWordCounter()
    {
        if (quizManager.Golds >= 50)
        {
            clickBuyWordCount++;
        }
    }

    public void ClickYes_OpenWord()
    {
        quizManager.openWord();
        Close_BuyWordPanel();
        clickBuyWordCount++;
    }

    /// <summary>
    /// Событие на кнопке Х2
    /// Событие на Yes в окне ShowRewardVideoPanel
    /// </summary>
    public void ClickYes_ShowRewardVideo()
    {
        sdkController.ShowRewarded();
        Close_TestRewardBanerPanel(); // вернул его обратно
        sdkController.adsManager.setRound();
        clickBuyWordCount = 0;
        sdkController.Message = "Обновился счетчик раундов до обычной рекламы: " + sdkController.adsManager.getCurrentRound() + " раундов осталось";
    }
    public void CallAds()
    {
        callAds?.Invoke();
    }
    public void Return_AdsButton()
    {
        ADSButton.SetActive(true);
    }
}
