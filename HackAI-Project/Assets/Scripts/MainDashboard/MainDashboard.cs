using System.Collections;
using System.Collections.Generic;
using Packages.Rider.Editor.UnitTesting;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainDashboard : MonoBehaviour
{
    ProfileManager profileManager;

    [SerializeField] RawImage profilePicture;
    [SerializeField] GraphPrinter mainDashboard;
    [SerializeField] Transform[] moodIndicators;
    [SerializeField] Color[] gameColors;
    [SerializeField] TextMeshProUGUI currentStreak;
    [SerializeField] TextMeshProUGUI gptSupportMessage;

    void Awake()
    {
        // Get the profile manager for all profile data
        profileManager = ProfileManager.Instance;

        // Populate the main dashboard with data
        List<DataElement> graphData = new List<DataElement>();
        foreach (GameData dat in profileManager.playerProfile.gameHistory) {
            switch(dat.name) {
                case "ZenMelody":
                    graphData.Add(new DataElement{
                        amount = Mathf.Round((dat.data[0] - profileManager.playerProfile.personalLows[0]) / 
                                             (profileManager.playerProfile.personalLows[0] - profileManager.playerProfile.personalBests[0])
                                            * 10f) / 10f, 
                        barColor = gameColors[0]});
                    break;
                case "BreathSync":
                    graphData.Add(new DataElement{
                        amount = Mathf.Round((dat.data[0] - profileManager.playerProfile.personalLows[1]) / 
                                             (profileManager.playerProfile.personalLows[1] - profileManager.playerProfile.personalBests[1])
                                            * 10f) / 10f, 
                        barColor = gameColors[1]});
                    break;
                case "ClearSight":
                    graphData.Add(new DataElement{
                        amount = Mathf.Round((dat.data[0] - profileManager.playerProfile.personalLows[2]) / 
                                             (profileManager.playerProfile.personalLows[2] - profileManager.playerProfile.personalBests[2])
                                            * 10f) / 10f, 
                        barColor = gameColors[2]});
                    break;
                case "ShadowSnap":
                    graphData.Add(new DataElement{
                        amount = Mathf.Round((dat.data[0] - profileManager.playerProfile.personalLows[3]) / 
                                             (profileManager.playerProfile.personalLows[3] - profileManager.playerProfile.personalBests[3])
                                            * 10f) / 10f, 
                        barColor = gameColors[3]});
                    break;
            }
        }
        mainDashboard.data = graphData.ToArray();
        mainDashboard.UpdateGraphGraphic();

        // Set the current streak
        currentStreak.text = $"{profileManager.playerProfile.currentStreak}";

        // Set the profile picture
        profilePicture.texture = profileManager.playerProfile.profilePicture;

        // Set the counts for each mood indicator
        for(int i = 0; i < 5; i++) {
            List<List<int>> moodData = profileManager.playerProfile.moodData;
            for(int j = Mathf.Max(moodData.Count - 7, 0); j < moodData.Count; j++) {
                moodIndicators[i].GetChild(0).GetComponent<TextMeshProUGUI>().text = $"- {moodData[j][i]}";
            }
        }

        // Come up with a chatGPT support message
        gptSupportMessage.text = "...";
        string prompt = $"Encoruage the user (their name, of which you MUST use, is ${profileManager.playerProfile.name}) to play one of the following games, supporting that the game trains one of the associated areas of the brain:";
        prompt += "  Zen Melody - memory, zen, pitch";
        prompt += "  Breath Sync - breathing, counting";
        prompt += "  Clear Sight - reaction, short-term memory";
        prompt += "  Shadow Snap - pattern recognition, shape identification";
        prompt += " LIMIT RESPONSE TO ONLY 2-3 SENTENCES";
        string environment = "You are a supportive coach for the user. They are using this app for brain training and for meditation to relax. Based on the requests, using a gentle tone, be supportive and engaged with the user's prompts when you reply/serve prompts. Limit all responses to AT MOST 3-4 SENTENCES. Your responses should be impactful, but brief. A prompt may ask for recommendations for games for the user to play, you will be supplied the list of games and their strengths in which you can recommend from there.";
        GPT.Prompt(environment, prompt, message => {
            gptSupportMessage.text = message;
        });
    }
}
