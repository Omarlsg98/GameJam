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
    public float demonSpawnProbability = 0.005f;
    
    private Grid grid;
    private Main main;

    private List<Demon> enemies;
    private int monsterSpawnPoint;

    void Start(){
        main = GetComponent<Main>();
        grid = main.actualGrid;
        monsterSpawnPoint = grid.getHorizontalSize() - 1;

        enemies = new List<Demon>();
        main.enemyDemons = enemies;
        spawnDemon();
    }

    void Update(){
        if(MyRandom.randomBool(demonSpawnProbability)){
            spawnDemon();
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