using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class LevelManager : Singleton<LevelManager>
{
    public enum Scenes
    {
        MainMenuScene,
        WorldScene,
        Mini1Scene,
        Mini2Scene,
        Mini3Scene
    }

    [System.Serializable]
    public struct SceneTitleDesciptionText
    {
        public Scenes sceneName;
        public string titleText;
        public string descriptionText;

    }
    [SerializeField] private GameObject loaderScreen;
    [SerializeField] private GameObject firstLoadScreen;
    [SerializeField] private Image loadingFullscreenImage;

    [SerializeField] private TextMeshProUGUI loaderTitleText;
    [SerializeField] private TextMeshProUGUI loaderDescriptionText;
    [SerializeField] private Image progressBar;
    [SerializeField] private SwappingImages[] loadingSceneBackgrounds;
    [SerializeField] private SceneTitleDesciptionText[] sceneTitleDesciptionTexts;

    private int loadingDuration = 5;
    
    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();
    }

    public async void LoadScene(Scenes toScene)
    {
        // Start preloading the video and loading the scene simultaneously
        Task videoPreloadTask = VideoManager.Instance.PreloadVideoForScene(toScene.ToString());
        
        // Let loading screen show for five seconds
        Task showLoadingScreen = ShowLoadingScreen(toScene);

        var scene = SceneManager.LoadSceneAsync(toScene.ToString());
        scene.allowSceneActivation = false;

        // switch (sceneName)
        // {
        //     case "MainMenuScene":
        //         ShowFullScreenBackground(loadText: "Return Main Menu");
        //         break;
        //     case "WorldScene":
        //         ShowFullScreenBackground(loadText: "Back to Stadium");
        //         break;
        //     case "Mini1Scene":
        //         ShowFullScreenBackground(loadText: "Mini1");
        //         break;
        //     case "Mini2Scene":
        //         ShowFullScreenBackground(loadText: "Mini2");
        //         break;
        //     case "Mini3Scene":
        //         ShowFullScreenBackground(loadText: "Mini3");
        //         break;
        //     default:
        //         break;
        // }

        loaderScreen.SetActive(true);
        
        // Wait for both video preload and loading screen display
        await Task.WhenAll(videoPreloadTask, showLoadingScreen);
        
        scene.allowSceneActivation = true;
        loaderScreen.SetActive(false);
        //Debug.Log("Loader Screen closed");
    }

    private void Start()
    {
        firstLoadScreen.SetActive(true);
    }

    private void Update(){
        //progressBar.fillAmount = Mathf.MoveTowards(progressBar.fillAmount, progress, 3 * Time.deltaTime);
    }

    private async Task ShowLoadingScreen(Scenes toScene)
    {
        //Debug.Log("Start timer");

        //First Load
        if (firstLoadScreen != null && progressBar != null) 
        {
            loadingFullscreenImage.gameObject.SetActive(false);
            await FillProgressBarAsync(loadingDuration);
        }

        else
        {
            loaderTitleText.text = sceneTitleDesciptionTexts[(int)toScene].titleText;
            loaderDescriptionText.text = sceneTitleDesciptionTexts[(int)toScene].descriptionText;
            loadingFullscreenImage.sprite = Array.Find(loadingSceneBackgrounds, sceneBackground => sceneBackground.imageName == toScene.ToString()).image;
            await Task.Delay(loadingDuration * 1000);
        }

        //Debug.Log("End timer");
    }

    // Asynchronous method to fill the progress bar
    private async Task FillProgressBarAsync(float duration)
    {
        float elapsedTime = 0f;

        // While the elapsed time is less than the duration (5 seconds)
        while (elapsedTime < duration)
        {
            elapsedTime += (float)Time.deltaTime; // Increase the elapsed time with each frame
            float progress = Mathf.Clamp01(elapsedTime / duration); // Calculate the progress (between 0 and 1)

            // Set the progress bar fill amount (assuming it's a UI Image with a fill method)
            progressBar.fillAmount = progress;

            // Wait for the next frame (approximately 1 frame delay)
            await Task.Yield();  // This will allow the UI to update and continue the loop in the next frame
        }

        // Ensure the progress bar is fully filled at the end (for precision)
        progressBar.fillAmount = 1f;
        loadingFullscreenImage.gameObject.SetActive(true);
        //firstLoadScreen.SetActive(false);
        Destroy(firstLoadScreen);
    }

}
