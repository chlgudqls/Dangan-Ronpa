using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DatabaseManager : MonoBehaviour
{
    public static DatabaseManager instance;

    [SerializeField] string csv_FileName;

    Dictionary<int, Dialogue> dialogueDic = new Dictionary<int, Dialogue>();

    public bool[] eventFlags = new bool[100];

    public static bool isFinish = false;


    // 메모리에 저장하는 부분
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DialogueParser theParser = GetComponent<DialogueParser>();
            // 리턴받은 배열을 배열변수안에 넣을때 
            Dialogue[] dialogues = theParser.Parse(csv_FileName);
            for (int i = 0; i < dialogues.Length; i++)
            {
                dialogueDic.Add(i + 1, dialogues[i]);
            }
            // 파싱이 완전히 끝난시점
            isFinish = true;
        }
    }
    // 인자값에 따라서 꺼내오는 부분
    // 원하는 datalist의 인덱스를 넘겨받음
    public Dialogue[] GetDialogue(int _StartNum, int _EndNum)
    {
        List<Dialogue> dialogueList = new List<Dialogue>();

        // ※ 인덱스는 원래 start 숫자보다 1이 작아서 
        for (int i = 0; i <= _EndNum - _StartNum; i++)
        {
            dialogueList.Add(dialogueDic[_StartNum + i]);
        }
        return dialogueList.ToArray();
    }
}
