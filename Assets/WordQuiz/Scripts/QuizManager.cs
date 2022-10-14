using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class QuizManager : MonoBehaviour
{

    public static QuizManager instance; //Instance to make is available in other scripts without reference
    [SerializeField] private GameObject gameComplete;
    //Scriptable data which store our questions data
    [SerializeField] private QuizDataScriptable questionDataScriptable;
    [SerializeField] private Image[] questionImage;           //image element to show the image
    [SerializeField] private WordData[] answerWordList;     //list of answers word in the game
    [SerializeField] private WordData[] optionsWordList;    //list of options word in the game

    private IEnumerator[] answerWordEffects;
    private IEnumerator[] optionsWordEffects;
    private IEnumerator moneyCounterEffect;
    private Vector3 answerWordLocalScale;
    private Vector3 optionWordLocalScale;
    private Vector3 nextButtonInitialPosition;

    private GameStatus gameStatus = GameStatus.Playing;     //to keep track of game status
    private char[] wordsArray = new char[12];               //array which store char of each options
    public List<char> Cyrillic = new List<char>();

    private List<int> selectedWordsIndex;                   //list which keep track of option word index w.r.t answer word index
    public int currentAnswerIndex = 0, currentQuestionIndex = 0;   //index to keep track of current answer and current question
    private bool correctAnswer = true;                      //bool to decide if answer is correct or not
    private string answerWord;                              //string to store answer of current question
    public GameObject panelMarket;
    [SerializeField]
    private int golds;

    public delegate void onGoldsChange();
    public event onGoldsChange OnGoldsChanged;

    public int goldsAddedPerRound;
    public TextMeshProUGUI GoldText;
    WordIndex wordIndex = new WordIndex();
    public int Golds
    {
        get
        {
            return golds;
        }
        set
        {
            golds = value;
            GoldText.text = Golds.ToString();
            OnGoldsChanged?.Invoke();
        }
    }
    public GameObject FinishPanel;
    public Text finisText;
    public Text LvlPlayer;
    // Word used for movement effect.
    public GameObject dummyWordPrefab;
    // List of dummy word prefabs.
    public List<GameObject> wordPrefabs;
    // Parent of dummy words
    public Transform dummyWordsContainer;
    public Text goldPopupText;
    public Animator goldPopupAnimator;
    public RectTransform nextButton;

    public List<int> levelsRatePanelIsShownOn;
    public List<int> levelsSharePanelIsShownOn;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this.gameObject);
    }
    

    // Start is called before the first frame update
    void Start()
    {        
        if (SaveIndexWords.LoadIndex() != null)
        {
            wordIndex = SaveIndexWords.LoadIndex();
        }
        for (int i = 1040; i < 1072; i++)
        {
            Cyrillic.Add((char)i);
            //добавляем Ё
            if (i == 1045)
                Cyrillic.Add((char)1025);
        }
        currentQuestionIndex = 0;
        if (!PlayerPrefs.HasKey("Golds")) // если нет голды то все начнут с нулевого уровня 
            Golds = 250;
        else
        {
            if (!Localization.Eng)
            {              
                if (PlayerPrefs.HasKey("PlayerLvlRU")) 
                    currentQuestionIndex = PlayerPrefs.GetInt("PlayerLvlRU");  // сохранение уровней на разных языках
            }
            else
            {
                if (PlayerPrefs.HasKey("PlayerLvlENG")) 
                    currentQuestionIndex = PlayerPrefs.GetInt("PlayerLvlENG");
            }

            print("1222");
            Golds = PlayerPrefs.GetInt("Golds");
        }
        selectedWordsIndex = new List<int>();           //create a new list at start
        SetQuestion();                                  //set question
        FinishPanel.SetActive(false);
        

        optionsWordEffects = new IEnumerator[optionsWordList.Length];
        answerWordEffects = new IEnumerator[answerWordList.Length];

        answerWordLocalScale = answerWordList[0].transform.localScale;
        optionWordLocalScale = optionsWordList[0].transform.localScale;

        nextButtonInitialPosition = nextButton.anchoredPosition;

        SDKController.instance.OnFinishInterAd.AddListener(OnInterAdShown);
    }
    private void Update()
    {
        if (Input.GetKeyDown("l"))
        {
            Finish();
        }
    }
    void SetQuestion()
    {
        gameStatus = GameStatus.Playing;                //set GameStatus to playing 

        //set the answerWord string variable
        // тут локализация
        if (!Localization.Eng)
            answerWord = questionDataScriptable.questions[currentQuestionIndex].answer;
        else
            answerWord = questionDataScriptable.questions[currentQuestionIndex].English;
        //set the image of question
        questionImage[0].sprite = questionDataScriptable.questions[currentQuestionIndex].questionImage[0];
        questionImage[1].sprite = questionDataScriptable.questions[currentQuestionIndex].questionImage[1];
        questionImage[2].sprite = questionDataScriptable.questions[currentQuestionIndex].questionImage[2];
        questionImage[3].sprite = questionDataScriptable.questions[currentQuestionIndex].questionImage[3];

        ResetQuestion();                               //reset the answers and options value to orignal     

        selectedWordsIndex.Clear();                     //clear the list for new question
        Array.Clear(wordsArray, 0, wordsArray.Length);  //clear the array

        //add the correct char to the wordsArray
        for (int i = 0; i < answerWord.Length; i++)
        {
            wordsArray[i] = char.ToUpper(answerWord[i]);
            // print(answerWord[i].ToString());
        }

        //add the dummy char to wordsArray
        for (int j = answerWord.Length; j < wordsArray.Length; j++)
        {
            if (!Localization.Eng)
                wordsArray[j] = Cyrillic[UnityEngine.Random.Range(1, Cyrillic.Count)];
            else
                wordsArray[j] = (char)UnityEngine.Random.Range(65, 90);
        }

        wordsArray = ShuffleList.ShuffleListItems<char>(wordsArray.ToList()).ToArray(); //Randomly Shuffle the words array

        //set the options words Text value
        for (int k = 0; k < optionsWordList.Length; k++)
        {
            optionsWordList[k].SetWord(wordsArray[k]);
            // print(wordsArray[k].ToString());
        }

    }

    //Method called on Reset Button click and on new question
    public void ResetQuestion()
    {
        //activate all the answerWordList gameobject and set their word to "_"
        for (int i = 0; i < answerWordList.Length; i++)
        {
            answerWordList[i].gameObject.SetActive(true);
            answerWordList[i].SetWord('_');
            answerWordList[i].Untouch(false);
        }

        //Now deactivate the unwanted answerWordList gameobject (object more than answer string length)
        for (int i = answerWord.Length; i < answerWordList.Length; i++)
        {
            answerWordList[i].gameObject.SetActive(false);
        }

        //activate all the optionsWordList objects
        for (int i = 0; i < optionsWordList.Length; i++)
        {
            optionsWordList[i].gameObject.SetActive(true);
        }

        currentAnswerIndex = 0;
    }

    /// <summary>
    /// When we click on any options button this method is called
    /// </summary>
    /// <param name="value"></param>
    public void SelectedOption(WordData value, bool untouch , int index)
    {
        //if gameStatus is next or currentAnswerIndex is more or equal to answerWord length
        if (gameStatus == GameStatus.Next || currentAnswerIndex >= answerWord.Length) return;
        //print(currentQuestionIndex);
        selectedWordsIndex.Add(value.transform.GetSiblingIndex()); //add the child index to selectedWordsIndex list
        value.gameObject.SetActive(false); //deactivate options object

        AudioManager.Instance.PlaySound(Sound.word_button);

        currentAnswerIndex++;   //increase currentAnswerIndex

        for (int i = 0; i < answerWord.Length; i++)
        {
            if (answerWordList[i].wordValue == '_' && !untouch)
            {
                answerWordList[i].SetWord(value.wordValue); //set the answer word list\
                answerWordList[i].Untouch(untouch);

                CreateDummyWord(value.wordValue, selectedWordsIndex[selectedWordsIndex.Count - 1], i);

                break;
            }
            // чтоб открывать в старте сразу и чтоб их нельзя было удалить
            if (untouch)
            {
                if (index == i)
                {
                    answerWordList[i].SetWord(value.wordValue); //set the answer word list\
                    answerWordList[i].Untouch(untouch);

                    CreateDummyWord(value.wordValue, selectedWordsIndex[selectedWordsIndex.Count - 1], i);
                }
            }
        }

        //if currentAnswerIndex is equal to answerWord length
        if (currentAnswerIndex == answerWord.Length)
        {
            correctAnswer = true;   //default value
            //loop through answerWordList
            for (int i = 0; i < answerWord.Length; i++)
            {
                //if answerWord[i] is not same as answerWordList[i].wordValue
                if (char.ToUpper(answerWord[i]) != char.ToUpper(answerWordList[i].wordValue))
                {
                    correctAnswer = false; //set it false
                    break; //and break from the loop
                }
            }

            //if correctAnswer is true
            if (correctAnswer)
            {
                currentQuestionIndex++; //increase currentQuestionIndex    
                if(!Localization.Eng)
                    PlayerPrefs.SetInt("PlayerLvlRU", currentQuestionIndex); // сохранение уровней на разных языках
                else if (Localization.Eng)
                    PlayerPrefs.SetInt("PlayerLvlENG", currentQuestionIndex);
                Debug.Log("Correct Answer");
                if (currentQuestionIndex >= questionDataScriptable.questions.Count)
                {
                    Debug.Log("Game Complete"); //else game is complete
                    gameComplete.SetActive(true);
                }
                else
                {
                    Finish();
                }
            }
            else
            {
                // If answer is wrong...
                // Play error sound,
                AudioManager.Instance.PlaySound(Sound.error);
                // And show error animation on all answer buttons.
                foreach (WordData answerWord in answerWordList)
                {
                    // Self explanatory
                    if (!answerWord.gameObject.active) continue;

                    AnswerError(answerWord.gameObject);
                }
            }
        }
    }
    private void Finish()
    {
        nextButton.localScale = Vector3.one;
        nextButton.anchoredPosition = nextButtonInitialPosition;
        FinishPanel.SetActive(true);
        LvlPlayer.text = currentQuestionIndex.ToString();
        Golds += goldsAddedPerRound;
        SaveGolds();
        gameStatus = GameStatus.Next; //set the game status
        if (currentQuestionIndex < questionDataScriptable.questions.Count) //if currentQuestionIndex is less that total available questions
        {
            Invoke("SetQuestion", 0.1f); //go to next question
        }
        print("Game Finish");
        
       
        if (!Localization.Eng)
        {
            PlayerPrefs.DeleteKey("CountWords");
            if (wordIndex.IndexWord != null)
                wordIndex.IndexWord.Clear();
        }
        if (Localization.Eng)
        {
            PlayerPrefs.DeleteKey("CountWordsENG");
            if (wordIndex.IndexWordENG != null)
                wordIndex.IndexWordENG.Clear();
        }
        SaveIndexWords.SaveIndex(wordIndex);

        AudioManager.Instance.PlaySound(Sound.win);
        ShowFinishTextCounterEffect(goldsAddedPerRound);
    }

    private void OnInterAdShown()
    {
        ShowRateOrSharePanel();
    }

    private void ShowRateOrSharePanel()
    {
        foreach (int a in levelsRatePanelIsShownOn)
        {
            if (currentQuestionIndex == a)
            {
                GameUIController.Instance.Open_RatePanel();
            }
        }

        foreach (int a in levelsSharePanelIsShownOn)
        {
            if (currentQuestionIndex == a)
            {
                GameUIController.Instance.Open_SharePanel();
            }
        }
    }

    public void ShowFinishTextCounterEffect(int added)
    {
        if (moneyCounterEffect != null)
            StopCoroutine(moneyCounterEffect);
        moneyCounterEffect = TextCounterEffect(Golds - added, Golds, finisText, added);
        StartCoroutine(moneyCounterEffect);
    }

    private IEnumerator TextCounterEffect(int value, int targetValue, Text text, int added)
    {
        finisText.text = value.ToString();
        finisText.gameObject.SetActive(false);
        goldPopupText.gameObject.SetActive(false);

        yield return new WaitForSeconds(0.2f);

        goldPopupText.text = $"+{added}";
        goldPopupText.gameObject.SetActive(true);
        goldPopupAnimator.Play("CounterAddedCountText");

        yield return new WaitForSecondsRealtime(1.35f);

        finisText.gameObject.SetActive(true);

        int iterNumber = targetValue - value;
        float temp = 0.03f;
        float nextTimeToCount = 0f;
        int i = 0;

        while (true)
        {
            if (i <= iterNumber && Time.time >= nextTimeToCount)
            {
                float delay = temp * 1.7f;
                delay = Mathf.Clamp(delay, 0, 0.5f);
                finisText.text = $"{value + i}";
                i++;

                nextTimeToCount = Time.time + delay;
            }

            yield return null;
        }
    }

    /*
    // нужна чтоб удалять буквы с конца
    public void ResetLastWord()
    {
        if (selectedWordsIndex.Count > 0)
        {
            int index = selectedWordsIndex[selectedWordsIndex.Count - 1];
            optionsWordList[index].gameObject.SetActive(true);
            selectedWordsIndex.RemoveAt(selectedWordsIndex.Count - 1);

            currentAnswerIndex--;
            answerWordList[currentAnswerIndex].SetWord('_');
        }
    }
    */

    // нужна чтоб удалть буквы выборочно 
    public void resetWord(char index)
    {
        currentAnswerIndex--;
        //StartCoroutine(RemoveWordFromAnswerEffect(currentAnswerIndex));

        for (int i = 0; i < optionsWordList.Length; i++)
        {
            if (optionsWordList[i].wordValue == index)
            {
                if (!optionsWordList[i].gameObject.activeSelf)
                {
                    optionsWordList[i].gameObject.SetActive(true);
                    break;
                }

            }
        }

        AudioManager.Instance.PlaySound(Sound.word_button);
    }

    // открывает буквы 
    public void openWord()
    {
        if (gameStatus == GameStatus.Next) return;
        if (Golds < 50) return;
        var currentIndex = 0;
        for (int i = 0; i < answerWordList.Length; i++)
        {
            if (answerWordList[i].wordValue == '_')
            {
                currentIndex = i;
                if (!Localization.Eng)
                    wordIndex.IndexWord.Add(i);// тут если русский
                else
                    wordIndex.IndexWordENG.Add(i);// тут если английский
                SaveIndexWords.SaveIndex(wordIndex);
                break;
            }
        }
        for (int i = 0; i < optionsWordList.Length; i++)
        {
            if (optionsWordList[i].enabled)
            {
                if (char.ToUpper(answerWord[currentIndex]) == optionsWordList[i].wordValue)
                {
                    optionsWordList[i].WordSelected(true, currentIndex);
                    break;
                }

            }
        }
        Golds -= 50;
        SaveGolds();//сохранение голды
    }
    // этот котрывает в старте уже открытые сохраненые буквы
    public void OpensWords(int index)
    {
        var currentIndex = index;
        //print(index);
        
        for (int i = 0; i < optionsWordList.Length; i++)
        {
            if (optionsWordList[i].enabled)
            {
                if (char.ToUpper(answerWord[currentIndex]) == optionsWordList[i].wordValue)
                {
                    optionsWordList[i].WordSelected(true,index);
                    break;
                }

            }
        }
    }

    public void next()
    {
        FinishPanel.SetActive(false);

        if(!SDKController.instance.AdWillBeShown())
        {
            ShowRateOrSharePanel();
        }
    }

    public void GoMenu()
    {
        SceneManager.LoadScene("Start");
    }
    public void Market(bool activ)
    {
        panelMarket.SetActive(activ);
    }

    public void SaveGolds()
    {
        PlayerPrefs.SetInt("Golds", Golds);
    }



    private void CreateDummyWord(char word, int optionIndex, int answerIndex)
    {
        Vector3 spawnPosition = optionsWordList[optionIndex].transform.position;

        var dummyWord = Instantiate(dummyWordPrefab, spawnPosition, Quaternion.identity, dummyWordsContainer);

        Text text = dummyWord.transform.GetChild(2).GetComponent<Text>();
        text.text = word.ToString();

        optionsWordList[optionIndex].transform.localScale = optionWordLocalScale;
        if (optionsWordEffects[optionIndex] != null)
            StopCoroutine(optionsWordEffects[optionIndex]);

        bool play = currentAnswerIndex != answerWord.Length;
        optionsWordEffects[optionIndex] = DummyWordEffect(dummyWord, answerWordList[answerIndex].transform.position, answerIndex, play);

        StartCoroutine(optionsWordEffects[optionIndex]);
    }

    private IEnumerator DummyWordEffect(GameObject dummyWord, Vector3 targetPosition, int answerIndex, bool playWobble)
    {
        RectTransform word = dummyWord.GetComponent<RectTransform>();
        Vector3 initialScale = word.localScale - Vector3.one * 0.1f;
        // How much the button is scaled when clicked on
        float clickSizeMultiplier = 2f;
        bool wobbleEffectPlayed = false;
        Vector3 currentVelocity = Vector3.zero;
        Vector3 scaleVelocity = Vector3.zero;

        word.localScale *= clickSizeMultiplier;

        while (true)
        {
            Vector3 newPosition = Vector3.SmoothDamp(word.position, targetPosition, ref currentVelocity, 0.07f);
            float distance = Vector3.Distance(word.position, targetPosition);
            // If button is "arrived" in target position, destroy it and break from coroutine.
            if (distance < 1f)
            {
                Destroy(dummyWord.gameObject);
                yield break;
            }

            if (distance < 100 && !wobbleEffectPlayed && playWobble)
            {
                wobbleEffectPlayed = true;
                AnswerWobble(answerIndex);
            }

            word.position = newPosition;

            word.localScale = Vector3.SmoothDamp(word.localScale, initialScale, ref scaleVelocity, 0.08f);

            yield return null;
        }
    }

    public void WordRemovedFromAnswer(GameObject word)
    {
        int index = 0;

        for (int i = 0; i < answerWordList.Length; i++)
        {
            if (answerWordList[i].gameObject == word)
            {
                index = i;
                break;
            }
        }

        answerWordList[index].transform.rotation = Quaternion.identity;
        answerWordList[index].transform.localScale = answerWordLocalScale;
        if (answerWordEffects[index] != null)
            StopCoroutine(answerWordEffects[index]);
        answerWordEffects[index] = RemoveWordFromAnswerEffect(word);
        StartCoroutine(answerWordEffects[index]);
    }

    private IEnumerator RemoveWordFromAnswerEffect(GameObject word)
    {
        RectTransform wordTransform = word.GetComponent<RectTransform>();
        Vector3 initialScale = wordTransform.localScale;
        float sizeMultiplier = 1.4f;
        Vector3 currentVelocity = Vector3.zero;

        wordTransform.localScale = wordTransform.localScale * sizeMultiplier;

        while (true)
        {
            if (Mathf.Approximately(wordTransform.localScale.x, initialScale.x))
            {
                yield break;
            }

            wordTransform.localScale = Vector3.SmoothDamp(wordTransform.localScale, initialScale, ref currentVelocity, 0.1f);

            yield return null;
        }
    }

    private void AnswerWobble(int index)
    {
        answerWordList[index].transform.rotation = Quaternion.identity;
        answerWordList[index].transform.localScale = answerWordLocalScale;
        if (answerWordEffects[index] != null)
        {
            StopCoroutine(answerWordEffects[index]);
        }
        answerWordEffects[index] = AnswerWobbleEffect(index);
        StartCoroutine(answerWordEffects[index]);
    }

    private IEnumerator AnswerWobbleEffect(int index)
    {
        GameObject word = answerWordList[index].gameObject;
        RectTransform wordTransform = word.GetComponent<RectTransform>();
        Vector3 initialScale = wordTransform.localScale;
        float sizeMultiplier = 1.4f;
        Vector3 currentVelocity = Vector3.zero;

        wordTransform.localScale = wordTransform.localScale * sizeMultiplier;

        while (true)
        {
            if (Mathf.Approximately(wordTransform.localScale.x, initialScale.x))
            {
                yield break;
            }

            wordTransform.localScale = Vector3.SmoothDamp(wordTransform.localScale, initialScale, ref currentVelocity, 0.08f);

            yield return null;
        }
    }

    private void AnswerError(GameObject word)
    {
        int index = 0;

        for (int i = 0; i < answerWordList.Length; i++)
        {
            if (answerWordList[i].gameObject == word)
            {
                index = i;
                break;
            }
        }

        answerWordList[index].transform.localScale = answerWordLocalScale;
        if (answerWordEffects[index] != null)
            StopCoroutine(answerWordEffects[index]);
        answerWordEffects[index] = AnswerErrorffect(word);
        StartCoroutine(answerWordEffects[index]);
    }

    private IEnumerator AnswerErrorffect(GameObject word)
    {
        RectTransform wordTransform = word.GetComponent<RectTransform>();
        Vector3 initialScale = wordTransform.localScale;
        float sizeMultiplier = 1.25f;
        float angle = UnityEngine.Random.Range(-30, 30);
        Vector3 currentVelocity = Vector3.zero;
        float currentAngleVelocity = 0;

        wordTransform.localScale = wordTransform.localScale * sizeMultiplier;
        wordTransform.eulerAngles = new Vector3(0, 0, angle);

        while (true)
        {
            if (Mathf.Approximately(wordTransform.localScale.x, initialScale.x) && Quaternion.Angle(transform.rotation, Quaternion.identity) > 0.2f)
            {
                yield break;
            }

            wordTransform.localScale = Vector3.SmoothDamp(wordTransform.localScale, initialScale, ref currentVelocity, 0.08f);
            float rotationZ = Mathf.SmoothDampAngle(wordTransform.eulerAngles.z, 0, ref currentAngleVelocity, 0.15f);
            wordTransform.eulerAngles = new Vector3(0, 0, rotationZ);
            yield return null;
        }
    }
}

[System.Serializable]
public class QuestionData
{
    public Sprite[] questionImage;
    public string answer;
    public string English;
}

public enum GameStatus
{
    Next,
    Playing
}
