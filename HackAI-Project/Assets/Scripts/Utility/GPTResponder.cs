using System.Collections;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public static class GPT {
    public static void Prompt(string environment, string prompt, System.Action<string> callback)
    {
        if (GPTResponder.Instance != null)
        {
            GPTResponder.Instance.GetChatGPTResponse(environment, prompt, callback);
        }
        else
        {
            Debug.LogError("ChatGPTClient.Instance is not initialized in the scene.");
        }
    }
}

public class GPTResponder : MonoBehaviour {
    public static GPTResponder Instance;
    private string apiKey;
    private string apiURL = "https://api.openai.com/v1/chat/completions";

    [System.Serializable]
    public class ChatMessage
    {
        public string role;
        public string content;
    }

    [System.Serializable]
    public class ChatChoice
    {
        public ChatMessage message;
    }

    [System.Serializable]
    public class ChatResponse
    {
        public ChatChoice[] choices;
    }

    [System.Serializable]
    public class SecretData
    {
        public string chatGPT_api_key;
    }

    void Awake()
    {
        // Run singleton to create this instance
        if(!Instance)
            Instance = this;
        else
            Destroy(gameObject);

        // Load api key
        apiKey = LoadApiKey();
    }

    public static string LoadApiKey()
    {
        string path = Path.Combine(Application.streamingAssetsPath, "Secrets\\api_keys.json");
        Debug.Log(path);
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            SecretData data = JsonUtility.FromJson<SecretData>(json);
            return data.chatGPT_api_key;
        }
        Debug.LogError("API key file not found!");
        return null;
    }

    public void GetChatGPTResponse(string environment, string prompt, System.Action<string> callback)
    {
        StartCoroutine(SendChatGPTRequest(environment, prompt, callback));
    }

    IEnumerator SendChatGPTRequest(string environment, string message, System.Action<string> callback)
    {
        string formattedJson = "{\"model\":\"o4-mini-2025-04-16\",\"messages\":[{\"role\":\"system\",\"content\":\"" + environment + "\"}, {\"role\":\"user\",\"content\":\"" + message + "\"}]}";

        UnityWebRequest request = new UnityWebRequest(apiURL, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(formattedJson);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + apiKey);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string json = request.downloadHandler.text;

            // JsonUtility needs a wrapper class to parse
            ChatResponse chatResponse = JsonUtility.FromJson<ChatResponse>(json);

            string reply = chatResponse.choices[0].message.content;
            callback?.Invoke(reply);
        }
        else
        {
            Debug.LogError("Request error: " + request.error);
            callback?.Invoke(null); // Pass null on error
        }
    }
}
