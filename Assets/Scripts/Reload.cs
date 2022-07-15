 using UnityEngine;
 using System.Collections;
 using UnityEngine.SceneManagement; 
 
 public class Reload : MonoBehaviour{
     void Update(){
         if(Input.GetKeyDown(KeyCode.R)){
             Scene scene = SceneManager.GetActiveScene(); SceneManager.LoadScene(scene.name);
         }
     }
 }