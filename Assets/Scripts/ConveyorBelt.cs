using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorBelt : MonoBehaviour
{
    public float speed;
    public List<GameObject> beltParts;
    public GameObject leftMilestone;
    public GameObject rightMilestone;
    private float minX;
    private float maxX;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        this.minX = leftMilestone.transform.position.x;
        this.maxX = rightMilestone.transform.position.x;
        Vector3 positionDelta = new Vector3(speed * Time.deltaTime, 0, 0);
        foreach(GameObject beltPart in beltParts){
            beltPart.transform.position += positionDelta;
            float newX = beltPart.transform.position.x;
            float adjustedX = newX;
            if (newX > maxX){
                adjustedX = minX;
            } else if(newX < minX){
                adjustedX = maxX;
            }
            beltPart.transform.position = new Vector3(adjustedX, beltPart.transform.position.y, beltPart.transform.position.z);
        }
    }
}
