using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarketPanel : MonoBehaviour
{
    public void OnEndAnimation(float animEvent)
    {
        this.gameObject.SetActive(false);
    }
}
