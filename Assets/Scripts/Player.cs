using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static Grid;
using static Main;
using static DiscreteCoordinate;
using static Demon;

public class Player : MonoBehaviour
{
    public GameObject demon;
    public GameObject[] parts = new GameObject[5];
    public GameObject head;

    public GameObject demonTablePlaceHolder;
    public GameObject partSpawnPoint;
    
    private Grid grid;
    private Main main;

    void Start(){
        main = GetComponent<Main>();
        grid = main.actualGrid;

        spawnDemon();
    }

    void Update(){

    }

    public void addBodyPart(GameObject part){
        PartData partData = part.GetComponent<PartConfiguration>().partData;
        int index = -1;
        int indexChild = -1;
        if (partData.type == PartType.Limb){
            bool enter = false;
            for (int i = 0; i < parts.Length; i++){
                if (parts[i] == null && i != 2){
                    enter = true;
                    index = i;
                    indexChild = i > 2? i + 1: i;
                    break;
                }
            }
            if (!enter){
                putPartInBox(part);
                //TODO(FELIVANS): PONDRA ALGO (PARTE LLENA)
            }
        }else if (partData.type == PartType.Chest){
           index = 2;
           indexChild = 2;
        }

        parts[index] = part;
        putPartInTable(part, indexChild);
    }

    public void removeBodyPart(GameObject part){
        for (int i = 0; i < parts.Length; i++){
            if (parts[i] == part){
                parts[i] = null;
                break;
            }
        }
        putPartInBox(part);
    }

    private void putPartInBox(GameObject part){
        part.transform.position = partSpawnPoint.transform.position;
        part.transform.rotation = Quaternion.Euler(0.0f, 0.0f, Random.Range(0.0f, 360.0f));
        part.transform.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        part.GetComponent<SpriteRenderer>().sortingOrder = 2;
    }

    private void putPartInTable(GameObject part, int indexChild){
        part.transform.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
        GameObject placeHolderPart = demonTablePlaceHolder.transform.GetChild(indexChild).gameObject;
        part.transform.position = placeHolderPart.transform.position;
        part.transform.rotation = placeHolderPart.transform.rotation;
    }

    private void spawnDemon(){
        Demon.instantiateDemon(demon, grid, parts, head, new DiscreteCoordinate(0,0), main.difficultyFactor, true);
        for (int i = 0; i < parts.Length; i++){
            parts[i] = null;
        }
    }
}