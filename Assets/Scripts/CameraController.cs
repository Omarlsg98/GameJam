using System.Collections;
using System.Collections.Generic;
using UnityEngine;


 public class CameraController : MonoBehaviour
 {
    public int Boundary = 50;
    public float baseSpeed = 10;
    public float speedChange = 0.3f;

    public float leftLimit = -4;
    public float rightLimit = 45;
    
    private int theScreenWidth;
    private float actualSpeed;
    

    void Start() 
    {
        theScreenWidth = Screen.width;
        actualSpeed = baseSpeed;
    }
    
    void Update() 
    {
        if (Input.mousePosition.x > theScreenWidth - Boundary && transform.position.x < rightLimit)
        {
            transform.position += new Vector3(actualSpeed * Time.deltaTime, 0, 0);
            actualSpeed += speedChange;
        }else if (Input.mousePosition.x < 0 + Boundary && transform.position.x > leftLimit)
        {
            transform.position -= new Vector3(actualSpeed * Time.deltaTime, 0, 0);
            actualSpeed += speedChange;
        }else{
             actualSpeed = baseSpeed;
        }
    }    
 }
