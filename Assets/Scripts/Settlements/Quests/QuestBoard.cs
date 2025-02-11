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

    public GameObject scrollQuestParent;
    public enum QuestStatus { InProgress, Completed }

    public List<Quest> quests;


    public void AddQuest(Quest newQuest)
    {
        quests.Add(newQuest);
        //add something to the scrollQuestParent
    }

    public void RemoveQuest(Quest removeQuest)
    {
        quests.Remove(removeQuest);
        //remove something to the scrollQuestParent
    }



}
