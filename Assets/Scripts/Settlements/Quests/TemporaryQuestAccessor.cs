using UnityEngine;

public class TemporaryQuestAccessor : MonoBehaviour
{
    public QuestBoard questBoard;
    public SettlementQuestBoard settlementBoard;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            questBoard.gameObject.SetActive(!questBoard.gameObject.activeSelf); 
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            settlementBoard.gameObject.SetActive(!settlementBoard.gameObject.activeSelf);
        }
        if (Input.GetKeyDown(KeyCode.J))
        {
            questBoard.AddQuest();
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            settlementBoard.AddQuest();
        }
    }
}
