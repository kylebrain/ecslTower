using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Helper functions for PreWave creation
/// </summary>
public class PreWaveCreator : MonoBehaviour
{

    /// <summary>
    /// Mandatory prefab so a Wave can be created and used
    /// </summary>
    public Wave wavePrefab;

    /// <summary>
    /// Prefab for the BenignAgent
    /// </summary>
    public BenignAgent benignAgent;

    /// <summary>
    /// Prefan for the MaliciousAgent
    /// </summary>
    public MaliciousAgent maliciousAgent;

    protected MapDisplay mapDisplay;

    private void Awake()
    {
        mapDisplay = GameObject.FindWithTag("MapDisplay").GetComponent<MapDisplay>();
        if (mapDisplay == null)
        {
            Debug.LogError("Could not find MapDisplay object in the scene. Either the tag was changed or the object is missing.");
        }
    }

    /// <summary>
    /// Flips the Malicious/Benign agent weighted randomly
    /// </summary>
    /// <param name="normalAgentPrefab">The original orientation of the Agent</param>
    /// <returns>The opposite Agent or the normal Agent</returns>
    protected Agent GetDecoy(Agent normalAgentPrefab, int decoyProbability)
    {
        if (normalAgentPrefab.GetType() != benignAgent.GetType() && normalAgentPrefab.GetType() != maliciousAgent.GetType())
        {
            Debug.LogError("Prefab must be either benign or malicious!");
            return null;
        }
        Agent prefab = normalAgentPrefab;

        //the probability for the random number to be equal or less to the decoyProbaility is decoyProbablity%
        if (Random.Range(1, 101) <= decoyProbability)
        {

            if (normalAgentPrefab.GetType() == benignAgent.GetType())
            {
                prefab = maliciousAgent;
            }
            else
            {
                prefab = benignAgent;
            }
        }
        return prefab;
    }

    protected int TraitsToMutate()
    {
        //chooses 0-2 traits to mutate
        int[] mutateWeight = {1, 8, 4 };

        int sum = 0;
        foreach (int i in mutateWeight)
        {
            sum += i;
        }
        int traitSelector = Random.Range(0, sum);
        int range = 0;
        for (int i = 0; i < mutateWeight.Length; i++)
        {
            range += mutateWeight[i];
            if (traitSelector < range)
            {
                return i;
            }
        }
        return 0;
    }

    /// <summary>
    /// Takes previous infected Attributes and mutate randomly 0-2 traits (shift up or down once)
    /// </summary>
    /// <param name="previousAttrribute">The current Attribute</param>
    /// <returns>The randomly mutated Attribute</returns>
    protected AgentAttribute MutateAttribute(AgentAttribute previousAttrribute)
    {
        AgentAttribute ret = previousAttrribute;
        //chooses 0-2 traits to mutate
        int traitMutateCount = TraitsToMutate();



        //repeats traitMutateCount number of times
        for (int i = 0; i < traitMutateCount; i++)
        {
            //picks which trait to mutate
            int traitToMutate = Random.Range(0, 3);
            //shifts it in either direction
            int direction = Random.Range(0, 2) * 2 - 1; // generates either -1 or 1
            switch (traitToMutate)
            {
                case 0:
                    //color
                    {
                        int numberColors = System.Enum.GetNames(typeof(AgentAttribute.PossibleColors)).Length - 1;
                        int newTrait = (int)previousAttrribute.Color + direction;
                        ret.Color = (AgentAttribute.PossibleColors)TraitIndexWithBounds(newTrait, numberColors);
                        break;
                    }
                case 1:
                    //size
                    {
                        int numberSizes = System.Enum.GetNames(typeof(AgentAttribute.PossibleSizes)).Length - 1;
                        int newTrait = (int)previousAttrribute.Size + direction;
                        ret.Size = (AgentAttribute.PossibleSizes)TraitIndexWithBounds(newTrait, numberSizes);
                        break;
                    }
                case 2:
                    //speed
                    {
                        int numberSpeed = System.Enum.GetNames(typeof(AgentAttribute.PossibleSpeeds)).Length - 1;
                        int newTrait = (int)previousAttrribute.Speed + direction;
                        ret.Speed = (AgentAttribute.PossibleSpeeds)TraitIndexWithBounds(newTrait, numberSpeed);
                        break;
                    }
                default:
                    //??
                    Debug.LogError("Out of range, perhap you changed the traitToMutate line?");
                    break;
            }
        }
        return ret;
    }

    /// <summary>
    /// Wraps the desired around the totalValid
    /// </summary>
    /// <param name="desired">The incremented or decremented value to be placed in bounds</param>
    /// <param name="totalValid">The total number of valid options</param>
    /// <returns></returns>
    protected int TraitIndexWithBounds(int desired, int totalValid)
    {
        if (desired < 0)
        {
            //if less than one, wrap around to the end of the object
            return totalValid - 1;
        }
        else if (desired >= totalValid)
        {
            //if greater or equal to totalValid, wrap around to the front of the object
            return 0;
        }
        else
        {
            //desired is already in bounds
            return desired;
        }
    }

    /// <summary>
    /// Returns a random WavePath from the MapDisplay
    /// </summary>
    /// <returns>Random WavePath</returns>
    protected WavePath GetRandomWavePath(MapDisplay mapDisplay)
    {
        return mapDisplay.WavePathList[Random.Range(0, mapDisplay.WavePathList.Count)];
    }

    /// <summary>
    /// Finds the number of Agents that would probably spawn of one unique Attribute
    /// </summary>
    /// <returns>The number of Agents that would probably spawn of one unique Attribute</returns>
    protected int GetInfectedAgentCount(int agentsPerWave)
    {
        //get probability of spawned float (1 / number of comboes)
        //multiply that by the total number spawned
        //round
        //for total number infected multiply by the count of attributes
        float percentInfected = 1f / GetAttributeComboNumber();
        return Mathf.RoundToInt(percentInfected * agentsPerWave);
    }

    /// <summary>
    /// Randomly generates an AgentAttribute for the Agent
    /// </summary>
    /// <returns>A random AgentAttribute</returns>
    public AgentAttribute GenerateAttribute()
    {
        AgentAttribute ret;
        int numberColors = System.Enum.GetNames(typeof(AgentAttribute.PossibleColors)).Length - 1;
        int numberSizes = System.Enum.GetNames(typeof(AgentAttribute.PossibleSizes)).Length - 1;
        int numberSpeed = System.Enum.GetNames(typeof(AgentAttribute.PossibleSpeeds)).Length - 1;

        ret.Color = (AgentAttribute.PossibleColors)Random.Range(0, numberColors);
        ret.Size = (AgentAttribute.PossibleSizes)Random.Range(0, numberSizes);
        ret.Speed = (AgentAttribute.PossibleSpeeds)Random.Range(0, numberSpeed);

        return ret;
    }

    /// <summary>
    /// Gets the total number of unique Attributes possible
    /// </summary>
    /// <returns>The total number of unique Attributes possible</returns>
    public int GetAttributeComboNumber()
    {
        int numberColors = System.Enum.GetNames(typeof(AgentAttribute.PossibleColors)).Length - 1;
        int numberSizes = System.Enum.GetNames(typeof(AgentAttribute.PossibleSizes)).Length - 1;
        int numberSpeed = System.Enum.GetNames(typeof(AgentAttribute.PossibleSpeeds)).Length - 1;

        return numberColors * numberSizes * numberSpeed;
    }

}
