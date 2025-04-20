using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;

[Serializable]
public class OnboardingEvent {
    public string event_name;
    public int event_index;
}
public class OnboardingManager : MonoBehaviour
{
    public string[] onboardingLines;
    public OnboardingEvent[] onboardingEvents;
    Dictionary<string, bool> onboardingEventStatus;
    ProfileManager profileManager;
    int currentOnboardingLine;
    [SerializeField] TextMeshProUGUI onboardingText;
    [SerializeField] GameObject onboardingNameInput;
    [SerializeField] GameObject onboardingPhotoInput;
    [SerializeField] GameObject onboardingSubmit;

    void Awake()
    {
        // Hide all event related elements
        onboardingNameInput.SetActive(false);
        onboardingPhotoInput.SetActive(false);
        onboardingSubmit.SetActive(false);

        // Configure the base onboarding events
        onboardingEventStatus = new Dictionary<string, bool>();
        onboardingEventStatus["username"] = false;
        onboardingEventStatus["profile_picture"] = false;
        onboardingEventStatus["finish"] = false;

        // Get the profile manager
        profileManager = ProfileManager.Instance;

        // Set onboarding to the first line
        onboardingText.text = onboardingLines[currentOnboardingLine];

        // Automatically call the next onboarding line
        DOTween.Sequence().AppendInterval(0.5f).AppendCallback(GetNextOnboardingLine);
    }

    public void CompleteEvent(string eventName) {
        // Switch on the completed event
        foreach(OnboardingEvent obEvent in onboardingEvents) { 
            if(obEvent.event_name == eventName)
                switch(obEvent.event_name) {
                    case "username":
                        // Set the username
                        onboardingNameInput.SetActive(false);
                        profileManager.playerProfile.name = onboardingNameInput.transform.GetChild(0).GetComponent<TMP_InputField>().text;
                        break;
                    case "profile_picture":
                        // Set the profile picture
                        onboardingPhotoInput.SetActive(false);
                        profileManager.playerProfile.profilePicture = onboardingPhotoInput.GetComponent<PhotoAlbumInterface>().profileTexture;
                        break;
                    case "finish":
                        onboardingSubmit.SetActive(false);
                        string savePath = Path.Combine(Application.persistentDataPath, "myProfile.json");
                        profileManager.SaveProfile(savePath);
                        break;
                }
        }

        // Set that onboarding status to true, and trigger the next onboarding line
        onboardingEventStatus[eventName] = true;

        // Automatically fetch the next onboarding line.
        GetNextOnboardingLine();
    }

    public void GetNextOnboardingLine() {
        // Check if there is an onboarding event here, and if so that its cleared
        foreach(OnboardingEvent obEvent in onboardingEvents) {
            if(obEvent.event_index == currentOnboardingLine && !onboardingEventStatus[obEvent.event_name])
                // Void call until condition is met
                return;
            
        }

        // Move on to the next onboarding line
        currentOnboardingLine++;

        // If this is the last line of onboarding, stop the loop
        if(onboardingLines.Length <= currentOnboardingLine) {
            return;
        }

        // Set the onboarding text to the next line of onboarding
        onboardingText.text = onboardingLines[currentOnboardingLine].Replace("<name>", profileManager.playerProfile.name);

        // Enable the respective element based on the event 
        foreach(OnboardingEvent obEvent in onboardingEvents) { 
            if(obEvent.event_index == currentOnboardingLine)
                switch(obEvent.event_name) {
                    case "username":
                        onboardingNameInput.SetActive(true);
                        break;
                    case "profile_picture":
                        onboardingPhotoInput.SetActive(true);
                        break;
                    case "finish":
                        onboardingSubmit.SetActive(true);
                        break;
                }
        }

        // Automatically call the next onboarding line
        DOTween.Sequence().AppendInterval(0.5f).AppendCallback(GetNextOnboardingLine);
    }
}
