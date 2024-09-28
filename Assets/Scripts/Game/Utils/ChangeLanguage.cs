using UnityEngine.Localization.Settings;
using UnityEngine.EventSystems;
using UnityEngine.Localization;
using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class ChangeLanguage : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private LocalizedString tooltipLocale;
    [SerializeField] private TextMeshProUGUI languageTEXT;
    [SerializeField] private DialogueManager dialogueManager;
    private ToolTipManager toolTipManager;
    private string languageName;

    private void Awake()
    {
        toolTipManager = FindFirstObjectByType<ToolTipManager>();
    }

    private void Start()
    {
        languageTEXT.text = LocalizationSettings.SelectedLocale.Identifier.Code;
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

        languageName = LocalizationSettings.SelectedLocale.Identifier.Code;
        languageTEXT.text = languageName;

        UpdateToolTip();
        dialogueManager.UpdatePageText();
        dialogueManager.UpdateLocalizedDialogue();
    }

    public void UpdateToolTip()
    {
        tooltipLocale.StringChanged += (localizedText) =>
        {
            languageName = localizedText;
            toolTipManager.SetToolTip(languageName);
        };
    }
}
