using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveController : MonoBehaviour
{

    public BenignAgent benignAgent;
    public MaliciousAgent maliciousAgent;
    Wave currentWave = null;
    /// <summary>
    /// Mandatory prefab so a Wave can be created and used
    /// </summary>
    public Wave wavePrefab;
    private WaveManager waveManager;
    List<AgentAttribute> infectedAttributes = new List<AgentAttribute>();
    private int infectedCount = 2;
    public int agentsPerWave = 100;
    public static int WaveCount = 0;

    void Awake()
    {
        waveManager = GameObject.FindWithTag("WaveManager").GetComponent<WaveManager>();
        if (waveManager == null)
        {
            Debug.LogError("Could not find WaveManager object in the scene. Either the tag was changed or the object is missing.");
        }
    }

    private void Start()
    {
        //InitWave();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            if (currentWave != null)
            {
                Debug.LogError("Playing Wave wait until it is over!");
            }
            else
            {
                //create a wave
                currentWave = Instantiate(wavePrefab, transform);
                currentWave.CreateWaveWithList(InitWave()); //allow initialization then push the wave
                WaveCount++;
            }
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            int i = 0;
            foreach (AgentAttribute attr in infectedAttributes)
            {
                Debug.Log("Attribute " + ++i + ": " + attr);
            }
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            AgentAttribute attr = GenerateAttribute();
            Debug.Log("Original attr: " + attr);
            AgentAttribute attr2 = MutateAttribute(attr);
            Debug.Log("Mutated attr: " + attr2);
        }

    }

    private List<AgentPath> InitWave()
    {

        //choose attributes to infect
        if (infectedCount > GetAttributeComboNumber())
        {
            Debug.LogError("Infected count is higher than possible combination! Make it less than or equal to: " + GetAttributeComboNumber());
            return null;
        }

        if (infectedAttributes.Count >= infectedCount)
        {
            for (int i = 0; i < infectedAttributes.Count; i++)
            {
                infectedAttributes[i] = MutateAttribute(infectedAttributes[i]);
            }
        }
        else
        {

            while (infectedAttributes.Count < infectedCount)
            {
                AgentAttribute attribute;
                do
                {
                    attribute = GenerateAttribute();
                } while (infectedAttributes.Contains(attribute));
                infectedAttributes.Add(attribute);
            }
        }

        //generate a wave with proportional number of malicious agents
        int infectedAgentCount = GetInfectedAgentCount();
        int totalBenign = agentsPerWave - (infectedAttributes.Count * infectedCount);
        totalBenign = (int)Mathf.Clamp(totalBenign, 0f, agentsPerWave);
        //generate a pure wave first

        List<AgentPath> ret = new List<AgentPath>();

        for (int i = 0; i < totalBenign; i++)
        {
            ret.Add(new AgentPath(benignAgent, GetRandomWavePath(), GenerateAttribute()));
        }
        foreach (AgentAttribute attr in infectedAttributes)
        {
            HashSet<int> agentIndices = new HashSet<int>();
            //generate a proportional number of unique indices
            for (int i = 0; i < infectedAgentCount; i++)
            {
                int index;
                do
                {
                    index = Random.Range(0, agentsPerWave); //make sure to check if this index in larger than the list and then add it
                } while (agentIndices.Contains(index));
                agentIndices.Add(index);
            }
            //insert malicious agents at those indices
            foreach (int index in agentIndices)
            {
                AgentPath currentAgentPath = new AgentPath(maliciousAgent, GetRandomWavePath(), attr);
                if (index > ret.Count - 1)
                {
                    ret.Add(currentAgentPath);
                }
                else
                {
                    ret.Insert(index, currentAgentPath);
                }
            }



        }

        return ret;


    }

    //take previous infected attributes and mutate randomly 0-2 traits (shift up or down once)
    private AgentAttribute MutateAttribute(AgentAttribute previousAttrribute)
    {
        AgentAttribute ret = previousAttrribute;
        int traitMutateCount = Random.Range(0, 3);
        for (int i = 0; i < traitMutateCount; i++)
        {
            int traitToMutate = Random.Range(0, 3);
            int direction = Random.Range(0, 2) * 2 - 1; // generates either -1 or 1
            switch (traitToMutate)
            {
                case 0:
                    //color
                    {
                        int numberColors = System.Enum.GetNames(typeof(AgentAttribute.possibleColors)).Length - 1;
                        int newTrait = (int)previousAttrribute.Color + direction;
                        ret.Color = (AgentAttribute.possibleColors)TraitIndexWithBounds(newTrait, numberColors);
                        break;
                    }
                case 1:
                    //size
                    {
                        int numberSizes = System.Enum.GetNames(typeof(AgentAttribute.possibleSizes)).Length - 1;
                        int newTrait = (int)previousAttrribute.Size + direction;
                        ret.Size = (AgentAttribute.possibleSizes)TraitIndexWithBounds(newTrait, numberSizes);
                        break;
                    }
                case 2:
                    //speed
                    {
                        int numberSpeed = System.Enum.GetNames(typeof(AgentAttribute.possibleSpeeds)).Length - 1;
                        int newTrait = (int)previousAttrribute.Speed + direction;
                        ret.Speed = (AgentAttribute.possibleSpeeds)TraitIndexWithBounds(newTrait, numberSpeed);
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

    private int TraitIndexWithBounds(int desired, int totalValid)
    {
        if (desired < 0)
        {
            return totalValid - 1;
        }
        else if (desired >= totalValid)
        {
            return 0;
        }
        else
        {
            return desired;
        }
    }

    private WavePath GetRandomWavePath()
    {
        return waveManager.WavePathList[Random.Range(0, waveManager.WavePathList.Count)];
    }

    //get probability of spawned float (1 / number of comboes)
    //multiply that by the total number spawned
    //round
    //for total number infected multiply by the count of attributes
    private int GetInfectedAgentCount()
    {
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
        int numberColors = System.Enum.GetNames(typeof(AgentAttribute.possibleColors)).Length - 1;
        int numberSizes = System.Enum.GetNames(typeof(AgentAttribute.possibleSizes)).Length - 1;
        int numberSpeed = System.Enum.GetNames(typeof(AgentAttribute.possibleSpeeds)).Length - 1;

        ret.Color = (AgentAttribute.possibleColors)Random.Range(0, numberColors);
        ret.Size = (AgentAttribute.possibleSizes)Random.Range(0, numberSizes);
        ret.Speed = (AgentAttribute.possibleSpeeds)Random.Range(0, numberSpeed);

        return ret;
    }





    //next wave
    //take previous infected attributes and mutate randomly 0-2 traits (shift up or down once)

    //after a given number of waves add another infected attributes
    //repeat same steps as above

    public int GetAttributeComboNumber()
    {
        int numberColors = System.Enum.GetNames(typeof(AgentAttribute.possibleColors)).Length - 1;
        int numberSizes = System.Enum.GetNames(typeof(AgentAttribute.possibleSizes)).Length - 1;
        int numberSpeed = System.Enum.GetNames(typeof(AgentAttribute.possibleSpeeds)).Length - 1;

        return numberColors * numberSizes * numberSpeed;
    }

}
