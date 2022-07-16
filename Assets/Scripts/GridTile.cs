using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static DiscreteCoordinate;

public class GridTile
{
    private GameObject tile;
    private bool isEmpty;
    public DiscreteCoordinate postionInGrid;
    public bool isPlayerOnTile;

    public GridTile(GameObject prefab, Vector3 position, DiscreteCoordinate postionInGrid, 
                GameObject gridHolder, bool isPlayerOnTile){
        this.tile = ScriptableObject.Instantiate(prefab, position, Quaternion.identity);
        this.tile.transform.parent = gridHolder.transform;
        this.postionInGrid = postionInGrid;
        this.isPlayerOnTile = isPlayerOnTile;
        this.isEmpty = true;
    }

    public void updateStatus(bool isEmpty, bool isPlayer){
        this.isPlayerOnTile = isPlayer && !isEmpty;
        this.isEmpty = isEmpty;
    }

    public bool getIsEmpty(){
        return this.isEmpty;
    }

    public Vector3 getCoordinates() {
        return tile.transform.position;
    }

    public Transform getTransform(){
        return tile.transform;
    } 
}