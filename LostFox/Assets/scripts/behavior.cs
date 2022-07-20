using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class behavior : MonoBehaviour
{
    const float PI = 3.1415926535897932384626433832795028841971693993751058209749445923078164062862089986280348253421170679821480865132823066470938446095505822317253594081284811174502841027019385211055596446229489549303819644288109756659334461284756482337867831652712f;
    [Range(0, 1)]
    public int instantOrSmooth = 0;

    private float movementSpeed = 0;
    [Range(0,100)]
    public float movementSpeedMax = 1;
    [Range(0, 100)]
    public float movementSpeedSpeedUp = 0.6f;
    [Range(0, 100)]
    public float jumpMod = 1.5f;

    private int health = 3;
    
    private float healthCoolDownMax = 5; //No longer really used
    private float healthCoolDown = 0;

    [Range(0, 100)]
    public float dashStrength = 10;
    private bool wantsToDash = false;

    [Range(0, 100)]
    public float sprintStrengthSaved = 2.5f;
    private float sprintStrength = 1.0f;
    private bool isSprinting = false;

    private bool isGrounded = false;

    public Animator fox;
    public RawImage[] foxHealth;

    [Range(0, 3)]
    public float flashTime = 0.2f;
    [Range(0, 3)]
    public float flashDuration = 1.0f;

    public ParticleSystem dust;

    public GameObject foxsss;

    float totTime = 0;


    //NeuralNetwork

    public NeuralNetwork network;
                                        //input nodes have angles, either its danger or not, isGrounded, current xyz velocity, isInvincible, lives, distance to next checkpoint
    public int[] layers = new int[4] { 39, 14, 11, 4 };
                                        //Possible outputs are - sprint, move left, move right, jump
    public float[] listOfAnglesForSenses = new float[] { PI / 6, PI / 4, PI / 2, PI / 2, 2 * PI / 3, 3 * PI / 4, 5 * PI / 6, PI, 7 * PI / 6, 5 * PI / 4, 4 * PI / 3, 3 * PI / 2, 5 * PI / 3, 7 * PI / 4, 11 * PI / 6, 2 * PI };
    public float activationValue = 0.7f;

    private int checkpointsReached = 0;
    public List<GameObject> CheckPointList = new List<GameObject>();
    public float timeForCheckPoints = 999.9f;
    public GameObject timerObject;
    public GameObject checkPointObject;

    // Start is called before the first frame update

    void Start()
    {
        if (SceneManager.GetActiveScene().name == "1")
            timer.singleton.resetTime();

        Time.timeScale = 1;

        network = new NeuralNetwork(layers);
    }

    // Update is called once per frame
    void Update()
    {
        if (health > 0)
        {
            //senseDistance();


            //distancesAndDanger
            List<float[]> dAD = new List<float[]>();
            for (int i = 0; i < listOfAnglesForSenses.Length; i++)
            {
                dAD.Add(senseDistance(listOfAnglesForSenses[i]));
            }

            //Feedforward

            //Why do it like this?????????
            //Just pass in the list nooooo???
            //I did this during the night while tired need to be changed at some point
            float[] input = new float[] { dAD[0][0], dAD[0][1], dAD[1][0], dAD[1][1], dAD[2][0], dAD[3][0], dAD[3][1], dAD[4][0], dAD[4][1], dAD[5][0], dAD[5][1], dAD[6][0], dAD[6][1], dAD[7][0], dAD[7][1], dAD[8][0], dAD[8][1],
            dAD[9][0], dAD[9][1],dAD[10][0],dAD[10][1],dAD[11][0],dAD[11][1],dAD[12][0],dAD[12][1],dAD[13][0],dAD[13][1],dAD[14][0],dAD[14][1],dAD[15][0],dAD[15][1],
            (isGrounded == true ? 1 : 0), this.transform.gameObject.GetComponent<Rigidbody>().velocity.x, this.transform.gameObject.GetComponent<Rigidbody>().velocity.y, totTime, health, getDistanceToNextCheckPoint()[0] };
            float[] output = network.feedForward(input);


            //Output action

            if (output[0] > activationValue)
            {
                isSprinting = true;
                if (isSprinting == true)
                {
                    sprintStrength = sprintStrengthSaved;
                }
                else
                {
                    sprintStrength = 1.0f;
                }
            }
            else
            {
                isSprinting = !true;
                if (isSprinting == true)
                {
                    sprintStrength = sprintStrengthSaved;
                }
                else
                {
                    sprintStrength = 1.0f;
                }
            }

            if (output[1] > activationValue)
            {
                switch (wantsToDash)
                {
                    case true:
                        changeSpeed(1, false, 1);
                        wantsToDash = false;
                        break;
                    case false:
                        changeSpeed(1);
                        wantsToDash = false;
                        break;
                }

                transform.GetChild(1).transform.LookAt(transform.position + new Vector3(0, 0, 1));

                fox.SetFloat("Speed", 0.6f);

            }
            else if (output[2] > activationValue)
            {

                switch (wantsToDash)
                {
                    case true:
                        changeSpeed(-1, false, 1);
                        wantsToDash = false;
                        break;
                    case false:
                        changeSpeed(-1);
                        wantsToDash = false;
                        break;
                }
                transform.GetChild(1).transform.LookAt(transform.position + new Vector3(0, 0, -1));

                fox.SetFloat("Speed", 0.6f);


            }
            else
            {
                switch (instantOrSmooth)
                {
                    case 0:
                        changeSpeed(1, true);
                        break;
                    case 1:
                        gameObject.GetComponent<Rigidbody>().velocity = new Vector3(gameObject.GetComponent<Rigidbody>().velocity.x / 1.05f, gameObject.GetComponent<Rigidbody>().velocity.y, gameObject.GetComponent<Rigidbody>().velocity.z);
                        if (gameObject.GetComponent<Rigidbody>().velocity.x >= -0.2 && gameObject.GetComponent<Rigidbody>().velocity.x <= 0.2)
                        {
                            gameObject.GetComponent<Rigidbody>().velocity = new Vector3(0, gameObject.GetComponent<Rigidbody>().velocity.y, gameObject.GetComponent<Rigidbody>().velocity.z);
                            fox.SetFloat("Speed", 0.0f);

                        }

                        break;
                }
            }



            if ((output[3] > activationValue) && isGrounded == true)
            {
                gameObject.GetComponent<Rigidbody>().velocity += (Vector3.up * jumpMod);
                isGrounded = false;
                fox.SetBool("jumping", isGrounded);

            }
            if (isGrounded == true)
            {
                createDust(1);
            }
            else
            {
                createDust(0);
            }
        }
    }



    public void changeSpeed(int direction, bool fullStop = false, float extraSpeed = 0)
    {
        switch (instantOrSmooth)
        {
            case 0:
                gameObject.GetComponent<Rigidbody>().velocity = new Vector3(movementSpeedMax * direction * sprintStrength, gameObject.GetComponent<Rigidbody>().velocity.y, gameObject.GetComponent<Rigidbody>().velocity.z);
                if (fullStop == true)
                {
                    gameObject.GetComponent<Rigidbody>().velocity = new Vector3(0, gameObject.GetComponent<Rigidbody>().velocity.y, gameObject.GetComponent<Rigidbody>().velocity.z);
                }
                if (extraSpeed == 1)
                {
                    //gameObject.GetComponent<Rigidbody>().velocity = new Vector3(dashStrength * direction, gameObject.GetComponent<Rigidbody>().velocity.y, gameObject.GetComponent<Rigidbody>().velocity.z);
                    transform.position += Vector3.right * dashStrength * direction;
                }
                break;
            case 1:
                gameObject.GetComponent<Rigidbody>().velocity = new Vector3(gameObject.GetComponent<Rigidbody>().velocity.x + movementSpeedSpeedUp * direction * sprintStrength, gameObject.GetComponent<Rigidbody>().velocity.y, gameObject.GetComponent<Rigidbody>().velocity.z);
                if (Mathf.Abs(gameObject.GetComponent<Rigidbody>().velocity.x) > Mathf.Abs(movementSpeedMax * sprintStrength))
                {
                    gameObject.GetComponent<Rigidbody>().velocity = new Vector3(movementSpeedMax * direction, gameObject.GetComponent<Rigidbody>().velocity.y, gameObject.GetComponent<Rigidbody>().velocity.z);
                }
                if (extraSpeed == 1)
                {
                    //gameObject.GetComponent<Rigidbody>().velocity = new Vector3(dashStrength * direction, gameObject.GetComponent<Rigidbody>().velocity.y, gameObject.GetComponent<Rigidbody>().velocity.z);
                    transform.position += Vector3.right * dashStrength * direction;
                }
                break;
        }
    }


    //Player flashed when damaged
    public void activateFlash()
    {
        StartCoroutine(flash(flashDuration, flashTime));
    }

    IEnumerator flash(float timeMax, float timeInterval)
    {
        totTime = 0;
        do
        {
            Renderer foxRender = GetComponentsInChildren<Renderer>()[1];

            foxRender.enabled = false;

            yield return new WaitForSeconds(timeInterval);
            totTime += Time.deltaTime;


            foxRender.enabled = true;

            yield return new WaitForSeconds(timeInterval);
            totTime += Time.deltaTime;


        } while (totTime < timeMax);
        healthCoolDown = 0;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.layer == LayerMask.NameToLayer("Death"))
        {
            if (healthCoolDown <= 0)
            {
                health--;
                activateFlash();
                if (health == 0)
                {
                    //this.gameObject.SetActive(false);
                    transform.position = new Vector3(0, 9.86f, 0);
                    gameObject.GetComponent<Rigidbody>().velocity = new Vector3(0, gameObject.GetComponent<Rigidbody>().velocity.y, gameObject.GetComponent<Rigidbody>().velocity.z);
                    //health = 3;
                    foxHealth[1].transform.position -= Vector3.up * 100;
                    foxHealth[2].transform.position -= Vector3.up * 100;
                    //resetCheckPoints();
                }
                else
                {
                    foxHealth[health].transform.position += Vector3.up * 100;
                }
                healthCoolDown = healthCoolDownMax;
            }
        }
        if(collision.gameObject.transform.position.y + collision.gameObject.GetComponent<Collider>().bounds.size.y / 2 < transform.position.y) //Make sure the collision is the underneath ish of the fox
        {
            isGrounded = true;
            fox.SetBool("jumping", isGrounded);
        }
        fox.SetBool("Running", isSprinting);
    }

    public void createDust(int num)
    {
        switch (num)
        {
            case 1:
                dust.Play();
                break;
            case 0:
                dust.Stop();
                break;
        }
    }


    //Neural Network
    /////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////

    public float[] getDistanceToNextCheckPoint()
    {
        //The way this will work is that it will calculate the distance from the fox to the next checkpoint and also return how many checkpoints passed

        float distance = Vector3.Distance(CheckPointList[checkpointsReached].transform.position, this.transform.position);
        float[] toReturn = {distance, checkpointsReached};

        return toReturn;

    }

    public float[][][] getWeights()
    {
        return this.network.getWeights();
    }

    public void copyNetWork(float[][][] weights)
    {
        this.network.copyWeights (weights);
    }

    public void mutateNetwork(float mutationAggression)
    {
        network.mutate(mutationAggression);
    }

    public float[] senseDistance(float angle) //Shoots a ray in a direction to let the ai know how far until an object in that area, and if it's danger
    {
        
        float[] distanceToObject = { 999.9f, 0.0f };
        RaycastHit hit;
        if (Physics.Raycast(this.transform.position, new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0.0f), out hit))
        {
            if (hit.collider != null)
            {
                distanceToObject[0] = (Vector3.Distance(hit.point, this.transform.position));
                if (hit.collider.gameObject.layer == 8)
                {
                    distanceToObject[1] = 1.0f;
                }
            }
        }

        return distanceToObject;
    }

    public void checkPointCompleted(int checkpointReached)
    {
        if (checkpointReached == checkPointObject.transform.childCount - 1)
        {
            timeForCheckPoints = timerObject.GetComponent<timer>().getTime();
        }
        checkpointsReached = checkpointReached;
        
    }

    public void resetCheckPoints()
    {
        this.gameObject.SetActive(true);
        checkpointsReached = 0;
        timeForCheckPoints = 0;
        health = 3;
    }

    public float getTimes()
    {
        return this.timeForCheckPoints;
    }

    public void saveWeights()
    {
        this.network.saveWeights();
    }

    public void loadWeightsFromFile()
    {
        this.network.loadWeightsFromFile();
    }

}
