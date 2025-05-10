using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class Mini2Camera : MonoBehaviour
{
    public float cameraSwitchTime {get; private set;}

    [SerializeField] private GameObject firstViewCamera;
    [SerializeField] private GameObject fallingCamera;
    [SerializeField] private GameObject gamePlayCamera;

    private float cameraSwitchBuffer = 0.5f;
    private float destroyDelay = 3f; 
    // Start is called before the first frame update
    private void Start()
    {
        cameraSwitchTime = Camera.main.GetComponent<CinemachineBrain>().m_DefaultBlend.m_Time + cameraSwitchBuffer;
    }

    public void ChangeToFallingCamera()
    {
        firstViewCamera.SetActive(false);
        gamePlayCamera.SetActive(false);
        fallingCamera.SetActive(true);
    }

    public void ChangeToGamePlayCamera()
    {
        firstViewCamera.SetActive(false);
        fallingCamera.SetActive(false);
        gamePlayCamera.SetActive(true);
    }

    public void DestroyIntroCameras()
    {
        Destroy(firstViewCamera, destroyDelay);
        Destroy(fallingCamera, destroyDelay);
    }

}
