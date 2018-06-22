using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : PreWaveCreator
{
    public TutorialTips tutorialTips;
    private float bufferTime;

    List<PreAgent> firstPreWave = new List<PreAgent>();
    int firstInitialCount;
    List<PreAgent> secondPreWave = new List<PreAgent>();
    int secondInitialCount;

    AgentAttribute[] infectedAttributes = new AgentAttribute[3];

    Wave currentWave;

    private bool  dismissed = false;

    private static int callFunction = -1;

    private void Start()
    {
        bufferTime = mapDisplay.currentMap.spawnRate / 2f;

        //Wave 1

        infectedAttributes[0] = GenerateAttribute();

        //1
        AddBenignAgent(15, firstPreWave, 0);
        //2
        AddMaliciousAgent(2, firstPreWave, 0, 0);
        //7
        AddBenignAgent(4, firstPreWave, 0);
        AddMaliciousAgent(1, firstPreWave, 0, 0);
        AddBenignAgent(4, firstPreWave, 0);
        AddMaliciousAgent(1, firstPreWave, 0, 0);
        AddBenignAgent(4, firstPreWave, 0);
        AddMaliciousAgent(1, firstPreWave, 0, 0);
        //8
        AddMaliciousAgent(4, firstPreWave, 0, 1);
        //9
        AddBenignAgent(15, firstPreWave);
        //10
        AddBenignAgent(4, firstPreWave, 0);
        AddMaliciousAgent(1, firstPreWave, 0, 0);
        AddBenignAgent(4, firstPreWave, 0);
        AddMaliciousAgent(1, firstPreWave, 0, 0);

        firstInitialCount = firstPreWave.Count;

        //Wave2
        infectedAttributes[1] = MutateAttribute(infectedAttributes[0]);
        do
        {
            infectedAttributes[2] = GenerateAttribute();
        } while (infectedAttributes[2].Equals(infectedAttributes[1]));

        //13
        AddMaliciousAgent(1, secondPreWave, 1, 0);
        //15
        AddMaliciousAgent(1, secondPreWave, 1, 0);
        AddMaliciousAgent(1, secondPreWave, 2, 0);
        //16 change to random later
        AddBenignAgent(4, secondPreWave);
        AddMaliciousAgent(1, secondPreWave, 1);
        AddBenignAgent(4, secondPreWave);
        AddMaliciousAgent(1, secondPreWave, 2);
        AddBenignAgent(4, secondPreWave);
        AddMaliciousAgent(1, secondPreWave, 1);
        AddBenignAgent(4, secondPreWave);
        AddMaliciousAgent(1, secondPreWave, 2);

        secondInitialCount = secondPreWave.Count;

        currentWave = Instantiate(wavePrefab, transform);
        currentWave.CreateWaveWithList(firstPreWave);

        StartCoroutine(RunTutorial());

        
    }

    IEnumerator RunTutorial()
    {
        //1
        tutorialTips.Show("Benign packets give you money when they reach their destination.");
        yield return new WaitUntil(() => currentWave.AgentsRemaining == firstInitialCount - 16);
        yield return new WaitForSeconds(bufferTime);
        currentWave.PauseSpawning();
        yield return new WaitUntil(() => WaitForFunction(0));
        currentWave.Pause();

        //2
        tutorialTips.Show("Malicious packets damage your health.");
        yield return new WaitUntil(() => DismissCheck());
        yield return new WaitForSeconds(bufferTime);

        //3
        tutorialTips.Show("Spend money to repair your health to full.", false);
        yield return new WaitUntil(() => Score.Health == Score.MaxHealth);
        tutorialTips.ShowDismiss();
        yield return new WaitUntil(() => DismissCheck());
        yield return new WaitForSeconds(bufferTime);
        currentWave.PauseSpawning(false);
        currentWave.Pause(false);
        yield return new WaitUntil(() => currentWave.AgentsRemaining == firstInitialCount - 17);
        currentWave.PauseSpawning();
        currentWave.Pause();

        //4
        tutorialTips.Show("The way to stop bad packets is by placing a router.");
        yield return new WaitUntil(() => DismissCheck());
        yield return new WaitForSeconds(bufferTime);
        tutorialTips.Show("Place a Router to filter out the malicious packet.", false);
        yield return new WaitUntil(() => WaitForFunction(1));
        tutorialTips.ShowDismiss();
        yield return new WaitUntil(() => DismissCheck());
        yield return new WaitForSeconds(bufferTime);

        //5
        tutorialTips.Show("Set the first two color and size filters to filter out the malicious packet.", false);
        AgentAttribute attr = infectedAttributes[0];
        attr.Speed = AgentAttribute.PossibleSpeeds.dontCare;
        yield return new WaitUntil(() => RouterSetCorrectly(attr));
        tutorialTips.ShowDismiss();
        yield return new WaitUntil(() => DismissCheck());
        yield return new WaitForSeconds(bufferTime);

        //6
        tutorialTips.Show("The ring packet on top of the router and in the HUD shows your filtering.");
        yield return new WaitUntil(() => DismissCheck());
        yield return new WaitForSeconds(bufferTime);

        //7
        currentWave.PauseSpawning(false);
        currentWave.Pause(false);
        yield return new WaitUntil(() => currentWave.AgentsRemaining == firstInitialCount - 32);

        //8
        LevelLookup.markMalicious = false;
        yield return new WaitUntil(() => currentWave.AgentsRemaining == firstInitialCount - 36);
        currentWave.PauseSpawning();
        yield return new WaitUntil(() => Score.Health == 0);
        currentWave.Pause();
        tutorialTips.Show("Not all levels will mark bad packets.");
        yield return new WaitUntil(() => DismissCheck());
        yield return new WaitForSeconds(bufferTime);

        //9
        tutorialTips.Show("When servers are down you won't take damage, but you can't make money.");
        yield return new WaitUntil(() => DismissCheck());
        yield return new WaitForSeconds(bufferTime);

        //10
        tutorialTips.Show("Rebuild your servers to keep making money", false);
        yield return new WaitUntil(() => WaitForFunction(2));
        tutorialTips.ShowDismiss();
        yield return new WaitUntil(() => DismissCheck());
        currentWave.PauseSpawning(false);
        currentWave.Pause(false);
        tutorialTips.Show("Your servers will be online once your health returns to full.");
        yield return new WaitUntil(() => currentWave == null);
        yield return new WaitForSeconds(bufferTime);

        //11
        tutorialTips.Show("When the wave is over you have time to rest.");
        yield return new WaitUntil(() => DismissCheck());
        yield return new WaitForSeconds(bufferTime);





    }

    private bool WaitForFunction(int index)
    {
        if(index == callFunction && index != -1 && callFunction != -1)
        {
            callFunction = -1;
            return true;
        } else
        {
            return false;
        }
    }

    public static void CallFunction(int index)
    {
        callFunction = index;
    }

    private bool RouterSetCorrectly(AgentAttribute attribute)
    {
        RouterBuilding router = FindObjectOfType<RouterBuilding>();
        if(router == null)
        {
            return false;
        }

        AgentAttribute routerAttribute = router.filter[0];
        return routerAttribute.Color == attribute.Color && routerAttribute.Size == attribute.Size && routerAttribute.Speed == attribute.Speed;

    }


    private bool DismissCheck()
    {
        if (dismissed)
        {
            dismissed = false;
            return true;
        } else
        {
            return false;
        }
    }

    public void Dismiss(bool _dismissed)
    {
        dismissed = _dismissed;
    }

    private void AddBenignAgent(int count, List<PreAgent> preWave, int pathNumber = -1)
    {
        WavePath wavePath;
        if (pathNumber <= -1)
        {
            wavePath = GetRandomWavePath(mapDisplay);
        }
        else
        {
            wavePath = mapDisplay.WavePathList[pathNumber];
        }
        for (int i = 0; i < count; i++)
        {
            AgentAttribute attr;
            do
            {
                attr = GenerateAttribute();
            } while (System.Array.Exists(infectedAttributes, a => a.Equals(attr)));
            preWave.Add(new PreAgent(benignAgent, wavePath, attr));
        }
    }

    private void AddMaliciousAgent(int count, List<PreAgent> preWave, int attr, int pathNumber = -1)
    {
        WavePath wavePath;
        if(pathNumber <= -1)
        {
            wavePath = GetRandomWavePath(mapDisplay);
        } else
        {
            wavePath = mapDisplay.WavePathList[pathNumber];
        }
        for (int i = 0; i < count; i++)
        {
            preWave.Add(new PreAgent(maliciousAgent, wavePath, infectedAttributes[attr]));
        }
    }
}
