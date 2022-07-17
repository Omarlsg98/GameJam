using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static PartData;
using TMPro;

public class StatsDisplayer : MonoBehaviour
{
    public GameObject[] valTexts = new GameObject[11];

    private string[] units = new string[]{"", " sl/act", " p/hit", " hit/s", " tl", " p", " kg", " %", " tl/s", " kg", ""}; 
    
    public void changeDataDisplayed(PartData data, bool isHorror){
        string title = isHorror? "Horror" : data.type.ToString();
        valTexts[0].GetComponent<TextMeshProUGUI>().text = title + " Stats:";
        if (data != null) {
            valTexts[1].GetComponent<TextMeshProUGUI>().text = data.energyConsumption + units[1];
            valTexts[2].GetComponent<TextMeshProUGUI>().text = data.damage + units[2];
            valTexts[3].GetComponent<TextMeshProUGUI>().text = data.attackSpeed + units[3];
            valTexts[4].GetComponent<TextMeshProUGUI>().text = data.range + units[4];
            valTexts[5].GetComponent<TextMeshProUGUI>().text = data.life + units[5];
            valTexts[6].GetComponent<TextMeshProUGUI>().text = data.loadCapacity + units[6];
            valTexts[7].GetComponent<TextMeshProUGUI>().text = data.luck + units[7];
            valTexts[8].GetComponent<TextMeshProUGUI>().text = data.movementSpeed + units[8];
            valTexts[9].GetComponent<TextMeshProUGUI>().text = data.weight + units[9];
        } else {
            for (int i = 1; i < valTexts.Length - 1; i ++){
                 valTexts[i].GetComponent<TextMeshProUGUI>().text = "0" + units[i];
            }
        }
        valTexts[10].GetComponent<TextMeshProUGUI>().color = isHorror? valTexts[9].GetComponent<TextMeshProUGUI>().color : data.rareType.color;
        valTexts[10].GetComponent<TextMeshProUGUI>().text = isHorror? "N/A" : data.rareType.type.ToString();
    }
}
