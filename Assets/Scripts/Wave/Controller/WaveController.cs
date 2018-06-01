using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaveController : MonoBehaviour
{
    public int agentsPerWave = 100;
    public int infectedCount = 2;
    public int infectedWeight = 1;
    public float timeBetweenWaves = 2f;

    public BenignAgent benignAgent;
    public MaliciousAgent maliciousAgent;
    public Text waveText;
    /// <summary>
    /// Mandatory prefab so a Wave can be created and used
    /// </summary>
    public Wave wavePrefab;

    public static int WaveCount = 0;

    private WaveManager waveManager;

    private List<AgentPath> currentPreWave = new List<AgentPath>();
    private Wave currentWave = null;
    private List<AgentAttribute> infectedAttributes = new List<AgentAttribute>();
    [HideInInspector]
    public bool Playing;
    private NextWaveButton waveButton;

    void Start()
    {
        waveManager = GameObject.FindWithTag("WaveManager").GetComponent<WaveManager>();
        if (waveManager == null)
        {
            Debug.LogError("Could not find WaveManager object in the scene. Either the tag was changed or the object is missing.");
        }
        StartCoroutine(FirstWave());
    }
    IEnumerator FirstWave()
    {
        yield return new WaitUntil(() => waveManager.WavePathList.Count > 0);
        currentPreWave = InitWave();
        if (currentPreWave == null)
        {
            Debug.LogError("Building Wave failed!");
        }
    }

    public void PlayWave(NextWaveButton button)
    {
        waveButton = button;
        currentWave = Instantiate(wavePrefab, transform);
        if (currentPreWave == null)
        {
            Debug.LogError("PreWave not found");
            return;
        }
        //currentPreWave will be created in FirstWave or at the end of this function
        currentWave.CreateWaveWithList(currentPreWave); //allow initialization then push the wave
        WaveCount++;

        if (WaveCount == 1)
        {
            waveText.transform.parent.gameObject.SetActive(true);
        }

        waveText.text = "Wave: " + WaveCount;

        Playing = true;
        currentPreWave = InitWave();
        if(currentPreWave == null)
        {
            Debug.LogError("Building Wave failed!");
        }

    }

    IEnumerator StopWave()
    {
        if (waveButton != null)
        {
            yield return new WaitForSeconds(timeBetweenWaves);
            waveButton.SetEnable(true);
        }
    }

    private void Update()
    {
        if (currentWave == null && Playing)
        {
            Playing = false;
            StartCoroutine(StopWave());
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            int i = 0;
            foreach (AgentAttribute attr in infectedAttributes)
            {
                Debug.Log("Attribute " + ++i + ": " + attr);
                //these attributes are for the preWave, store them somewhere if you want to show the current Wave's attributes
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
            Debug.LogError("Infected count is higher than possible combination! Make it less than: " + GetAttributeComboNumber());
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
        if (infectedWeight < 1)
        {
            Debug.LogWarning("Infected Weight must be greater than 1!");
            infectedWeight = 1;
        }
        int infectedAgentCount = infectedWeight * GetInfectedAgentCount();
        int totalBenign = agentsPerWave - (infectedAttributes.Count * infectedAgentCount);
        if (infectedAgentCount < 0)
        {
            Debug.LogError("Infected Agent Count is negative, perhaps the Infected Weight is negative?");
            return null;
        }
        if (totalBenign < 0)
        {
            Debug.LogError("Infected Weight is too high!");
            return null;
        }
        //generate a pure wave first

        List<AgentPath> ret = new List<AgentPath>();

        for (int i = 0; i < totalBenign; i++)
        {
            ret.Add(new AgentPath(benignAgent, GetRandomWavePath(), GenerateAttribute()));
        }
        HashSet<int> totalAgentIndices = new HashSet<int>();
        foreach (AgentAttribute attr in infectedAttributes)
        {
            HashSet<int> thisAgentIndices = new HashSet<int>();
            //generate a proportional number of unique indices
            for (int i = 0; i < infectedAgentCount; i++)
            {
                if (totalAgentIndices.Count >= agentsPerWave)
                {
                    Debug.LogError("Infected count is higher than possible combination! Make it less than: " + GetAttributeComboNumber());
                    return null;
                }
                int index;
                do
                {
                    index = Random.Range(0, agentsPerWave); //make sure to check if this index in larger than the list and then add it
                } while (totalAgentIndices.Contains(index));
                totalAgentIndices.Add(index);
                thisAgentIndices.Add(index);
            }
            //insert malicious agents at those indices
            foreach (int index in thisAgentIndices)
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

        if (ret.Count != agentsPerWave)
        {
            Debug.LogError("Something went wrong, Wave is not the same count as desired!");
            return null;
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
