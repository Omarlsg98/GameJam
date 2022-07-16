using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static Grid; 
using static Demon;


public class Main : MonoBehaviour
{   
    public GameObject gridHolder;
    public Grid actualGrid;
    public float difficultyFactor = 0.5f;
    public int gridHorizontal = 30;
    public int gridVertical = 5;
    public float gridMinX = -3.0f;
    public float gridMinY = -1.0f;
    public float gridHorizontalStep = 1.5f;
    public float gridVerticalStep = 1.5f;
    public GameObject tilePrefab;

    public GameObject textKilled;
    public List<Demon> playerDemons;
    public List<Demon> enemyDemons;


    void Awake()
    {
        actualGrid = new Grid(gridHorizontal, gridVertical,gridMinX, gridMinY, gridHorizontalStep, gridVerticalStep, tilePrefab, gridHolder);
    }

    void Update()
    {

    }
}
