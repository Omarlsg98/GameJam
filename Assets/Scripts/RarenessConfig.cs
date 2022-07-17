using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static Main;
using static Player;

public enum RarenessType {
    Common,
    Uncommon,
    Rare,
    Epic,
    Legendary
}
[System.Serializable]
public class RarenessData{
    public RarenessType type;
    public int probability;
    public float multiplier;
    public Color color;
}

public class RarenessConfig : MonoBehaviour
{
    public RarenessData[] rarenessTypes;

    public RarenessData getRandomRareness(){
        System.Random rand = new System.Random();
        int valRand = rand.Next(1, 101);
        for (int i = 0; i < rarenessTypes.Length; i++){
            if(valRand < rarenessTypes[i].probability){
                return rarenessTypes[i];
            }
        }
        return rarenessTypes[0];
    }
}