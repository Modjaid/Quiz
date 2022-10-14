using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;



public class StartGame : MonoBehaviour
{
    public static StartGame instance; //Instance to make is available in other scripts without reference

    private IEnumerator startGameRoutine;
    public int Golds;
    private int currentQuestionIndex;
    public Text Lvltext;
    public Text Goldtext;
    public GameObject panelSetting;
    public GameObject panelMarket;
    private bool song;
    public bool[] setting;

    [SerializeField] private GameObject songButton;
    [SerializeField] private GameObject notifyButton;
    [SerializeField] private GameObject dataButton;

    DataSetting data = new DataSetting();
    private bool datas;
    private bool push;

    // Start is called before the first frame update
    private void Awake()
    {
        setting = new bool[3];
        if (instance == null)
            instance = this;
        else
            Destroy(this.gameObject);
    }
    void Start()
    {
        if (SaveSystem.LoadData() != null)
        {
            data = SaveSystem.LoadData();
        }

        if (!PlayerPrefs.HasKey("Golds"))
        {
            Golds = 250;
        }
        else
        {
            if (PlayerPrefs.GetString("true")=="false")
                currentQuestionIndex = PlayerPrefs.GetInt("PlayerLvlRU");
            else if (PlayerPrefs.GetString("true") == "true")
                currentQuestionIndex = PlayerPrefs.GetInt("PlayerLvlENG");
            Golds = PlayerPrefs.GetInt("Golds");
        }

        Lvltext.text = currentQuestionIndex.ToString();
        Goldtext.text = Golds.ToString();
        panelSetting.SetActive(false);

        song = data.setting[0];
        datas = data.setting[1];
        push = data.setting[2];
        Song(song);
        PlayerData(datas);
        Notification(push);
        panelSetting.SetActive(false);

    }

    // Update is called once per frame
    public void startGame()
    {
        StartCoroutine(StartGameRoutine());
    }

    public IEnumerator StartGameRoutine()
    {
        yield return new WaitForSeconds(0.4f);
        SceneManager.LoadScene("Game");
    }

    public void panelOffOn(bool activ)
    {
        panelSetting.SetActive(activ);
    }
    public void Market(bool activ)
    {
        panelMarket.SetActive(activ);
    }

    public void Song()
    {
        song = !song;
        Song(song);
        data.setting[0] = song;
        SaveSystem.SaveSetting(data);
    }

    public void Song(bool value)
    {
        GameObject button = songButton;
        if (value)
        {
            AudioListener.volume = 0;
            button.transform.GetChild(1).gameObject.SetActive(false);
            button.transform.GetChild(2).gameObject.SetActive(true);
            Vector2 EndPos = new Vector2(310, -17.1f);
            button.transform.GetChild(3).localPosition = Vector3.Lerp(transform.localPosition, EndPos, 5);
        }
        else
        {
            AudioListener.volume = 1;
            button.transform.GetChild(1).gameObject.SetActive(true);
            button.transform.GetChild(2).gameObject.SetActive(false);
            Vector2 EndPos = new Vector2(367, -17.1f);
            button.transform.GetChild(3).localPosition = Vector3.Lerp(transform.localPosition, EndPos, 5);
        }
    }

    public void Notification() // пуш уведомление
    {
        push = !push;
        Notification(push);
        data.setting[2] = push;
        SaveSystem.SaveSetting(data);
    }

    public void Notification(bool value) // пуш уведомление
    {
        GameObject button = notifyButton;
        if (value)
        {
            button.transform.GetChild(1).gameObject.SetActive(false);
            button.transform.GetChild(2).gameObject.SetActive(true);
            Vector2 EndPos = new Vector2(310, -1);
            button.transform.GetChild(3).localPosition = Vector3.Lerp(transform.localPosition, EndPos, 5);
        }
        else
        {
            button.transform.GetChild(1).gameObject.SetActive(true);
            button.transform.GetChild(2).gameObject.SetActive(false);
            Vector2 EndPos = new Vector2(367, -1);
            button.transform.GetChild(3).localPosition = Vector3.Lerp(transform.localPosition, EndPos, 5);
        }
    }
    public void problem()
    {

    }
    //данные игрока
    public void PlayerData()
    {
        datas = !datas;
        PlayerData(datas);
        data.setting[1] = datas;
        SaveSystem.SaveSetting(data);
    }
    private void PlayerData(bool value)
    {
        GameObject button = dataButton;
        if (value)
        {
            button.transform.GetChild(1).gameObject.SetActive(false);
            button.transform.GetChild(2).gameObject.SetActive(true);
            Vector2 EndPos = new Vector2(310, 27);
            button.transform.GetChild(3).localPosition = Vector3.Lerp(transform.localPosition, EndPos, 5);
        }
        else
        {
            button.transform.GetChild(1).gameObject.SetActive(true);
            button.transform.GetChild(2).gameObject.SetActive(false);
            Vector2 EndPos = new Vector2(367, 27);
            button.transform.GetChild(3).localPosition = Vector3.Lerp(transform.localPosition, EndPos, 5);
        }
    }

    public void OpenPrivacyPolicy()
    {
        Application.OpenURL("https://quiz-4-photo-offlin.flycricket.io/privacy.html");
    }

    public void OpenProblemEmail()
    {
        string email = "info@deltagames.ru";
        string subject = MyEscapeURL("Problem in Quiz4in1");
        string body = MyEscapeURL("");
        Application.OpenURL("mailto:" + email + "?subject=" + subject + "&body=" + body);
    }

    string MyEscapeURL(string url)
    {
        return WWW.EscapeURL(url).Replace("+", "%20");
    }

    public void PrivatePolise()
    {

    }
    public void Compani()
    {

    }
}
