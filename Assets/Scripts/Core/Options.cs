using TMPro;
using UnityEngine;
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

    public void FullScreen()
    {
        Screen.fullScreen = fullscreenToggle.isOn;
        Debug.Log("Fullscreen: " + fullscreenToggle.isOn);
    }

    public void SetSFX()
    {
        Debug.Log(sfx.value * 100f);
        sfxAmount.text = (Mathf.FloorToInt(sfx.value * 100f)).ToString();
    }

    public void SetMusic()
    {
        Debug.Log(music.value * 100f);
        musicAmount.text = (Mathf.FloorToInt(music.value * 100f)).ToString();
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
