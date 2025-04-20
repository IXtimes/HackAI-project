using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameHintController : MonoBehaviour {
    public UnityEvent startGame;
    public void CloseHint() {
        // Evoke the call needed to start the game
        startGame?.Invoke();

        // Disable this hint screen
    }
}
