using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CharacterPriority
{
    public CharacterSO characterSO;
    public int CurrentPriority;
}

public class DialogueSpawner : MonoBehaviour
{
    [SerializeField] private CharacterListSO characterSOList;
    [SerializeField] private DialogueManager dialogueManager;
    [SerializeField] private LegendsManager legendsManager;
    [SerializeField] private List<CharacterPriority> characterPriorityList;

    private void OnEnable()
    {
        DialogueManager.OnDialogueEND += UpdateCharacterPriority;
    }

    private void OnDisable()
    {
        DialogueManager.OnDialogueEND -= UpdateCharacterPriority;

    }

    private void Start()
    {
        CreateCharactersPriority();
    }

    private void CreateCharactersPriority()
    {
        foreach (var character in characterSOList.characterList)
        {
            CharacterPriority newCharacterPriority = new CharacterPriority();
            newCharacterPriority.characterSO = character;
            newCharacterPriority.CurrentPriority = character.DefaultPriority;

            characterPriorityList.Add(newCharacterPriority);
        }

        PreserveDialogues();
    }

    private void UpdateCharacterPriority(object sender, System.EventArgs e)
    {
        foreach (var characterPriority in characterPriorityList)
        {
            characterPriority.CurrentPriority--;

            if (legendsManager.currentLegendsStats.Count <= 0 && dialogueManager.GetDialogueCount() <= 0)
            {
                PreserveDialogues();
            }

            if (characterPriority.CurrentPriority <= 0)
            {
                int randomPriority = Random.Range((characterPriority.characterSO.DefaultPriority / 2) + 1, characterPriority.characterSO.DefaultPriority);
                characterPriority.CurrentPriority = randomPriority;

                if (legendsManager.HaveBookSpace(characterPriority.characterSO))
                    AddCharacterDialogue(characterPriority);
            }
        }
    }

    private void PreserveDialogues()
    {
        for (int i = 0; i < 3; i++)
        {
            characterPriorityList[i].CurrentPriority = 0;
        }
    }

    private void AddCharacterDialogue(CharacterPriority characterPriority)
    {
        bool isRegistred = legendsManager.LegendIsRegistred(characterPriority.characterSO);

        if (isRegistred)
        {
            int randDefaulDialogue = Random.Range(0, characterPriority.characterSO.defaultDialogues.Count);
            dialogueManager.AddDialogue(characterPriority.characterSO.defaultDialogues[randDefaulDialogue]);
        }
        else
        {
            dialogueManager.AddDialogue(characterPriority.characterSO.firstDialogue);
        }
    }

    public void RemoveCharacter(CharacterSO characterToRemove)
    {
        foreach (var character in characterPriorityList)
        {
            if(character.characterSO == characterToRemove)
            {
                characterPriorityList.Remove(character);
                break;
            }
        }
    }
} 
