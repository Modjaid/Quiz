using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class WordData : MonoBehaviour
{
    [SerializeField] private Text wordText;
    public bool untouchable;
    private void Start()
    {
       
    }
    public void Untouch(bool untouch)
    {        
        untouchable = untouch;
    }
    [HideInInspector]
    public char wordValue;

    private Button buttonComponent;

    private void Awake()
    {
        if (transform.tag == "Answer") return;
        buttonComponent = GetComponent<Button>();
        if (buttonComponent)
        {
            buttonComponent.onClick.AddListener(() => WordSelected(false,0));
            //print("gg");
        }
    }

    public void SetWord(char value)
    {
        wordText.text = value + "";
        wordValue = value;
    }

    public void WordSelected(bool untouch , int index)
    {
        QuizManager.instance.SelectedOption(this,untouch,index);

    }

    public void SelfReset()
    {
        if (untouchable) return;
        if (wordValue == '_') return;
        QuizManager.instance.resetWord(wordValue);
        wordValue = '_';
        wordText.text = wordValue.ToString();
        print(untouchable);
        QuizManager.instance.WordRemovedFromAnswer(this.gameObject);
    }
    
}
