using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DefaultColliderData
{
    [field: SerializeField] public float Height {get; private set;} = 2.2f;
    [field: SerializeField] public float CenterY {get; private set;} = 1.1f;
    [field: SerializeField] public float Radius {get; private set;} = 0.35f;
}
