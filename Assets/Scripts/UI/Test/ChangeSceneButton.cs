using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ChangeSceneButton : MonoBehaviour
{
    public void ChangeScene(LevelManager.Scenes sceneName)
    {
        LevelManager.Instance.LoadScene(sceneName);
    }
}
