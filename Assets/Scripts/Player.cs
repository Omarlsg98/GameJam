using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static Grid;
using static Main;
using static DiscreteCoordinate;
using static Demon;

public class Player : MonoBehaviour
{
    public GameObject demon;
    public GameObject[] parts = new GameObject[5];
    public GameObject head;
    
    private Grid grid;

    void Start(){
        Main main = GetComponent<Main>();
        grid = main.actualGrid;

        Demon.instantiateDemon(demon, grid, parts, head, new DiscreteCoordinate(0,0), main.difficultyFactor, true);
    }
}