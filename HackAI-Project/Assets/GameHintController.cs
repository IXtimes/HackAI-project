using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GameHintController : MonoBehaviour {
    public UnityEvent startGame;
    [SerializeField] RawImage playerProfile;
    void Awake()
    {
        playerProfile.texture = ProfileManager.Instance.playerProfile.profilePicture;   
    }
    public void CloseHint() {
        // Evoke the call needed to start the game
        startGame?.Invoke();

        // Disable this hint screen
        gameObject.SetActive(false);
    }
}
