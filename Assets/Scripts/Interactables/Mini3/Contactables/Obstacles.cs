using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacles : Mini3Contactables
{
    public override void ApplyEffect(Mini3PlayerContact mini3Player)
    {
        mini3Player.TakeDamage(damage);
        
        mini3Player.TriggerTrippedAnimation();
    }

}
