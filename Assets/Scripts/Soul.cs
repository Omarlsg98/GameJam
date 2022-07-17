using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
}

[System.Serializable]
public class SoulInventory{
    public GameObject soulReference;
    public int inventory;

    private SoulData soulData;
    public int consumeSoulFromInventory(){
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

