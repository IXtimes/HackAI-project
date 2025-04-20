using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GraphPrinter : MonoBehaviour
{
    [SerializeField] GameObject barGraphic;
    [SerializeField] TextMeshProUGUI[] tickMarkIndicators;
    [SerializeField] Transform barGroup;
    public float[] data;
    public int scope;
    float maximum;
    float minimum;

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
        while(difference != 0) {
            if(difference > 0)
                Destroy(barGroup.GetChild(barGroup.childCount - 1));
            else
                Instantiate(barGraphic, barGroup);
            difference = barGroup.childCount - dataPoints;
        }
        
        // Get at most the last scopes amount of data, first computing the max and min
        int startIndex = Mathf.Max(data.Length - scope, 0);
        float localMin = data[startIndex];
        float localMax = data[startIndex];
        for(int i = startIndex; i < startIndex + dataPoints; i++) {
            if(data[i] < localMin)
                localMin = data[i];
            if(data[i] > localMax)
                localMax = data[i];
        }

        // Set the recorded maximum as 5% higher and 5% lower than the actual
        maximum = Mathf.Round(localMax * 1.05f * 10f) / 10f;
        minimum = Mathf.Round(localMin * .95f * 10f) / 10f;

        // Now set the data as a percentage between the min and maximum value
        int childIndex = 0;
        for(int i = startIndex; i < startIndex + dataPoints; i++) {
            RectTransform barChild = barGroup.GetChild(childIndex).GetComponent<RectTransform>();
            barChild.sizeDelta = new Vector2(barChild.sizeDelta.x, Mathf.Lerp(0f, 1250f, (data[i] - minimum) / (maximum - minimum)));
            barChild.GetChild(0).GetComponent<TextMeshProUGUI>().text = $"{Mathf.Round(data[i] * 10f) / 10f}";
            childIndex++;
        }

        // Set the tick mark indicator texts to the maximum, minimum, and a mid point value
        tickMarkIndicators[0].text = $"{minimum}";
        tickMarkIndicators[1].text = $"{(minimum + maximum) / 2}";
        tickMarkIndicators[2].text = $"{maximum}";
    }

}
