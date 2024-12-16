using UnityEngine;
using UnityEngine.SceneManagement;

public class Control1 : MonoBehaviour
{
    public void NextScene()
    {
        SceneManager.LoadScene("Gra");
    }
}