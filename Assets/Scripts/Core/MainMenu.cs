using UnityEditor.Rendering.LookDev;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [Header("Title")]
    [SerializeField] private RectTransform titlePanel;
    [SerializeField] private Vector3 T_shownPos;
    [SerializeField] private Vector3 T_hiddenPos;
    [SerializeField] private bool titleIsShowing = false;

    [Header("Exit Confirmation")]
    [SerializeField] private RectTransform exitConfirmPanel;
    [SerializeField] private Vector3 E_shownPos;
    [SerializeField] private Vector3 E_hiddenPos;
    [SerializeField] private bool exitConfirmIsShowing = false;

    [Header("Options")]
    [SerializeField] private RectTransform optionsPanel;
    [SerializeField] private Vector3 O_shownPos;
    [SerializeField] private Vector3 O_hiddenPos;
    [SerializeField] private bool optionsIsShowing = false;

    [Header("Credits")]
    [SerializeField] private RectTransform creditsPanel;
    [SerializeField] private Vector3 C_shownPos;
    [SerializeField] private Vector3 C_hiddenPos;
    [SerializeField] private bool creditsIsShowing = false;

    [Header("Overwrite Save")]
    [SerializeField] private RectTransform overwritePanel;
    [SerializeField] private Vector3 W_shownPos;
    [SerializeField] private Vector3 W_hiddenPos;
    [SerializeField] private bool overwriteIsShowing = false;

    [Header("Continue")]
    [SerializeField] private Button continueButton;

    [Header("New Game")]
    [SerializeField] private GameObject fade;
    [SerializeField] private Image fadePanel;
    [SerializeField] private Color fadeColour;

    private void Start()
    {
        //if (check for save)
        //continueButton.interactable = false;
        //else
        continueButton.interactable = false;
    }

    public void NewGame()
    {
        //if (check for save)
        //ToggleOverwriteSave();
        //ToggleTitle();
        //else
        fade.SetActive(true);
        LeanTween.value(fade, 0, 1f, 1.5f).setOnUpdate((float val) =>
        {
            Color c = fadePanel.color;
            c.a = val;
            fadePanel.color = c;
        }).setEase(LeanTweenType.easeOutBack);

        Invoke("StartGame", 1.25f);
    }

    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }

    public void Continue()
    {
        //OverWrite();
    }

    public void OverWrite()
    {
        ToggleOverwriteSave();
        ToggleTitle();
    }

    public void Credits()
    {
        ToggleCredits();
        ToggleTitle();
    }

    public void Options()
    {
        ToggleOptions();
        ToggleTitle();
    }

    public void Exit()
    {
        ToggleExitConfirm();
        ToggleTitle();
        
    }

    public void ExitYes()
    {
        Application.Quit();
            Debug.Log("Quit");
    }

    #region _Panel_Toggles_

    public void ToggleTitle()
    {
        if (titleIsShowing)
            LeanTween.move(titlePanel, T_hiddenPos, 0.5f).setEase(LeanTweenType.easeOutCubic);
        else
            LeanTween.move(titlePanel, T_shownPos, 0.5f).setEase(LeanTweenType.easeOutCubic);

        titleIsShowing = !titleIsShowing;
    }

    public void ToggleExitConfirm()
    {
        T_hiddenPos = -E_hiddenPos;
        if (exitConfirmIsShowing)
            LeanTween.move(exitConfirmPanel, E_hiddenPos, 0.5f).setEase(LeanTweenType.easeOutCubic);
        else
            LeanTween.move(exitConfirmPanel, E_shownPos, 0.5f).setEase(LeanTweenType.easeOutCubic);

        exitConfirmIsShowing = !exitConfirmIsShowing;
    }

    public void ToggleOptions()
    {
        T_hiddenPos = -O_hiddenPos;
        if (optionsIsShowing)
            LeanTween.move(optionsPanel, O_hiddenPos, 0.5f).setEase(LeanTweenType.easeOutCubic);
        else
            LeanTween.move(optionsPanel, O_shownPos, 0.5f).setEase(LeanTweenType.easeOutCubic);

        optionsIsShowing = !optionsIsShowing;
    }

    public void ToggleCredits()
    {
        T_hiddenPos = -C_hiddenPos;
        if (creditsIsShowing)
            LeanTween.move(creditsPanel, C_hiddenPos, 0.5f).setEase(LeanTweenType.easeOutCubic);
        else
            LeanTween.move(creditsPanel, C_shownPos, 0.5f).setEase(LeanTweenType.easeOutCubic);

        creditsIsShowing = !creditsIsShowing;
    }

    public void ToggleOverwriteSave()
    {
        T_hiddenPos = -W_hiddenPos;
        if (overwriteIsShowing)
            LeanTween.move(overwritePanel, W_hiddenPos, 0.5f).setEase(LeanTweenType.easeOutCubic);
        else
            LeanTween.move(overwritePanel, W_shownPos, 0.5f).setEase(LeanTweenType.easeOutCubic);

        overwriteIsShowing = !overwriteIsShowing;
    }

    #endregion
}
