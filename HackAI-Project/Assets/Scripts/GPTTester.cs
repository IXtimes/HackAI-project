using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GPTTester : MonoBehaviour
{
    public TextMeshProUGUI textBox;
    public TMP_InputField prompt;
    public TMP_InputField environment;

    void Awake()
    {
        textBox.text = "...";   
    }

    public void MakeCall() {
        GPT.Prompt(environment.text, prompt.text, response => {
            textBox.text = response;
        });
    }
}
