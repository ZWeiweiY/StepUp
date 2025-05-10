using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugButton : MonoBehaviour
{
    public void ChangeState(int idx){
        switch(idx){
            case 0:
                GameManager.Instance.StartGame();
                break;
            case 1:
                GameManager.Instance.ReturnToMainMenu();
                break;
            case 2:
                GameManager.Instance.StartMiniGame1();
                break;
            case 3:
                GameManager.Instance.StartMiniGame2();
                break;
            case 4:
                GameManager.Instance.StartMiniGame3();
                break;
        }
    }
}
