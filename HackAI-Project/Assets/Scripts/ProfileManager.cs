using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData {
    public float highestScore;
    public float lowestScore;
    public float[] data;
}

public class Profile {
    public string name;
    public Texture2D profilePicture;
    public Dictionary<string, GameData> gameData;
    public Dictionary<string, int[]> moodData;
    public int currentStreak; 
}

public class ProfileManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
