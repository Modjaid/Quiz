using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class LocalizMarket : MonoBehaviour
{
    public bool Eng { get; private set; }
    public localMarket datas;
    public Text GoldText;
    public Image image;

    public Text premim;
    public Text premimButton;
    public Text button120;
    public Text button250;
    public Text button350;
    public Text button800;
    void Start()
    {
        UpdateGolds();

        QuizManager.instance.OnGoldsChanged += UpdateGolds;
    }
    private void Update()
    {
        if (!PlayerPrefs.HasKey("true"))
        {
            if (Application.systemLanguage == SystemLanguage.Russian)
                Eng = false;
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

    private void UpdateGolds()
    {
        if (QuizManager.instance != null)
        {
            GoldText.text = QuizManager.instance.Golds.ToString();
        }
        else
        {
            datas = new localMarket();
            if (!PlayerPrefs.HasKey("Golds"))
            {
                GoldText.text = 250.ToString();
                PlayerPrefs.SetInt("Golds", 250);
            }
            else
            {
                GoldText.text = PlayerPrefs.GetInt("Golds").ToString();
            }
        }
    }

    public void LoadTrans()
    {
        image.sprite = Resources.Load<Sprite>("pr");
        PlayerPrefs.SetString("true", "true");
        TextAsset file = Resources.Load("Data") as TextAsset;
        string path = file.ToString();
        if (!File.Exists(path))
        {
            datas = JsonUtility.FromJson<localMarket>(path);

            premim.text = datas.premim;
            premimButton.text = datas.premimButton;
            button120.text = datas.button120;
            button250.text = datas.button250;
            button350.text = datas.button350;
            button800.text = datas.button800;
        }

    }


}
public class localMarket
{
    public string premim;
    public string premimButton;
    public string button120;
    public string button250;
    public string button350;
    public string button800;
}
