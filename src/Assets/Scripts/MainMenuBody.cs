using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuBody : MonoBehaviour
{

    static bool mouseDown = false;

    void Update() {
        if (Input.GetMouseButtonDown(0) && mouseDown == false) {
            RaycastHit  hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            mouseDown = true;
            
            if (Physics.Raycast(ray, out hit)) {
                string clickName = hit.transform.name;

                switch (clickName) {
                    case "LevelSelect":
                        LevelController.GoToLevelSelection();
                        break;
                    case "About":
                        LevelController.GoToAboutPage();
                        break;
                    case "MainMenu":
                        LevelController.GoToMainMenu();
                        break;
                    default:
                        // all other buttons are level selections
                        LevelController.GoToLevel(clickName);
                        break;
                }
            }
        } 

        if (!Input.GetMouseButtonDown(0) && mouseDown == true) {
            mouseDown = false;
        } 
           

    }

}
