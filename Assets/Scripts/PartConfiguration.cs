using System.Collections;
using System.Collections.Generic;

public enum SpecialSkill{
    None,
    Unstoppable
}

public enum PartType{
    Limb,
    Chest
}

public class Part
{
    public float damage = 10.0f;
    public float attackSpeed = 1.0f; //attacks per second
    public int range = 1; //tiles ahead
    public float life = 100.0f;
    public float loadCapacity = 10.0f; //kilos
    public float weight = 1.0f; //kilos
    public int energyConsumption = 1; //Energy consume per action
    public float luck = 0.4f;  //chance of scavenge a part
    public float movementSpeed = 2; //tiles per second
    public List<SpecialSkill> skill;
    public PartType type;
}