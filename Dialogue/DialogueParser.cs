using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueParser : MonoBehaviour
{
    public Dialogue[] Parse(string _CSVFileName)
    {
        List<Dialogue> dialogueList = new List<Dialogue>(); // 대사 리스트 생성
        TextAsset csvData = Resources.Load<TextAsset>(_CSVFileName); // CSV 파일 가져옴 

        // 가져온것을 엔터 단위로 나눈다고함
        string[] data = csvData.text.Split(new char[] { '\n' });

        for (int i = 1; i < data.Length;)
        {
            // if문에서 i를 증가시키지않았으면 무한루프에 빠짐
            //Debug.Log(data[i]);
            string[] row = data[i].Split(new char[]{','});
            Dialogue dialogue = new Dialogue();

            dialogue.name = row[1];

            //Debug.Log(row[1]);

            List<string> contextList = new List<string>();
            List<string> spriteList = new List<string>();
            List<string> voiceList = new List<string>();

            do
            {
                contextList.Add(row[2]);
                spriteList.Add(row[3]);
                voiceList.Add(row[4]);
                //Debug.Log(row[2]);
                // if문을 이렇게 쓰기만해도 i값은 증가한다
                if (++i < data.Length)
                {
                    // 다음줄로 이동시켰지만 이 안에서만 돌기때문에 row가 안쪼개짐
                    row = data[i].Split(new char[] { ',' });
                }
                else
                {
                    break;
                }
            } while (row[0].ToString() == "");

            dialogue.contexts = contextList.ToArray();
            dialogue.spriteName = spriteList.ToArray();
            dialogue.voiceName = voiceList.ToArray();

            // 요소하나하나 대입한후 마지막엔 전역변수로 선언되어있는 list에 통째로추가
            dialogueList.Add(dialogue);
        }
        return dialogueList.ToArray();
    }

}
