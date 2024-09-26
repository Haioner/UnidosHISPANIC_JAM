using UnityEngine;

public class NPCManager : MonoBehaviour
{
    [SerializeField] private SpriteRenderer gfxRenderer;
    [SerializeField] private DialogueManager dialogueManager;
    [SerializeField] private LegendsManager legendsManager;
    [SerializeField] private StatsCanvas statsCanvas;
    [SerializeField] private ParticleSystem deathParticle;
    [SerializeField] private GameObject[] deathDeactiveObjects;

    private void OnEnable()
    {
        MovementController.OnMoveEnter += UpdateCharacter;
        legendsManager.onLegendDies += CharacterDies;
    }

    private void OnDisable()
    {
        MovementController.OnMoveEnter -= UpdateCharacter;
    }

    private void CharacterDies(int legendCount)
    {
        foreach (var item in deathDeactiveObjects)
        {
            item.SetActive(false);
        }
        Instantiate(deathParticle, transform.position, Quaternion.identity);
        gfxRenderer.sprite = null;
    }

    private void UpdateCharacter(object sender, System.EventArgs e)
    {
        if (dialogueManager.GetDialogueCount() <= 0) return;
        UpdateCharacterGFX();
        UpdateStats();
    }

    private void UpdateCharacterGFX()
    {
        foreach (var item in deathDeactiveObjects)
        {
            item.SetActive(true);
        }
        gfxRenderer.sprite = dialogueManager.GetDialogueCharacter().characterSprite;
    }

    private void UpdateStats()
    {
        statsCanvas.UpdateStatsCanvas(legendsManager.GetLegendStat(dialogueManager.GetDialogueCharacter()));
    }
}
