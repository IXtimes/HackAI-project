using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class BreathingGame : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public TextMeshProUGUI statusText;
    public TextMeshProUGUI roundText;
    public Button holdButton;

    public List<float> timingResults = new List<float>(); // Public list to track performance

    private bool isHolding = false;
    private bool gameActive = false;
    private bool gameFinished = false;
    private int roundCount = 0;

    private float targetHoldTime = 0f;
    private float holdStartTime = 0f;
    private float autoReturnDelay = 8f; // Time before auto-return to MainPage

    void Start()
    {
        if (holdButton == null || statusText == null || roundText == null)
        {
            Debug.LogError("❌ UI references not assigned in Inspector");
            return;
        }

        roundCount = 0;
        timingResults.Clear();
        roundText.text = "Round: 0 / 7";
        statusText.text = "Touch & Hold to Start";
        holdButton.gameObject.SetActive(true);
    }

    void Update()
    {
        // No need to check anything here — game logic handled via input events
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (gameFinished)
        {
            SceneManager.LoadScene("MainPage");
            return;
        }

        if (!gameActive && roundCount < 7)
        {
            gameActive = true;
            isHolding = true;
            StartCoroutine(PerformRound());
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isHolding = false;

        if (gameActive)
        {
            float actualHoldTime = Time.time - holdStartTime;
            float difference = Mathf.Abs(targetHoldTime - actualHoldTime);
            float rounded = Mathf.Round(difference * 10f) / 10f;

            timingResults.Add(rounded);
            Debug.Log($"🎯 Target: {targetHoldTime:F1}s | Held: {actualHoldTime:F1}s | Diff: {rounded}");

            roundCount++;
            roundText.text = $"Round: {roundCount} / 7";
            gameActive = false;

            if (roundCount >= 7)
            {
                // ✅ Show all round results
                string resultsText = "✅ All rounds complete!\n\nYour Results:\n";
                for (int i = 0; i < timingResults.Count; i++)
                {
                    resultsText += $"Round {i + 1}: {timingResults[i]:F1} sec\n";
                }

                statusText.text = resultsText;
                gameFinished = true;
                StartCoroutine(ReturnToMainAfterDelay());
            }
            else
            {
                statusText.text = $"You were off by {rounded:F1} seconds";
                StartCoroutine(BreathOutPause());
            }
        }
    }

    IEnumerator PerformRound()
    {
        statusText.text = "Get Ready...";
        yield return new WaitForSeconds(1f);

        // Set target time
        targetHoldTime = Random.Range(7f, 10f); // Breathe In countdown from 7 to 10
        holdStartTime = Time.time;

        float visibleTime = 2f;
        float startTime = Time.time;
        float countdown = targetHoldTime;

        // Show countdown for first 2 seconds only
        while (Time.time - startTime < visibleTime && countdown > 0)
        {
            if (!isHolding)
            {
                GameOver("Released too early!");
                yield break;
            }

            countdown = targetHoldTime - (Time.time - startTime);
            statusText.text = $"Breathe In: {countdown:F1}s";
            yield return null;
        }

        // Hide timer but keep holding
        statusText.text = "Keep Holding...";
        while (isHolding)
        {
            yield return null;
        }

        // OnPointerUp will handle result calculation
    }

    IEnumerator BreathOutPause()
    {
        statusText.text = "Breathe Out...";
        yield return new WaitForSeconds(3f);
        statusText.text = "Touch & Hold to Start";
    }

    IEnumerator ReturnToMainAfterDelay()
    {
        yield return new WaitForSeconds(autoReturnDelay);
        SceneManager.LoadScene("MainPage");
    }

    void GameOver(string reason)
    {
        Debug.LogWarning($"❌ Game Over: {reason}");
        statusText.text = $"{reason}\nTap to try again";
        gameActive = false;
    }
}
