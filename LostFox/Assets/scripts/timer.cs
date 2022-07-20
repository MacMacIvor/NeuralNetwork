using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.SceneManagement;

public class timer : MonoBehaviour
{
    public static timer singleton;
    private void Awake()
    {
        if (singleton == null)
        {
            singleton = this;
            DontDestroyOnLoad(gameObject);
            return;
        }
        Destroy(this);
    }

    private float timePassed = 0;
    public TMP_Text timerString;
    public TMP_Text timerShowString;
    bool stillCredits = false;
    // Start is called before the first frame update
    void Start()
    {
        timePassed = PlayerPrefs.GetFloat("timePassed", 0.0f);
    }

    // Update is called once per frame
    void Update()
    {
        if (SceneManager.GetActiveScene().name == "Credits")
        {
            if (stillCredits == false)
            {
                stillCredits = true;
                string objectsData;

                using (var line = new StreamReader(System.IO.Directory.GetCurrentDirectory() + "/timeSaved/bestTime.txt"))
                    while ((objectsData = line.ReadLine()) != null) //Set up this way in case in the future we want to add times for each individual level
                    {

                        timerShowString.text = objectsData;

                    }
            }
        }
        else
        {
            timePassed += Time.deltaTime;
            timerString.text = "Time: " + timePassed.ToString();
        }
    }
    public void saveTime()
    {
        string objectsData;
        float savedTime = 99999;

        using (var line = new StreamReader(System.IO.Directory.GetCurrentDirectory() + "/timeSaved/bestTime.txt"))
            while ((objectsData = line.ReadLine()) != null) //Set up this way in case in the future we want to add times for each individual level
            {
                savedTime = float.Parse(objectsData);

            }
        if (savedTime > timePassed)
        {
            using (StreamWriter outputFile = new StreamWriter(System.IO.Directory.GetCurrentDirectory() + "/timeSaved/bestTime.txt"))
            {
                outputFile.Write(timePassed);
            }
        }
    }

    private void OnDestroy()
    {
        PlayerPrefs.SetFloat("timePassed", timePassed);
    }
    public void resetTime()
    {
        timePassed = 0;
    }

    public float getTime()
    {
        return timePassed;
    }
}
