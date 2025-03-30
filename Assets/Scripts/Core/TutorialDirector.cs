using UnityEngine;

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

    [SerializeField] RectTransform highlightFrame;
    [SerializeField] float moveTime;
    [SerializeField] GameObject tempTarget;

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

    private void Update()
    {

        if (Input.GetKey(KeyCode.Backspace))
        {
            HighlightElement(tempTarget, new Vector3(3.36f, 1));
        }
    }
}
