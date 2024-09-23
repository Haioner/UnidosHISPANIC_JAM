using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    [Header("CACHE")]
    [SerializeField] private GameObject dialogueHolder;
    [SerializeField] private GameObject inputHolder;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private TextMeshProUGUI characterNameText;
    [SerializeField] private LegendsManager legendsManager;

    [Header("Dialogue")]
    [SerializeField] private float typeSpeed = 0.05f;
    [SerializeField] private int currentDialogueIndex;

    [Space]
    [SerializeField] private List<DialogueSO> dialogueList = new List<DialogueSO>();
    [SerializeField] private List<string> currentDialogue = new List<string>();

    public static event System.EventHandler OnDialogueEND;

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

                if (!inDialogue)
                {
                    inDialogue = true;
                    SetCanDialoguePass(!dialogueList[0].needAwnser);
                }
            }
        }
        else if (waitingForChoice)
        {
            if (!canDialoguePass)
                inputHolder.SetActive(true);

            if (Input.GetKeyDown(KeyCode.Y))
            {
                ApplyConsequence(true);
                inputHolder.SetActive(false);
            }
            
            if (Input.GetKeyDown(KeyCode.N))
            {
                ApplyConsequence(false);
                inputHolder.SetActive(false);
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

    public void NextDialogue()
    {
        if (dialogueList.Count <= 0) return;

        if (dialogueList.Count > 0 && currentDialogue.Count <= 0)
            currentDialogue.AddRange(dialogueList[0].dialogue);


        if (isTyping)
        {
            CompleteTyping();
        }
        else
        {
            characterNameText.text = dialogueList[0].characterOwner.name;
            characterNameText.color = dialogueList[0].characterOwner.nameColor;

            dialogueText.text = "";
            typingCoroutine = StartCoroutine(TypeDialogue(GetCurrentDialogue()));
        }
    }

    private void SetCanDialoguePass(bool state)
    {
        canDialoguePass = state;
    }

    private void ApplyConsequence(bool isYes)
    {
        AwnserConsequence consequence = isYes ? dialogueList[0].YES_AwnserConsequences : dialogueList[0].NO_AwnserConsequences;
        //Stats
        foreach (var stats in consequence.ConsequenceStats)
            legendsManager.ChangeLegendStats(stats);

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
        DialogueDefaultSouls(!canDialoguePass);
        legendsManager.SetSouls(consequence.SoulsAmount);

        onGetStats?.Invoke(legendsManager.GetLegendStat(dialogueList[0].characterOwner));
        legendsManager.KillLegend(dialogueList[0].characterOwner);
        RemoveDeadDialogues(dialogueList[0].characterOwner);

        if (consequence.DialogueAfterAwnser.Count <= 0)
            EndDialogue();

        waitingForChoice = false;
    }

    private void DialogueDefaultSouls(bool canChangeSouls)
    {
        if (canChangeSouls)
        {
            legendsManager.SetSouls(dialogueList[0].DefaultSouls);
        }
    }

    private void EndDialogue()
    {
        currentDialogue.Clear();
        DialogueDefaultSouls(canDialoguePass);

        if(dialogueList.Count > 0)
        {
            dialogueList.RemoveAt(0);
        }
        
        currentDialogueIndex = 0;
        dialogueText.text = "";
        characterNameText.text = "";
        waitingForChoice = false;
        SetCanDialoguePass(false);
        dialogueHolder.SetActive(false);
        inputHolder.SetActive(false);
        inDialogue = false;
        OnDialogueEND?.Invoke(this, System.EventArgs.Empty);
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

        dialogueText.text = GetCurrentDialogue();
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

    private string GetCurrentDialogue()
    {
        return currentDialogue[currentDialogueIndex];
    }

    private bool IsLastDialogueLine()
    {
        return currentDialogueIndex >= currentDialogue.Count;
    }
}
