using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using static DiscreteCoordinate;
using static Grid;
using static PartConfiguration;

using static CoolDown;
using static DemonSoundController;
using static Head;

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

    private AudioSource audioSource;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Grid grid;
    private float difficultyFactor;
    private bool isPlayer;
    private List<Demon> adversaryDemons;

    private static string[] partNames = new string[]{"BackArm", "BackLeg", "Chest", "Head", "FrontLeg", "FrontArm"};

    public static Demon instantiateDemon(GameObject prefab, Grid grid, GameObject[] parts, Head head, 
                                        DiscreteCoordinate position, float difficultyFactor, bool isPlayer,
                                        List<Demon> adversaryDemons){
        GameObject demonGameObject = Instantiate(prefab, grid.getTilePosition(position), Quaternion.identity);
        Demon demon = demonGameObject.GetComponent<Demon>();
        demon.setupDemon(grid, parts, head, position, difficultyFactor, isPlayer, adversaryDemons);
        return demon;
    }

    public void setupDemon(Grid grid, GameObject[] parts, Head head, DiscreteCoordinate actPosition, 
                            float difficultyFactor, bool isPlayer, List<Demon> adversaryDemons){
        this.grid = grid;
        grid.getTile(actPosition).updateStatus(false, isPlayer);
        this.adversaryDemons = adversaryDemons;

        this.vanishCoolDown = new CoolDown(20.0f);
        this.vanishCoolDown.turnOnCooldown();
        this.spawCoolDown = new CoolDown(0.15f);
        this.spawnIndex = 0;
        
        this.isPlayer = isPlayer;

        if (this.isPlayer){
            this.parts = (GameObject[]) parts.Clone();
            this.hasLegs = parts[1] == null || parts[3] != null;
            this.head = new Head(head);
            this.totalStats = PartData.getTotalStats(parts);
        }else {
            this.spawnIndex = 7;
        }

        this.maxLife = this.totalStats.life;
        this.actualLife = this.maxLife;
        this.movementCoolDown = new CoolDown(1/this.totalStats.movementSpeed);
        this.attackCoolDown = new CoolDown(1/this.totalStats.attackSpeed);

        this.actPosition = actPosition;
        this.difficultyFactor = difficultyFactor;
    }


    void Start(){
        audioSource = gameObject.GetComponent<AudioSource>();
        soundController.setAudioSource(audioSource);
    }

    void Update(){
        spawnDemon();
        attackCoolDown.updateCoolDown();
        if (isAlive()){
            think();
        }else{
            animateVanishProcess();
        }
    }

    public void spawnDemon(){
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
                    part.transform.position = getChildPosition(Demon.partNames[spawnIndex]);
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
        } else if (spawnIndex == 6){
            for (int i = 0; i < 6; i++){
                Destroy(gameObject.transform.GetChild(i).gameObject);
            }
            spawnIndex += 1;
            movementCoolDown.turnOnCooldown();
        }
    }

    public void think(){
        if (head.type == HeadType.Warrior){
            move(this.isPlayer? 1 : -1);
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
            move(this.isPlayer? 1 : -1);
        }
    }
    
    public void attack(Demon adversary){
        if(attackCoolDown.isReady()){
            animateAttack();
            adversary.applyHit(this.totalStats.damage);
            attackCoolDown.turnOnCooldown();
        }
    }
    
    public int move(int horizontalAxis)
    {
        if (movementCoolDown.isReady()){     
            DiscreteCoordinate newPosition =  new DiscreteCoordinate(actPosition.y, actPosition.x + horizontalAxis);
            if (grid.verifyPosition(newPosition)){
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

    public void applyPushBack(){
        movementCoolDown.turnOffCooldown();
        attackCoolDown.turnOnCooldown();
        move(this.isPlayer? -1 : 1);
    }

    public void applyHit(float damage){
        this.actualLife -= isPlayer? (damage * difficultyFactor) : damage;
        //float lifePercentage = (float)this.actualLife/this.maxLife;
        //spriteRenderer.material.color = new Color(1.0f, lifePercentage, lifePercentage);
        //soundController.reproduceDamage();
        //animateDamage();
        if (!isAlive()){
            grid.getTile(actPosition).updateStatus(true, isPlayer);
            animateDead();
        }
    }

    public bool isAlive(){
        return this.actualLife > 0 && this.spawnIndex == 7;
    }

    private void animateDead(){
        for (int i = 0; i < gameObject.transform.childCount; i++){
            gameObject.transform.GetChild(i).gameObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        }
    }

    private void animateVanishProcess(){
        vanishCoolDown.updateCoolDown();
        if (vanishCoolDown.isReady()){
            Destroy(gameObject);
        }
        float vanishFactor = vanishCoolDown.getPercentageToWait();
        vanishFactor = vanishFactor < 0.2f ? 0.2f : vanishFactor;
        for (int i = 0; i < gameObject.transform.childCount; i++){
            SpriteRenderer renderer = gameObject.transform.GetChild(i).GetChild(0).gameObject.GetComponent<SpriteRenderer>();
            Color tmp = renderer.material.color;
            tmp.a = vanishFactor;
            renderer.material.color = tmp;
        } 
    }

    private void animateAttack(){
        for (int i = 0; i < gameObject.transform.childCount; i++){
            Animator animator = gameObject.transform.GetChild(i).gameObject.GetComponent<Animator>();
            animator.SetTrigger("Attacking");
        }
    }

    private void animateDamage(){
        //animator.SetTrigger("Hurt");
    }

    private Vector3 getChildPosition(string name){
        for (int i = 0; i <= 6; i++){
            Transform child = gameObject.transform.GetChild(i);
            if(child.name == name){
                return child.position;
            }
        }
        return new Vector3(0.0f, 0.0f, 0.0f);
    }

    public bool isInPosition(DiscreteCoordinate position){
        return this.actPosition.isEquals(position);
    }
}
