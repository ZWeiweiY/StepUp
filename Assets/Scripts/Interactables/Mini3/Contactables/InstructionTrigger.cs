using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstructionTrigger : Mini3Contactables
{
    public string instructionText;
    public override void ApplyEffect(Mini3PlayerContact mini3Player)
    {
        mini3Player.InstructionTrigger(instructionText);
    }
}
