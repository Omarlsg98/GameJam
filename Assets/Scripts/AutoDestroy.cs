using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static CoolDown;
public class AutoDestroy : MonoBehaviour
{
    public CoolDown timeToDestroy;
    void Start()
    {
        timeToDestroy.turnOnCooldown();
    }

    void Update()
    {
        timeToDestroy.updateCoolDown();
        if(timeToDestroy.isReady()){
            Destroy(gameObject);
        }
    }
}
