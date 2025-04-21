using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Settings : MonoBehaviour
{
    public bool fullScreen;

    [Range(0f, 1f)]
    public float sfx = 0.5f;
    [Range(0f, 1f)]
    public float music = 0.5f;

    public Options options;
    public AudioManager audioManager;

    public static Settings instance;
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);

        options = FindAnyObjectByType<Options>();
        options.settings = this;
        options.audioManager = audioManager;
        

        UpdateSettingsUI();
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("OnSceneLoaded: " + scene.name);
        Debug.Log(mode);

        options = FindAnyObjectByType<Options>();
        options.settings = this;
        options.audioManager = audioManager;

        UpdateSettingsUI();
    }

    private void OnEnable()
    {
        Debug.Log("here");
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void UpdateSettingsUI()
    {
        options.SetSFX(sfx);
        options.SetMusic(music);
        options.FullScreen(fullScreen);
    }
}
