using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static DiscreteCoordinate;

public class GridTile
{
    private GameObject tile;
    public bool isEmpty;
    public DiscreteCoordinate postionInGrid;

    public GridTile(GameObject prefab, Vector3 position, DiscreteCoordinate postionInGrid, GameObject gridHolder){
        this.tile = ScriptableObject.Instantiate(prefab, position, Quaternion.identity);
        this.tile.transform.parent = gridHolder.transform;
        this.postionInGrid = postionInGrid;
        this.isEmpty = true;
    }

    public Vector3 getCoordinates() {
        return tile.transform.position;
    }

    public Transform getTransform(){
        return tile.transform;
    } 
}