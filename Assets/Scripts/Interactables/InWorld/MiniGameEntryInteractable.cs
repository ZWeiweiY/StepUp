using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MiniGameEntryInteractable : MonoBehaviour, IInteractable
{
    public int gameIndex;

    // Update is called once per frame
    private void Update()
    {
        
    }

    public void Interact(Transform interactorTransform)
    {
        //Debug.Log("MiniGameEntryInteractable: Interacting with MiniGameEntryInteractable");
        SoundManager.Instance.StopMusic();

        switch(gameIndex){
            case 1:
                GameManager.Instance.StartMiniGame1();
                break;
            case 2:
                GameManager.Instance.StartMiniGame2();
                break;
            case 3:
                GameManager.Instance.StartMiniGame3();
                break;
        }

    }

    public string GetInteractPrompt()
    {
        return "Enter";
    }

    public Transform GetTransform()
    {
        return transform;
    }

}
