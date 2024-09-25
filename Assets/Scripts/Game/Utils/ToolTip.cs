using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Localization;

public class ToolTip : MonoBehaviour,IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private LocalizedString tooltipString;
    private ToolTipManager toolTipManager;
    private DialogueManager dialogueManager;
    private string toolTipText;

    public void UpdateToolTip()
    {
        tooltipString.StringChanged += (localizedText) =>
        {
            toolTipText = localizedText;
            toolTipManager.SetToolTip(toolTipText);
        };
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        UpdateToolTip();
        dialogueManager.canDialogue = false;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        toolTipManager.DeactiveToolTip();
        dialogueManager.canDialogue = true;
    }

    private void Awake()
    {
        toolTipManager = FindFirstObjectByType<ToolTipManager>();
        dialogueManager = FindFirstObjectByType<DialogueManager>();
    }

}
