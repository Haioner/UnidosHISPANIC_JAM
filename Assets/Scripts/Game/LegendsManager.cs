using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Collections;

public class LegendsManager : MonoBehaviour
{
    [Header("CACHE")]
    [SerializeField] private TextMeshProUGUI legendsCountText;
    [SerializeField] private TextMeshProUGUI populationText;
    [SerializeField] private TextMeshProUGUI bookCountText;
    [SerializeField] private TextMeshProUGUI soulsText;
    [SerializeField] private long population = 8000000000;
    [SerializeField] private int bookCount;
    [SerializeField] private long souls = 100;
    [SerializeField] private long soulThreshold = 100000;
    [SerializeField] private long populationMax = 20000000000;
    private int jarCount = 0;
    private float soulsAccumulated = 0f;

    public List<LegendStats> currentLegendsStats = new List<LegendStats>();
    [SerializeField] private List<LegendStats> deadLegendsStats = new List<LegendStats>();

    public delegate void OnLegendDies(int legendsCount);
    public event OnLegendDies onLegendDies;

    public static event System.EventHandler OnPopulationMax;

    private void OnEnable()
    {
        DialogueManager.OnDialogueEND += UpdateUI;
    }

    private void OnDisable()
    {
        DialogueManager.OnDialogueEND -= UpdateUI;
    }

    private void Start()
    {
        UpdateUI(this, System.EventArgs.Empty);
    }

    public void SetSouls(long amount)
    {
        souls += amount;

        if (amount > 0)
        {
            soulsAccumulated += amount;

            StartCoroutine(SpawnJarsWithDelay());
        }
        else
        {
            float totalToRemove = -amount; // amount is negative when removing

            while (jarCount > 0 && totalToRemove >= soulThreshold * jarCount)
            {
                totalToRemove -= soulThreshold * jarCount;
                FindFirstObjectByType<SoulsController>().RemoveJar();
                jarCount--;
            }
        }

        UpdateSoulsCount();
    }

    private IEnumerator SpawnJarsWithDelay()
    {
        while (soulsAccumulated >= soulThreshold * (jarCount + 1))
        {
            FindFirstObjectByType<SoulsController>().SpawnJar();

            soulsAccumulated -= soulThreshold * (jarCount + 1);
            jarCount++;

            yield return new WaitForSeconds(0.2f);
        }
    }

    public float GetSoulsAmount()
    {
        return souls;
    }

    #region World Population
    public void UpdateWorldPopulation()
    {
        CalculatePopulationBirthAndDeath();
        //CalculateLegendDeaths(characterSO);

        if (population < 0)
            population = 0;

        if(population >= populationMax)
            OnPopulationMax?.Invoke(this, System.EventArgs.Empty);

        UpdateUI(this,System.EventArgs.Empty);
    }

    private void CalculatePopulationBirthAndDeath()
    {
        float maxRange = population * 0.00084f; //8bi population = 6 mi/month
        float minRange = maxRange * 0.5f; // 50% of 6mi = 3mi
        long randomBirths = (long)Random.Range(minRange, maxRange);
        population += randomBirths;

        foreach (var legend in currentLegendsStats)
        {
            //1 power = 96mil deaths
            long deathsCausedByLegend = (long)(legend.Power * 96000);
            population -= deathsCausedByLegend;
            legend.AcumulatedDeaths += deathsCausedByLegend;
        }
    }

    public void CalculateLegendDeaths(CharacterSO characterSO)
    {
        foreach (var legend in currentLegendsStats)
        {
            if (legend.Character == characterSO)
            {
                //50% souls
                float soulsCollected = legend.AcumulatedDeaths * 0.5f;
                SetSouls((long)soulsCollected);
                legend.AcumulatedDeaths = 0;
                FindFirstObjectByType<FloatNumberManager>().SpawnGainFloat("<sprite=1> " + NumberConverter.ConvertNumberToString(soulsCollected));
            }
        }
    }
    #endregion

    public bool LegendIsDead(CharacterSO character)
    {
        foreach (var stats in deadLegendsStats)
        {
            if(stats.Character == character)
            {
                return true;
            }
        }
        return false;
    }

    public bool LegendIsRegistred(CharacterSO character)
    {
        foreach (var stats in currentLegendsStats)
        {
            if (stats.Character == character)
                return true;
        }
        return false;
    }

    public bool HaveBookSpace(CharacterSO character)
    {
        int maxLegends = bookCount * 10;

        //Character alredy in the book
        if (currentLegendsStats.Exists(legend => legend.Character == character))
            return true;

        return currentLegendsStats.Count < maxLegends;
    }

    public void AddBook(int amount)
    {
        bookCount += amount;
        FindFirstObjectByType<FloatNumberManager>().SpawnGainFloat("<sprite=3> " + amount.ToString());
        UpdateBookCount();
    }

    public LegendStats GetLegendStat(CharacterSO legendCharacter)
    {
        foreach (var stats in currentLegendsStats)
        {
            if (stats.Character == legendCharacter)
            {
                return stats;
            }
        }
        return legendCharacter.DefaultStats;
    }

    public void AddLegendStats(LegendStats legendStats)
    {
        if (LegendIsDead(legendStats.Character) || !HaveBookSpace(legendStats.Character)) return;

        LegendStats newStats = new LegendStats
        {
            Character = legendStats.Character,
            Power = legendStats.Power + legendStats.Character.DefaultStats.Power,
        };

        currentLegendsStats.Add(newStats);
        FindFirstObjectByType<FloatNumberManager>().SpawnGainFloat("<sprite=2> 1");
        UpdateLegendsCount();
    }

    public void ChangeLegendStats(LegendStats newStats)
    {
        if(currentLegendsStats.Count <= 0)
        {
            AddLegendStats(newStats);
            return;
        }

        bool characterFound = false;

        foreach (var stats in currentLegendsStats)
        {
            if (stats.Character == newStats.Character)
            {
                stats.Power += newStats.Power;
                characterFound = true;
            }
        }

        if (!characterFound)
            AddLegendStats(newStats);
    }

    public void KillLegend(CharacterSO characterSO)
    {
        if (currentLegendsStats.Count <= 0) return;

        foreach (var characterStats in currentLegendsStats)
        {
            if (characterStats.Character == characterSO)
            {
                if (characterStats.Power <= 0)
                {
                    RemoveCharacter(characterSO, characterStats);
                    FindFirstObjectByType<FloatNumberManager>().SpawnGainFloat("<sprite=2><color=red> -1");
                    onLegendDies?.Invoke(currentLegendsStats.Count);
                }

                UpdateLegendsCount();
                break;
            }

        }
    }

    private void RemoveCharacter(CharacterSO characterSO, LegendStats characterStats)
    {
        deadLegendsStats.Add(characterStats);
        FindFirstObjectByType<DialogueSpawner>().RemoveCharacter(characterSO);
        currentLegendsStats.Remove(characterStats);
    }

    public void UpdateUI(object sender, System.EventArgs e)
    {
        UpdateLegendsCount();
        UpdatePopulationCount();
        UpdateBookCount();
        UpdateSoulsCount();
    }

    private void UpdateLegendsCount()
    {
        int maxLegends = bookCount * 10;
        legendsCountText.text = currentLegendsStats.Count.ToString() + "/" + maxLegends.ToString();
    }

    private void UpdatePopulationCount()
    {
        populationText.text = NumberConverter.ConvertNumberToString(population);
    }

    private void UpdateBookCount()
    {
        bookCountText.text = bookCount.ToString();
    }

    private void UpdateSoulsCount()
    {
        soulsText.text = NumberConverter.ConvertNumberToString(souls);
    }
}
