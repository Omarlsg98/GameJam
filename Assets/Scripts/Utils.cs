using UnityEngine;

[System.Serializable]
public class CoolDown
{
    public float timeToWait;
    private float actualTimeLeft = 0;

    public CoolDown(float timeToWait){
        this.timeToWait = timeToWait;
    }

    public float getPercentageToWait(){
        return this.actualTimeLeft/this.timeToWait;
    }

    public void updateCoolDown(){
        if (!isReady()){
            actualTimeLeft -= Time.deltaTime;
        }
    }

    public void turnOnCooldown(){
        actualTimeLeft = (float)timeToWait;
    }

    public void turnOffCooldown(){
        actualTimeLeft = 0;
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

public class SpriteEffects{
    public static void changeSpriteAlpha(SpriteRenderer renderer, float alpha){
        if (renderer == null)
            return;
        Color tmp = renderer.material.color;
        tmp.a = alpha;
        renderer.material.color = tmp;
    }
}