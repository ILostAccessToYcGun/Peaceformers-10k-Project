using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Options : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    //audio manager
    //settings or something

    [SerializeField] Toggle fullscreenToggle;
    [SerializeField] Slider sfx;
    [SerializeField] Slider music;
    [SerializeField] TextMeshProUGUI sfxAmount;
    [SerializeField] TextMeshProUGUI musicAmount;

    [SerializeField] Toggle tutorialToggle;

    public Settings settings;
    public AudioManager audioManager;

    public void Awake()
    {
        audioManager = GameObject.FindAnyObjectByType<AudioManager>();
    }


    public void FullScreen()
    {
        Screen.fullScreen = fullscreenToggle.isOn;
        Debug.Log("Fullscreen: " + fullscreenToggle.isOn);
        settings.fullScreen = fullscreenToggle.isOn;
    }
    
    public void FullScreen(bool toggle)
    {
        fullscreenToggle.isOn = toggle;
        Screen.fullScreen = fullscreenToggle.isOn;
        Debug.Log("Fullscreen: " + fullscreenToggle.isOn);
        settings.fullScreen = fullscreenToggle.isOn;
    }

    public void Tutorial()
    {
        settings.playTutorial = tutorialToggle.isOn;
        Debug.Log("Tutorial: " + settings.playTutorial);
    }

    public void Tutorial(bool toggle)
    {
        fullscreenToggle.isOn = toggle;
        settings.playTutorial = fullscreenToggle.isOn;
        Debug.Log("Tutorial: " + fullscreenToggle.isOn);
    }

    public void SetSFX()
    {
        Debug.Log(sfx.value * 100f);
        sfxAmount.text = (Mathf.FloorToInt(sfx.value * 100f)).ToString();
        settings.sfx = sfx.value;
        audioManager.UpdateVolumes();
    }
    
    public void SetSFX(float value)
    {
        sfx.value = value;
        Debug.Log(sfx.value * 100f);
        sfxAmount.text = (Mathf.FloorToInt(sfx.value * 100f)).ToString();
        settings.sfx = sfx.value;
        audioManager.UpdateVolumes();
    }

    public void SetMusic()
    {
        Debug.Log(music.value * 100f);
        musicAmount.text = (Mathf.FloorToInt(music.value * 100f)).ToString();
        settings.music = music.value;
        audioManager.UpdateVolumes();
    }
    
    public void SetMusic(float value)
    {
        music.value = value;
        Debug.Log(music.value * 100f);
        musicAmount.text = (Mathf.FloorToInt(music.value * 100f)).ToString();
        settings.music = music.value;
        audioManager.UpdateVolumes();
    }

    public void ResetSettings()
    {
        fullscreenToggle.isOn = true;
        FullScreen();
        sfx.value = 0.5f;
        SetSFX();
        music.value = 0.5f;
        SetMusic();
    }
}
