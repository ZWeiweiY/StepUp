using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMapCollider : MonoBehaviour
{
    [SerializeField] private UIMini2 mini2UIManager;
    // Start is called before the first frame update
    private void Start()
    {
        Collider collider = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            //Change UI Map
            mini2UIManager.ChangeMapToImage(gameObject.name.ToString());
        }
        
    }
}
