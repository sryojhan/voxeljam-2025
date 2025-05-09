using UnityEngine;

public class MainMenu : MonoBehaviour
{

    public string playScene;

    public void Play()
    {
        SceneTransition.SceneTransitionManager.instance.ChangeScene(playScene);
        SaveSystem.instance.SetSaveTime();
    }

    public void Options()
    {
        //TODO: options
    }

    public void Credits()
    {
        //TODO: credits
    }

    public void DeleteSaveData()
    {
        SaveSystem.instance.ClearSaveData();
    }

    public void Exit()
    {
        Application.Quit();
    }
}
