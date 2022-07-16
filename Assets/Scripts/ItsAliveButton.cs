using UnityEngine;

public class ItsAliveButton : MonoBehaviour
{
    private Player player;

    void Start(){
        player = GameObject.FindWithTag("GameController").GetComponent<Player>();
    }

    void OnMouseDown(){
        player.spawnDemon();
    }
}
