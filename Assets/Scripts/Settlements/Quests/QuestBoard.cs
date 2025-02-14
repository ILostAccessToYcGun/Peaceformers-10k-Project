using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

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

    [SerializeField] protected GameObject scrollContent;
    public enum QuestStatus { InProgress, Completed } //????

    [SerializeField] protected List<QuestDisplay> quests;

    [SerializeField] protected GameObject baseQuestUI;
    [SerializeField] protected QuestObject baseQuestObject;

    //public void ToggleQuestBoardVisibility()
    //{
    //    if (this.gameObject.activeSelf)
    //        this.gameObject.SetActive(false);
    //    else
    //        this.gameObject.SetActive(true);
    //}
    public virtual void AddQuest(QuestObject newObject = null)
    {
        Debug.Log(baseQuestObject);
        if (newObject == null)
        {

            newObject = Instantiate(baseQuestObject);
            newObject.SetResourceCount(Random.Range(0, newObject.GetResourceRequirement()));
        }
            
        GameObject questUI = Instantiate(baseQuestUI, scrollContent.transform);
        QuestDisplay newDisplay = questUI.GetComponent<QuestDisplay>();


        
        newDisplay.SetQuestBoard(this); //streamline this eventually
        newDisplay.SetQuestObject(newObject);
        quests.Add(newDisplay);

        
        //instantiate quest UI
        //add something to the scrollQuestParent
    }

    public void RemoveQuest(QuestDisplay removeQuest)
    {
        quests.Remove(removeQuest);
        Destroy(removeQuest.gameObject);
        //remove something to the scrollQuestParent

        //I need to show a confirmation prompt to the plyer to make sure they want to adandon the quest.
    }
}
