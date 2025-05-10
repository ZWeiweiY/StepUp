using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShoeFragment : Mini3Collectables
{
    [SerializeField] private Sound shoePieceSound;
    [SerializeField] private Transform shoeBubbleParticleParent;
    public override void Collect(Mini3PlayerCollect mini3Player)
    {
        mini3Player.AddPowerUp();
        mini3Player.TriggerParticles(gameObject, shoeBubbleParticleParent);
        SoundManager.Instance.PlaySFX(shoePieceSound, gameObject.transform, 1f, true);
        // Debug.Log("Collected Shoe Fragment");
        Destroy(gameObject);
    }
}
