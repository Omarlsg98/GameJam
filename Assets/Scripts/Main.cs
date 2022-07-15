using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

using static Grid; 

public class Main : MonoBehaviour
{
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

    private int demonsKilled = 0;

    // Start is called before the first frame update
    void Awake()
    {
        actualGrid = new Grid(gridHorizontal, gridVertical,gridMinX, gridMinY, gridHorizontalStep, gridVerticalStep, tilePrefab);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
