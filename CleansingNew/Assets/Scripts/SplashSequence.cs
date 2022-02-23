using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SplashSequence : MonoBehaviour
{
    public static int SceneNumber;
    void Start()
    {
        if (SceneNumber == 0)
        {
            StartCoroutine(ToMainMenu());
        }
    }

    IEnumerator ToMainMenu()
    {
        yield return new WaitForSeconds(1);
        SceneNumber = 1;
        SceneManager.LoadScene(1);
    }

}
