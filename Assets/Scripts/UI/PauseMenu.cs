using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : UIInstance
{
    protected override void OnOpen()
    {
        Time.timeScale = 0f;
    }
    protected override void OnClose()
    {
        Time.timeScale = 1f;
    }
    public void SetSFXVolume(float volume) => JSAM.AudioManager.SoundVolume = volume;
    public void SetMusicVolume(float volume) => JSAM.AudioManager.MusicVolume = volume;
    public void SetVolume(float volume) => JSAM.AudioManager.MasterVolume = volume;
    public void Quit() => Application.Quit();
    public void Restart() => SceneManager.LoadScene(0);
}
