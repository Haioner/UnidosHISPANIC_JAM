using UnityEngine;
using UnityEngine.UI;

public class StatsCanvas : MonoBehaviour
{
    [SerializeField] private Slider popularitySlider;
    [SerializeField] private Slider powerSlider;
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
        popularitySlider.value = stats.Popularity;
        powerSlider.value = stats.Power;
    }
}
