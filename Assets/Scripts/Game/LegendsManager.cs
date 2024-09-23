using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LegendsManager : MonoBehaviour
{
    [Header("CACHE")]
    [SerializeField] private TextMeshProUGUI legendsCountText;
    [SerializeField] private TextMeshProUGUI populationText;
    [SerializeField] private TextMeshProUGUI bookCountText;
    [SerializeField] private TextMeshProUGUI soulsText;
    [SerializeField] private long population = 8000000000;
    [SerializeField] private int bookCount;
    [SerializeField] private float souls = 100;

    public List<LegendStats> currentLegendsStats = new List<LegendStats>();
    [SerializeField] private List<LegendStats> deadLegendsStats = new List<LegendStats>();

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

    public void SetSouls(float amount)
    {
        souls += amount;
    }

    public void AddLegendStats(LegendStats legendStats)
    {
        if (LegendIsDead(legendStats.Character) || !HaveBookSpace(legendStats.Character)) return;

        LegendStats newStats = new LegendStats();
        newStats.Character = legendStats.Character;
        newStats.Popularity = newStats.Character.DefaultStats.Popularity;
        newStats.Power = newStats.Character.DefaultStats.Power;

        currentLegendsStats.Add(newStats);
        UpdateLegendsCount();
    }

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

    public void ChangeLegendStats(LegendStats newStats)
    {
        if(currentLegendsStats.Count <= 0)
            AddLegendStats(newStats);

        bool characterFound = false;

        foreach (var stats in currentLegendsStats)
        {
            if (stats.Character == newStats.Character)
            {
                stats.Popularity += newStats.Popularity;
                stats.Power += newStats.Power;
                characterFound = true;
                break;
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
                if (characterStats.Popularity <= 0 || characterStats.Power <= 0)
                {
                    RemoveCharacter(characterSO, characterStats);
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
        soulsText.text = souls.ToString();
    }
}
