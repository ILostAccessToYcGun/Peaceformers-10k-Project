using TMPro;
using UnityEngine;
using UnityEngine.UI;
//using UnityEngine.UIElements;

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
    [SerializeField] RectTransform boost;
    [SerializeField] RectTransform battery;
    [SerializeField] RectTransform time;
    [SerializeField] RectTransform date;
    [SerializeField] RectTransform camp;
    [SerializeField] RectTransform materials;
    [SerializeField] RectTransform inventory;
    [SerializeField] RectTransform items;
    [SerializeField] RectTransform itemsSize;
    [SerializeField] RectTransform trash;
    [SerializeField] RectTransform drop;
    [SerializeField] RectTransform ammo;
    [SerializeField] RectTransform navigation;
    [SerializeField] RectTransform travel;
    [SerializeField] RectTransform questBoard;
    [SerializeField] RectTransform questDetails;
    [SerializeField] RectTransform midgardInteract;
    [SerializeField] RectTransform handInQuest;
    [SerializeField] RectTransform settlementBars;
    [SerializeField] RectTransform startGame;

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
    [Header("Materials")]
    [SerializeField] GameObject tutoNode;
    [SerializeField] Image eKey;
    [SerializeField] TextMeshProUGUI eText;
    [SerializeField] bool eBool;

    [Space]
    [Header("Camp")]
    [SerializeField] GameObject playerCamp;
    [SerializeField] GameObject campHighlight;

    [Space]
    [Header("Inventory")]
    [SerializeField] Image tabKey;
    [SerializeField] TextMeshProUGUI tabText;

    [Header("Ammo")]
    [SerializeField] Image rKey;
    [SerializeField] TextMeshProUGUI rText;

    [Header("Navigation")]
    [SerializeField] Button midgardNavigation;

    [Header("QuestBoard")]
    [SerializeField] Image fKey;
    [SerializeField] TextMeshProUGUI fText;
    
    [Header("QuestDetails")]
    [SerializeField] Image escKey;
    [SerializeField] TextMeshProUGUI escText;
    
    [Header("Settlement Bars")]
    [SerializeField] Image tKey;
    [SerializeField] TextMeshProUGUI tText;

    [Space]
    [Header("Managers")]
    [SerializeField] DayNightCycleManager dm;
    [SerializeField] CalendarManger cm;
    [SerializeField] MapDirector md;
    [SerializeField] Settings settings;

    [Space]
    [Header("Player")]
    [SerializeField] PlayerMovement player;
    [SerializeField] PlayerGun gun;
    [SerializeField] PlayerUIToggler ui;
    [SerializeField] PlayerNavigation nav;
    [SerializeField] Inventory playerInv;

    [Space]
    [Header("Quest")]
    [SerializeField] QuestGiver qg;
    [SerializeField] QuestObject tutoQuest;
    [SerializeField] SettlementQuestBoard sqb;
    [SerializeField] RectTransform sqbContent;

    #region _Highlight_
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

    #endregion



    #region _Toggle_Keys_

    public void ToggleW(Image key, TextMeshProUGUI letter, bool toggle)
    {
        if (toggle)
        {
            LeanTween.color(key.rectTransform, Color.white, moveTime);
            LeanTween.value(letter.gameObject, Wcallback, wText.color, Color.black, 0.5f);
        }
        else
        {
            LeanTween.color(key.rectTransform, Color.black, moveTime);
            LeanTween.value(letter.gameObject, Wcallback, wText.color, Color.white, 0.5f);
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
            LeanTween.value(letter.gameObject, Acallback, aText.color, Color.black, 0.5f);
        }
        else
        {
            LeanTween.color(key.rectTransform, Color.black, moveTime);
            LeanTween.value(letter.gameObject, Acallback, aText.color, Color.white, 0.5f);
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
            LeanTween.value(letter.gameObject, Scallback, sText.color, Color.black, 0.5f);
        }
        else
        {
            LeanTween.color(key.rectTransform, Color.black, moveTime);
            LeanTween.value(letter.gameObject, Scallback, sText.color, Color.white, 0.5f);
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
            LeanTween.value(letter.gameObject, Dcallback, dText.color, Color.black, 0.5f);
        }
        else
        {
            LeanTween.color(key.rectTransform, Color.black, moveTime);
            LeanTween.value(letter.gameObject, Dcallback, dText.color, Color.white, 0.5f);
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
            LeanTween.value(letter.gameObject, SPACEcallback, spaceText.color, Color.black, 0.5f);
        }
        else
        {
            LeanTween.color(key.rectTransform, Color.black, moveTime);
            LeanTween.value(letter.gameObject, SPACEcallback, spaceText.color, Color.white, 0.5f);
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
            LeanTween.value(letter.gameObject, SHIFTcallback, shiftText.color, Color.black, 0.5f);
        }
        else
        {
            LeanTween.color(key.rectTransform, Color.black, moveTime);
            LeanTween.value(letter.gameObject, SHIFTcallback, shiftText.color, Color.white, 0.5f);
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
            LeanTween.value(letter.gameObject, Qcallback, qText.color, Color.black, 0.5f);
        }
        else
        {
            LeanTween.color(key.rectTransform, Color.black, moveTime);
            LeanTween.value(letter.gameObject, Qcallback, qText.color, Color.white, 0.5f);
        }
        qBool = toggle;
    }
    public void Qcallback(Color val)
    {
        qText.color = val;
    }




    public void ToggleE(Image key, TextMeshProUGUI letter, bool toggle)
    {
        if (toggle)
        {
            LeanTween.color(key.rectTransform, Color.white, moveTime);
            LeanTween.value(letter.gameObject, Ecallback, eText.color, Color.black, 0.5f);
        }
        else
        {
            LeanTween.color(key.rectTransform, Color.black, moveTime);
            LeanTween.value(letter.gameObject, Ecallback, eText.color, Color.white, 0.5f);
        }
        eBool = toggle;
    }
    public void Ecallback(Color val)
    {
        eText.color = val;
    }




    public void ToggleTAB(Image key, TextMeshProUGUI letter, bool toggle)
    {
        if (toggle)
        {
            LeanTween.color(key.rectTransform, Color.white, moveTime);
            LeanTween.value(letter.gameObject, TABcallback, tabText.color, Color.black, 0.5f);
        }
        else
        {
            LeanTween.color(key.rectTransform, Color.black, moveTime);
            LeanTween.value(letter.gameObject, TABcallback, tabText.color, Color.white, 0.5f);
        }
    }
    public void TABcallback(Color val)
    {
        tabText.color = val;
    }

    public void ToggleR(Image key, TextMeshProUGUI letter, bool toggle)
    {
        if (toggle)
        {
            LeanTween.color(key.rectTransform, Color.white, moveTime);
            LeanTween.value(letter.gameObject, Rcallback, rText.color, Color.black, 0.5f);
        }
        else
        {
            LeanTween.color(key.rectTransform, Color.black, moveTime);
            LeanTween.value(letter.gameObject, Rcallback, rText.color, Color.white, 0.5f);
        }
    }
    public void Rcallback(Color val)
    {
        rText.color = val;
    }

    public void ToggleF(Image key, TextMeshProUGUI letter, bool toggle)
    {
        if (toggle)
        {
            LeanTween.color(key.rectTransform, Color.white, moveTime);
            LeanTween.value(letter.gameObject, Fcallback, fText.color, Color.black, 0.5f);
        }
        else
        {
            LeanTween.color(key.rectTransform, Color.black, moveTime);
            LeanTween.value(letter.gameObject, Fcallback, fText.color, Color.white, 0.5f);
        }
    }
    public void Fcallback(Color val)
    {
        fText.color = val;
    }

    public void ToggleESC(Image key, TextMeshProUGUI letter, bool toggle)
    {
        if (toggle)
        {
            LeanTween.color(key.rectTransform, Color.white, moveTime);
            LeanTween.value(letter.gameObject, ESCcallback, escText.color, Color.black, 0.5f);
        }
        else
        {
            LeanTween.color(key.rectTransform, Color.black, moveTime);
            LeanTween.value(letter.gameObject, ESCcallback, escText.color, Color.white, 0.5f);
        }
    }
    public void ESCcallback(Color val)
    {
        escText.color = val;
    }
    
    public void ToggleT(Image key, TextMeshProUGUI letter, bool toggle)
    {
        if (toggle)
        {
            LeanTween.color(key.rectTransform, Color.white, moveTime);
            LeanTween.value(letter.gameObject, Tcallback, tText.color, Color.black, 0.5f);
        }
        else
        {
            LeanTween.color(key.rectTransform, Color.black, moveTime);
            LeanTween.value(letter.gameObject, Tcallback, tText.color, Color.white, 0.5f);
        }
    }
    public void Tcallback(Color val)
    {
        tText.color = val;
    }

    #endregion




    public void MoveTutorialScreen(RectTransform tuto, Vector3 position)
    {
        LeanTween.move(tuto, position, moveTime).setEase(LeanTweenType.easeOutExpo);
    }

    //public void ScriptedReasourceNode()
    //{
    //    md.SpawnNode(0, this.transform.position);
    //}

    public void AdvanceTutorial()
    {
        //this should increment the index by 1 and then perform actions based on what index we are at
        GameObject.FindObjectOfType<AudioManager>().Play("UI_Click");

        switch (tutoIndex)
        {
            case 0:
                MoveTutorialScreen(intro, offPos);
                break;

            case 1:
                MoveTutorialScreen(controls, offPos);
                break;

            case 2:
                MoveTutorialScreen(boost, offPos);
                break;

            case 3:
                MoveTutorialScreen(battery, offPos);
                break;

            case 4:
                MoveTutorialScreen(time, offPos);
                break;

            case 5:
                MoveTutorialScreen(date, offPos);
                break;

            case 6:
                MoveTutorialScreen(camp, offPos);
                ui.SetUIOpenBool(false);
                campHighlight.SetActive(false);
                break;

            case 7:
                MoveTutorialScreen(materials, offPos);
                break;

            case 8:
                MoveTutorialScreen(inventory, offPos);
                break;

            case 9:
                MoveTutorialScreen(items, offPos);
                break;
            case 10:
                MoveTutorialScreen(itemsSize, offPos);
                break;

            case 11:
                MoveTutorialScreen(trash, offPos);
                break;
            case 12:
                MoveTutorialScreen(drop, offPos);
                break;

            case 13:
                MoveTutorialScreen(ammo, offPos);
                break;
            
            case 14:
                MoveTutorialScreen(navigation, offPos);
                break;

            case 15:
                MoveTutorialScreen(travel, offPos);
                break;
            
            case 16:
                MoveTutorialScreen(questBoard, offPos);
                break;
            
            case 17:
                MoveTutorialScreen(questDetails, offPos);
                break;
            
            case 18:
                MoveTutorialScreen(midgardInteract, offPos);
                break;
            
            case 19:
                MoveTutorialScreen(handInQuest, offPos);
                break;
            
            case 20:
                MoveTutorialScreen(settlementBars, offPos);
                break;
        }

        ++tutoIndex;

        switch (tutoIndex)
        {
            case 1:
                MoveTutorialScreen(controls, onPos);
                break;

            case 2:
                MoveTutorialScreen(boost, onPos);
                break;

            case 3:
                MoveTutorialScreen(battery, onPos);
                break;

            case 4:
                MoveTutorialScreen(time, onPos);
                break;

            case 5:
                MoveTutorialScreen(date, onPos);
                break;

            case 6:
                MoveTutorialScreen(camp, onPos);
                nav.navigationTarget = playerCamp;
                campHighlight.SetActive(true);
                break;

            case 7:
                MoveTutorialScreen(materials, onPos);
                nav.navigationTarget = tutoNode;
                break;

            case 8:
                MoveTutorialScreen(inventory, onPos);
                break;

            case 9:
                MoveTutorialScreen(items, onPos);
                break;

            case 10:
                MoveTutorialScreen(itemsSize, onPos);
                break;

            case 11:
                MoveTutorialScreen(trash, onPos);
                break;

            case 12:
                MoveTutorialScreen(drop, onPos);
                break;

            case 13:
                MoveTutorialScreen(ammo, onPos);
                break;
            
            case 14:
                midgardNavigation.onClick.AddListener(NavigationAdvance);
                MoveTutorialScreen(navigation, onPos);
                break;

            case 15:
                MoveTutorialScreen(travel, onPos);
                break;

            case 16:
                MoveTutorialScreen(questBoard, onPos);
                break;
            
            case 17:
                MoveTutorialScreen(questDetails, onPos);
                break;
            
            case 18:
                MoveTutorialScreen(midgardInteract, onPos);
                break;
            
            case 19:
                MoveTutorialScreen(handInQuest, onPos);
                //we need to somehow add an advance tutorial listener to the hand in button on the settlement quest UI
                Button[] buttons = sqbContent.gameObject.GetComponentsInChildren<Button>();
                foreach (Button button in buttons)
                {
                    if (button.gameObject.name == "HandInQuestButton")
                        button.onClick.AddListener(AdvanceTutorial); 
                }
                break;
            
            case 20:
                MoveTutorialScreen(settlementBars, onPos);
                break;

            case 21:
                ui.ToggleSettlementUI();
                ui.SetUIOpenBool(true);
                MoveTutorialScreen(startGame, onPos);
                break;
        }
    }

    public void StartGame()
    {
        MoveTutorialScreen(startGame, offPos);
        if (tutoNode != null)
            tutoNode.SetActive(false);
        playerInv.ClearInventory();
        player.ResetPos();
        gun.ResetAmmo();
        qg.gameObject.transform.position = new Vector3(-15.52f, 1.5f, 474);
        dm.InitializeManger();
        dm.ToggleTime(true);
        cm.InitializeManager();
        ui.SetUIOpenBool(false);
    }

    public void NavigationAdvance()
    {
        midgardNavigation.onClick.RemoveListener(NavigationAdvance);
        AdvanceTutorial();
    }

    private void Awake()
    {
        settings = FindAnyObjectByType<Settings>();
        if (settings.playTutorial)
        {
            ui.SetUIOpenBool(true);
            qg.gameObject.transform.position = new Vector3(-15.52f, 1.5f, 100);
        }
        else
        {
            intro.gameObject.SetActive(false);
            controls.gameObject.SetActive(false);
            boost.gameObject.SetActive(false);
            battery.gameObject.SetActive(false);
            time.gameObject.SetActive(false);
            date.gameObject.SetActive(false);
            camp.gameObject.SetActive(false);
            materials.gameObject.SetActive(false);
            inventory.gameObject.SetActive(false);
            items.gameObject.SetActive(false);
            itemsSize.gameObject.SetActive(false);
            trash.gameObject.SetActive(false);
            drop.gameObject.SetActive(false);
            ammo.gameObject.SetActive(false);
            navigation.gameObject.SetActive(false);
            travel.gameObject.SetActive(false);
            questBoard.gameObject.SetActive(false);
            questDetails.gameObject.SetActive(false);
            midgardInteract.gameObject.SetActive(false);
            handInQuest.gameObject.SetActive(false);
            settlementBars.gameObject.SetActive(false);
            startGame.gameObject.SetActive(false);
        }
            
        
    }

    private void Start()
    {
        if (settings.playTutorial)
        {
            tutoQuest.SetCalenderManager();
            tutoQuest.SetTimeManager();
            qg.AddQuestToGiver(tutoQuest);
        }
        else
            Invoke("StartGame", 0.01f);


    }

    private void Update()
    {

        if (Input.GetKey(KeyCode.Backspace))
        {
            HighlightElement(tempTarget, new Vector3(3.36f, 1));
        }

        switch (tutoIndex)
        {
            case 1:
                if (Input.GetKey(KeyCode.W) && !wBool)
                    ToggleW(wKey, wText, true);
                if (Input.GetKey(KeyCode.A) && !aBool)
                    ToggleA(aKey, aText, true);
                if (Input.GetKey(KeyCode.S) && !sBool)
                    ToggleS(sKey, sText, true);
                if (Input.GetKey(KeyCode.D) && !dBool)
                    ToggleD(dKey, dText, true);
                if (Input.GetKey(KeyCode.Space) && !spaceBool)
                    ToggleSPACE(spaceKey, spaceText, true);
                if (Input.GetKey(KeyCode.LeftShift) && !shiftBool)
                    ToggleSHIFT(shiftKey, shiftText, true);
                if (Input.GetKey(KeyCode.Q) && !qBool)
                    ToggleQ(qKey, qText, true);

                if (wBool && aBool && sBool && dBool && spaceBool && shiftBool && qBool && !IsInvoking())
                {
                    Invoke("AdvanceTutorial", 2f);
                }
                    
                break;
            case 7:
                if (Input.GetKey(KeyCode.E) && !eBool)
                    ToggleE(eKey, eText, true);

                if (playerInv.FindItemCountOfName(Item.Name.Wood) >= 10 && !IsInvoking())
                    Invoke("AdvanceTutorial", 1f);
                break;

            case 8:
                if (Input.GetKey(KeyCode.Tab) && !IsInvoking())
                {
                    ToggleTAB(tabKey, tabText, true);
                    Invoke("AdvanceTutorial", 1f);
                }  
                break;

            case 13:
                if (Input.GetKey(KeyCode.R))
                    ToggleR(rKey, rText, true);
                break;

            case 15:
                if (Vector3.Distance(player.transform.position, qg.transform.position) <= 30f)
                {
                    AdvanceTutorial();
                }
                break;

            case 16:
                if (Input.GetKey(KeyCode.F) && !IsInvoking())
                {
                    ToggleF(fKey, fText, true);
                    Invoke("AdvanceTutorial", 1f);
                }
                break;
            
            case 17:
                if (Input.GetKey(KeyCode.Escape) && !IsInvoking())
                {
                    ToggleESC(escKey, escText, true);
                    Invoke("AdvanceTutorial", 1f);
                }
                else if (Input.GetKey(KeyCode.F) && !IsInvoking())
                {
                    Invoke("AdvanceTutorial", 1f);
                }
                break;

            case 18:
                if (sqb.GetCurrentViewingQuestGiver() == qg)
                {
                    AdvanceTutorial();
                }
                break;

            case 20:
                if (Input.GetKey(KeyCode.T) && !IsInvoking())
                {
                    ToggleT(tKey, tText, true);
                    Invoke("AdvanceTutorial", 3f);
                }
                break;
        }
    }
}
