using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static Main;

using static CoolDown;
using static SoulInventory;

public class HeadQuarter : MonoBehaviour
{
    public float soulsLevel;
    public float objectiveSoulLevel;
    public CoolDown newSoulCoolDown;

    public SoulInventory greenSoul;
    public SoulInventory blueSoul;
    public SoulInventory redSoul;
    private float sumSoulProbabilities;
    public bool isPlayer;
    public GameObject soulLevelBar;
    public GameObject nextSoulBar;
    
    void Start(){
        modifySoulBar();
        greenSoul.setSoulData();
        redSoul.setSoulData();
        blueSoul.setSoulData();
        this.sumSoulProbabilities = greenSoul.getSoulData().spawnProbability + redSoul.getSoulData().spawnProbability + blueSoul.getSoulData().spawnProbability;
    }

    void Update(){
        if (isPlayer){
            if (hasWon()){
                Debug.Log("WOOOOON");
            } else if (hasLost()){
                Debug.Log("LOOOOOST");
            }
            generateRandomNewSoul();
        }
    }

    public bool hasWon(){
        return soulsLevel > objectiveSoulLevel;
    }

     public bool hasLost(){
        return soulsLevel <= 0;
    }

    public void generateRandomNewSoul(){
        newSoulCoolDown.updateCoolDown();
        modifyNextSoulBar();
        if (newSoulCoolDown.isReady()){
            SoulType soulToAdd;
            float randomNumber = Random.Range(0, sumSoulProbabilities);
            if (randomNumber < greenSoul.getSoulData().spawnProbability){
                soulToAdd = SoulType.green;
            }else if(randomNumber < greenSoul.getSoulData().spawnProbability + redSoul.getSoulData().spawnProbability){
                soulToAdd = SoulType.red;
            }else{
                soulToAdd = SoulType.blue;
            }
            addSoulsToStock(soulToAdd, 1);
            newSoulCoolDown.turnOnCooldown();
        }
    }

    public void increaseSoulLevel(SoulType soulType){
        int souls;
        switch (soulType)
        {
            case SoulType.blue:
            souls = blueSoul.consumeSoulFromInventory();
            break;

            case SoulType.green:
            souls = greenSoul.consumeSoulFromInventory();
            break;

            case SoulType.red:
            souls = redSoul.consumeSoulFromInventory();
            break;

            default:
            return;
        }
        this.soulsLevel += souls;
        modifySoulBar();
    }

    public void addSoulsToStock(SoulType soulType, int souls){
        switch (soulType)
        {
            case SoulType.blue:
            blueSoul.inventory += souls;
            break;

            case SoulType.green:
            greenSoul.inventory += souls;
            break;

            case SoulType.red:
            redSoul.inventory += souls;
            break;

            default:
            return;
        }
    }

    public void applyHit(int damage){
        this.soulsLevel -= damage;
        modifySoulBar();
        //TODO(omar): Spawn soul
    }

    private void modifySoulBar(){
        this.soulLevelBar.transform.localScale = new Vector3(this.soulsLevel/this.objectiveSoulLevel, 1, 1);
    }

    private void modifyNextSoulBar(){
        this.nextSoulBar.transform.localScale = new Vector3(1 - this.newSoulCoolDown.getPercentageToWait(), 1, 1);
    }
}