using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    private IEnumerator WaitForSceneLoad(string sceneName)
    {
        yield return new WaitForSeconds(2);
        SceneManager.LoadScene(sceneName);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            StartCoroutine(WaitForSceneLoad("WinScreen"));
        }
        else if (other.gameObject.CompareTag("AI"))
        {
            StartCoroutine(WaitForSceneLoad("LoseScreen"));
        }
    }
}
