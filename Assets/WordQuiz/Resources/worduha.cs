using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class worduha : MonoBehaviour
{
    // чтоб перебирать знакомые слова
    public string wordas;
    [SerializeField] private QuizDataScriptable questionDataScriptable;
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < questionDataScriptable.questions.Count; i++)
        {
            //  Debug.Log(questionDataScriptable.questions[i].answer);
            if (wordas == questionDataScriptable.questions[i].answer)
            {
                Debug.Log("Есть совпадение");
                break;
            }
            else
                Debug.Log("Не совпало");
            
            //Debug.Log("Есть совпадение");
        }
        //  Debug.Log(questionDataScriptable.questions[0].answer);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
