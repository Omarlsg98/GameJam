using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static Grid;
using static Main;
using static DiscreteCoordinate;
using static Demon;
using static Head;
using static SpriteEffects;
using static SoulType;
using static HeadQuarter;
using static StatsDisplayer;

public class Player : MonoBehaviour
{
    public StatsDisplayer statsDisplayer;
    public GameObject demonPrefab;
    private GameObject[] parts = new GameObject[5];
    private Head head;

    public GameObject demonTablePlaceHolder;
    public GameObject partSpawnPoint;
    public GameObject partsParent;
    public GameObject rowSelectorTile;
    public CoolDown rowSelectorCoolDown;

    private int rowSelected;
    private GameObject headInTable;

    private Grid grid;
    private Main main;
    private HeadQuarter playerHq;

    private List<Demon> demons;

    public int[] soulInventory = new int[]{0, 0, 0};
    public SoulsDisplayer soulsDisplayer;

    void Start(){
        main = GetComponent<Main>();
        playerHq = GetComponent<HeadQuarter>();
        grid = main.actualGrid;
        demons = new List<Demon>();
        main.playerDemons = demons;
        main.playerController = this;
        rowSelected = -1;
        moveSelectedRow(0);
        parts = new GameObject[5];
        displayCurrentStats();
        soulsDisplayer.updateSoulsCounter(0, 0, 0);
    }

    void Update(){
        moveSelectedRowInput();
    }

    private void moveSelectedRowInput(){
        rowSelectorCoolDown.updateCoolDown();
        if (rowSelectorCoolDown.isReady()){
            float verticalAxis = Input.GetAxis("Vertical");
            int newRowSelected = rowSelected;
            if (verticalAxis > 0){
                newRowSelected += 1;
            }else if (verticalAxis < 0){
                newRowSelected -= 1;
            }
            moveSelectedRow(newRowSelected);
        }
    }

    private void moveSelectedRow(int newRowSelected){
        DiscreteCoordinate newPosition = new DiscreteCoordinate(newRowSelected, 0);
        if (rowSelected != newRowSelected && grid.verifyIsInRange(newPosition)){
            rowSelected = newRowSelected;
            rowSelectorTile.transform.position = new Vector3(rowSelectorTile.transform.position.x, 
                                                            grid.getTilePosition(newPosition).y,
                                                            rowSelectorTile.transform.position.z);
            rowSelectorCoolDown.turnOnCooldown();
        }
    }

    public void addBodyPart(GameObject part){
        PartData partData = part.GetComponent<PartConfiguration>().partData;
        int index = -1;
        int indexChild = -1;
        bool enter = false;
        if (partData.type == PartType.Limb){
            for (int i = 0; i < parts.Length; i++){
                if (parts[i] == null && i != 2){
                    enter = true;
                    index = i;
                    indexChild = i > 2? i + 1: i;
                    break;
                }
            }
        }else if (partData.type == PartType.Chest && parts[2] == null){
           index = 2;
           indexChild = 2;
           enter = true;
        }

        if (!enter){
            putPartInBox(part);
            //TODO(FELIVANS): PONDRA ALGO (PARTE LLENA)
            return;
        }

        parts[index] = part;
        putPartInTable(part, indexChild);
        displayCurrentStats();
    }

    public void removeBodyPart(GameObject part){
        for (int i = 0; i < parts.Length; i++){
            if (parts[i] == part){
                parts[i] = null;
                break;
            }
        }
        putPartInBox(part);
    }

    public void changeHead(Head head, GameObject surgeryHead){
        if (headInTable != null){
            Destroy(headInTable);
        }
        this.head = head;
        this.headInTable = Instantiate(surgeryHead, partsParent.transform);
        putPartInTable(this.headInTable, 3);
    }

    public void addSoulToTable(SoulType soulType){
        soulInventory[(int)soulType] += 1;
        soulsDisplayer.updateSoulsCounter(soulInventory[0], soulInventory[1], soulInventory[2]);
    }

    public bool removeSoulFromTable(SoulType soulType){
        if (soulInventory[(int)soulType] > 0){
            soulInventory[(int)soulType] -= 1;
            soulsDisplayer.updateSoulsCounter(soulInventory[0], soulInventory[1], soulInventory[2]);
            return true;
        }
        return false;
    }

    public void putPartInBox(GameObject part){
        PartConfiguration partConf = part.GetComponent<PartConfiguration>();
        if (partConf != null){
            partConf.inBox = true;
            partConf.onTable = false;
        }
        part.layer = LayerMask.NameToLayer("BoxLayer");
        part.transform.SetParent(partsParent.transform);
        part.transform.position = partSpawnPoint.transform.position;
        part.transform.rotation = Quaternion.Euler(0.0f, 0.0f, Random.Range(0.0f, 360.0f));
        part.transform.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        SpriteRenderer renderer = part.transform.GetChild(0).GetComponent<SpriteRenderer>();
        renderer.sortingOrder = 103;
        renderer.flipX = false;
        SpriteEffects.changeSpriteAlpha(renderer, 1.0f);
    }

    private void putPartInTable(GameObject part, int indexChild){
        PartConfiguration partConf = part.GetComponent<PartConfiguration>();
        if (partConf != null){
            partConf.inBox = false;
            partConf.onTable = true;
        }
        part.transform.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
        GameObject placeHolderPart = demonTablePlaceHolder.transform.GetChild(indexChild).gameObject;
        part.transform.position = placeHolderPart.transform.position;
        part.transform.rotation = placeHolderPart.transform.rotation;
    }

    private int consumeTableSouls(){
        int totalEnergyObtained = 0;
        for (int i = 0; i < soulInventory.Length; i++){
            switch ((SoulType) i)
            {
                case SoulType.blue:
                for (int j = 0; j < soulInventory[i]; j++)
                    totalEnergyObtained += playerHq.blueSoul.getSoulData().consumeSoul();
                break;

                case SoulType.green:
                for (int j = 0; j < soulInventory[i]; j++)
                    totalEnergyObtained += playerHq.greenSoul.getSoulData().consumeSoul();
                break;

                case SoulType.red:
                for (int j = 0; j < soulInventory[i]; j++)
                    totalEnergyObtained += playerHq.redSoul.getSoulData().consumeSoul();
                break;

                default:
                break;
            }
            soulInventory[i] = 0;
        }
        return totalEnergyObtained;
    }

    private bool hasSouls(){
        for (int i = 0; i < soulInventory.Length; i++){
            if(soulInventory[i] > 0)
                return true;
        }
        return false;
    }

    public void spawnDemon(){
        DiscreteCoordinate newPosition = new DiscreteCoordinate(rowSelected, 0);
        if (head != null && parts[2] != null && !grid.getTile(newPosition).isPlayerOnTile && hasSouls()){
            foreach(Demon adversary in main.enemyDemons){
                if (adversary.isInPosition(newPosition)){
                    adversary.applyPushBack();
                }
            }
            int totalEnergy = consumeTableSouls();
            Demon newDemon = Demon.instantiateDemon(demonPrefab, grid, parts, head, 
                                                    newPosition,
                                                    true, main, totalEnergy);
            demons.Add(newDemon);
            head = null;
            Destroy(headInTable);
            for (int i = 0; i < parts.Length; i++){
                parts[i] = null;
            }
        } else {
            //TODO(felivans): spawn demon withou enough parts feedback.
        }
    }

    public void displayCurrentStats(){
        this.statsDisplayer.changeDataDisplayed(PartData.getTotalStats(parts), true);
    }
}