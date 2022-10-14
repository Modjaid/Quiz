using NotificationSamples;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Notifical : MonoBehaviour
{
    private int Delay = 15;
    // Start is called before the first frame update
    void Start()
    {
        Notyfier.sendMessageToPush("Алга", "играть", Delay);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
