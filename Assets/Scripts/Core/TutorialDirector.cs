using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialDirector : MonoBehaviour
{

    // basically whats going to happen is that certain parts of the screen will be highlighted to teach the player a certain feature
    //or more specifically, I will darken parts of the screen so parts become the focus
    //this means that I will need several functions that other objects can call the first time something appears aka a UI screen

    //i will need one function that basically:
    /* lerps time to stop
     * moves a fade ui panel to a position, where the center is basically un affected, but the outsides are darkened
     * fades it in with a text box and button.
     * when the button is pressed, fade out the fade panel and lerp time to 1.
     * 
     * We will also want to make overloads for this method so we can manually change parameters of the highlight
     * as well as another overload for the 1st tutorial fade, to add a button to skip all tutorial pop-ups
     

    I guess since I also want the player to walk around during the tutorial,
    we will need a system that disables pretty much all the starting functionality and only starts it after the tutorial is done,
     so maybe we do an if check and if the player unchecked the tutorial,
    the tutorial will activate everything on start-up, else after they finish the tutorial. 
    This includes warping the player back to the start and probs like a fade to black
    That basically means that I need to script things, like enemies and settlement quests
    which sounds like a pain so its probably easier just to make a tutorial scene
    //but oh well


        *I think the tutorial will be like this
        *1 opening screen saying welcome to this game
        *"You are the Mech guardian of this land and your job is to form peace between the 3 settlements, Draupnir, Midgard, and Rubicon"
            *show settlement icons
        *Show Basic controls > movement, shooting, interacting
        *"Make sure their upkeep stays nice and high!" + show settlement (T)
        *"this is your health, dont let it reach zero!"
        *"this is your boost meter, you need boost to perform some actions" + show controls for jump, dash and sprint
        *"this is your energy meter, performing some actions will drain your battery"
        *"if your health or energy reach 0, the day will end prematurely so be careful!"
        *"To keep the settlements happy, you will need to complete their quests."
        *"Press _ to open the Quest log!"
        *"Quests require resources from the land to be completed." + show images of the nodes
        *"Press TAB to open your inventory"
        *"Items and ammo take up space, Organization is key!"
        *"Completing Quests refills a settlement's upkeep, so complete them as best as you can!"
        *"The harsh winter here lasts 14 days, Good Luck!"
     
     */
    [Space]
    [Header("Tutorial")]
    [SerializeField] RectTransform highlightFrame;
    [SerializeField] float moveTime;
    [SerializeField] GameObject tempTarget;
    [SerializeField] int tutoIndex = 0;

    [SerializeField] Vector3 onPos;
    [SerializeField] Vector3 offPos;
    [Space]
    [Header("Screens")]
    [SerializeField] RectTransform intro;
    [SerializeField] RectTransform controls;

    [Space]
    [Header("Controls")]
    #region _Keys_

    [SerializeField] Image wKey;
    [SerializeField] TextMeshProUGUI wText;
    [SerializeField] bool wBool;
    [Space]
    [SerializeField] Image aKey;
    [SerializeField] TextMeshProUGUI aText;
    [SerializeField] bool aBool;
    [Space]
    [SerializeField] Image sKey;
    [SerializeField] TextMeshProUGUI sText;
    [SerializeField] bool sBool;
    [Space]
    [SerializeField] Image dKey;
    [SerializeField] TextMeshProUGUI dText;
    [SerializeField] bool dBool;
    [Space]
    [SerializeField] Image spaceKey;
    [SerializeField] TextMeshProUGUI spaceText;
    [SerializeField] bool spaceBool;
    [Space]
    [SerializeField] Image shiftKey;
    [SerializeField] TextMeshProUGUI shiftText;
    [SerializeField] bool shiftBool;
    [Space]
    [SerializeField] Image qKey;
    [SerializeField] TextMeshProUGUI qText;
    [SerializeField] bool qBool;


    #endregion


    [Space]
    [Header("Managers")]
    [SerializeField] DayNightCycleManager dm;
    [SerializeField] CalendarManger cm;
    [SerializeField] MapDirector md;

    [SerializeField] PlayerUIToggler ui;

    public void HighlightElement(GameObject focus, Vector2 highLightSize)
    {
        highlightFrame.LeanScale(highLightSize, moveTime).setEase(LeanTweenType.easeOutExpo);
        MoveHighlight(focus.transform.localPosition);
        //LeanTween.value()
    }

    public void SetHighlightPosition(Vector3 position)
    {
        highlightFrame.transform.position = position;
    }

    public void MoveHighlight(Vector3 position)
    {
        LeanTween.move(highlightFrame, position, moveTime).setEase(LeanTweenType.easeOutExpo);
    }



    #region _Toggle_Keys_

    public void ToggleW(Image key, TextMeshProUGUI letter, bool toggle)
    {
        if (toggle)
        {
            LeanTween.color(key.rectTransform, Color.white, moveTime);
            LeanTween.value(letter.gameObject, Wcallback, wText.color, Color.black, moveTime);
        }
        else
        {
            LeanTween.color(key.rectTransform, Color.black, moveTime);
            LeanTween.value(letter.gameObject, Wcallback, wText.color, Color.white, moveTime);
        }
        wBool = toggle;
    }
    public void Wcallback(Color val)
    {
        wText.color = val;
    }

    public void ToggleA(Image key, TextMeshProUGUI letter, bool toggle)
    {
        if (toggle)
        {
            LeanTween.color(key.rectTransform, Color.white, moveTime);
            LeanTween.value(letter.gameObject, Acallback, aText.color, Color.black, moveTime);
        }
        else
        {
            LeanTween.color(key.rectTransform, Color.black, moveTime);
            LeanTween.value(letter.gameObject, Acallback, aText.color, Color.white, moveTime);
        }
        aBool = toggle;
    }
    public void Acallback(Color val)
    {
        aText.color = val;
    }

    public void ToggleS(Image key, TextMeshProUGUI letter, bool toggle)
    {
        if (toggle)
        {
            LeanTween.color(key.rectTransform, Color.white, moveTime);
            LeanTween.value(letter.gameObject, Scallback, sText.color, Color.black, moveTime);
        }
        else
        {
            LeanTween.color(key.rectTransform, Color.black, moveTime);
            LeanTween.value(letter.gameObject, Scallback, sText.color, Color.white, moveTime);
        }
        sBool = toggle;
    }
    public void Scallback(Color val)
    {
        sText.color = val;
    }

    public void ToggleD(Image key, TextMeshProUGUI letter, bool toggle)
    {
        if (toggle)
        {
            LeanTween.color(key.rectTransform, Color.white, moveTime);
            LeanTween.value(letter.gameObject, Dcallback, dText.color, Color.black, moveTime);
        }
        else
        {
            LeanTween.color(key.rectTransform, Color.black, moveTime);
            LeanTween.value(letter.gameObject, Dcallback, dText.color, Color.white, moveTime);
        }
        dBool = toggle;
    }
    public void Dcallback(Color val)
    {
        dText.color = val;
    }

    public void ToggleSPACE(Image key, TextMeshProUGUI letter, bool toggle)
    {
        if (toggle)
        {
            LeanTween.color(key.rectTransform, Color.white, moveTime);
            LeanTween.value(letter.gameObject, SPACEcallback, spaceText.color, Color.black, moveTime);
        }
        else
        {
            LeanTween.color(key.rectTransform, Color.black, moveTime);
            LeanTween.value(letter.gameObject, SPACEcallback, spaceText.color, Color.white, moveTime);
        }
        spaceBool = toggle;
    }
    public void SPACEcallback(Color val)
    {
        spaceText.color = val;
    }

    public void ToggleSHIFT(Image key, TextMeshProUGUI letter, bool toggle)
    {
        if (toggle)
        {
            LeanTween.color(key.rectTransform, Color.white, moveTime);
            LeanTween.value(letter.gameObject, SHIFTcallback, shiftText.color, Color.black, moveTime);
        }
        else
        {
            LeanTween.color(key.rectTransform, Color.black, moveTime);
            LeanTween.value(letter.gameObject, SHIFTcallback, shiftText.color, Color.white, moveTime);
        }
        shiftBool = toggle;
    }
    public void SHIFTcallback(Color val)
    {
        shiftText.color = val;
    }

    public void ToggleQ(Image key, TextMeshProUGUI letter, bool toggle)
    {
        if (toggle)
        {
            LeanTween.color(key.rectTransform, Color.white, moveTime);
            LeanTween.value(letter.gameObject, Qcallback, qText.color, Color.black, moveTime);
        }
        else
        {
            LeanTween.color(key.rectTransform, Color.black, moveTime);
            LeanTween.value(letter.gameObject, Qcallback, qText.color, Color.white, moveTime);
        }
        qBool = toggle;
    }
    public void Qcallback(Color val)
    {
        qText.color = val;
    }



    #endregion




    public void MoveTutorialScreen(RectTransform tuto, Vector3 position)
    {
        LeanTween.move(tuto, position, moveTime).setEase(LeanTweenType.easeOutExpo);
    }

    public void ScriptedReasourceNode()
    {
        md.SpawnNode(0, this.transform.position);
    }

    public void AdvanceTutorial()
    {
        //this should increment the index by 1 and then perform actions based on what index we are at

        switch (tutoIndex)
        {
            case 0:
                Debug.Log("before");
                MoveTutorialScreen(intro, offPos);
                ui.SetUIOpenBool(false);
                break;

            case 1:
                Debug.Log("1--");
                MoveTutorialScreen(controls, offPos + new Vector3(1300, 0, 0));
                //controls.gameObject.SetActive(false);
                break;

            case 2:
                Debug.Log("2--");
                break;

            case 3:
                Debug.Log("3--");
                break;

            case 4:
                Debug.Log("4--");
                break;
        }

        ++tutoIndex;

        switch (tutoIndex)
        {
            case 0:
                Debug.Log("HUH");
                break;

            case 1:
                Debug.Log("1++");
                MoveTutorialScreen(controls, onPos);
                break;

            case 2:
                Debug.Log("2++");
                break;

            case 3:
                Debug.Log("3++");
                break;

            case 4:
                Debug.Log("4++");
                break;
        }
    }

    public void InitializeManagers()
    {
        dm.InitializeManger();
        dm.ToggleTime(true);
        cm.InitializeManager();
    }

    private void Awake()
    {

        //InitializeManagers();
        //ScriptedReasourceNode();
        ui.SetUIOpenBool(true);
        //AdvanceTutorial();
    }

    private void Update()
    {

        if (Input.GetKey(KeyCode.Backspace))
        {
            HighlightElement(tempTarget, new Vector3(3.36f, 1));
        }

        if (tutoIndex == 1)
        {
            //check every frame for the inputs specific inputs
            //once an input has been pressed, toggle a boolean and change the colour and text of the button
            //after every input has been pressed, move on to the next stage after a bit

            if (Input.GetKey(KeyCode.W) && !wBool)
            {
                ToggleW(wKey, wText, true);
            }
            if (Input.GetKey(KeyCode.A) && !aBool)
            {
                ToggleA(aKey, aText, true);
            }
            if (Input.GetKey(KeyCode.S) && !sBool)
            {
                ToggleS(sKey, sText, true);
            }
            if (Input.GetKey(KeyCode.D) && !dBool)
            {
                ToggleD(dKey, dText, true);
            }
            if (Input.GetKey(KeyCode.Space) && !spaceBool)
            {
                ToggleSPACE(spaceKey, spaceText, true);
            }
            if (Input.GetKey(KeyCode.LeftShift) && !shiftBool)
            {
                ToggleSHIFT(shiftKey, shiftText, true);
            }
            if (Input.GetKey(KeyCode.Q) && !qBool)
            {
                ToggleQ(qKey, qText, true);
            }

            if (wBool && aBool && sBool && dBool && spaceBool && shiftBool && qBool)
            {
                Invoke("AdvanceTutorial", 2.5f);
            }
        }
    }
}
