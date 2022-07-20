using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class PopulationManager : MonoBehaviour
{
    public GameObject populationPrefab;
    public GameObject timerHolder;
    public GameObject checkpointHolder;
    public List<GameObject> populationContainer = new List<GameObject>();
    private Vector3 initPos;

    public int populationCount = 10;

    //wait times for allowing things to run
    private float timeToWait;
    public float timeToWaitConst = 5.0f;

    public float mutationAggression = 0.1f;

    private int populationNumber = 1;

    public TMP_Text populationString;

    bool saveWeights = false;
    bool loadWeights = false;

    // Start is called before the first frame update
    void Start()
    {
        timeToWait = timeToWaitConst;
        populationPrefab = GameObject.Find("Fox");
        initPos = populationPrefab.transform.position;
        initNetwork();
        populationPrefab.gameObject.SetActive(false);
    }


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.S))
        {
            saveWeights = true;
        }
        if (Input.GetKey(KeyCode.L))
        {
            loadWeights = true;
        }
        timeToWait -= Time.deltaTime;
        if (timeToWait < 0)
        {
            //Reset
            remakeNetwork();
            timeToWait = timeToWaitConst;
            populationNumber++;
            populationString.text = "Current Generation: " + populationNumber.ToString();
            timerHolder.GetComponent<timer>().resetTime();

        }
       
    }

    public void initNetwork()
    {
        for (int i = 0; i < populationCount; i++)
        {
            GameObject newObject = GameObject.Instantiate(populationPrefab, this.transform);
            newObject.transform.position = initPos;
            populationContainer.Add(newObject);
        }

    }

    public void remakeNetwork()
    {
        //Find best fox
        int indexOfBestFox = 0;
        float[] currentBest = { 0, 0 };
        for (int i = 0; i < populationContainer.Count; i++)
        {
            switch (i)
            {
                case 0: 
                    currentBest = populationContainer[i].GetComponent<behavior>().getDistanceToNextCheckPoint();
                    break;
                default:
                    float[] checking = populationContainer[i].GetComponent<behavior>().getDistanceToNextCheckPoint();
                    if (currentBest[1] < checking[1])
                    {
                        indexOfBestFox = i;
                        currentBest = checking;
                    }
                    if (currentBest[1] == checking[1] && currentBest[0] > checking[0])
                    {

                        indexOfBestFox = i;
                        currentBest = checking;
                    }
                    else if (currentBest[1] == (checkpointHolder.transform.childCount - 1) && checking[1] == (checkpointHolder.transform.childCount - 1))
                    {
                        
                        if (populationContainer[indexOfBestFox].GetComponent<behavior>().getTimes() > populationContainer[i].GetComponent<behavior>().getTimes())
                        {
                            indexOfBestFox = i;
                            currentBest = checking;
                        }
                            
                    }
                    break;
            }
        }

        Debug.Log(indexOfBestFox);

        //Delete the rest of them
        //for (int i = 0;i < indexOfBestFox; i++)
        //{
        //    Destroy(populationContainer[0].gameObject);
        //}
        //while (populationContainer.Count > 1) 
        //{
        //    Destroy(populationContainer[1].gameObject);
        //    populationContainer.RemoveAt(1);
        //
        //}; 
        if (populationCount == populationContainer.Count)
        {
            for (int i = 0; i < populationCount; i++)
            {
                if (i == indexOfBestFox)
                {
                    if (saveWeights)
                    {
                        populationContainer[i].GetComponent<behavior>().saveWeights();
                        saveWeights = false;
                    }
                    populationContainer[i].transform.position = initPos;
                    populationContainer[i].transform.gameObject.GetComponent<Rigidbody>().velocity = Vector2.zero;
                    populationContainer[i].GetComponent<behavior>().resetCheckPoints();

                }
                else
                {
                    populationContainer[i].transform.position = initPos;
                    populationContainer[i].transform.gameObject.GetComponent<Rigidbody>().velocity = Vector2.zero;
                    populationContainer[i].GetComponent<behavior>().copyNetWork(populationContainer[indexOfBestFox].GetComponent<behavior>().getWeights());
                    populationContainer[i].GetComponent<behavior>().mutateNetwork(mutationAggression);
                    populationContainer[i].GetComponent<behavior>().resetCheckPoints();
                }
            }
        }
        else if (populationCount > populationContainer.Count)
        {
            for (int i = 0; i < populationContainer.Count; i++)
            {
                if (i == indexOfBestFox)
                {
                    if (saveWeights)
                    {
                        populationContainer[i].GetComponent<behavior>().saveWeights();
                        saveWeights = false;
                    }
                    populationContainer[i].transform.position = initPos;
                    populationContainer[i].transform.gameObject.GetComponent<Rigidbody>().velocity = Vector2.zero;
                    populationContainer[i].GetComponent<behavior>().resetCheckPoints();

                }
                else
                {
                    populationContainer[i].transform.position = initPos;
                    populationContainer[i].transform.gameObject.GetComponent<Rigidbody>().velocity = Vector2.zero;
                    populationContainer[i].GetComponent<behavior>().copyNetWork(populationContainer[indexOfBestFox].GetComponent<behavior>().getWeights());
                    populationContainer[i].GetComponent<behavior>().mutateNetwork(mutationAggression);
                    populationContainer[i].GetComponent<behavior>().resetCheckPoints();
                }
            }
            populationPrefab.gameObject.SetActive(true);
            for (int i = populationContainer.Count; i < populationCount; i++)
            {
                GameObject newObject = GameObject.Instantiate(populationPrefab, this.transform);
                newObject.transform.position = initPos;
                populationContainer.Add(newObject);
                populationContainer[i].GetComponent<behavior>().copyNetWork(populationContainer[indexOfBestFox].GetComponent<behavior>().getWeights());
                populationContainer[i].GetComponent<behavior>().mutateNetwork(mutationAggression);
            }
            populationPrefab.gameObject.SetActive(false);
        }
        else
        {
            populationContainer[indexOfBestFox].transform.SetAsFirstSibling();
            if (saveWeights)
            {
                populationContainer[0].GetComponent<behavior>().saveWeights();
                saveWeights = false;
            }
            for (int i = populationContainer.Count - 1; i > populationCount - 1; i--)
            {
                Destroy(populationContainer[i].gameObject);
                populationContainer.RemoveAt(i);
            }
        }

        if (loadWeights)
        {
            populationContainer[0].GetComponent<behavior>().loadWeightsFromFile();
            for (int i = 1; i < populationContainer.Count; i++)
            {
                populationContainer[i].GetComponent<behavior>().copyNetWork(populationContainer[0].GetComponent<behavior>().getWeights());
            }
            loadWeights = false;
        }

        //populationContainer[0].transform.position = initPos;
        //populationContainer[0].transform.gameObject.GetComponent<Rigidbody>().velocity = Vector2.zero;

        //Create a certain amount of the ai's depending of the population size
        //for (int i = 0; i < populationCount - 1; i++)
        //{
        //    GameObject newObject = GameObject.Instantiate(populationContainer[0].gameObject, this.transform);
        //    newObject.transform.position = initPos; //This should not be needed but just in case
        //    newObject.GetComponent<behavior>().copyNetWork(populationContainer[0].GetComponent<behavior>().getNeuralNetwork());
        //    newObject.GetComponent<behavior>().mutateNetwork(mutationAggression);
        //    populationContainer.Add(newObject);
        //}

    }
}
