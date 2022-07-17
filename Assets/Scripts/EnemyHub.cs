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
    public int demonSpawnProbability = 40;
    
    private Grid grid;
    private Main main;

    public float timeForEachWave = 10.0f;
    private CoolDown waveCoolDown;
    private System.Random ranProbSpawn;

    private List<Demon> enemies;
    private int monsterSpawnPoint;

    void Start(){
        main = GetComponent<Main>();
        grid = main.actualGrid;
        monsterSpawnPoint = grid.getHorizontalSize() - 1;

        enemies = new List<Demon>();
        main.enemyDemons = enemies;
        waveCoolDown = new CoolDown(timeForEachWave);
        waveCoolDown.turnOnCooldown();
        ranProbSpawn = new System.Random();
        spawnDemon();
    }

    void Update(){
        spawnWave(Time.unscaledTime);
    }

    public void spawnWave(float currTime){
        waveCoolDown.updateCoolDown();
        if(waveCoolDown.isReady()){
            int waveNum = (int)System.Math.Round(currTime/timeForEachWave);
            int totalDemonsTrySpawn = waveNum*waveNum;
            for (int i = 0; i < totalDemonsTrySpawn; i++) {
                if(ranProbSpawn.Next(1,101) <= demonSpawnProbability){
                    spawnDemon();
                }
            }
            Debug.Log(waveNum);
            waveCoolDown.turnOnCooldown();
        }
    }
    public void spawnDemon(){
        int lineNumber = Random.Range(0, grid.getVerticalSize());
        DiscreteCoordinate newPosition = new DiscreteCoordinate(lineNumber, monsterSpawnPoint);

        foreach(Demon adversary in  main.playerDemons){
            if (adversary.isInPosition(newPosition)){
                adversary.applyPushBack();
            }
        }

        Demon newDemon = Demon.instantiateDemon(enemiesPrefabs[0], grid, null, null, 
                                                newPosition, 
                                                false, main, 200);
        enemies.Add(newDemon);
    }
}