using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : Mini3Collectables
{
    [SerializeField] private Sound coinSound;
    [SerializeField] private Transform coinParticleParent;
    public override void Collect(Mini3PlayerCollect mini3Player)
    {
        mini3Player.AddCoins(1);
        // Debug.Log("Coin Added");
        mini3Player.TriggerParticles(gameObject, coinParticleParent);
        SoundManager.Instance.PlaySFX(coinSound, gameObject.transform, 1f, true);
        Destroy(gameObject);
    }
}
