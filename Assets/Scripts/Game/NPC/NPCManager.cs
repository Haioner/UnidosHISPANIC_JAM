using UnityEngine;

public class NPCManager : MonoBehaviour
{
    [SerializeField] private SpriteRenderer gfxRenderer;
    [SerializeField] private DialogueManager dialogueManager;
    [SerializeField] private LegendsManager legendsManager;
    [SerializeField] private StatsCanvas statsCanvas;

    private void OnEnable()
    {
        MovementController.OnMoveEnter += UpdateCharacter;
    }

    private void OnDisable()
    {
        MovementController.OnMoveEnter -= UpdateCharacter;
    }

    private void UpdateCharacter(object sender, System.EventArgs e)
    {
        if (dialogueManager.GetDialogueCount() <= 0) return;
        UpdateCharacterGFX();
        UpdateStats();
    }

    private void UpdateCharacterGFX()
    {
        gfxRenderer.sprite = dialogueManager.GetDialogueCharacter().characterSprite;
    }

    private void UpdateStats()
    {
        statsCanvas.UpdateStatsCanvas(legendsManager.GetLegendStat(dialogueManager.GetDialogueCharacter()));
    }
}
