using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MarketManager : MonoBehaviour
{    
    public StartGame startGame;
    private int Gold;
    void Start()
    {
        PurchaseManager.OnPurchaseNonConsumable += PurchaseManager_OnPurchaseNonConsumable;
        PurchaseManager.OnPurchaseConsumable += PurchaseManager_OnPurchaseConsumable;
        PurchaseManager.OnPurchaseNonConsumable += (x) => Debug.Log("ПОКУПКА PurchaseNonConsumable");
        PurchaseManager.OnPurchaseConsumable += (x) => Debug.Log("ПОКУПКА PurchaseConsumable");
    }

    private void PurchaseManager_OnPurchaseNonConsumable(PurchaseEventArgs args)
    {
        if (!PlayerPrefs.HasKey("Golds"))
            Gold = 250;
        else
            Gold = PlayerPrefs.GetInt("Golds");
        if (PlayerPrefs.HasKey("Timer")) return;
        if (args.purchasedProduct.definition.id == "premium") 
        {
            Debug.Log("покупка сделана");
            print(Gold);
            Gold += 800;
            if (SceneManager.GetActiveScene().name == "Game") QuizManager.instance.Golds = Gold;
            if (SceneManager.GetActiveScene().name == "Start") StartGame.instance.Goldtext.text = Gold.ToString();
            PlayerPrefs.SetInt("Golds", Gold);
            SDKController.instance.setTimer(99999);
            PlayerPrefs.SetInt("Timer", 99999);
            Debug.Log("покупка Окончательно Сделанооооо");
        }
    }

    private void PurchaseManager_OnPurchaseConsumable(PurchaseEventArgs args)
    {
        if (!PlayerPrefs.HasKey("Golds"))
            Gold = 250;
        else
            Gold = PlayerPrefs.GetInt("Golds");
        if (args.purchasedProduct.definition.id == "money120")
        {
            Gold += 1000;
        }
        else if (args.purchasedProduct.definition.id == "money250")
        {
            Gold += 2000;
        }
        else if (args.purchasedProduct.definition.id == "money350")
        {
            Gold += 4000;
        }
        else if (args.purchasedProduct.definition.id == "money800")
        {
            Gold += 10000;
        }
        if (SceneManager.GetActiveScene().name == "Game") QuizManager.instance.Golds = Gold;
        if (SceneManager.GetActiveScene().name == "Start") StartGame.instance.Goldtext.text = Gold.ToString();
        PlayerPrefs.SetInt("Golds", Gold);
        Debug.Log("покупка шикос" + args.purchasedProduct.definition.id);
    }

   
    
}
