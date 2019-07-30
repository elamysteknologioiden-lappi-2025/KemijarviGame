using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pLab_RotateObject : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.RotateAround(transform.position,Vector3.up, 0.31f * Time.deltaTime);
    }
}
