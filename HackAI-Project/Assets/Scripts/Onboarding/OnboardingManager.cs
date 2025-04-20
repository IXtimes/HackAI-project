using System.Collections;
using System.Collections.Generic;
using DG.Tweening.Plugins;
using TMPro;
using UnityEngine;

public class OnboardingEvent {
    public string event_name;
    public string event_index;
}
public class OnboardingManager : MonoBehaviour
{
    public string[] onboardingLines;
    public OnboardingEvent[] oboardingEvents;
    int currentOnboardingLine;
    [SerializeField] TextMeshProUGUI onboardingText;
    [SerializeField] Transform onboardingNameInput;
    [SerializeField] PhotoAlbumInterface photoInput;

    public void GetNextOnboardingLine() {
        // Set the onboarding text to the next line of onboarding
    }
}
