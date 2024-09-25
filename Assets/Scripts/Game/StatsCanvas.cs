using TMPro;
using UnityEngine;
using UnityEngine.Localization;

public class StatsCanvas : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI powerTEXT;
    [SerializeField] private DialogueManager dialogueManager;
    [SerializeField] private LocalizedString powerLocale;

    private void OnEnable()
    {
        dialogueManager.onGetStats += UpdateStatsCanvas;
    }

    private void OnDisable()
    {
        dialogueManager.onGetStats -= UpdateStatsCanvas;
    }

    public void UpdateStatsCanvas(LegendStats stats)
    {
        powerTEXT.text = powerLocale.GetLocalizedString() + " " + stats.Power.ToString("F0");
    }
}
