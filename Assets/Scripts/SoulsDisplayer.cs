using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static PartData;
using TMPro;

public class SoulsDisplayer : MonoBehaviour
{
    public GameObject[] texts = new GameObject[6];

    public void setSoulsValues(SoulData green, SoulData blue, SoulData red){
        texts[0].GetComponent<TextMeshProUGUI>().text = green.lowRange + " - " + green.highRange;
        texts[1].GetComponent<TextMeshProUGUI>().text = blue.lowRange + " - " + blue.highRange;
        texts[2].GetComponent<TextMeshProUGUI>().text = red.lowRange + " - " + red.highRange;
    }
 
    public void updateSoulsCounter(int green, int blue, int red){
        texts[3].GetComponent<TextMeshProUGUI>().text = "x" + green;
        texts[4].GetComponent<TextMeshProUGUI>().text = "x" + blue;
        texts[5].GetComponent<TextMeshProUGUI>().text = "x" + red;
    }
}
