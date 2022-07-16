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
    
    private Grid grid;
    private Main main;

    private List<Demon> enemies;
    private int monsterSpawnPoint;

    void Start(){
        main = GetComponent<Main>();
        grid = main.actualGrid;
        monsterSpawnPoint = grid.getHorizontalSize() - 1;

        enemies = new List<Demon>();
        spawnDemon();
    }

    void Update(){

    }

    public void spawnDemon(){
        int lineNumber = Random.Range(0, grid.getVerticalSize());
        Demon newDemon = Demon.instantiateDemon(enemiesPrefabs[0], grid, null, null, 
                                                new DiscreteCoordinate(lineNumber, monsterSpawnPoint), 
                                                main.difficultyFactor, false);
        enemies.Add(newDemon);
    }
}