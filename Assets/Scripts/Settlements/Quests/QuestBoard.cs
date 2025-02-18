using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class QuestBoard : MonoBehaviour
{
    /*
     * this is the player's quest UI
     * here the player will be able to see all their accepted quests
     * including:
     *  what settlement it came from
     *  quest status (in-progress, completed
     *  time remaining/due date
     *  the material type
     *  quest requirements (e.g 12/18 resources)
     * 
     * the player can technically hold an infinite number of quests, but if they take too many then the settlement meters will drop too much
     * 
     
     */
    [SerializeField] public enum RemoveType { Remove, Hand_In, Clear }

    [Space]
    [Header("OtherObjects")]
    [SerializeField] protected GameObject scrollContent;
    [SerializeField] protected QuestBoard otherQuestBoard;
    [SerializeField] protected Inventory playerInventory;

    [Space]
    [Header("Quests")]
    [SerializeField] protected List<QuestDisplay> quests;

    [Space]
    [Header("Base Objects")]
    [SerializeField] protected GameObject baseQuestUI;
    [SerializeField] protected QuestObject baseQuestObject;

    [Space]
    [Header("Abandon Stuff")]
    [SerializeField] protected GameObject abandonVerification;
    [SerializeField] protected Button abandonConfirmButton;

    #region _Quests_

    public virtual QuestDisplay AddQuestToBoard(QuestObject newObject = null)
    {
        Debug.Log(baseQuestObject);
        if (newObject == null)
        {
            newObject = Instantiate(baseQuestObject);
            //newObject.SetResourceCount(Random.Range(0, newObject.GetResourceRequirement()));
        }

        GameObject questUI = Instantiate(baseQuestUI, scrollContent.transform);
        QuestDisplay newDisplay = questUI.GetComponent<QuestDisplay>();

        newObject.SetCorrespondingPlayerQuestDisplayUI(newDisplay);

        newDisplay.SetAbandonVerification(abandonVerification);
        newDisplay.SetAbandonConfirmButton(abandonConfirmButton);
        newDisplay.SetParentQuestBoard(this); //streamline this eventually
        newDisplay.SetOtherQuestBoard(otherQuestBoard);
        newDisplay.SetQuestObject(newObject);
        quests.Add(newDisplay);
        UpdateQuests();
        return newDisplay;
    }

    public virtual void RemoveQuestFromBoard(QuestDisplay removeQuest, RemoveType removeType = RemoveType.Clear)
    {
        Debug.Log(removeQuest);
        if (removeQuest == null) { return; }
        quests.Remove(removeQuest);
        Destroy(removeQuest.gameObject);
    }

    public void UpdateQuests()
    {
        //update the quest object resource count
        foreach (QuestDisplay display in quests) //loop through the player's quests
        {
            Debug.Log("updating...");
            Item.Name resourceToFind = display.questObject.GetResource();//find the needed material name/type
            Debug.Log("resourceToFind = " + resourceToFind);
            int newResourceCount = playerInventory.FindItemCountOfName(resourceToFind);
            Debug.Log("newResourceCount = " + newResourceCount);

            display.questObject.SetResourceCount(newResourceCount);
            display.UpdateSliderUI();
        }
    }

    #endregion


    public void AbortAbandon()
    {
        abandonVerification.SetActive(false);
    }


}
