using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class cameraBehavior : MonoBehaviour
{
    public GameObject foxPopulationHolder;
    int foxToFollow = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.Q))
        {
            foxToFollow++;
            if (foxToFollow > foxPopulationHolder.transform.childCount - 1)
            {
                foxToFollow = 0;
            }
            Debug.Log(foxToFollow);
        }
        try
        {
            transform.position = new Vector3(foxPopulationHolder.transform.GetChild(foxToFollow).transform.position.x, foxPopulationHolder.transform.GetChild(foxToFollow).transform.position.y, -10);
        }
        catch (Exception ex)
        {
            transform.position = new Vector3(0, 6.99f, -10);
        }
    }
}
