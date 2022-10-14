using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class SaveWords : MonoBehaviour
{
    public QuizManager manager;
    WordIndex wordIndex = new WordIndex();
    private int CountWordsRU;
    private int CountWordsENG;
    void Start()
    {
        if (SaveIndexWords.LoadIndex() != null)
        {
            wordIndex = SaveIndexWords.LoadIndex();
        }
        CountWordsRU = 0;
        CountWordsENG = 0;
        if (PlayerPrefs.HasKey("CountWords"))
        {
            StartCoroutine(openWord());
        }
        else return;
    }
    IEnumerator openWord()
    {
        yield return new WaitForSeconds(0.2f);
        
        if (!Localization.Eng)
        {
            if (PlayerPrefs.HasKey("CountWords"))
            {
                for (int i = 0; i < PlayerPrefs.GetInt("CountWords"); i++)
                {
                    manager.OpensWords(wordIndex.IndexWord[i]);
                }
            }
        }
        else
        {
            if (PlayerPrefs.HasKey("CountWordsENG"))
            {
                for (int i = 0; i < PlayerPrefs.GetInt("CountWordsENG"); i++)
                {
                    manager.OpensWords(wordIndex.IndexWordENG[i]);
                }
            }
        }
    }
    
    public void countWords()
    {
        // вызывается в кнопке открытий букв
        if (!Localization.Eng)
        {
            CountWordsRU++;
            PlayerPrefs.SetInt("CountWords", CountWordsRU);
        }
        else
        {
            CountWordsENG++;
            PlayerPrefs.SetInt("CountWordsENG", CountWordsENG);
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
[System.Serializable]
public class WordIndex
{
    public List<int> IndexWord;
    public List<int> IndexWordENG;
    public WordIndex()
    {
        IndexWord = new List<int>();
        IndexWordENG = new List<int>();
    }
}

public class SaveIndexWords
{
    public static void SaveIndex(WordIndex wordIndex)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/Words.txt";

        using (FileStream stream = new FileStream(path, FileMode.Create))
        {
            formatter.Serialize(stream, wordIndex);
        }
    }

    public static WordIndex LoadIndex()
    {
        string path = Application.persistentDataPath + "/Words.txt";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            using (FileStream stream = new FileStream(path, FileMode.Open))
            {
                WordIndex data = formatter.Deserialize(stream) as WordIndex;

                return data;
            }
        }
        else
        {
            Debug.Log("Ошибка нету такого файла" + path);
            return null;
        }
    }
}
