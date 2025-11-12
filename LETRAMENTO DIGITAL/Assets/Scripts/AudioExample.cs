using UnityEngine;

public class AudioExample : MonoBehaviour
{
    public void OnButtonClick()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayButtonClickSound();
        }
    }

    public void OnNFCRead()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayNFCReadSound();
        }
    }

    public void OnCorrectAnswer()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayCorrectSound();
        }
    }

    public void OnWrongAnswer()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayWrongSound();
        }
    }

    public void OnGameEnd()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayGameEndSound();
        }
    }

    public void StartBackgroundMusic()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayBackgroundMusic();
        }
    }

    public void StopBackgroundMusic()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.StopBackgroundMusic();
        }
    }
}
