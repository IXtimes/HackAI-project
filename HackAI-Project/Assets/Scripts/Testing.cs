using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Testing : MonoBehaviour
{
   public float startTime;//predifine in unity
   float internalTimer;
   public TextMeshProUGUI timerText;//referenct text(unit) in Unity
   private AudioSource sfx;//reference 
   bool timerElapsed = false;
   public AudioClip timerEndSFX;//original source
   

   void Awake(){
    sfx = gameObject.GetComponent<AudioSource>();
    internalTimer = startTime;
   }

   //never have loop in here
   void Update(){
    if(internalTimer > 0f){
        internalTimer -= Time.deltaTime; 
        timerText.text = $"Timer: {Mathf.Round(internalTimer*100)/100}";
    }
    else if (!timerElapsed){
        internalTimer = 0f;
        TimerComplete();
        timerElapsed = true;
    }
    
   }

   void TimerComplete(){
    timerText.text = "Time for your mom!!!";
    sfx.PlayOneShot(timerEndSFX);
   }

   public void ResetButton(){
    timerElapsed = false;
    internalTimer = startTime;

   }
}
