using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class Mini1Camera : MonoBehaviour
{
    public float cameraSwitchTime { get; private set; }

    [SerializeField] private GameObject introCamera;
    [SerializeField] private GameObject introBookZoomInCamera;
    [SerializeField] private GameObject introBookZoomOutCamera;
    [SerializeField] private GameObject interactCamera;
    [SerializeField] private GameObject resultCamera;
    [SerializeField] private GameObject endingCameraMid;
    [SerializeField] private GameObject endingCameraLeft;
    [SerializeField] private GameObject endingCameraRight;

    private float cameraSwitchBuffer = 0.5f;

    private Mini1Manager mini1Manager;
    // Start is called before the first frame update
    private void Start()
    {
        cameraSwitchTime = Camera.main.GetComponent<CinemachineBrain>().m_DefaultBlend.m_Time + cameraSwitchBuffer;
        mini1Manager = FindAnyObjectByType<Mini1Manager>();
        endingCameraLeft.SetActive(false);
        endingCameraRight.SetActive(false);
    }


    public void ChangeToIntroCamera()
    {
        introCamera.SetActive(true);
        introBookZoomInCamera.SetActive(false);
        introBookZoomOutCamera.SetActive(false);
        interactCamera.SetActive(false);
        resultCamera.SetActive(false);
        endingCameraMid.SetActive(false);
        //Debug.Log("Camera: Change to Intro Camera");
    }
    public void ChangeToIntroBookZoomInCamera()
    {
        introCamera.SetActive(false);
        introBookZoomInCamera.SetActive(true);
        introBookZoomOutCamera.SetActive(false);
        interactCamera.SetActive(false);
        resultCamera.SetActive(false);
        endingCameraMid.SetActive(false);
        //Debug.Log("Camera: Change to Intro Book Zoom In Camera");
    }
    public void ChangeToIntroBookZoomOutCamera()
    {
        introCamera.SetActive(false);
        introBookZoomInCamera.SetActive(false);
        introBookZoomOutCamera.SetActive(true);
        interactCamera.SetActive(false);
        resultCamera.SetActive(false);
        endingCameraMid.SetActive(false);
        //Debug.Log("Camera: Change to Intro Book Zoom Out Camera");
    }

    public void ChangeToInteractCamera()
    {
        if (introCamera != null && introBookZoomInCamera != null && introBookZoomOutCamera != null)
        {   
            introCamera.SetActive(false);
            introBookZoomInCamera.SetActive(false);
            introBookZoomOutCamera.SetActive(false);
        }
        interactCamera.SetActive(true);
        resultCamera.SetActive(false);
        endingCameraMid.SetActive(false);
        //Debug.Log("Camera: Change to Interact Camera");
    }

    public void ChangeToResultCamera()
    {   
        if (introCamera != null && introBookZoomInCamera != null && introBookZoomOutCamera != null)
        {   
            introCamera.SetActive(false);
            introBookZoomInCamera.SetActive(false);
            introBookZoomOutCamera.SetActive(false);
        }
        interactCamera.SetActive(false);
        resultCamera.SetActive(true);
        endingCameraMid.SetActive(false);
        //Debug.Log("Camera: Change to Result Camera");
    }

    public void ChangeToEndingCamera()
    {
        if (introCamera != null && introBookZoomInCamera != null && introBookZoomOutCamera != null)
        {
            introCamera.SetActive(false);
            introBookZoomInCamera.SetActive(false);
            introBookZoomOutCamera.SetActive(false);
        }
        interactCamera.SetActive(false);
        resultCamera.SetActive(false);
        endingCameraMid.SetActive(true);
    }

    public IEnumerator TriggerEndingCameraMovement()
    {
        yield return new WaitForSeconds(cameraSwitchTime);
        endingCameraMid.SetActive(false);
        endingCameraLeft.SetActive(true);
        yield return new WaitForSeconds(cameraSwitchTime);
        yield return new WaitForSeconds(1f);
        endingCameraLeft.SetActive(false);
        endingCameraRight.SetActive(true);
        yield return new WaitForSeconds(cameraSwitchTime);
        yield return new WaitForSeconds(1f);
        endingCameraRight.SetActive(false);
        resultCamera.SetActive(true);
        yield return new WaitForSeconds(cameraSwitchTime);
        yield return new WaitForSeconds(2.5f);
        mini1Manager.endingCameraMovement = true;
    }
}
