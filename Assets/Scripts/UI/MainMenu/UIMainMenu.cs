using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMainMenu : MonoBehaviour
{
    [SerializeField] private Button enterGameButton;
    // Start is called before the first frame update
    private void Start()
    {
        enterGameButton.onClick.AddListener(HandleEnterGameClicked);
    }

    private void HandleEnterGameClicked()
    {
        GameManager.Instance.StartGame();
        SoundManager.Instance.StopMusic();
        enterGameButton.interactable = false;
        gameObject.SetActive(false);
        // SoundManager.Instance.PlayMusic("InWorld_BGM", true);
    }
}
