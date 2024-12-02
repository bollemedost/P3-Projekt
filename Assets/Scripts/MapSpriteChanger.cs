using UnityEngine;
using UnityEngine.UI;  // Add the UI namespace to use Image

public class MapSpriteChanger : MonoBehaviour
{
    [Header("Sprites")]
    public Sprite mapSpriteAfter1Level; // Sprite for after 1st level completed
    public Sprite mapSpriteAfter2Levels; // Sprite for after 2nd level completed
    public Sprite mapSpriteAfter3Levels; // Sprite for after 3rd level completed
    public Sprite mapSpriteAfter4Levels; // Sprite for after 4th level completed

    private Image mapImage;  // Use Image instead of SpriteRenderer

    private void Start()
    {
        // Get the Image component attached to this object (ensure the object has an Image component)
        mapImage = GetComponent<Image>();

        // Update the sprite based on the completed levels
        UpdateMapSprite();
    }

    private void UpdateMapSprite()
    {
        int completedLevels = 0;

        // Check PlayerPrefs to determine which levels have been completed
        if (PlayerPrefs.GetInt("Level2Completed", 0) == 2) completedLevels++;
        if (PlayerPrefs.GetInt("Level3Completed", 0) == 2) completedLevels++;
        if (PlayerPrefs.GetInt("Level4Completed", 0) == 2) completedLevels++;
        if (PlayerPrefs.GetInt("Level5Completed", 0) == 2) completedLevels++;

        // Change the map sprite based on the number of completed levels
        switch (completedLevels)
        {
            case 1:
                mapImage.sprite = mapSpriteAfter1Level;
                break;
            case 2:
                mapImage.sprite = mapSpriteAfter2Levels;
                break;
            case 3:
                mapImage.sprite = mapSpriteAfter3Levels;
                break;
            case 4:
                mapImage.sprite = mapSpriteAfter4Levels;
                break;
            default:
                // Optionally, set a default sprite or leave it unchanged if no levels are completed
                break;
        }
    }
}
           
