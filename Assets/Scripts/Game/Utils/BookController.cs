using System.Collections.Generic;
using UnityEngine;

public class BookController : MonoBehaviour
{
    [SerializeField] private List<Sprite> bookSprites = new List<Sprite>();
    [SerializeField] private SpriteRenderer spriteRenderer;
    private int currentBook = -1;

    public void AddBook()
    {
        if (currentBook < bookSprites.Count - 1)
        {
            currentBook++;
            spriteRenderer.sprite = bookSprites[currentBook];
        }
    }
}
