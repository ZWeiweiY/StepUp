using UnityEngine;
using UnityEngine.Video;
using System.Threading.Tasks;

public class VideoManager : Singleton<VideoManager>
{
    [SerializeField] private GameObject videoScreen;
    [SerializeField] private VideoPlayer videoPlayer;
    
    [System.Serializable]
    public class SceneVideoMapping
    {
        public string sceneName;
        public VideoClip videoClip;
    }

    [SerializeField] private SceneVideoMapping[] sceneVideos;
    private VideoClip currentVideo;
    private bool isPreloaded = false;

    protected override void Awake()
    {
        base.Awake();
        // Ensure frame is hidden at start
        if (videoScreen != null)
        {
            videoScreen.SetActive(false);
        }
    }

    public async Task PreloadVideoForScene(string sceneName)
    {
        var sceneVideo = System.Array.Find(sceneVideos, mapping => mapping.sceneName == sceneName);
        
        if (sceneVideo != null && sceneVideo.videoClip != null)
        {
            videoScreen.SetActive(true);
            videoPlayer.gameObject.SetActive(true);

            videoPlayer.clip = sceneVideo.videoClip;
            videoPlayer.Prepare();

            while (!videoPlayer.isPrepared)
            {
                await Task.Delay(100);
            }

            currentVideo = sceneVideo.videoClip;
            isPreloaded = true;

            videoScreen.SetActive(false);
            //Debug.Log($"Video for scene {sceneName} preloaded successfully");
        }
    }

    public void SetVideoPlaybackSpeed(float speed)
    {
        videoPlayer.playbackSpeed = speed;
    }

    public void PlayVideo()
    {
        if (currentVideo != null && isPreloaded)
        {
            videoScreen.SetActive(true);
            videoPlayer.Play();
        }
    }

    public void StopVideo()
    {
        if (videoPlayer.isPlaying)
        {
            videoPlayer.Stop();
            videoScreen.SetActive(false);
            isPreloaded = false;
        }
    }

    public void CloseVideoScreen()
    {
        videoScreen.SetActive(false);
        videoPlayer.Stop();
        isPreloaded = false;
        
    }

    public bool IsVideoPlaying()
    {
        return videoPlayer.isPlaying;
    }

    public bool IsVideoReady()
    {
        return currentVideo != null && videoPlayer.isPrepared;
    }

    private void OnDisable()
    {
        StopVideo();
    }
}