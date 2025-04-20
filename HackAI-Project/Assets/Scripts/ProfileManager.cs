using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameData {
    public float highestScore;
    public float lowestScore;
    public float[] data;
}

public class Profile {
    public string name;
    public Texture2D profilePicture;
    public Dictionary<string, GameData[]> gameData;
    public Dictionary<string, int[]> moodData;
    public int currentStreak; 

    public Profile() {
        name = "";
        profilePicture = null;
        gameData = new Dictionary<string, GameData[]>();

        // Create sample 
        moodData = new Dictionary<string, int[]>();
        currentStreak = 0;
    }
}

[System.Serializable]
public class SerializableGameData
{
    public float highestScore;
    public float lowestScore;
    public float[] data;
}

[System.Serializable]
public class SerializableProfile
{
    public string name;
    public string profilePicturePath;
    public List<string> gameKeys;
    public List<SerializableGameData[]> gameValues;
    public List<string> moodKeys;
    public List<int[]> moodValues;
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
        // Save texture to file
        string imagePath = Path.Combine(Application.persistentDataPath, playerProfile.name + "_pic.png");
        byte[] imageData = playerProfile.profilePicture.EncodeToPNG();
        File.WriteAllBytes(imagePath, imageData);

        // Build serializable object
        SerializableProfile sp = new SerializableProfile
        {
            name = playerProfile.name,
            profilePicturePath = imagePath,
            gameKeys = new List<string>(playerProfile.gameData.Keys),
            gameValues = new List<SerializableGameData[]>(),
            moodKeys = new List<string>(playerProfile.moodData.Keys),
            moodValues = new List<int[]>(),
            currentStreak = playerProfile.currentStreak
        };

        foreach (var gameEntry in playerProfile.gameData)
        {
            var serialArray = gameEntry.Value.Select(gd => new SerializableGameData
            {
                highestScore = gd.highestScore,
                lowestScore = gd.lowestScore,
                data = gd.data
            }).ToArray();

            sp.gameValues.Add(serialArray);
        }

        foreach (var mood in playerProfile.moodData.Values)
        {
            sp.moodValues.Add(mood);
        }

        // Convert to JSON and save
        string json = JsonUtility.ToJson(sp, true);
        File.WriteAllText(savePath, json);
    }

    public bool LoadProfile(string savePath)
    {
        if (!File.Exists(savePath))
        {
            Debug.LogWarning("Profile file not found at: " + savePath);
            playerProfile = new Profile();
            return false; // Or create and return a default Profile if needed
        }

        string json = File.ReadAllText(savePath);
        SerializableProfile sp = JsonUtility.FromJson<SerializableProfile>(json);

        // Load image
        byte[] imageData = File.ReadAllBytes(sp.profilePicturePath);
        Texture2D tex = new Texture2D(2, 2);
        tex.LoadImage(imageData);

        // Build original profile object
        Profile profile = new Profile
        {
            name = sp.name,
            profilePicture = tex,
            gameData = new Dictionary<string, GameData[]>(),
            moodData = new Dictionary<string, int[]>(),
            currentStreak = sp.currentStreak
        };

        for (int i = 0; i < sp.gameKeys.Count; i++)
        {
            var deserializedArray = sp.gameValues[i].Select(sgd => new GameData
            {
                highestScore = sgd.highestScore,
                lowestScore = sgd.lowestScore,
                data = sgd.data
            }).ToArray();

            profile.gameData.Add(sp.gameKeys[i], deserializedArray);
        }

        for (int i = 0; i < sp.moodKeys.Count; i++)
        {
            profile.moodData.Add(sp.moodKeys[i], sp.moodValues[i]);
        }

        playerProfile = profile;
        return true;
    }
}
