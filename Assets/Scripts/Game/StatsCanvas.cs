using TMPro;
using UnityEngine;

public class StatsCanvas : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI powerTEXT;
    [SerializeField] private DialogueManager dialogueManager;

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
        powerTEXT.text = "Power " + stats.Power.ToString("F0");
    }
}
