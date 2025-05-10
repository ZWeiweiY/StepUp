using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mini3FragmentSelfSpin : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.localScale = Vector3.one;

        transform.Rotate(10f * Time.deltaTime, 10f * Time.deltaTime, 10f * Time.deltaTime, Space.Self);
    }
}
