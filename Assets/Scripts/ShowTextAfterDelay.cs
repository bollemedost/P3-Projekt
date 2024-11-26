using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShowTextsAfterDelay : MonoBehaviour
{
    public GameObject[] buttons; 
    public GameObject text; 
    public float delay = 9f; 
    public float fadeDuration = 3f; 

    void Start()
    {
        // Initialize buttons and text to be fully transparent and inactive
        foreach (GameObject button in buttons)
        {
            SetAlpha(button, 0f); 
            button.SetActive(false); 
        }

        if (text != null)
        {
            SetAlpha(text, 0f); 
            text.SetActive(false); 
        }

        // Start the coroutine to fade in the buttons and text after the delay
        StartCoroutine(FadeInElementsAfterDelay());
    }

    IEnumerator FadeInElementsAfterDelay()
    {
        // Wait for the specified delay before starting fade-in
        yield return new WaitForSeconds(delay);

        // Fade in each button in the array
        foreach (GameObject button in buttons)
        {
            button.SetActive(true); // Activate the button
            StartCoroutine(FadeInObject(button)); // Start fading in the button
        }

        // Fade in the standalone text
        if (text != null)
        {
            text.SetActive(true); // Activate the text
            StartCoroutine(FadeInObject(text)); // Start fading in the text
        }
    }

    IEnumerator FadeInObject(GameObject obj)
    {
        float elapsedTime = 0f; // Track time elapsed since fade started

        // Get the Image component for buttons and all Text components for fading
        var uiImage = obj.GetComponent<Image>();
        var texts = obj.GetComponentsInChildren<TMP_Text>();

        // Gradually increase alpha value from 0 to 1 over the fade duration
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime; // Increment elapsed time
            float alpha = Mathf.Clamp01(elapsedTime / fadeDuration); // Calculate alpha as a proportion of time elapsed

            // Apply new alpha to the Image component (if present)
            if (uiImage != null)
            {
                Color color = uiImage.color;
                color.a = alpha; // Update the alpha value
                uiImage.color = color;
            }

            // Apply new alpha to all Text components (if present)
            foreach (var text in texts)
            {
                Color color = text.color;
                color.a = alpha; // Update the alpha value
                text.color = color;
            }

            yield return null; // Wait for the next frame
        }

        // Ensure the alpha is fully set to 1 at the end of the fade
        if (uiImage != null)
        {
            Color color = uiImage.color;
            color.a = 1f; // Set alpha to 1 (fully visible)
            uiImage.color = color;
        }

        foreach (var text in texts)
        {
            Color color = text.color;
            color.a = 1f; // Set alpha to 1 (fully visible)
            text.color = color;
        }
    }

    // Helper method to set the alpha of an object and its children
    void SetAlpha(GameObject obj, float alpha)
    {
        // Get the Image component for buttons
        var uiImage = obj.GetComponent<Image>();
        // Get all Text components for buttons or standalone text
        var texts = obj.GetComponentsInChildren<TMP_Text>();

        // Set the alpha for the Image component (if present)
        if (uiImage != null)
        {
            Color color = uiImage.color;
            color.a = alpha; // Set the alpha value
            uiImage.color = color;
        }

        // Set the alpha for all Text components (if present)
        foreach (var text in texts)
        {
            Color color = text.color;
            color.a = alpha; // Set the alpha value
            text.color = color;
        }
    }
}
