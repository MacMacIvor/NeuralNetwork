using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
public class NeuralNetwork : MonoBehaviour
{
    private int[] layers; //The layers of the nodes
    private float[][] nodes; //The node matrix
    private float[][][] weights; //The weight matrix
    private float evolvedPosition; //How evolved is the network

    public NeuralNetwork(int[] layers) //the constructor for the neuralnetwork class
    {
        //Copies the layers into this class object
        this.layers = new int[layers.Length];
        for (int i = 0; i < layers.Length; i++)
        {
            this.layers[i] = layers[i];
        }

        //called to initialize the nodes
        initializeNeurons();
        //Called to initialize the weights
        initializeWeight();
    }

    //Create a copy of anything that can be referenced in the network
    public NeuralNetwork(NeuralNetwork copyNetwork)
    {
        this.layers = new int[copyNetwork.layers.Length];
        for (int i = 0; i < copyNetwork.layers.Length; i++)
        {
            this.layers[i] = copyNetwork.layers[i];
        }
        initializeNeurons();
        initializeWeight();
        copyWeights(copyNetwork.weights);
    }

    private void initializeNeurons()
    {
        //Initialization of the nodes
        List<float[]> nodeList = new List<float[]>();

        //Runs through all the layers
        for (int i = 0; i < layers.Length; i++)
        {
            //Adds the layers of nodes
            nodeList.Add(new float[layers[i]]);
        }

        //Converts the list to an array
        nodes = nodeList.ToArray();
    }

    //Initialization of the weights
    private void initializeWeight()
    {
        //List of the weights that will be converted into a weight array
        List<float[][]> weightsList = new List<float[][]>();

        //Going through all the nodes that have weights
        for (int i = 1; i < layers.Length; i++)
        {
            //list of the current layer's weights
            List<float[]> layerWeightsList = new List<float[]>();

            //the nodes in the previous layer
            int previousLayerNodes = layers[i - 1];

            //Go through all the nodes that are in the current layer
            for (int j = 0; j < nodes[i].Length; j++)
            {
                //The weights for the nodes
                float[] nodeWeights = new float[previousLayerNodes];

                //set a random value to the weights between 0.5f and -0.5f
                for (int k = 0; k < previousLayerNodes; k++)
                {
                    //Give a random value
                    nodeWeights[k] = (float)(UnityEngine.Random.Range(-0.5f, 0.5f));
                }
                //Add the current layer's node's weights to the list
                layerWeightsList.Add(nodeWeights);
            }
            //Add the weight layer's node's weights to the list
            weightsList.Add(layerWeightsList.ToArray());
        }
        //Convert the data into an array
        weights = weightsList.ToArray();

    }

    //Progresses to the next step of the neural network
    public float[] feedForward(float[] input)
    {
        //Adds the input to the first layer of the nodes
        for (int i = 0; i < input.Length; i++)
        {
            //Sets the first layer of the nodes to the input value
            nodes[0][i] = input[i];
        }
        //Goes over all nodes to calculate the feedforward values
        for (int i = 1; i < layers.Length; i++)
        {
            for (int j = 0; j < nodes[i].Length; j++)
            {
                float value = 0.0f;
                for (int k = 0; k < nodes[i - 1].Length; k++)
                {
                    //Adds all the weights of connections in this layer with the previous layer
                    value += weights[i - 1][j][k] * nodes[i - 1][k];
                }
                //Hyperbolic tangent activation
                nodes[i][j] = (float)(Math.Tanh(value));
            }
        }
        //Returns the nodes
        return nodes[nodes.Length - 1];
    }

    //Custom Feed Forward
    //Multiply values together in a certain way
    //Restriction restricts the numbers used in a layer (should add a check for non-grouped numbers later)
    //Pairing gives what should be multiplied together
    //Too lazy to add a thing for it to continue past the pairing so for now add all the things thats are not pared into their own thing
    public float[] feedForwardCustom(float[] input, int[][] restriction, bool[] restrictionBool, int[][][] pairing, bool[] pairingBool)
    {
        //Adds the input to the first layer of the nodes
        for (int i = 0; i < input.Length; i++)
        {
            //Sets the first layer of the nodes to the input value
            nodes[0][i] = input[i];
        }

        //Goes over all nodes to calculate the feedforward values
        for (int i = 1; i < layers.Length; i++)
        {
            float value = 0.0f;


            if (restrictionBool[i - 1])
            {
                //Pairing Process
                if (pairingBool[i - 1])
                {
                    for (int l = 0; l < pairing[i - 1].Length; l++)
                    {
                        value = 0.0f;

                        for (int m = 0; m < pairing[i - 1][l].Length; m++)
                        {
                            bool wasInList = false;
                            for (int n = 0; n < pairing[i - 1][l].Length; n++)
                            {
                                                    //Bigger than min               smaller than max
                                                    //Thats what we want, so we look for the opposite
                                if (restriction[i - 1][0] >= m && restriction[i-1][1] <= m) 
                                {
                                    wasInList = true;
                                }
                            }
                            if (!wasInList)
                            {
                                //Now that we have gotten to only the pairings, we add them together
                                value += weights[i - 1][l][pairing[i - 1][l][m]] * nodes[i - 1][pairing[i - 1][l][m]];
                            }
                        }
                        //Save that node
                        //We assume that there are enough nodes in the next layer
                        //Could add a check but nahhhh
                        nodes[i][l] = (float)(Math.Tanh(value));

                    }
                }
                else
                {
                    for (int j = 0; j < nodes[i].Length; j++)
                    {
                        value = 0.0f;
                        for (int k = 0; k < nodes[i - 1].Length; k++)
                        {
                            bool wasInList = false;
                            for (int n = 0; n < pairing[i - 1][j].Length; n++)
                            {
                                                    //Bigger than min               smaller than max
                                                    //Thats what we want, so we look for the opposite
                                if (restriction[i - 1][0] >= k && restriction[i-1][1] <= k) 
                                {
                                    wasInList = true;
                                }
                            }
                            if (!wasInList)
                            {
                                //Adds all the weights of connections in this layer with the previous layer
                                value += weights[i - 1][j][k] * nodes[i - 1][k];
                            }
                        }
                        nodes[i][j] = (float)(Math.Tanh(value));

                    }
                }
            }
            //No restrictions
            else
            {
                //Pairing Process
                if (pairingBool[i - 1])
                {
                    for (int l = 0; l < pairing[i - 1].Length; l++)
                    {
                        value = 0.0f;

                        for (int m = 0; m < pairing[i - 1][l].Length; m++)
                        {
                            //Now that we have gotten to only the pairings, we add them together
                            value += weights[i - 1][l][pairing[i - 1][l][m]] * nodes[i - 1][pairing[i - 1][l][m]];
                        }
                        //Save that node
                        //We assume that there are enough nodes in the next layer
                        //Could add a check but nahhhh
                        nodes[i][l] = (float)(Math.Tanh(value));

                    }
                }
                else
                {
                    for (int j = 0; j < nodes[i].Length; j++)
                    {
                        value = 0.0f;
                        for (int k = 0; k < nodes[i - 1].Length; k++)
                        {
                            //Adds all the weights of connections in this layer with the previous layer
                            value += weights[i - 1][j][k] * nodes[i - 1][k];
                        }
                        nodes[i][j] = (float)(Math.Tanh(value));

                    }
                }
            }



            //Hyperbolic tangent activation
            nodes[i][j] = (float)(Math.Tanh(value));


        }
        //Returns the nodes
        return nodes[nodes.Length - 1];

    }

    public void mutate(float agressivity)
    {
        //Go through every single node
        for (int i = 0; i < weights.Length; i++)
        {
            for (int j = 0; j < weights[i].Length; j++)
            {
                //Go through every single connection that node has in the previous layer
                for (int k = 0; k < weights[i][j].Length; k++)
                {
                    float weight = weights[i][j][k];

                    //Mutate the weight
                    float randomNumber = (float)UnityEngine.Random.Range(1.0f - (agressivity/100), 1.0f + (agressivity/100.0f));
                    weight *= randomNumber;

                    if (weight > 1.0f)
                        weight = 1.0f;
                    else if (weight < -1.0f)
                        weight = -1.0f;

                    weights[i][j][k] = weight;
                }
            }
        }
    }

    //Copies the weights of the network
    public void copyWeights(float[][][] _copyWeights)
    {
        //Go through every single node
        for (int i = 0; i < weights.Length; i++)
        {
            for (int j = 0; j < weights[i].Length; j++)
            {
                //Go through every single connection that node has in the previous layer
                for (int k = 0; k < weights[i][j].Length; k++)
                {
                    //Set every single weight to the copied weights
                    weights[i][j][k] = _copyWeights[i][j][k];
                }
            }
        }
    }

    //Adds to the evolved position
    public void addEvolvedPosition(float pos)
    {
        evolvedPosition += pos;
    }
    //Sets the total distance to the evolved position
    public void setEvolvedPosition(float pos)
    {
        evolvedPosition = pos;
    }
    //Returns the distance that the balls had traveled
    public float getEvolvedPosition()
    {
        return evolvedPosition;
    }

    public NeuralNetwork GetNeural()
    {
        return this;
    }

    public float[][][] getWeights()
    {
        return this.weights;
    }

    public void saveWeights()
    {

        using (StreamWriter outputFile = new StreamWriter(System.IO.Directory.GetCurrentDirectory() + "/SavedNeuralNetwork/NeuralNetwork.txt"))
        {
            //Go through every single node
            for (int i = 0; i < weights.Length; i++)
            {
                for (int j = 0; j < weights[i].Length; j++)
                {
                    //Go through every single connection that node has in the previous layer
                    for (int k = 0; k < weights[i][j].Length; k++)
                    {
                        //Set every single weight to the copied weights
                        outputFile.WriteLine(weights[i][j][k]);
                    }
                }
            }
        }

    }

    public void loadWeightsFromFile()
    {
        string objectsData;
        int i = 0;
        int j = 0;
        int k = 0;
        using (var line = new StreamReader(System.IO.Directory.GetCurrentDirectory() + "/SavedNeuralNetwork/NeuralNetwork.txt"))
            while ((objectsData = line.ReadLine()) != null) //Set up this way in case in the future we want to add times for each individual level
            {

                weights[i][j][k] = Single.Parse(objectsData);
                k++;
                if (k == weights[i][j].Length)
                {
                    k = 0;

                    j++;
                }
                if (i == weights.Length)
                    break;
                if (j == weights[i].Length)
                {
                    j = 0;
                    i++;

                }

            }
    }

}
