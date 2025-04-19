using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Example : MonoBehaviour {
    public TextMeshProUGUI exampleText;
    public float percentage;

    public void SaySomething() {
        Debug.Log(exampleText.text);
    }

    // Start is called before the first frame update
    void Awake() {
        exampleText.text = "Your mom";
    }

    // Update is called once per frame
    void Update() {
        
    }
}
