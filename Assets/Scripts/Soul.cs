using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using static HeadQuarter;
public enum SoulType{
    green,
    blue,
    red
}
[System.Serializable]
public class SoulData{
    public int lowRange = 50;
    public int highRange = 100;
    public float spawnProbability = 0.5f;
    public SoulType soulType;

    public int consumeSoul(){
        int totalSouls = 0;
        int dadoDice = Random.Range(1, 11);
        float m =  (highRange - lowRange) / 9;
        float b = (10*lowRange - highRange) / 9;
        totalSouls = (int)(m*dadoDice + b);
        return totalSouls;
    }
}

public class Soul: MonoBehaviour{
    public SoulData soulData;
    public bool buttonOnTable;
    private HeadQuarter playerHeadQuarter;

    void Start(){
        GameObject gameController = GameObject.FindWithTag("GameController");
        playerHeadQuarter = gameController.GetComponent<HeadQuarter>();
    }

    void OnMouseOver(){
        if (buttonOnTable){
            if(Input.GetMouseButtonDown(0)){
                playerHeadQuarter.putSoulTable(soulData.soulType);
            }
            if(Input.GetMouseButtonDown(1)){
                playerHeadQuarter.removeSoulTable(soulData.soulType);
            }
        } else {
            if(Input.GetMouseButtonDown(0)){
                playerHeadQuarter.increaseSoulLevel(soulData.soulType);
            } 
            if(Input.GetMouseButtonDown(1)){
                playerHeadQuarter.spawnPart(soulData.soulType);
            }
        }
    }
}

[System.Serializable]
public class SoulInventory{
    public GameObject soulReference;
    public int inventory;

    private SoulData soulData;
    public int consumeSoulFromInventory(){
        if (inventory <= 0){
            //Not enough inventory
            return 0;
        }
        this.inventory -= 1;
        return soulData.consumeSoul();
    }
    public void setSoulData(){
         this.soulData= soulReference.GetComponent<Soul>().soulData;
    }
    public SoulData getSoulData(){
        return this.soulData;
    }
}

