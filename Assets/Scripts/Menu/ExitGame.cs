using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitGame : MonoBehaviour
{
    [SerializeField] private RectTransform imageTransform;
    [SerializeField] private Vector2 targetPosition;
    [SerializeField] private float rotationAngle;
    [SerializeField] private float targetHeight;
    [SerializeField] private float duration = 0.5f;


    public void MoveRotateResize()
    {
        StartCoroutine(SmoothTransition());
    }

    private IEnumerator SmoothTransition()
    {
        Vector2 initialPosition = imageTransform.anchoredPosition;
        Quaternion initialRotation = imageTransform.rotation;
        Vector2 initialSize = imageTransform.sizeDelta;

        Quaternion targetRotation = Quaternion.Euler(0, 0, rotationAngle);
        Vector2 targetSize = new Vector2(imageTransform.sizeDelta.x, targetHeight);

        float elapsedTime = 0;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;

            imageTransform.anchoredPosition = Vector2.Lerp(initialPosition, targetPosition, t);
            imageTransform.rotation = Quaternion.Lerp(initialRotation, targetRotation, t);
            imageTransform.sizeDelta = Vector2.Lerp(initialSize, targetSize, t);

            yield return null;
        }

        // Ensure the final values are set
        imageTransform.anchoredPosition = targetPosition;
        imageTransform.rotation = targetRotation;
        imageTransform.sizeDelta = targetSize;

        // QUIT
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;

        #else
        Application.Quit();
        
        #endif
    }


#if UNITY_EDITOR
    public void QuitGame()
    {
        UnityEditor.EditorApplication.isPlaying = false;
    }
#else
    public void QuitGame()
    {
        Application.Quit();
    }
#endif
}
