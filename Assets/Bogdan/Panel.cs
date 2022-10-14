using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Panel : MonoBehaviour
{
    private void OnEnable()
    {
        GetComponent<Animation>().Play("OpenQuestionPanel");
    }
    public void OnEndAnimation(float theValue)
    {
        this.gameObject.SetActive(false);
    }
    public void ActiveOff()
    {
        GetComponent<Animation>().Play("CloseQuestionPanel");
    }
}
