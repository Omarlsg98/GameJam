using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static Main;
using static Player;

[System.Serializable]
public class PartData
{
    private static int partsNumber = 5;

    public float damage = 10.0f;
    public float attackSpeed = 1.0f; //attacks per second
    public int range = 1; //tiles ahead
    public float life = 10.0f;
    public float loadCapacity = 10.0f; //kilos
    public float weight = 1.0f; //kilos
    public float energyConsumption = 1.0f; //Energy consume per action
    public float luck = 0.4f;  //chance of scavenge a part
    public float movementSpeed = 2; //tiles per second
    public List<SpecialSkill> skills;
    public PartType type;

    public PartData(PartData part){
        this.damage = part.damage;
        this.attackSpeed = part.attackSpeed;
        this.range = part.range;
        this.life = part.life;
        this.loadCapacity = part.loadCapacity;
        this.weight = part.weight;
        this.energyConsumption = part.energyConsumption;
        this.luck = part.luck;
        this.movementSpeed = part.movementSpeed;
        this.skills = new List<SpecialSkill>(part.skills);
        this.type = part.type;
    }

    public static PartData getTotalStats(GameObject[] parts){
        PartData result = null;
        for(int i = 0; i < parts.Length; i++){
            if (parts[i] != null){
                PartData part = parts[i].GetComponent<PartConfiguration>().partData;
                if (result == null){
                    result =  new PartData(part);
                }else{ 
                    result.damage += part.damage;
                    result.attackSpeed += part.attackSpeed;
                    result.range += part.range;
                    result.life += part.life;
                    result.loadCapacity += part.loadCapacity;
                    result.weight += part.weight;
                    result.energyConsumption += part.energyConsumption;
                    result.luck += part.luck;
                    result.movementSpeed += part.movementSpeed;
                    result.skills.AddRange(part.skills);
                }
            }
        }
        result.damage /= PartData.partsNumber;
        //result.life = result.life;
        //result.energyConsumption = result.energyConsumption;

        result.attackSpeed /= PartData.partsNumber;
        result.range = (int)(result.range/PartData.partsNumber);
        result.range = result.range == 0? 1 : result.range;
        result.loadCapacity /= PartData.partsNumber;
        result.weight /= PartData.partsNumber;
        result.luck /= PartData.partsNumber;
        result.movementSpeed /= PartData.partsNumber;
        
        return result;
    }
}

public class PartConfiguration : MonoBehaviour
{
    public PartData partData;
    public bool inBox = false;
    public bool onTable = false;

    private Player player;

    void Start(){
        player = GameObject.FindWithTag("GameController").GetComponent<Player>();
    }

    void OnMouseEnter()
    {
        Debug.Log("Mouse is over " + gameObject.transform.name);
    }

    void OnMouseOver(){
        if(Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1)){
            if (inBox) {
                player.addBodyPart(gameObject);
                this.onTable = true;
                this.inBox = false; 
            } else if (onTable) {
                player.removeBodyPart(gameObject);
                this.onTable = false;
                this.inBox = true;
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.name == "DeleteLine"){
            Destroy(gameObject);
        }
    }

    public void setOnField(){
        this.inBox = false;
        this.onTable = false;
    }
}

public enum SpecialSkill{
    None,
    Unstoppable
}

public enum PartType{
    Limb,
    Chest
}

