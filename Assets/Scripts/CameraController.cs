using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ParallaxObject {
    public GameObject gameObject;
    public float speedPercentage;
}

 public class CameraController : MonoBehaviour
 {
    public int Boundary = 50;
    public float baseSpeed = 10;
    public float speedChange = 0.3f;

    public float leftLimit = -4;
    public float rightLimit = 45;
    
    public List<ParallaxObject> parallaxObjs;
    private int theScreenWidth;
    private float actualSpeed;
    

    void Start() 
    {
        theScreenWidth = Screen.width;
        actualSpeed = baseSpeed;
    }
    
    void Update() 
    {
        int direction = 0;
        if (Input.mousePosition.x > theScreenWidth - Boundary && transform.position.x < rightLimit)
        {
            direction = 1;
        }else if (Input.mousePosition.x < 0 + Boundary && transform.position.x > leftLimit)
        {
            direction = -1;
        }

        if (direction != 0){
            Vector3 positionDelta = new Vector3(actualSpeed * Time.deltaTime * direction, 0, 0);
            transform.position += positionDelta;
            foreach (ParallaxObject obj in parallaxObjs){
                obj.gameObject.transform.position += new Vector3(positionDelta.x * obj.speedPercentage, 0, 0);
            }
            actualSpeed += speedChange;
        }
        else{
             actualSpeed = baseSpeed;
        }
    }    
 }
