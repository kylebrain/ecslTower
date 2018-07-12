using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;

/// <summary>
/// Basic, random AI that controls the Waves
/// </summary>
public class WaveController : PreWaveCreator
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

    /* test area */
    string placeholder = "abc";
    int setSame = 0;
    int randomSet = 0;
    int currentTrial = 0;
    AgentAttribute[] agentArray = new AgentAttribute[2];

    //change these values to run the tests
    int totalTrials = 0;
    int trialsPerFrame = 0;
    

    void Start()
    {

        WaveCount = 0;
        StartCoroutine(FirstWave());

        for (int i = 0; i < agentArray.Length; i++)
        {
            AgentAttribute attr;
            do
            {
                attr = GenerateAttribute();
            } while (agentArray.Contains(attr));
            agentArray[i] = attr;
        }
        UniqueCheck(agentArray);
    }

    private void UniqueCheck(IEnumerable<AgentAttribute> attributeArray)
    {
        if (attributeArray.GroupBy(x => x).Where(x => x.Count() > 1).Select(x => x.Key).ToArray().Length > 0)
        {
            Debug.LogWarning("Unique check failed!");
        }
    }

    private void MutateTest()
    {

        Debug.Log("" + placeholder[(int)agentArray[0].Color] + placeholder[(int)agentArray[0].Size] + placeholder[(int)agentArray[0].Speed] + " | " + placeholder[(int)agentArray[1].Color] + placeholder[(int)agentArray[1].Size] + placeholder[(int)agentArray[1].Speed]);

        List<AgentAttribute> mutatedArray = new List<AgentAttribute>();

        for (int i = 0; i < agentArray.Length; i++)
        {
            AgentAttribute attr;
            int mutateCount;
            do
            {
                attr = MutateAttribute(agentArray[i], out mutateCount);
            } while (mutatedArray.Contains(attr));
            if (mutateCount == 0)
            {
                setSame++;
            }
            mutatedArray.Add(attr);
        }

        UniqueCheck(mutatedArray);

        foreach (AgentAttribute attr in mutatedArray)
        {
            if (agentArray.Contains(attr))
            {
                randomSet++;
            }
        }

        currentTrial++;
        agentArray = mutatedArray.ToArray();
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

        if (currentTrial < totalTrials)
        {
            for (int i = 0; i < trialsPerFrame; i++)
            {
                MutateTest();
            }
        }
        else if(currentTrial > 0)
        {
            Debug.Log("Random/set mutation percentage is: " + (float)randomSet / currentTrial * 100 + "\n" + randomSet + " out of " + currentTrial);
            Debug.Log("Set same mutation percantage: " + (float)setSame / currentTrial * 100 + "\n" + setSame + " out of " + currentTrial);
        }

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
            int _bogey;
            AgentAttribute attr2 = MutateAttribute(attr, out _bogey);
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

        while (infectedAttributes.Count < infectedCount)
        {
            AgentAttribute attribute;
            do
            {
                attribute = GenerateAttribute();
            } while (infectedAttributes.Contains(attribute));
            infectedAttributes.Add(attribute);
        }

        if (infectedAttributes.Count >= infectedCount)
        {
            for (int i = 0; i < infectedAttributes.Count; i++)
            {
                AgentAttribute attr = infectedAttributes[i];
                do
                {
                    int _bogey;
                    infectedAttributes[i] = MutateAttribute(attr, out _bogey); //mutates every Attribute if they have already been choosen
                } while (infectedAttributes.GroupBy(x => x).Where(x => x.Count() > 1).Select(x => x.Key).ToArray().Length > 0); //if there are two of the same attribute, keep mutating
            }
        }

        //generate a wave with proportional number of malicious agents
        if (infectedWeight < 1)
        {
            Debug.LogWarning("Infected Weight must be greater than 1!");
            infectedWeight = 1;
        }

        //number of Malicious per Attribute
        int infectedAgentCount = infectedWeight * GetInfectedAgentCount(agentsPerWave);
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
            Agent prefab = GetDecoy(benignAgent, decoyProbability);  //randomly converts some Agents to the other side based on the decoy value
            //BenignAgent Attributes and WavePath are completely random
            AgentAttribute attr;
            do
            {
                attr = GenerateAttribute();
            } while (infectedAttributes.Contains(attr));
            ret.Add(new PreAgent(prefab, GetRandomWavePath(mapDisplay), attr));
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
                Agent prefab = GetDecoy(maliciousAgent, decoyProbability); //randomly converts some Agents to the other side based on the decoy value
                PreAgent currentPreAgent = new PreAgent(prefab, GetRandomWavePath(mapDisplay), attr);
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


}
