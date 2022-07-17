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
    public SoulInventory redSoul;
    public SoulInventory blueSoul;
    private float sumSoulProbabilities;
    public bool isPlayer;
    
    void Start(){
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
        //TODO(omar): Spawn soul
    }
}