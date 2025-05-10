using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldMapHandler : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private Transform regionsParent;
    [SerializeField] private float activeDistance = 10f;
    [SerializeField] private float loopInterval = 1f;

    [Header("Cloud Entries")]
    [SerializeField] GameObject entryCloudsForMini2;
    [SerializeField] GameObject entryCloudsForMini3;

    // Start is called before the first frame update
    private void Start()
    {
        HandleClouds();
        // StartCoroutine(CheckRegionDistance());
        if(GameManager.Instance.CurrentState != GameManager.GameState.Ending)
        {
            SoundManager.Instance.PlayMusic("InWorld_BGM", true);
        }
    }

    private IEnumerator CheckRegionDistance()
    {
        while (true)
        {
            foreach (Transform region in regionsParent.transform)
            {
                float distanceToPlayer = (player.position - region.position).magnitude;
                if (distanceToPlayer <= activeDistance)
                {
                    // Activate the region
                    region.gameObject.SetActive(true);
                }
                else
                {
                    // Deactivate the region
                    region.gameObject.SetActive(false);
                }
            }
            yield return new WaitForSeconds(loopInterval);
        }
    }

    private void HandleClouds()
    {
        int completeMiniCount = GameManager.Instance.finishedMiniCount;

        switch (completeMiniCount)
        {
            case >= 2:
                entryCloudsForMini2.SetActive(false);
                entryCloudsForMini3.SetActive(false);
                break;
            case >= 1:
                entryCloudsForMini2.SetActive(false);
                entryCloudsForMini3.SetActive(true);
                break;
            default:
                entryCloudsForMini2.SetActive(true);
                entryCloudsForMini3.SetActive(true);
                break;
        }
    }

}
