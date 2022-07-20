using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class winGame : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnCollisionEnter(Collision collision)
    {
        callSaveFile();
        SceneManager.LoadScene("Credits");
    }

    private void callSaveFile()
    {
        timer.singleton.saveTime();

    }

}
