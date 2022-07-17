using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static PartData;
using TMPro;

public class SoulsExtractedDisplayer : MonoBehaviour
{
    public GameObject[] texts = new GameObject[1];

    public void setSoulsObtained(int[] values){
        string valuesAsString = "";
        int total = 0;
        for (int i = 0; i < values.Length; i++){
            total += values[i];
            valuesAsString += values[i];
            if (i != values.Length - 1)
                valuesAsString += " + ";
        }
        texts[0].GetComponent<TextMeshProUGUI>().text = "SoulsExtracted:\n"+ valuesAsString + "\n= "+ total;
    }
}
