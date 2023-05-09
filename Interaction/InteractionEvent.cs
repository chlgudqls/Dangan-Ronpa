using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionEvent : MonoBehaviour
{
    [SerializeField] bool isAutoEvent = false;

    // ※ 이벤트를 여러개로 만듬  - ※ 조건에 맞는 이벤트만의 인덱스가 필요
    [SerializeField] DialogueEvent[] dialogueEvent;
    int currentCount;

    private void Start()
    {
        bool t_Flag = CheckEvent();

        gameObject.SetActive(t_Flag);
    }

    // 원래 체크는 씬이 전환될때만 호출을 했기때문에 start에 있었는데 대화가진행될때마다 체크를 해야됨이제
    bool CheckEvent()
    {
        bool t_Flag = true;

        for (int x = 0; x < dialogueEvent.Length; x++)
        {
            t_Flag = true;

            for (int i = 0; i < dialogueEvent[x].eventTiming.eventConditions.Length; i++)
            {
                // ※ 새로운 방식
                if (DatabaseManager.instance.eventFlags[dialogueEvent[x].eventTiming.eventConditions[i]] != dialogueEvent[x].eventTiming.conditionFlag)
                {
                    t_Flag = false;
                    break;
                }
            }
            // ※※ 여기서 주의할점 이미 한번실행했다고 end가 eventflag와 같다고 생각하면안됨 eventflag가 2이고 endflag가 10이될수가있음
            // 그래서 여기서 걸리지않고 true반환해서 상호작용 후인 2번째event의 b대화가 호출될수있음 
            // 무조건 한번실행하면 end가 false가 되서 b대화가 호출되지 못할거라는 생각을 했음
            if (DatabaseManager.instance.eventFlags[dialogueEvent[x].eventTiming.eventEndNum])
                t_Flag = false;

            if(t_Flag)
            {
                currentCount = x;
                break;
            }
        }
        return t_Flag;
    }

    // dialogueEvent 속성값이 있는데 위치값이 덮어씌워지면서 null이되버림
    // 변수생성하고 타겟은 따로 대입
    public Dialogue[] GetDialogue()
    {
        // ※ b대화가 완전히 안나오는건 아니고 eventEndNum과 eventNum가 다를수도있어서 그런건아니네
        // ※ End 조건이 만족했을때 null을 반환하는데 이때 문개방시킨다
        if (DatabaseManager.instance.eventFlags[dialogueEvent[currentCount].eventTiming.eventEndNum])
            return null;
        // 상호작용 전 대화
        if (!DatabaseManager.instance.eventFlags[dialogueEvent[currentCount].eventTiming.eventNum] || dialogueEvent[currentCount].isSame)
        {
            DatabaseManager.instance.eventFlags[dialogueEvent[currentCount].eventTiming.eventNum] = true;
            // 자기자신의 것을 넘겨서 자기자신에 대입함
            dialogueEvent[currentCount].dialogues = SettingDialogue(dialogueEvent[currentCount].dialogues, (int)dialogueEvent[currentCount].line.x, (int)dialogueEvent[currentCount].line.y);
                return dialogueEvent[currentCount].dialogues;
        }
        // 상호작용 후 대화
        else
        {
            dialogueEvent[currentCount].dialoguesB = SettingDialogue(dialogueEvent[currentCount].dialoguesB, (int)dialogueEvent[currentCount].lineB.x, (int)dialogueEvent[currentCount].lineB.y);
            return dialogueEvent[currentCount].dialoguesB;
        }

    }

    // 새로운 방식으로 기존의 GetDialogue 을 뒤엎음
    // 매개변수의 타입이 Dialogue라서 event에서 타입 바꿈
    Dialogue[] SettingDialogue(Dialogue[] p_Dialogue, int p_lineX, int p_lineY)
    {
        Dialogue[] t_Dialogues = DatabaseManager.instance.GetDialogue(p_lineX, p_lineY);

        for (int i = 0; i < dialogueEvent[currentCount].dialogues.Length; i++)
        {
            t_Dialogues[i].tf_Target = p_Dialogue[i].tf_Target;
            t_Dialogues[i].cameraType = p_Dialogue[i].cameraType;
        }

        return t_Dialogues;
    }

    public AppearType GetAppearType()
    {
        return dialogueEvent[currentCount].appearType;
    }
    public GameObject[] GetTargets()
    {
        return dialogueEvent[currentCount].go_Targets;
    }

    public GameObject GetNextEvent()
    {
        return dialogueEvent[currentCount].go_NextEvent;
    }

    public int GetEventNum()
    {
        CheckEvent();
        return dialogueEvent[currentCount].eventTiming.eventNum;
    }

    
    private void Update()
    {
        // ※ TransferManager.isFinished가 false로 바뀔때 실행을 막게 함
        if (isAutoEvent && DatabaseManager.isFinish && TransferManager.isFinished)
        {
            DialogueManager theDM = FindObjectOfType<DialogueManager>();
            // ※ 이 시점에서 true로 바꾸고 대기시간 0.5초를 만듬
            DialogueManager.isWaiting = true;

            if (GetAppearType() == AppearType.Appear) theDM.SetAppearObjects(GetTargets());
            else if (GetAppearType() == AppearType.Disappear) theDM.SetDisappearObjects(GetTargets());
            theDM.SetNextEvent(GetNextEvent());
            theDM.ShowDialogue(GetDialogue());

            gameObject.SetActive(false);
        }
    }
}
