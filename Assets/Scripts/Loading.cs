using UnityEngine;
using UnityEngine.SceneManagement;

public class Loading : MonoBehaviour
{
    public void LoadLevel()
    {
        string name = "Level" + PlayerData.startedLevel;
        SceneManager.LoadScene(name);
    }
}
