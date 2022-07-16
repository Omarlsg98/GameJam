using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using static DiscreteCoordinate;
using static Grid;
using static PartConfiguration;

using static CoolDown;
using static DemonSoundController;
using static Head;
using static Main;
using static MyRandom;

public class Demon : MonoBehaviour
{
    public DemonSoundController soundController;

    private GameObject[] parts;
    public Head head;
    public PartData totalStats;
    public bool hasLegs = true;

    private float maxLife;
    private float actualLife;

    private DiscreteCoordinate actPosition;
    private CoolDown movementCoolDown;
    private CoolDown attackCoolDown;

    private CoolDown vanishCoolDown;
    private CoolDown spawCoolDown;
    private int spawnIndex = 0;
    private int spawnColumnIndex;

    private AudioSource audioSource;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Grid grid;
    private float difficultyFactor;
    private bool isPlayer;

    private Main mainController;
    private List<Demon> adversaryDemons;
    private List<Demon> toScavenge;
    private GameObject lootBag;
    private float currentLoad;
    

    private static string[] partNames = new string[]{"BackArm", "BackLeg", "Chest", "Head", "FrontLeg", "FrontArm"};

    public static Demon instantiateDemon(GameObject prefab, Grid grid, GameObject[] parts, Head head, 
                                        DiscreteCoordinate position, bool isPlayer,
                                        Main main){
        GameObject demonGameObject = Instantiate(prefab, grid.getTilePosition(position), Quaternion.identity);
        Demon demon = demonGameObject.GetComponent<Demon>();
        demon.setupDemon(grid, parts, head, position, isPlayer, main);
        return demon;
    }

    public void setupDemon(Grid grid, GameObject[] parts, Head head, DiscreteCoordinate actPosition, 
                            bool isPlayer, Main main){
        this.grid = grid;
        grid.getTile(actPosition).updateStatus(false, isPlayer);

        this.vanishCoolDown = new CoolDown(60.0f);
        this.vanishCoolDown.turnOnCooldown();
        this.spawCoolDown = new CoolDown(0.15f);
        
        this.mainController = main;
        this.toScavenge = mainController.toScavenge;
        this.isPlayer = isPlayer;
        this.adversaryDemons = main.enemyDemons;

        if (this.isPlayer){
            this.spawnIndex = 0;
            this.spawnColumnIndex = 0;
            this.parts = (GameObject[]) parts.Clone();
            this.hasLegs = parts[1] == null || parts[3] != null;
            this.head = new Head(head);
            this.totalStats = PartData.getTotalStats(parts);
        }else {
            this.spawnColumnIndex = main.gridHorizontal - 1;
            this.spawnIndex = 7;
            this.adversaryDemons = main.playerDemons;
        }

        this.maxLife = this.totalStats.life;
        this.actualLife = this.maxLife;
        this.movementCoolDown = new CoolDown(1/this.totalStats.movementSpeed);
        this.attackCoolDown = new CoolDown(1/this.totalStats.attackSpeed);
        this.lootBag = getChildByName("LootBag");
        this.currentLoad = 0.0f;

        this.actPosition = actPosition;
        this.difficultyFactor = main.difficultyFactor;
    }


    void Start(){
        audioSource = gameObject.GetComponent<AudioSource>();
        soundController.setAudioSource(audioSource);
    }

    void Update(){
        if (spawnDemon())
            return;
        attackCoolDown.updateCoolDown();
        if (isAlive()){
            think();
        }else{
            animateVanishProcess();
        }
    }

    public bool spawnDemon(){
        if(spawnIndex < 6){
            spawCoolDown.updateCoolDown();
            if (spawCoolDown.isReady()){
                GameObject part = null;
                if (spawnIndex < 3){
                    part = parts[spawnIndex];
                }
                else if (spawnIndex == 3){
                    part = Instantiate(this.head.prefab, gameObject.transform);
                }else{
                    part = parts[spawnIndex - 1];
                }
                if (part != null) {
                    part.transform.SetParent(gameObject.transform); 
                    part.transform.position = getChildByName(Demon.partNames[spawnIndex]).transform.position;
                    if (this.hasLegs){
                        part.transform.position -= new Vector3(0.0f, 0.8f, 0.0f);
                    }
                    part.transform.rotation = Quaternion.Euler(0.0f, 0.0f, Random.Range(0.0f, 0.0f));
                    part.transform.GetChild(0).GetComponent<SpriteRenderer>().sortingOrder = spawnIndex;
                    part.transform.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
                    if (spawnIndex != 3)
                        part.transform.GetComponent<PartConfiguration>().setOnField();
                    spawCoolDown.turnOnCooldown();
                }
                spawnIndex += 1;
            }
            return true;
        } else if (spawnIndex == 6){
            for (int i = 0; i < 6; i++){
                Destroy(gameObject.transform.GetChild(i).gameObject);
            }
            spawnIndex += 1;
            movementCoolDown.turnOnCooldown();
        }
        return false;
    }

    public void think(){
        if (head.type == HeadType.Warrior){
            move(0, 1);
            int modifier = isPlayer? totalStats.range : -totalStats.range;
            DiscreteCoordinate attackPosition =  new DiscreteCoordinate(this.actPosition.y, this.actPosition.x + modifier);
            foreach (Demon adversary in adversaryDemons)
            {
                if (adversary.isInPosition(attackPosition) && adversary.isAlive()){
                    attack(adversary);
                    break;
                }
            }
        } else if (head.type == HeadType.Scavenger){
            int horizontalAxis = 0;
            int verticalAxis = 0;
            if (this.toScavenge.Count != 0 && !isLootFull()){
                Demon closestDemonToScavenge = getClosestDemonToScavenge();
                (int verticalDiff, int horizontalDiff) = closestDemonToScavenge.getPositionDiffs(this.actPosition);
                if (horizontalDiff == 1 || horizontalDiff == -1){
                    tryToScavangePart(closestDemonToScavenge);
                    return;
                }
                if (verticalDiff != 0)
                    verticalAxis = verticalDiff > 0 ? 1 : -1;
                if (verticalDiff != 0 && this.actPosition.x != this.spawnColumnIndex){
                    horizontalAxis = -1;
                }else {
                    horizontalAxis = horizontalDiff > 0 ? 1 : -1;
                }
            }else{
                if (this.actPosition.x == this.spawnColumnIndex){
                    if (this.toScavenge.Count == 0) {
                        recallDemon();
                        return;
                    } else if (isLootFull()){
                        depositParts();
                        return;
                    }
                }
                horizontalAxis = -1;
            }
            move(verticalAxis, horizontalAxis);
        }
    }
    
    private int move(int verticalAxis, int horizontalAxis, bool pushedBack = false)
    {   
        horizontalAxis = isPlayer? horizontalAxis : -horizontalAxis;
        if (movementCoolDown.isReady()){     
            DiscreteCoordinate newPosition = null;
            if ((verticalAxis == 1 | verticalAxis == -1) && this.actPosition.x == this.spawnColumnIndex){
                newPosition =  new DiscreteCoordinate(actPosition.y + verticalAxis, actPosition.x);
            }
            else if (horizontalAxis == 1 | horizontalAxis == -1){
                newPosition =  new DiscreteCoordinate(actPosition.y, actPosition.x + horizontalAxis);
            }
            if (grid.verifyPosition(newPosition) && newPosition != null){
                if(!pushedBack)
                    flipDemon(horizontalAxis);
                grid.getTile(actPosition).updateStatus(true, isPlayer);
                grid.getTile(newPosition).updateStatus(false, isPlayer);
                this.actPosition = newPosition;
                gameObject.transform.position = grid.getTilePosition(newPosition);
                movementCoolDown.turnOnCooldown();
                //soundController.reproduceMovement();
                return 1;
            }else {
                return 0;
            }
        } else {
            movementCoolDown.updateCoolDown();
        }
        return -1;
    }

    public void attack(Demon adversary){
        if(attackCoolDown.isReady()){
            animateAttack();
            adversary.applyHit(this.totalStats.damage);
            attackCoolDown.turnOnCooldown();
        }
    }
    
    private void tryToScavangePart(Demon deadBody){
        if(attackCoolDown.isReady()){
            animateAttack(); //TODO(felivans): animateScavange
            GameObject part = deadBody.removePart();
            if(MyRandom.randomBool(this.totalStats.luck)){
                part.transform.parent = this.lootBag.transform;
                part.transform.position = this.lootBag.transform.position;
                part.transform.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
                part.transform.rotation = Quaternion.Euler(0.0f, 0.0f, Random.Range(0.0f, 360.0f));
                SpriteEffects.changeSpriteAlpha(part.transform.GetChild(0).GetComponent<SpriteRenderer>(), 1.0f);
                this.currentLoad += part.GetComponent<PartConfiguration>().partData.weight;
            } else {
                Destroy(part);
            }
            attackCoolDown.turnOnCooldown();
        }
    }

    public GameObject removePart(){
        GameObject part = null;
        if(gameObject.transform.childCount > 0){
            part = gameObject.transform.GetChild(0).gameObject;
            part.transform.parent = null;
            if (gameObject.transform.childCount - 1 == 0){
                destroyDemon();
            }
        }
        return part;
    }

    private void flipDemon(int horizontalAxis){
        if (horizontalAxis == 0)
            return;
        float yRotation = horizontalAxis == 1? 0.0f : 180.0f;
        gameObject.transform.rotation = Quaternion.Euler(0.0f, yRotation, 0.0f);
    }

    public void applyPushBack(){
        movementCoolDown.turnOffCooldown();
        attackCoolDown.turnOnCooldown();
        move(0, -1, true);
    }

    public void applyHit(float damage){
        this.actualLife -= isPlayer? (damage * difficultyFactor) : damage;
        //float lifePercentage = (float)this.actualLife/this.maxLife;
        //spriteRenderer.material.color = new Color(1.0f, lifePercentage, lifePercentage);
        //soundController.reproduceDamage();
        //animateDamage();
        if (!isAlive()){
            grid.getTile(actPosition).updateStatus(true, isPlayer);
            mainController.toScavenge.Add(this);
            freeScavengeLoot();
            animateDead();
        }
    }

    private bool isLootFull(){
        return this.currentLoad > this.totalStats.loadCapacity;
    }
    public bool isAlive(){
        return this.actualLife > 0 && this.spawnIndex == 7;
    }

    private void freeScavengeLoot(){
        for (int i = 0; i < this.lootBag.transform.childCount; i++){
            this.lootBag.transform.GetChild(i).parent = gameObject.transform;
        }
        Destroy(this.lootBag);
    }

    private void animateDead(){
        for (int i = 0; i < gameObject.transform.childCount; i++){
            Transform child = gameObject.transform.GetChild(i);
            if (child == null)
                continue;
            if (child.name == "Head")
                Destroy(child.gameObject);
            Rigidbody2D rigidBody = child.gameObject.GetComponent<Rigidbody2D>();
            if(rigidBody != null)
                rigidBody.bodyType = RigidbodyType2D.Dynamic;
        }
    }

    private void animateVanishProcess(){
        vanishCoolDown.updateCoolDown();
        if (vanishCoolDown.isReady()){
            destroyDemon();
            return;
        }
        float vanishFactor = vanishCoolDown.getPercentageToWait();
        vanishFactor = vanishFactor < 0.2f ? 0.2f : vanishFactor;
        for (int i = 0; i < gameObject.transform.childCount; i++){
            Transform child = gameObject.transform.GetChild(i).GetChild(0);
            if (child == null)
                continue;
            SpriteRenderer renderer = child.gameObject.GetComponent<SpriteRenderer>();
            SpriteEffects.changeSpriteAlpha(renderer, vanishFactor);
        } 
    }

    private void animateAttack(){
        for (int i = 0; i < gameObject.transform.childCount; i++){
            Animator animator = gameObject.transform.GetChild(i).gameObject.GetComponent<Animator>();
            if (animator != null)
                animator.SetTrigger("Attacking");
        }
    }

    private void animateDamage(){
        //animator.SetTrigger("Hurt");
    }

    private GameObject getChildByName(string name){
        for (int i = 0; i <= gameObject.transform.childCount; i++){
            Transform child = gameObject.transform.GetChild(i);
            if(child.name == name){
                return child.gameObject;
            }
        }
        return null;
    }

    public bool isInPosition(DiscreteCoordinate position){
        return this.actPosition.isEquals(position);
    }

    public int getDistanceTo(DiscreteCoordinate position){
        return this.actPosition.getDistanceTo(position);
    }

    public (int, int) getPositionDiffs(DiscreteCoordinate position){
        int verticalDiff = this.actPosition.y - position.y;
        int horizontalDiff = this.actPosition.x - position.x;
        return (verticalDiff, horizontalDiff);
    }

    private Demon getClosestDemonToScavenge(){
        Demon closest = null;
        int minDistance = 10000;
        foreach(Demon deadBody in toScavenge){
            int distance = deadBody.getDistanceTo(this.actPosition);
            if (distance < minDistance){
                minDistance = distance;
                closest = deadBody;
            }
        }
        return closest;
    }

    private void destroyDemon(){
        if(this.isPlayer){
            mainController.playerDemons.Remove(this);
        } else {
            mainController.enemyDemons.Remove(this);
        }
        mainController.toScavenge.Remove(this);
        Destroy(gameObject);
    }

    private void recallDemon(){
        depositParts();
        List<GameObject> parts = new List<GameObject>();
        for (int i = 0; i < this.gameObject.transform.childCount ; i++){
            GameObject part = gameObject.transform.GetChild(i).gameObject;
            if (part.tag == "NotLootable"){
                continue;
            }
            parts.Add(part);
        }
        this.putPartsInBox(parts);
        grid.getTile(actPosition).updateStatus(true, isPlayer);
        destroyDemon();
    }

    private void depositParts(){
        List<GameObject> parts = new List<GameObject>();
         for (int i = 0; i < this.lootBag.transform.childCount; i++){
            GameObject part = this.lootBag.transform.GetChild(i).gameObject;
            parts.Add(part);
        }
        this.putPartsInBox(parts);
        this.currentLoad = 0;
    }

    private void putPartsInBox(List<GameObject> parts){
        foreach(GameObject part in parts){
            if (isPlayer){
                mainController.playerController.putPartInBox(part);
            }else{
                Destroy(part);
            }
        }
    }
}
