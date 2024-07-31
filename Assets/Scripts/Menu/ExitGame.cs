using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitGame : MonoBehaviour
{
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
