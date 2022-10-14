using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartLocaliz : MonoBehaviour
{
    public Text StartButtonText;
    public Text SettingButtonText;
    public GameObject panelSetting;
    
    #region
    public Text Setting;
    public Text Song;
    public Text Notification;
    public Text Problem;
    public Text PlayerData;
    public Text PrivatePolis;
    public Text Company;
    #endregion

    public SettingWords datas;
    private bool Eng;
    private void Awake()
    {
        datas = new SettingWords();
        if (!PlayerPrefs.HasKey("true"))
        {
            if (Application.systemLanguage == SystemLanguage.Russian)
            {
                Eng = false;
                PlayerPrefs.SetString("true", "false");
            }

            else
            {
                Eng = true;
                LoadTrans();
            }
        }
        else
        {
            if (PlayerPrefs.GetString("true") == "true")
            {
                Eng = true;
                LoadTrans();
            }
            else
                return;
        }
    }
   

    
    // просто сцену перезагрузит чтоб сделать все по русский
    public void RU()
    {
        SceneManager.LoadScene("Start");
        PlayerPrefs.SetString("true", "false");
    }
    // Update is called once per frame
    public void LoadTrans()
    {
        PlayerPrefs.SetString("true", "true");
       // string path = File.ReadAllText(Application.dataPath + "PlayerData.json");
        TextAsset file = Resources.Load("Data") as TextAsset;
        string path = file.ToString();
        if (!File.Exists(path))
        {
            datas = JsonUtility.FromJson<SettingWords>(path);

            StartButtonText.text = datas.StartButtonText;
            SettingButtonText.text = datas.SettingButtonText;
            

            Setting.text = datas.Setting;
            Song.text = datas.Song;
            Notification.text = datas.Notification;
            Problem.text = datas.Problem;
            PlayerData.text = datas.PlayerData;
            PrivatePolis.text = datas.PrivatePolis;
            Company.text = datas.Company;
        }
        panelSetting.SetActive(false);
    }

    public void reboot()
    {
        SceneManager.LoadScene("Start");
    }//перезагрузка сцены для англииского языка

    [System.Serializable]
    public class SettingWords
    {
        public string StartButtonText;
        public string SettingButtonText;
        public string Lvl;

        public string Setting;
        public string Song;
        public string Notification;
        public string Problem;
        public string PlayerData;
        public string PrivatePolis;
        public string Company;

    }
}
