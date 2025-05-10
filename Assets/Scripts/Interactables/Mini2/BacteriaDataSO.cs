using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Bacteria", menuName = "ScriptableObjects/Bacteria")]
public class BacteriaDataSO : ScriptableObject
{
   public string bacteriaName;
   public float value;
   public GameObject prefab;
//    public Sprite icon;
}
