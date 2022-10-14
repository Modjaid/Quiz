using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Localization : MonoBehaviour
{
    public PlayerData datas;
    // Game Scene 
    #region 
    // Finish Panel
    public Text FinishPanelText;
    public Text FinishPanelButtonText;
    // BuyWord_Panel
    public Text BuyWord_PanelText;
    public Text BuyWord_PanelButtonText;
    //ShowRewardVideo_Panel
    public Text ShowRewardVideo_PanelText;
    public Text ShowRewardVideo_PanelButtonText;
    //Reward_Panel
    public Text Reward_PanelText;
    public Text Reward_PanelButtonTex;
    //BuyVip_Panel
    public Text BuyVip_PanelText;
    public Text BuyVip_PanelButtonText;
    //NoMoney_Panel
    public Text NoMoney_PanelText;
    public Text NoMoney_PanelButtonText;
    //GameOverPanel
    public Text GameOverText;
    #endregion

    public static bool Eng;
    private void Awake()
    {
        datas = new PlayerData();
        if (PlayerPrefs.GetString("true") == "true")
        {
            Eng = true;
            Loadtransfer();
        }
        else
            Eng = false;
    }
    void Start()
    {
        
        
    }    
    
    
   
    void Loadtransfer()
    {
        // string path = File.ReadAllText(Application.dataPath + "PlayerData.json");
        TextAsset file = Resources.Load("Data") as TextAsset;
        string path = file.ToString();
        if (!File.Exists(path))
        {
            datas = JsonUtility.FromJson<PlayerData>(path);

            FinishPanelText.text = datas.FinishPanelText;
            FinishPanelButtonText.text = datas.FinishPanelButtonText;

            BuyWord_PanelText.text = datas.BuyWord_PanelText;
            BuyWord_PanelButtonText.text = datas.BuyWord_PanelButtonText;

            ShowRewardVideo_PanelText.text = datas.ShowRewardVideo_PanelText;
            ShowRewardVideo_PanelButtonText.text = datas.ShowRewardVideo_PanelButtonText;

            Reward_PanelText.text = datas.Reward_PanelText;
            Reward_PanelButtonTex.text = datas.Reward_PanelButtonTex;

            BuyVip_PanelText.text = datas.BuyVip_PanelText;
            BuyVip_PanelButtonText.text = datas.BuyVip_PanelButtonText;

            NoMoney_PanelText.text = datas.NoMoney_PanelText;
            NoMoney_PanelButtonText.text = datas.NoMoney_PanelButtonText;

            GameOverText.text = datas.GameOverText;
        }
        else
            print("11");
    }
}


[System.Serializable]
public class PlayerData
{
    public string FinishPanelText;
    public string FinishPanelButtonText;

    public string BuyWord_PanelText;
    public string BuyWord_PanelButtonText;

    public string ShowRewardVideo_PanelText;
    public string ShowRewardVideo_PanelButtonText;

    public string Reward_PanelText;
    public string Reward_PanelButtonTex;

    public string BuyVip_PanelText;
    public string BuyVip_PanelButtonText;

    public string NoMoney_PanelText;
    public string NoMoney_PanelButtonText;

    public string GameOverText;

}

