using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Basic, random AI that controls the Waves
/// </summary>
public class WaveController : MonoBehaviour
{
    #region Public inspector variables

    /// <summary>
    /// The number of AgentAttributes that will be malicious
    /// </summary>
    public int infectedCount = 2;

    /// <summary>
    /// Weight of the Malicious Agent in the total Wave makeup
    /// </summary>
    /// <remarks>
    /// Total number of MaliciousAgents is equal to the fractional number of Agents of the infected Attributes time the infectedWeight
    /// Fractional number is the total number of Agents multiples by the count infected divided by the number of Attribute combination
    /// </remarks>
    public int infectedWeight = 1;

    /// <summary>
    /// Number of seconds between Waves
    /// </summary>
    /// <remarks>
    /// Perhaps make this a per Map option
    /// </remarks>
    public float timeBetweenWaves = 5f;

    #endregion

    #region Prefab public variable

    /// <summary>
    /// Prefab for the BenignAgent
    /// </summary>
    public BenignAgent benignAgent;

    /// <summary>
    /// Prefan for the MaliciousAgent
    /// </summary>
    public MaliciousAgent maliciousAgent;

    /// <summary>
    /// Mandatory prefab so a Wave can be created and used
    /// </summary>
    public Wave wavePrefab;

    #endregion

    #region UI variables

    /// <summary>
    /// Script reference that displays information on the Wave
    /// </summary>
    public WaveInfo waveInfo;
    
    /// <summary>
    /// Current Wave player is on
    /// </summary>
    public static int WaveCount = 0;

    /// <summary>
    /// Second left before the Wave is done spawning Agents
    /// </summary>
    public static float SecondsLeft;

    /// <summary>
    /// Number of Agents yet to be spawned in a Wave
    /// </summary>
    public static int AgentsRemaining;

    /// <summary>
    /// Reference to display EndScreen when Waves are over
    /// </summary>
    public EndScreen endScreen;

    MapDisplay mapDisplay;

    #endregion

    #region LevelLookup variables

    /// <summary>
    /// The percent chance that an Agent will become a decoy
    /// </summary>
    /// <remarks>
    /// Decoy is when the Agent orientation is switched
    /// </remarks>
    private int decoyProbability = LevelLookup.decoyProbability;

    /// <summary>
    /// The number of Agent in each Wave
    /// </summary>
    private int agentsPerWave = LevelLookup.spawnPerWave;

    /// <summary>
    /// The number of Waves in a Map
    /// </summary>
    private int maxWaveCount = LevelLookup.waveCount;

    #endregion

    #region Wave creation variables

    /// <summary>
    /// List of PreAgents to be used as the next Wave
    /// </summary>
    private List<PreAgent> currentPreWave = new List<PreAgent>();

    /// <summary>
    /// The Wave the Player is currently facing
    /// </summary>
    private Wave currentWave = null;

    /// <summary>
    /// Malicious Agent attributes tied to the PreWave
    /// </summary>
    private List<AgentAttribute> infectedAttributes = new List<AgentAttribute>();

    /// <summary>
    /// Malicious Agent attributes tied to the currentWave
    /// </summary>
    private List<AgentAttribute> currentWaveAttributes = new List<AgentAttribute>();

    /// <summary>
    /// If a Player is playing a Wave or waiting for the next one
    /// </summary>
    private bool Playing;
    #endregion

    void Start()
    {
        WaveCount = 0;
        mapDisplay = GameObject.FindWithTag("MapDisplay").GetComponent<MapDisplay>();
        if (mapDisplay == null)
        {
            Debug.LogError("Could not find MapDisplay object in the scene. Either the tag was changed or the object is missing.");
        }
        StartCoroutine(FirstWave());
    }

    #region First Wave Initialization

    /// <summary>
    /// Creates the first Wave
    /// </summary>
    /// <remarks>
    /// Waits for the Map to be loaded before creating the PreWave
    /// </remarks>
    IEnumerator FirstWave()
    {
        yield return new WaitUntil(() => MapDisplay.mapLoaded);
        LookupValues();
        currentPreWave = InitWave();
        if (currentPreWave == null)
        {
            Debug.LogError("Building Wave failed!");
        }
    }

    /// <summary>
    /// Looks up the variables that vary per Map in LevelLookup
    /// </summary>
    private void LookupValues()
    {
        agentsPerWave = LevelLookup.spawnPerWave;
        decoyProbability = LevelLookup.decoyProbability;
        maxWaveCount = LevelLookup.waveCount;
    }

    #endregion

    #region Starting/Stopping Waves

    /// <summary>
    /// Creates the Wave and initializes variables for the start of the Wave
    /// </summary>
    public void PlayWave()
    {
        currentWave = Instantiate(wavePrefab, transform);
        if (currentPreWave == null)
        {
            Debug.LogError("PreWave not found");
            return;
        }
        //currentPreWave will be created in FirstWave or at the end of this function
        currentWave.CreateWaveWithList(currentPreWave); //allow initialization then push the wave
        currentWaveAttributes = new List<AgentAttribute>(infectedAttributes);

        InitStaticsForWave();

        //if this is the first Wave display the waveInfo
        if (WaveCount == 1)
        {
            waveInfo.Show();
        }

        Playing = true;

        //creates the PreWave for the next Wave
        currentPreWave = InitWave();
        if (currentPreWave == null)
        {
            Debug.LogError("Building Wave failed!");
        }

    }

    /// <summary>
    /// Ends game if Waves are done or plays the next Wave after waiting timeBetweenWaves seconds
    /// </summary>
    /// <remarks>
    /// Runs after each Wave
    /// </remarks>
    IEnumerator StopWave()
    {
        if (WaveCount == maxWaveCount)
        {
            EndGame();
            yield return null;
        }
        else
        {
            InitStaticsForWait();
            yield return new WaitForSeconds(timeBetweenWaves - AudioManager.GetLength("PowerUp"));
            AudioManager.Play("PowerUp");
            yield return new WaitForSeconds(AudioManager.GetLength("PowerUp"));
            PlayWave();
        }
    }

    /// <summary>
    /// Sets all the statics used by WaveInfo to display Wave information
    /// </summary>
    private void InitStaticsForWave()
    {
        AgentsRemaining = currentWave.AgentsRemaining;
        WaveCount++;
        //countdown is set to count for a Wave
        waveInfo.SetCountDownText(true);
        //seconds are not 100% accurate but very close
        SecondsLeft = LevelLookup.spawnPerWave * LevelLookup.spawnRate;
    }

    /// <summary>
    /// Set all the static used by waveInfo to display info about waiting for the next Wave
    /// </summary>
    private void InitStaticsForWait()
    {
        waveInfo.SetCountDownText(false);
        SecondsLeft = timeBetweenWaves;
    }

    /// <summary>
    /// Ends game with a win
    /// </summary>
    private void EndGame()
    {
        endScreen.EndGame(true, Score.Money);
    }

    #endregion

    #region Update and WaveInfo Update

    /// <summary>
    /// Checks for some hotkeys to display information
    /// Also Updates the display statics and checks if the Wave has ended
    /// </summary>
    private void Update()
    {
        //Wave will delete itself when it is finished so currentWave will be set to null
        if (currentWave == null && Playing)
        {
            Playing = false;
            StartCoroutine(StopWave());
        }

        UpdateStatics();

        if (Input.GetKeyDown(KeyCode.K))
        {
            int i = 0;
            foreach (AgentAttribute attr in currentWaveAttributes)
            {
                Debug.Log("Attribute " + ++i + ": " + attr);
            }
        }

        //for testing mutations
        if (Input.GetKeyDown(KeyCode.I))
        {
            AgentAttribute attr = GenerateAttribute();
            Debug.Log("Original attr: " + attr);
            AgentAttribute attr2 = MutateAttribute(attr);
            Debug.Log("Mutated attr: " + attr2);
        }

    }

    /// <summary>
    /// Called by Update
    /// Updates the AgentRemaining through the Wave reference and decrements SecondsLeft based on the time
    /// </summary>
    private void UpdateStatics()
    {
        if (currentWave != null)
        {
            AgentsRemaining = currentWave.AgentsRemaining;
            if (AgentsRemaining == 0) //making sure that the seconds and the remaining stay the same
            {
                AgentsRemaining = 1; //I know its kinda cheesy
            }
        }

        if (SecondsLeft > 0f)
        {
            SecondsLeft -= Time.deltaTime;
            if (SecondsLeft < 0f)
            {
                SecondsLeft = 0f;
            }
        }

        //AgentsRemaining will wait at one until SecondsLeft is zero before finishing
            //Just so the display looks nice and accurate
        if (SecondsLeft == 0f && AgentsRemaining == 1)
        {
            AgentsRemaining = 0;
        }
    }

    #endregion

    #region PreWave Creation

    /// <summary>
    /// Creates the PreWave for the next Wave
    /// </summary>
    /// <returns>PreWave: List of PreAgent to be spawned in the next Wave</returns>
    private List<PreAgent> InitWave()
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
                infectedAttributes[i] = MutateAttribute(infectedAttributes[i]); //mutates every Attribute if they have already been choosen
                //***there is a chance that the attribute might mutate to the same as the other
            }
        }
        else
        {
            //generates infectedCount number of unique Attributes to infect
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

        //***if a new attribute is create it should mutate all that exist

        //generate a wave with proportional number of malicious agents
        if (infectedWeight < 1)
        {
            Debug.LogWarning("Infected Weight must be greater than 1!");
            infectedWeight = 1;
        }

        //number of Malicious per Attribute
        int infectedAgentCount = infectedWeight * GetInfectedAgentCount();
        //Benign makes up the rest
            //total Agent count subtracted by the total Malicious count
        int totalBenign = agentsPerWave - (infectedAttributes.Count * infectedAgentCount);
        if (infectedAgentCount < 0)
        {
            //there cannot be negative MalciousAgents
            Debug.LogError("Infected Agent Count is negative, perhaps the Infected Weight is negative?");
            return null;
        }
        if (totalBenign < 0)
        {
            //there cannot be negative BenignAgents
            Debug.LogError("Infected Weight is too high!");
            return null;
        }

        //generate a pure wave first
        List<PreAgent> ret = new List<PreAgent>();

        //adds totalBenign number of BenignAgents
        for (int i = 0; i < totalBenign; i++)
        {
            //probility to become a decoy is decoyRate%
            Agent prefab = GetDecoy(benignAgent);  //randomly converts some Agents to the other side based on the decoy value
            //BenignAgent Attributes and WavePath are completely random
            ret.Add(new PreAgent(prefab, GetRandomWavePath(), GenerateAttribute())); 
            //***Might Generate a BenignAgent with the traits of a MaliciousAgent
        }

        //generates indices to place MaliciousAgents at
        HashSet<int> totalAgentIndices = new HashSet<int>();
        foreach (AgentAttribute attr in infectedAttributes)
        {
            HashSet<int> thisAgentIndices = new HashSet<int>();
            //generate a proportional number of unique indices for each infectedAgent
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
                    index = Random.Range(0, agentsPerWave); //generates an index until a completely unique (all infectedAttributes) one is found
                } while (totalAgentIndices.Contains(index));
                totalAgentIndices.Add(index);
                thisAgentIndices.Add(index);
            }
            //insert malicious agents at those indices
            foreach (int index in thisAgentIndices)
            {
                Agent prefab = GetDecoy(maliciousAgent); //randomly converts some Agents to the other side based on the decoy value
                PreAgent currentPreAgent = new PreAgent(prefab, GetRandomWavePath(), attr);
                if (index > ret.Count - 1) //adds it to the back if the index is too larger
                {
                    ret.Add(currentPreAgent);
                }
                else
                {
                    ret.Insert(index, currentPreAgent); //inserts it if the index is contained in the List
                }
            }



        }

        if (ret.Count != agentsPerWave)
        {
            //the PreWave should be as larger as the agentsPerWave value if everything goes right
            Debug.LogError("Something went wrong, Wave is not the same count as desired!");
            return null;
        }

        return ret;

        //after a given number of waves add another infected attributes
        //repeat same steps as above
    }

    #region PreWave Helper Functions

    /// <summary>
    /// Flips the Malicious/Benign agent weighted randomly
    /// </summary>
    /// <param name="normalAgentPrefab">The original orientation of the Agent</param>
    /// <returns>The opposite Agent or the normal Agent</returns>
    private Agent GetDecoy(Agent normalAgentPrefab)
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
    /// <summary>
    /// Takes previous infected Attributes and mutate randomly 0-2 traits (shift up or down once)
    /// </summary>
    /// <param name="previousAttrribute">The current Attribute</param>
    /// <returns>The randomly mutated Attribute</returns>
    private AgentAttribute MutateAttribute(AgentAttribute previousAttrribute)
    {
        AgentAttribute ret = previousAttrribute;
        //chooses 0-2 traits to mutate
        int traitMutateCount = Random.Range(0, 3);
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
    private int TraitIndexWithBounds(int desired, int totalValid)
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
    private WavePath GetRandomWavePath()
    {
        return mapDisplay.WavePathList[Random.Range(0, mapDisplay.WavePathList.Count)];
    }

    /// <summary>
    /// Finds the number of Agents that would probably spawn of one unique Attribute
    /// </summary>
    /// <returns>The number of Agents that would probably spawn of one unique Attribute</returns>
    private int GetInfectedAgentCount()
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

    #endregion
    #endregion

}
