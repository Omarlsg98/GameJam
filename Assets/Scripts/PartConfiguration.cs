using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartConfiguration : MonoBehaviour
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

    public PartConfiguration(PartConfiguration part){
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

    public static PartConfiguration getTotalStats(GameObject[] parts){
        PartConfiguration result = null;
        for(int i = 0; i < parts.Length; i++){
            PartConfiguration part = parts[i].GetComponent<PartConfiguration>();
            if (part != null){
                if (result == null){
                    result =  new PartConfiguration(part);
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
        result.damage /= PartConfiguration.partsNumber;
        //result.life = result.life;
        //result.energyConsumption = result.energyConsumption;

        result.attackSpeed /= PartConfiguration.partsNumber;
        result.range = (int)(result.range/PartConfiguration.partsNumber);
        result.loadCapacity /= PartConfiguration.partsNumber;
        result.weight /= PartConfiguration.partsNumber;
        result.luck /= PartConfiguration.partsNumber;
        result.movementSpeed /= PartConfiguration.partsNumber;
        
        return result;
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

