using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static Grid;
using static Main;
using static DiscreteCoordinate;
using static Demon;
using static Head;


public class EnemyHub : MonoBehaviour
{
    public List<GameObject> enemiesPrefabs;
    public List<float> enemiesProbabilities;
    public int demonSpawnVariability = 5;
    public int demonWaveIncrease = 2;
    public int waveToIncrementRails = 10;
    public CoolDown spawnBetweenMonsterCooldown;
    public CoolDown waveCoolDown;

    private Grid grid;
    private Main main;
    private List<Demon> enemies;
    private int monsterSpawnPoint;

    private int waveNum;
    private int demonsToSpawn;
    private List<int> availableLines;
    private float totalProbability;
    

    void Start(){
        main = GetComponent<Main>();
        grid = main.actualGrid;
        monsterSpawnPoint = grid.getHorizontalSize() - 1;

        enemies = new List<Demon>();
        main.enemyDemons = enemies;
        waveNum = 0;
        demonsToSpawn = 0;
        foreach(float prob in enemiesProbabilities){
            totalProbability += prob;
        }
    }

    void Update(){
        spawnWave();

        spawnBetweenMonsterCooldown.updateCoolDown();
        if (demonsToSpawn >= 0 && spawnBetweenMonsterCooldown.isReady()){
            spawnDemon();
            spawnBetweenMonsterCooldown.turnOnCooldown();
            waveCoolDown.turnOnCooldown();
        }
    }

    public void spawnWave(){
        waveCoolDown.updateCoolDown();
        if(waveCoolDown.isReady()){
            int lowRange = 1 + waveNum * demonWaveIncrease;
            int upperRange = 1 + demonSpawnVariability + waveNum * demonWaveIncrease;
            this.demonsToSpawn = Random.Range(lowRange, upperRange);

            int numberOfRails = (int)(waveNum/waveToIncrementRails) + Random.Range(0,2);
            if (numberOfRails > grid.getVerticalSize()){
                numberOfRails = grid.getVerticalSize();
            }else if (numberOfRails == 0){
                numberOfRails = 1;
            }
            availableLines = new List<int>();
            for (int i = 0; i < numberOfRails; i++){
                availableLines.Add(Random.Range(0, grid.getVerticalSize()));
            }
            
            this.waveNum += 1;
            waveCoolDown.turnOnCooldown();
        }
    }
    public void spawnDemon(){
        DiscreteCoordinate newPosition = new DiscreteCoordinate(getLineNumber(), monsterSpawnPoint);

        foreach(Demon adversary in main.playerDemons){
            if (adversary.isInPosition(newPosition)){
                adversary.applyPushBack();
            }
        }
        int randomIndex = chooseRandomEnemyType();
        Demon newDemon = Demon.instantiateDemon(enemiesPrefabs[randomIndex], grid, null, null, 
                                                newPosition, 
                                                false, main, 200);
        for (int i = 0; i < newDemon.transform.childCount; i++){
            Transform child = newDemon.transform.GetChild(i);
            if(child.transform.GetComponent<PartConfiguration>()!=null){
                newDemon.transform.GetChild(i).transform.GetComponent<PartConfiguration>().partData.setRandomRareness();
            }
        }
        enemies.Add(newDemon);
        this.demonsToSpawn -= 1;
    }

    private int getLineNumber(){
        return availableLines[Random.Range(0, availableLines.Count)];
    }

    private int chooseRandomEnemyType(){
        int index = -1;
        float rand = Random.Range(0.0f, totalProbability);
        float probAcumulada = 0;
        foreach(float prob in enemiesProbabilities){
            probAcumulada += prob;
            index +=1;
            if(rand < probAcumulada){
                return index;
            }
        }
        return index;
    }
}