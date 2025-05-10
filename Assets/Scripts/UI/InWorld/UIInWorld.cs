using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIInWorld : MonoBehaviour
{

    [Tooltip("InWorld UI")]
    public MyJoystick joystick;
    public GameObject interactButton;
    public TextMeshProUGUI interactText;

    public Button mainMenuButton;
    // Start is called before the first frame update

    private void Awake(){
        InitializeInWorldUI();
    }

    private void OnDisable(){
        ClearInWorldUI();
    }

    private void InitializeInWorldUI()
    {
        joystick.enabled = true;

        interactButton.SetActive(false);
        interactText.gameObject.SetActive(false);
    
        mainMenuButton.onClick.AddListener(HandleMainMenuClicked);
    }

    public void ClearInWorldUI()
    {
        joystick.enabled = false;
        interactButton.SetActive(false);
        interactText.gameObject.SetActive(false);
    }

    private void HandleMainMenuClicked(){
        GameManager.Instance.ReturnToMainMenu();
        SoundManager.Instance.StopMusic();
    }
}
