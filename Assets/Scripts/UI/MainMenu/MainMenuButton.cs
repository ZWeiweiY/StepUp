using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuButton : MonoBehaviour
{
    // [SerializeField] private Button startButton;
    // [SerializeField] private Button optionButton;
    // [SerializeField] private Button quitButton;


    public void StartGame(){
        GameManager.Instance.StartGame();
    }

}
