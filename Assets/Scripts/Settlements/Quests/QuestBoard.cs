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

    public GameObject scrollContent;
    public enum QuestStatus { InProgress, Completed }

    public List<QuestDisplay> quests;

    public GameObject baseQuestUI;
    public QuestObject baseQuestObject;


    public void AddQuest(QuestObject newObject = null)
    {
        Debug.Log(baseQuestObject);
        if (newObject == null)
        {
            newObject = Instantiate(baseQuestObject);
        }
            
        GameObject questUI = Instantiate(baseQuestUI, scrollContent.transform);
        QuestDisplay newDisplay = questUI.GetComponent<QuestDisplay>();


        newObject.SetResourceCount(Random.Range(0, newObject.GetResourceRequirement()));
        newDisplay.SetQuestObject(newObject);
        quests.Add(newDisplay);

        
        //instantiate quest UI
        //add something to the scrollQuestParent
    }

    public void RemoveQuest(QuestDisplay removeQuest)
    {
        quests.Remove(removeQuest);
        //remove something to the scrollQuestParent
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            AddQuest();
        }
    }
}
