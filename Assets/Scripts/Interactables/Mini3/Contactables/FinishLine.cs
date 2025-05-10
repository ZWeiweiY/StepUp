using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishLine :Mini3Contactables
{
    public override void ApplyEffect(Mini3PlayerContact mini3Player)
    {
        mini3Player.FinishedGame();
    }

}
