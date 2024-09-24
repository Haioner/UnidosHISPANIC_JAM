using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class DialogueManager : MonoBehaviour
{
    [Header("CACHE")]
    [SerializeField] private GameObject dialogueHolder;
    [SerializeField] private GameObject inputHolder;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private TextMeshProUGUI priceText;
    [SerializeField] private TextMeshProUGUI characterNameText;
    [SerializeField] private LegendsManager legendsManager;
    [SerializeField] private Transform npcTransform;

    [Header("DOT")]
    [SerializeField] private DOTweenAnimation yesDOT;
    [SerializeField] private DOTweenAnimation noDOT;

    [Header("Dialogue")]
    [SerializeField] private float typeSpeed = 0.05f;
    [SerializeField] private int currentDialogueIndex;

    [Space]
    [SerializeField] private List<DialogueSO> dialogueList = new List<DialogueSO>();
    [SerializeField] private List<string> currentDialogue = new List<string>();
    private DialogueSO lastDialogue;

    public static event System.EventHandler OnDialogueEND;
    public static event System.EventHandler OnDialogueStart;

    public delegate void OnGetStats(LegendStats legendStats);
    public event OnGetStats onGetStats;

    private bool inDialogue = false;
    private bool isTyping = false;
    private bool waitingForChoice = false;
    private bool canDialoguePass = false;
    private Coroutine typingCoroutine;

    private void Update()
    {
        if (dialogueList.Count <= 0) return;
        StartDefaultDialogues();

        if (IsLastDialogueLine() && canDialoguePass && Input.anyKeyDown)
        {
            EndDialogue();
        }

        if (!IsLastDialogueLine())
        {
            if (Input.anyKeyDown)
            {
                NextDialogue();
            }
        }
        else if (waitingForChoice)
        {
            if (!canDialoguePass)
                inputHolder.SetActive(true);

            if (Input.GetKeyDown(KeyCode.Y))
            {
                ApplyConsequence(true);
                yesDOT.DORestart();
            }
            
            if (Input.GetKeyDown(KeyCode.N))
            {
                ApplyConsequence(false);
                noDOT.DORestart();
            }
        }
    }

    public CharacterSO GetDialogueCharacter()
    {
        return dialogueList[0].characterOwner;
    }

    public int GetDialogueCount()
    {
        return dialogueList.Count;
    }

    private void StartDefaultDialogues()
    {
        if(dialogueList.Count <= 1 && legendsManager.currentLegendsStats.Count > 0)
        {
            int randLegend = Random.Range(0, legendsManager.currentLegendsStats.Count);
            int randDialogue = Random.Range(0, legendsManager.currentLegendsStats[randLegend].Character.defaultDialogues.Count);
            AddDialogue(legendsManager.currentLegendsStats[randLegend].Character.defaultDialogues[randDialogue]);
        }
    }

    public void AddDialogue(DialogueSO newDialogue)
    {
        dialogueList.Add(newDialogue);
    }

    public void RemoveRegistredFirstDialogue(DialogueSO newDialogue)
    {
        if (dialogueList.Count <= 0) return;
        bool isRegistred = legendsManager.LegendIsRegistred(newDialogue.characterOwner);
        if (!isRegistred) return;

        for (int i = dialogueList.Count - 1; i >= 0; i--)
        {
            if (dialogueList[i] == newDialogue)
            {
                dialogueList.RemoveAt(i);
            }
        }
    }

    private void RemoveDeadDialogues(CharacterSO deadCharacter)
    {
        if (dialogueList.Count <= 0) return;
        if (legendsManager.LegendIsDead(dialogueList[0].characterOwner))
        {
            for (int i = dialogueList.Count - 1; i >= 0; i--)
            {
                if (dialogueList[i].characterOwner == deadCharacter)
                {
                    dialogueList.RemoveAt(i);
                }
            }
        }
    }

    private float GetSoulPrice(float soulAmount)
    {
        if (legendsManager.GetSoulsAmount() > Mathf.Abs(soulAmount) * 2 && soulAmount != 0)
            return soulAmount + (-legendsManager.GetSoulsAmount() * 0.6f);
        else
            return soulAmount;
    }

    private void SetPriceText()
    {
        float priceAmount = Mathf.Abs(GetSoulPrice(dialogueList[0].YES_AwnserConsequences.SoulsAmount));
        if (priceAmount > 0)
        {
            priceText.text = "<sprite=1> " + NumberConverter.ConvertNumberToString(priceAmount);
        }
        else
        {
            priceText.text = "";
        }
    }

    public void NextDialogue()
    {
        if (dialogueList.Count <= 0) return;

        SetPriceText();

        if (dialogueList.Count > 0 && currentDialogue.Count <= 0)
            currentDialogue.AddRange(dialogueList[0].dialogue);

        if (!inDialogue)
        {
            if (dialogueList[0].isTaxes)
                legendsManager.CalculateLegendDeaths(GetDialogueCharacter());

            OnDialogueStart?.Invoke(this, System.EventArgs.Empty);
            inDialogue = true;
            SetCanDialoguePass(!dialogueList[0].needAwnser);
        }

        if (isTyping)
        {
            CompleteTyping();
        }
        else
        {
            characterNameText.text = dialogueList[0].characterOwner.name;
            characterNameText.color = dialogueList[0].characterOwner.nameColor;

            dialogueText.text = "";
            typingCoroutine = StartCoroutine(TypeDialogue(GetCurrentDialogueLine()));
        }
    }

    private void SetCanDialoguePass(bool state)
    {
        canDialoguePass = state;
    }

    private bool EnoughSouls(AwnserConsequence consequence)
    {
        float totalSoulRequired = Mathf.Abs(GetSoulPrice(dialogueList[0].YES_AwnserConsequences.SoulsAmount));
        bool hasEnough = legendsManager.GetSoulsAmount() >= totalSoulRequired;

        if (!hasEnough)
        {
            SetCanDialoguePass(true);
            currentDialogue.Clear();
            currentDialogueIndex = 0;
            currentDialogue.Add("Not enough souls");
            NextDialogue();

            if (legendsManager.LegendIsRegistred(dialogueList[0].characterOwner))
            {
                LegendStats currentLegendStats = new LegendStats();
                currentLegendStats.Character = dialogueList[0].characterOwner;
                currentLegendStats.Power = -1;
                legendsManager.ChangeLegendStats(currentLegendStats);
                PowerFloatNumber(currentLegendStats);

                onGetStats?.Invoke(legendsManager.GetLegendStat(dialogueList[0].characterOwner));
                legendsManager.KillLegend(dialogueList[0].characterOwner);
                RemoveDeadDialogues(dialogueList[0].characterOwner);
            }
        }

        return hasEnough;
    }

    private void ApplyConsequence(bool isYes)
    {
        AwnserConsequence consequence = isYes ? dialogueList[0].YES_AwnserConsequences : dialogueList[0].NO_AwnserConsequences;

        if (!EnoughSouls(consequence)) return;
        
        //Stats
        foreach (var stats in consequence.ConsequenceStats)
        {
            legendsManager.ChangeLegendStats(stats);
            PowerFloatNumber(stats);
        }

        //Add Dialogues ballons
        if (consequence.ConsequenceDialogues.Count > 0)
        {
            foreach (var dialogue in consequence.ConsequenceDialogues)
                AddDialogue(dialogue);
        }
        
        //Dialogue after awnser
        if (consequence.DialogueAfterAwnser.Count > 0)
        {
            SetCanDialoguePass(true);
            currentDialogue.Clear();
            currentDialogueIndex = 0;
            currentDialogue.AddRange(consequence.DialogueAfterAwnser);
            NextDialogue();
        }

        //Instantiate Effects
        if (consequence.ConsequenceEffector.Count > 0)
        {
            foreach (var effector in consequence.ConsequenceEffector)
            {
                Instantiate(effector);
            }
        }

        //Souls
        DialogueFREESouls(!canDialoguePass);

        float soulsAmount = consequence.SoulsAmount;

        if (isYes)
            soulsAmount = GetSoulPrice(consequence.SoulsAmount);

        string additionalString = "";
        if (soulsAmount < 0) additionalString = "<color=red> -";

        if (soulsAmount != 0)
            FindFirstObjectByType<FloatNumberManager>().
                SpawnGainFloat("<sprite=1>" + additionalString + NumberConverter.ConvertNumberToString(Mathf.Abs(GetSoulPrice(consequence.SoulsAmount))));
        legendsManager.SetSouls(soulsAmount);

        //Update
        onGetStats?.Invoke(legendsManager.GetLegendStat(dialogueList[0].characterOwner));
        legendsManager.KillLegend(dialogueList[0].characterOwner);
        RemoveDeadDialogues(dialogueList[0].characterOwner);

        if (consequence.DialogueAfterAwnser.Count <= 0)
            EndDialogue();

        waitingForChoice = false;
    }

    private void PowerFloatNumber(LegendStats legendStats)
    {
        string additionalString = "";
        if (legendStats.Power > 0) additionalString = "+";
        else additionalString = "<color=red>";

        if (legendStats.Power != 0)
            FindFirstObjectByType<FloatNumberManager>().SpawnFloatNumber(additionalString + legendStats.Power.ToString(), npcTransform.position);
    }

    private void DialogueFREESouls(bool canChangeSouls)
    {
        if (canChangeSouls)
        {
            legendsManager.SetSouls(dialogueList[0].FreeSouls_END);
            if (dialogueList[0].FreeSouls_END != 0)
                FindFirstObjectByType<FloatNumberManager>().SpawnGainFloat("<sprite=1>" + NumberConverter.ConvertNumberToString(dialogueList[0].FreeSouls_END));
        }
    }

    private void EndDialogue()
    {
        legendsManager.UpdateWorldPopulation();

        currentDialogue.Clear();
        DialogueFREESouls(canDialoguePass);

        if(dialogueList.Count > 0)
        {
            lastDialogue = dialogueList[0];
            dialogueList.RemoveAt(0);
        }

        currentDialogueIndex = 0;
        dialogueText.text = "";
        characterNameText.text = "";
        waitingForChoice = false;
        SetCanDialoguePass(false);
        dialogueHolder.SetActive(false);
        inDialogue = false;
        OnDialogueEND?.Invoke(this, System.EventArgs.Empty);

        if (lastDialogue != null)
        {
            RemoveRegistredFirstDialogue(lastDialogue.characterOwner.firstDialogue);
        }
    }

    private IEnumerator TypeDialogue(string dialogue)
    {
        dialogueHolder.SetActive(true);
        onGetStats?.Invoke(legendsManager.GetLegendStat(dialogueList[0].characterOwner));

        isTyping = true;
        foreach (char letter in dialogue.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typeSpeed);
        }

        isTyping = false;
        currentDialogueIndex++;
        EnableChoice();
    }

    private void CompleteTyping()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        dialogueText.text = GetCurrentDialogueLine();
        isTyping = false;
        currentDialogueIndex++;
        EnableChoice();
    }

    private void EnableChoice()
    {
        if (IsLastDialogueLine() && !waitingForChoice)
        {
            waitingForChoice = true;
        }
    }

    private string GetCurrentDialogueLine()
    {
        return currentDialogue[currentDialogueIndex];
    }

    private bool IsLastDialogueLine()
    {
        return currentDialogueIndex >= currentDialogue.Count;
    }
}
