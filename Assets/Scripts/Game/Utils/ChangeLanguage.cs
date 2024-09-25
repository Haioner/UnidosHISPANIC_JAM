using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class ChangeLanguage : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private LocalizedString tooltipLocale;
    [SerializeField] private TextMeshProUGUI languageTEXT;
    [SerializeField] private DialogueManager dialogueManager;
    private ToolTipManager toolTipManager;

    private void Awake()
    {
        toolTipManager = FindFirstObjectByType<ToolTipManager>();

        string languageName = LocalizationSettings.SelectedLocale.Identifier.Code;
        languageTEXT.text = languageName;
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

    public void ChangeLanguageBUTTON()
    {
        StartCoroutine(ChangeLanguageCoroutine());
    }

    private IEnumerator ChangeLanguageCoroutine()
    {
        GetComponent<Image>().raycastTarget = false;
        GetComponentInChildren<TextMeshProUGUI>().raycastTarget = false;

        int currentLocaleIndex = LocalizationSettings.AvailableLocales.Locales.IndexOf(LocalizationSettings.SelectedLocale);
        int nextLocaleIndex = (currentLocaleIndex + 1) % LocalizationSettings.AvailableLocales.Locales.Count;
        yield return LocalizationSettings.InitializationOperation;
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[nextLocaleIndex];

        GetComponent<Image>().raycastTarget = true;
        GetComponentInChildren<TextMeshProUGUI>().raycastTarget = true;

        string languageName = LocalizationSettings.SelectedLocale.Identifier.Code;
        languageTEXT.text = languageName;

        UpdateToolTip();
        dialogueManager.UpdatePageText();
        dialogueManager.UpdateLocalizedDialogue();
    }

    private void UpdateToolTip()
    {
        toolTipManager.SetToolTip(tooltipLocale.GetLocalizedString());
    }
}
