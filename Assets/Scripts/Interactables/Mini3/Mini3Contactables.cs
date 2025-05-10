using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Mini3Contactables : MonoBehaviour
{
    public int damage = 30;
    public abstract void ApplyEffect(Mini3PlayerContact mini3Player);
}
