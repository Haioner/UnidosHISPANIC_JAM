using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/CharacterLIST")]
public class CharacterListSO : ScriptableObject
{
    public List<CharacterSO> characterList = new List<CharacterSO>();
}
