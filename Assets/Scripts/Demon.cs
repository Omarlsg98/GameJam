using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using static DiscreteCoordinate;
using static Grid;
using static PartConfiguration;

using static CoolDown;
using static DemonSoundController;


public class Demon : MonoBehaviour
{
    public DemonSoundController soundController;

    private GameObject[] parts;
    private GameObject head;
    private PartData totalStats;

    private float maxLife;
    private float actualLife;

    private DiscreteCoordinate actPosition;
    private CoolDown movementCoolDown;

    private CoolDown spawCoolDown;
    private int spawnIndex = 0;

    private AudioSource audioSource;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Grid grid;
    private float difficultyFactor;
    private bool isPlayer;

    public static Demon instantiateDemon(GameObject prefab, Grid grid, GameObject[] parts, GameObject head, DiscreteCoordinate position, float difficultyFactor, bool isPlayer){
        GameObject demonGameObject = Instantiate(prefab, grid.getTilePosition(position), Quaternion.identity);
        Demon demon = demonGameObject.GetComponent<Demon>();
        demon.setupDemon(grid, parts, head, position, difficultyFactor, isPlayer);
        return demon;
    }

    public void setupDemon(Grid grid, GameObject[] parts, GameObject head, DiscreteCoordinate actPosition, float difficultyFactor, bool isPlayer){
        this.grid = grid;
        grid.getTile(actPosition).isEmpty = false;

        this.parts = parts;
        this.head = head;
        this.totalStats = PartData.getTotalStats(parts);
        this.maxLife = this.totalStats.life;
        this.actualLife = this.maxLife;
        this.movementCoolDown = new CoolDown(1/this.totalStats.movementSpeed);

        this.actPosition = actPosition;
        this.difficultyFactor = difficultyFactor;
        this.isPlayer = isPlayer;

        this.spawCoolDown = new CoolDown(0.2f);
        this.spawnIndex = 0;
    }


    void Start(){
        /*
        GameObject capsuleChild = gameObject.transform.GetChild(0).gameObject;
        animator = capsuleChild.GetComponent<Animator>();        
        spriteRenderer = capsuleChild.GetComponent<SpriteRenderer>();
        */
        audioSource = gameObject.GetComponent<AudioSource>();
        soundController.setAudioSource(audioSource);
        if (isPlayer){
            //spriteRenderer.flipX = true;
        }
    }


    void Update(){
       spawnDemon();
       move(1);
    }

    public void spawnDemon(){
        if(spawnIndex < 6){
            spawCoolDown.updateCoolDown();
            if (spawCoolDown.isReady()){
                GameObject prefab;
                if (spawnIndex < 3){
                    prefab = parts[spawnIndex];
                }
                else if (spawnIndex == 3){
                    prefab = this.head;
                }else{
                    prefab = parts[spawnIndex - 1];
                }
                GameObject part = Instantiate(prefab, gameObject.transform);
                part.transform.position = gameObject.transform.GetChild(spawnIndex).position;
                part.GetComponent<SpriteRenderer>().sortingOrder = spawnIndex;
                spawnIndex += 1;
                spawCoolDown.turnOnCooldown();
            }
        } else if (spawnIndex == 6){
            for (int i = 0; i < 6; i++){
                Destroy(gameObject.transform.GetChild(i).gameObject);
            }
            spawnIndex += 1;
            movementCoolDown.turnOnCooldown();
        }
    }

    public int move(int horizontalAxis)
    {
        if (movementCoolDown.isReady() && isAlive()){     
            DiscreteCoordinate newPosition =  new DiscreteCoordinate(actPosition.y, actPosition.x + horizontalAxis);
            if (grid.verifyPosition(newPosition)){
                grid.getTile(actPosition).isEmpty = true;
                grid.getTile(newPosition).isEmpty = false;
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

    public void applyHit(int damage){
        this.actualLife -= isPlayer? (int) (damage * difficultyFactor) : damage;
        //float lifePercentage = (float)this.actualLife/this.maxLife;
        //spriteRenderer.material.color = new Color(1.0f, lifePercentage, lifePercentage);
        //soundController.reproduceDamage();
        //animateDamage();
        if (!isAlive()){
            grid.getTile(actPosition).isEmpty = true;
            Destroy(gameObject);
        }
    }

    public bool isAlive(){
        return this.actualLife > 0 && this.spawnIndex == 7;
    }
    
    public void attack(){
        /*
        AttackConfig atcConfig = getAttackConfig(selector);
        if (atcConfig.attackCoolDown.isReady() && atcConfig.isUsable() && isAlive()){
            soundController.reproduceAttack();
            animateAttack();
            atcConfig.addUsage();
            Attack attack = Attack.getAttackInstance(isPlayer, actPosition, grid, atcConfig);
            attacksInProgress.Add(attack);
            atcConfig.attackCoolDown.turnOnCooldown();
        }
        */
    }


    private void animateAttack(){
        //animator.SetTrigger("Attack01");
    }

    private void animateDamage(){
        //animator.SetTrigger("Hurt");
    }
}
