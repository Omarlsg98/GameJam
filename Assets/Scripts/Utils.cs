using UnityEngine;

[System.Serializable]
public class CoolDown
{
    public float timeToWait;
    private float actualTimeLeft = 0;

    public void updateCoolDown(){
        if (!isReady()){
            actualTimeLeft -= Time.deltaTime;
        }
    }

    public void turnOnCooldown(){
        actualTimeLeft = (float)timeToWait;
    }

    public bool isReady(){
        return actualTimeLeft <= 0;
    }
}

public class MyRandom{
    public static bool randomBool(float probability){
        return (Random.value < probability);
    }

    public static DiscreteCoordinate randomCoordinate(int min_x, int max_x, int min_y, int max_y){
        int randomX = Random.Range(min_x, max_x);
        int randomY = Random.Range(min_y, max_y);
        return new DiscreteCoordinate(randomX, randomY);
    }
}