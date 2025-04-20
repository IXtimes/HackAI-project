using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class BreathingGame : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI timerText;
    [SerializeField] TextMeshProUGUI resultText;
    [SerializeField] GameObject breatheButton;
    [SerializeField] GameObject finishButton;
    [SerializeField] GameResultController gameResultController;
    float internalTimer;
    float startTime;
    bool breathingIn;
    public List<float> resultData;
    public Vector2 breatheInTime;
    bool gameActive = false;

    public void SetupGame() {
        // Activate the game
        gameActive = true;

        // Set the breathing state to breathing out to idle
        breathingIn = false;
    }

    void Update()
    {
        // Only process updates when the game is active
        if(!gameActive) return;

        // Decrement the internal timer
        internalTimer -= Time.deltaTime;

        // Draw text elements based on the current state of the game
        if(breathingIn) {
            // Hide the stop button and result text
            finishButton.SetActive(false);
            resultText.text = "";
            if(internalTimer <= startTime - 2f) {
                // Hide the timer
                timerText.text = "Release and breathe out when you think the timer has hit 0s!";
            } else {
                // Show the timer
                timerText.text = $"Breathe in for {Mathf.Round(internalTimer * 10f) / 10f}s...";
            }
        } else {
            // Show the stop button
            finishButton.SetActive(true);
            // The timer text encourages to breathe out and play again.
            timerText.text = "Breathe out... and when you are ready, breathe in again.\nIf you are done for today, you can quit!";
            if(resultData.Count > 0)
                resultText.text = $"You were {resultData[resultData.Count - 1]}s off from 0s.";
            else
                resultText.text = "";
        }
    }

    public void ReleaseButton() {
        // Ignore if not breathing in
        if(!breathingIn) return;

        // Otherwise we started breathing out
        breathingIn = false;

        // Calculate how far off the player was from the true end time, and log it in data.
        resultData.Add(Mathf.Round(Mathf.Abs(internalTimer) * 10f) / 10f);
    }

    public void HoldingButton() {
        // Ignore if not breathing out
        if(breathingIn) return;

        // Otherwise we started breathing in
        breathingIn = true;

        // Set the timer
        startTime = Random.Range(breatheInTime.x, breatheInTime.y);
        internalTimer = startTime;
    }

    public void ExitGame() {
        // Deactivate the game
        gameActive = false;

        // Callback to results screen
        gameResultController.committedData = resultData;
        gameResultController.ShowResults();
    }
}
