using UnityEngine;

using static Player;

public enum HeadType {
    Scavenger,
    Warrior
}

[System.Serializable]
public class Head
{
    public HeadType type;
    public GameObject prefab;

    public Head(Head head){
        this.type = head.type;
        this.prefab = head.prefab;
    }
}

public class SurgeryHead : MonoBehaviour
{
    public Head head;

    private Player player;

    void Start(){
        player = GameObject.FindWithTag("GameController").GetComponent<Player>();
    }

    void OnMouseOver(){
        if(Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1)){
            player.changeHead(this.head, gameObject);
        }
    }
}
