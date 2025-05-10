using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable] 
public class PowerUpEvent : UnityEvent<float> { }

public class Mini3PlayerCollect : MonoBehaviour
{
    public Animator animator;
    [SerializeField] private GameObject shoeVisual;

    private SkinnedMeshRenderer shoeMeshRenderer;
    [SerializeField] private float shoeSparklingDuration;
    [SerializeField] private Material shoeOriginalMaterial;
    [SerializeField] private Material shoeSparklingMaterial;
    [SerializeField] private Transform powerUpVFXParent;
    [SerializeField] private GameObject powerUpTrailPrefab;

    [Header("PowerUp Parameters")]
    [SerializeField] private float powerUpDuration = 20f;
    public bool isPowerUp = false;
    private float powerUpFeedbackSpeedUp = 1.5f;
    
    [SerializeField] private Mini3PlayerContact playerContact;
    [SerializeField] private UIMini3 uiMini3;
    private int coinCount = 0;
    private int powerUpCount = 0;
    private float remainingPowerUpTime = 0f;

    public PowerUpEvent OnPowerUpActivated;
    public UnityEvent OnPowerUpDeactivated;

    private GameObject currentPowerUpVFXTrail;
    

    private void Start()
    {
        shoeVisual.SetActive(false);
        shoeMeshRenderer = shoeVisual.GetComponent<SkinnedMeshRenderer>();
        animator.SetFloat("speed", 1f);
        SoundManager.Instance.SetMusicPitch(1f);
    }
    
    private void Update()
    {
        if(powerUpCount >= 5 && !isPowerUp && !playerContact.gameEnded)
        {
            ActivatePowerUp();
        }
        if(isPowerUp)
        {
            remainingPowerUpTime -= Time.deltaTime;
            float normalizedRemainingTime = remainingPowerUpTime / powerUpDuration;
            uiMini3.UpdatePowerUpCountdown(normalizedRemainingTime);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        Mini3Collectables collectable = other.GetComponent<Mini3Collectables>();
        if (collectable != null)
        {
            collectable.Collect(this);
            return;
        }
    }

    public void AddCoins(int amount)
    {
        coinCount += amount;
        uiMini3.UpdateCoinCount(coinCount);
    }

    public void TriggerParticles(GameObject gameObject, Transform particleParent)
    {
        Transform particles = Instantiate(particleParent, gameObject.transform.position, Quaternion.identity);
        foreach (Transform childFx in particleParent)
        {
            childFx.GetComponent<ParticleSystem>().Play();
        }
        Destroy(particles.gameObject, 1f);
    }

    public void AddPowerUp()
    {
        if(powerUpCount < 5)
        {
            powerUpCount++;
        }
        uiMini3.UpdatePowerUpCount(powerUpCount);
        // if(powerUpCount >= 5)
        // {
        //     ActivatePowerUp();
        // }
    }

    private void ActivatePowerUp()
    {
        if(!isPowerUp)
        {
            isPowerUp = true;
            animator.SetFloat("speed", powerUpFeedbackSpeedUp);
            shoeVisual.SetActive(true);
            StartCoroutine(SparklingShoeCountdown());
            currentPowerUpVFXTrail = Instantiate(powerUpTrailPrefab, powerUpVFXParent);
            powerUpCount = 0;
            remainingPowerUpTime = powerUpDuration;
            uiMini3.UpdatePowerUpCount(powerUpCount);
            uiMini3.ShowPowerUpCountdown(true);
            uiMini3.SwitchPowerUpCountGroup();
            SoundManager.Instance.SetMusicPitch(powerUpFeedbackSpeedUp);

            OnPowerUpActivated.Invoke(powerUpDuration);

            StartCoroutine(PowerUpTimerCountdown());
        }
    }

    private IEnumerator PowerUpTimerCountdown()
    {
       while (remainingPowerUpTime > 0)
        {
            yield return null;
        }
        DeactivatePowerUp(); 
    }

    private IEnumerator SparklingShoeCountdown()
    {
        yield return new WaitForSeconds(shoeSparklingDuration);
        if(shoeMeshRenderer != null)
        {
            shoeMeshRenderer.material = shoeOriginalMaterial;
        }
        else
        {
            Debug.LogError("[Mini3Collect]: Can't find shoe mesh renderer!");
        }
    }

    private void DeactivatePowerUp()
    {
        
        shoeVisual.SetActive(false);
        if(shoeMeshRenderer != null)
        {
            shoeMeshRenderer.material = shoeSparklingMaterial;
        }
        else
        {
            Debug.LogError("[Mini3Collect]: Can't find shoe mesh renderer!");
        }
        Destroy(currentPowerUpVFXTrail);
        uiMini3.ShowPowerUpCountdown(false);
        uiMini3.SwitchPowerUpCountdownFillImage();
        animator.SetFloat("speed", 1f);
        SoundManager.Instance.SetMusicPitch(1f);
        isPowerUp = false;
        OnPowerUpDeactivated.Invoke();
    }
}
