using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class CheckPoint : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        other.gameObject.GetComponent<behavior>().checkPointCompleted((int)Single.Parse(this.gameObject.name));
    }
}
