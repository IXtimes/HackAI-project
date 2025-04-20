using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class DataElement {
    public float amount;
    public Color barColor;
}

public class GraphPrinter : MonoBehaviour
{
    [SerializeField] GameObject barGraphic;
    [SerializeField] TextMeshProUGUI[] tickMarkIndicators;
    [SerializeField] Transform barGroup;
    public DataElement[] data;
    public int initalScope;
    int scope;
    float maximum;
    float minimum;

    void Awake()
    {
        // Set the scope of the graph. 
        scope = initalScope;  
    }

    void Update()
    {
        // Update the graph visuals
        UpdateGraphGraphic(); 
    }

    void UpdateGraphGraphic() {
        // Determine the number of data points to draw
        int dataPoints = Mathf.Min(data.Length, scope);
        int difference = barGroup.childCount - dataPoints;

        // Create or destroy to that many data points in the bar group.
        int attempts = 0;
        while(difference != 0) {
            if(difference > 0) {
                Transform removedBar = barGroup.GetChild(barGroup.childCount - 1);
                removedBar.SetParent(null);
                Destroy(removedBar);
            }
            else
                Instantiate(barGraphic, barGroup);
            difference = barGroup.childCount - dataPoints;
            attempts++;
            if(attempts > 100)
                throw new Exception();
        }

        // If there is no data to render, stop :)
        if(dataPoints == 0) {
            tickMarkIndicators[0].text = $"0";
            tickMarkIndicators[1].text = $"0.5";
            tickMarkIndicators[2].text = $"1";
            return;
        }
        
        // Get at most the last scopes amount of data, first computing the max and min
        int startIndex = Mathf.Max(data.Length - scope, 0);
        float localMin = data[startIndex].amount;
        float localMax = data[startIndex].amount;
        for(int i = startIndex; i < startIndex + dataPoints; i++) {
            if(data[i].amount < localMin)
                localMin = data[i].amount;
            if(data[i].amount > localMax)
                localMax = data[i].amount;
        }

        // Set the recorded maximum as 5% higher and 5% lower than the actual
        maximum = Mathf.Round(localMax * 1.05f * 10f) / 10f;
        minimum = Mathf.Round(localMin * .95f * 10f) / 10f;

        // Now set the data as a percentage between the min and maximum value
        int childIndex = 0;
        for(int i = startIndex; i < startIndex + dataPoints; i++) {
            // Size the bar
            RectTransform barChild = barGroup.GetChild(childIndex).GetComponent<RectTransform>();
            barChild.sizeDelta = new Vector2(barChild.sizeDelta.x, Mathf.Lerp(0f, 1250f, (data[i].amount - minimum) / (maximum - minimum)));

            // Update the text of the bar
            TextMeshProUGUI barText = barChild.GetChild(0).GetComponent<TextMeshProUGUI>();
            barText.text = $"{Mathf.Round(data[i].amount * 10f) / 10f}";

            // Change the color of the bar and its text.
            barText.color = data[i].barColor;
            barChild.GetComponent<Image>().color = data[i].barColor;

            // Get the next bar
            childIndex++;
        }

        // Set the tick mark indicator texts to the maximum, minimum, and a mid point value
        tickMarkIndicators[0].text = $"{minimum}";
        tickMarkIndicators[1].text = $"{(minimum + maximum) / 2}";
        tickMarkIndicators[2].text = $"{maximum}";
    }

}
