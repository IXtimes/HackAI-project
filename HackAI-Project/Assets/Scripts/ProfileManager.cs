using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameData {
    public string name;
    public List<float> data;
    public GameData() {
        data = new List<float>();
    }
}

public class Profile {
    public string name;
    public Texture2D profilePicture;
    public List<float> personalBests;
    public List<float> personalLows;
    public List<GameData> gameHistory;
    public List<List<int>> moodData;
    public int currentStreak; 

    public Profile() {
        name = "";
        profilePicture = null;

        personalBests = new List<float>();
        personalLows = new List<float>();
        gameHistory = new List<GameData>();
        moodData = new List<List<int>>();
        moodData.Add(
            new List<int>{0, 0, 0, 0, 0}
        );

        currentStreak = 0;
    }
}

[System.Serializable]
public class SerializableGameData
{
    public string name;
    public List<float> data;
}

[System.Serializable]
public class SerializableProfile
{
    public string name;
    public string profilePicturePath;

    public List<float> personalBests;
    public List<float> personalLows;
    public List<SerializableGameData> gameHistory;
    public List<List<int>> moodData;

    public int currentStreak;
}

public class ProfileManager : MonoBehaviour
{
    public Profile playerProfile;
    public static ProfileManager Instance;
    // Start is called before the first frame update
    void Awake() {
        // Set the singleton instance.
        if(!Instance)
            Instance = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);

        // Look for a prexisting profile on the disk save location
        string savePath = Path.Combine(Application.persistentDataPath, "myProfile.json");
        if(!LoadProfile(savePath)) {
            // Enter onboarding
            SceneManager.LoadScene("Onboarding");
        } else {
            // Otherwise, enter the main page.
            SceneManager.LoadScene("MainPage");
        }
    }

    public void SaveProfile(string savePath)
    {
        // Save profile picture to disk
        string imagePath = Path.Combine(Application.persistentDataPath, playerProfile.name + "_pic.png");
        if (playerProfile.profilePicture != null)
        {
            byte[] imageBytes = playerProfile.profilePicture.EncodeToPNG();
            File.WriteAllBytes(imagePath, imageBytes);
        }

        // Convert game history to serializable format
        List<SerializableGameData> serialGameHistory = new List<SerializableGameData>();
        foreach (var g in playerProfile.gameHistory)
        {
            serialGameHistory.Add(new SerializableGameData
            {
                name = g.name,
                data = new List<float>(g.data)
            });
        }

        // Build serializable profile
        SerializableProfile sp = new SerializableProfile
        {
            name = playerProfile.name,
            profilePicturePath = imagePath,
            personalBests = new List<float>(playerProfile.personalBests),
            personalLows = new List<float>(playerProfile.personalLows),
            gameHistory = serialGameHistory,
            moodData = new List<List<int>>(playerProfile.moodData),
            currentStreak = playerProfile.currentStreak
        };

        string json = JsonUtility.ToJson(sp, true);
        File.WriteAllText(savePath, json);
    }

    public bool LoadProfile(string savePath)
    {
        if (!File.Exists(savePath))
        {
            Debug.LogWarning("Profile file not found: " + savePath);
            playerProfile = new Profile();
            return false;
        }

        string json = File.ReadAllText(savePath);
        Debug.Log(json);
        SerializableProfile sp = JsonUtility.FromJson<SerializableProfile>(json);

        // Load profile picture from disk
        Texture2D texture = new Texture2D(2, 2);
        if (File.Exists(sp.profilePicturePath))
        {
            byte[] imageBytes = File.ReadAllBytes(sp.profilePicturePath);
            texture.LoadImage(imageBytes);
        }

        // Convert serializable game history back to GameData
        List<GameData> gameHistory = new List<GameData>();
        foreach (var g in sp.gameHistory)
        {
            gameHistory.Add(new GameData
            {
                name = g.name,
                data = new List<float>(g.data)
            });
        }

        List<List<int>> moodData = sp.moodData != null ? new List<List<int>>(sp.moodData) : new List<List<int>>();

        if (moodData.Count == 0)
        {
            moodData.Add(new List<int> { 0, 0, 0, 0, 0 });  // Default entry
        }

        // Build final profile
        Profile profile = new Profile
        {
            name = sp.name,
            profilePicture = texture,
            personalBests = new List<float>(sp.personalBests),
            personalLows = new List<float>(sp.personalLows),
            gameHistory = gameHistory,
            moodData = moodData,
            currentStreak = sp.currentStreak
        };

        playerProfile = profile;
        return true;
    }
}
