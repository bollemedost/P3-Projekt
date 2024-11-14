using UnityEngine;

public class PlayerPositionManager : MonoBehaviour
{
    private void Start()
    {
        // Check if position data exists
        if (PlayerPrefs.HasKey("PlayerPosX") && PlayerPrefs.HasKey("PlayerPosY") && PlayerPrefs.HasKey("PlayerPosZ"))
        {
            float x = PlayerPrefs.GetFloat("PlayerPosX");
            float y = PlayerPrefs.GetFloat("PlayerPosY");
            float z = PlayerPrefs.GetFloat("PlayerPosZ");

            // Set player's position to the saved position on the map
            transform.position = new Vector3(x, y, z);
        }
    }
}
