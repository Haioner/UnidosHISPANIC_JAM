using System.Collections.Generic;
using UnityEngine;

public class PictureManager : MonoBehaviour
{
    [SerializeField] private PictureItem pictureItem;

    [SerializeField] private List<CharacterSO> currentCharacterList = new List<CharacterSO>();
    [SerializeField] private List<PictureItem> currentPicturesList = new List<PictureItem>();

    private float offsetX = 0.7f;
    private float offsetY = 0.9f;
    private int itemsPerRow = 8;
    private int totalSpawned = 0;

    public void SpawnPicture(CharacterSO character)
    {
        int row = totalSpawned / itemsPerRow;
        int column = totalSpawned % itemsPerRow;

        float posX = column * offsetX;
        float posY = row * offsetY;

        PictureItem currentItem = Instantiate(pictureItem, new Vector3(posX, posY, 0) + transform.position, Quaternion.identity, transform);
        currentItem.SetPictureItem(character.characterSprite);
        currentPicturesList.Add(currentItem);

        currentCharacterList.Add(character);
        totalSpawned++;
    }

    public void RemovePicture(CharacterSO character)
    {
        int index = currentCharacterList.IndexOf(character);

        if (index != -1)
        {
            // Remove o PictureItem e o CharacterSO
            PictureItem pictureToRemove = currentPicturesList[index];
            currentPicturesList.RemoveAt(index);
            Destroy(pictureToRemove.gameObject);

            currentCharacterList.RemoveAt(index);
            totalSpawned--;

            // Reorganiza os itens restantes
            RearrangePictures();
        }
    }

    private void RearrangePictures()
    {
        for (int i = 0; i < currentPicturesList.Count; i++)
        {
            int row = i / itemsPerRow;
            int column = i % itemsPerRow;

            float posX = column * offsetX;
            float posY = row * offsetY;

            // Move a PictureItem para a nova posição
            currentPicturesList[i].transform.localPosition = new Vector3(posX, posY, 0);
        }
    }
}
