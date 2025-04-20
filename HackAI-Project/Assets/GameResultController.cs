using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameResultController : MonoBehaviour
{
    [SerializeField] GameObject content;
    [SerializeField] GraphPrinter gameGraph;
    [SerializeField] TextMeshProUGUI supportBlurb;
    [SerializeField] TextMeshProUGUI gameRules;
    ProfileManager profileManager;
    public Color graphBarColor;
    public Games gameName;
    public List<float> committedData;
    public bool invertPerception = false;

    void Awake()
    {
        profileManager = ProfileManager.Instance;   
    }

    public void ShowResults() {
        content.SetActive(true);

        // Populate graph with the comitted data
        List<DataElement> graphData = new List<DataElement>();
        foreach(float datapoint in committedData) {
            graphData.Add(new DataElement{
                amount = datapoint,
                barColor = graphBarColor
            });
        }
        gameGraph.data = graphData.ToArray();
        gameGraph.UpdateGraphGraphic();

        string gameStringName = "";
        switch(gameName) {
            case Games.BreathSync:
                gameStringName = "BreathSync";
                break;
            case Games.ClearSight:
                gameStringName = "ClearSight";
                break;
            case Games.ShadowSnap:
                gameStringName = "ShadowSnap";
                break;
            case Games.ZenMelody:
                gameStringName = "ZenMelody";
                break;
        }
        // Check for profile personal best and low
        bool hitPersonalBest = false;
        bool hitPersonalLow = false;
        foreach(float datapoint in committedData) {
            if(datapoint > profileManager.playerProfile.personalBests[(int)gameName]) {
                hitPersonalBest = true;
                hitPersonalLow = false;
                profileManager.playerProfile.personalBests[(int)gameName] = datapoint;
            }
            
            if(datapoint < profileManager.playerProfile.personalLows[(int)gameName] && !hitPersonalBest) {
                hitPersonalLow = true;
                profileManager.playerProfile.personalLows[(int)gameName] = datapoint;
            }
        }
        
        // Invert perception if need be
        if(invertPerception && hitPersonalBest) {
            hitPersonalBest = false;
            hitPersonalLow = true;
        } else if(invertPerception && hitPersonalLow) {
            hitPersonalLow = false;
            hitPersonalBest = true;
        }

        // Append comitted data to profile
        foreach(float datapoint in committedData) {
            List<float> tempData = new List<float>();
            tempData.Add(datapoint);
            profileManager.playerProfile.gameHistory.Add(new GameData{data = new List<float>(tempData), name = gameStringName});
        }

        // Write a message based on the player's performance
        string prompt = "";
        if(hitPersonalBest) {
            prompt = $"Congradulate {profileManager.playerProfile.name} on their hitting their personal best score of ${profileManager.playerProfile.personalBests[(int)gameName]}! KEEP TO AT MOST 2 SENTENCES";
        } else {
            prompt = $"Support {profileManager.playerProfile.name} for their effort, and provide them a tip for how they can improve that the game based on your understanding of the rules. KEEP TO AT MOST 2 SENTENCES";
        }
        string environment = $"You are a supportive coach for the user. They are using this app for brain training and for meditation to relax. Based on the requests, using a gentle tone, be supportive and engaged with the user's prompts when you reply/serve prompts. Limit all responses to AT MOST 3-4 SENTENCES. Your responses should be impactful, but brief. Note that your response should reflect an understanding of this games rules, those being: {gameRules.text.Replace("\n", "")}";
        supportBlurb.text = "...";
        GPT.Prompt(environment, prompt, message => {
            supportBlurb.text = message;
        });
    }

    public void GetMood(int mood) {
        // Increment on the player profile what mood they felt
        profileManager.playerProfile.moodData[(int)mood]++;

        // Save
        string savePath = Path.Combine(Application.persistentDataPath, "myProfile.json");
        profileManager.SaveProfile(savePath);

        // Go back to dashboard
        SceneManager.LoadScene("MainPage");
    }
    
}
