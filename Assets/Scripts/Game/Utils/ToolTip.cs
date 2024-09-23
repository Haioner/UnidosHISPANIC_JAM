using UnityEngine;
using UnityEngine.EventSystems;

public class ToolTip : MonoBehaviour,IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private string toolTipText;
    private ToolTipManager toolTipManager;

    public void OnPointerEnter(PointerEventData eventData)
    {
        toolTipManager.SetToolTip(toolTipText);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        toolTipManager.DeactiveToolTip();
    }

    private void Awake()
    {
        toolTipManager = FindFirstObjectByType<ToolTipManager>();
    }
}
