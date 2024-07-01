using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    public void Play(){
        print("load scene 1");
        SceneManager.LoadSceneAsync(1);
        if(Time.timeScale == 0){
            print("timeScale = 0 -> set timescale = 1");
            Time.timeScale = 1;
        }
    }

    public void Quit(){
        print("quit");
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
