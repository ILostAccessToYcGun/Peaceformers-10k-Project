using UnityEngine;

public class TemporaryQuestAccessor : MonoBehaviour
{
    public QuestBoard questBoard;
    public SettlementQuestBoard settlementBoard;
    public QuestGiver questGivingSettlement;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            //settlementBoard.gameObject.SetActive(!settlementBoard.gameObject.activeSelf);
        }
        if (Input.GetKeyDown(KeyCode.J))
        {
            questBoard.AddQuestToBoard();
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            questGivingSettlement.AddQuestToGiver();
        }
    }
}
