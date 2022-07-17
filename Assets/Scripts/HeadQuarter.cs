using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static Main;

using static CoolDown;
using static SoulInventory;
using static Player;

using TMPro;

public class HeadQuarter : MonoBehaviour
{
    public GameObject chestPrefab;
    public List<GameObject> parts;
    public float soulsLevel;
    public float objectiveSoulLevel;
    public CoolDown newSoulCoolDown;

    public SoulInventory greenSoul;
    public SoulInventory blueSoul;
    public SoulInventory redSoul;
    private float sumSoulProbabilities;
    public bool isPlayer;
    public GameObject soulLevelBar;
    public TextMeshProUGUI soulLevelText;
    public GameObject nextSoulBar;

    private Player playerController; 
    public SoulsDisplayer soulsDisplayer;
    public SoulsExtractedDisplayer soulExtractedDisplayer;

    private Main mainController;
    
    void Start(){
        playerController = GetComponent<Player>();
        mainController = GetComponent<Main>();
        modifySoulBar();
        greenSoul.setSoulData();
        redSoul.setSoulData();
        blueSoul.setSoulData();
        this.sumSoulProbabilities = greenSoul.getSoulData().spawnProbability + redSoul.getSoulData().spawnProbability + blueSoul.getSoulData().spawnProbability;
        soulsDisplayer.setSoulsValues(greenSoul.getSoulData(), blueSoul.getSoulData(), redSoul.getSoulData());
        soulsDisplayer.updateSoulsCounter(greenSoul.inventory, blueSoul.inventory, redSoul.inventory);
    }

    void Update(){
        if(!mainController.gameIsOnPause){
            if (isPlayer){
                if (hasWon()){
                    Debug.Log("WOOOOON");
                } else if (hasLost()){
                    Debug.Log("LOOOOOST");
                }
                generateRandomNewSoul();
            }
        }
    }

    public void spawnPart(SoulType soulType){
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
        if(souls != 0){
            soulExtractedDisplayer.setSoulsObtained(new int[1]{souls});
            soulsDisplayer.updateSoulsCounter(greenSoul.inventory, blueSoul.inventory, redSoul.inventory);
            int moreParts = (int)(souls/100);
            playerController.putPartInBox(Instantiate(chestPrefab, gameObject.transform));
            for (int i = 0; i < moreParts; i++){
                playerController.putPartInBox(Instantiate(parts[Random.Range(0, parts.Count)], gameObject.transform));
            }
        }
    }

    public bool hasWon(){
        return soulsLevel >= objectiveSoulLevel;
    }

     public bool hasLost(){
        return getTotalInventory() == 0;
    }

    public void putSoulTable(SoulType soulType){
        if (addSoulsToStock(soulType, -1)){
            playerController.addSoulToTable(soulType);
        }
    }

    public void removeSoulTable(SoulType soulType){
        if (playerController.removeSoulFromTable(soulType)){
            addSoulsToStock(soulType, 1);
        }
    }


    public void generateRandomNewSoul(){
        newSoulCoolDown.updateCoolDown();
        modifyNextSoulBar();
        if (newSoulCoolDown.isReady()){
            SoulType soulToAdd = generateRandomSoulType();
            addSoulsToStock(soulToAdd, 1);
            newSoulCoolDown.turnOnCooldown();
        }
    }

    public SoulType generateRandomSoulType(){
        SoulType soulType;
        float randomNumber = Random.Range(0, sumSoulProbabilities);
        if (randomNumber < greenSoul.getSoulData().spawnProbability){
            soulType = SoulType.green;
        }else if(randomNumber < greenSoul.getSoulData().spawnProbability + redSoul.getSoulData().spawnProbability){
            soulType = SoulType.red;
        }else{
            soulType = SoulType.blue;
        }
        return soulType;
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
        soulExtractedDisplayer.setSoulsObtained(new int[1]{souls});
        soulsDisplayer.updateSoulsCounter(greenSoul.inventory, blueSoul.inventory, redSoul.inventory);
        this.soulsLevel += souls;
        modifySoulBar();
    }

    public bool addSoulsToStock(SoulType soulType, int souls){
        bool valueUpdated = false;
        switch (soulType)
        {
            case SoulType.blue:
                if (souls + blueSoul.inventory >= 0){
                    blueSoul.inventory += souls;
                    valueUpdated = true;
                }
            break;

            case SoulType.green:
                if (souls + greenSoul.inventory >= 0){
                    greenSoul.inventory += souls;
                    valueUpdated = true;
                }
            break;

            case SoulType.red:
                if (souls + redSoul.inventory >= 0){
                    redSoul.inventory += souls;
                    valueUpdated = true;
                }
            break;

            default:
            break;
        }
        if (valueUpdated){
            soulsDisplayer.updateSoulsCounter(greenSoul.inventory, blueSoul.inventory, redSoul.inventory);
            return true;
        }
        return valueUpdated;
    }

    public int getTotalInventory(){
        return greenSoul.inventory + blueSoul.inventory + redSoul.inventory;
    }
    public void applyHit(){
        if(isPlayer){
            if (getTotalInventory() != 0){
                SoulType soulToAdd = generateRandomSoulType();
                while(!addSoulsToStock(soulToAdd, -1)){
                    soulToAdd = generateRandomSoulType();
                }
            }
        }
        //TODO(omar): Spawn soul
    }

    private void modifySoulBar(){
        if(isPlayer){
            soulLevelText.text = this.soulsLevel+"/"+this.objectiveSoulLevel;
            this.soulLevelBar.transform.localScale = new Vector3(this.soulsLevel/this.objectiveSoulLevel, 1, 1);
        }
    }

    private void modifyNextSoulBar(){
        this.nextSoulBar.transform.localScale = new Vector3(1 - this.newSoulCoolDown.getPercentageToWait(), 1, 1);
    }
}