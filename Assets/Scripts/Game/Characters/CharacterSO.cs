using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Character")]
public class CharacterSO : ScriptableObject
{
    //public Sprite characterIcon;
    public Sprite characterSprite;
    public Color nameColor;

    [Header("Legend Stats")]
    public LegendStats DefaultStats;

    [Header("Dialogues")]
    public int DefaultPriority;
    public bool nomadic = false; //dont registry
    public DialogueSO firstDialogue;
    public List<DialogueSO> defaultDialogues = new List<DialogueSO>(); 
}
