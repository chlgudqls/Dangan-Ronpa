using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SlideManager : MonoBehaviour
{
    [SerializeField] Image img_SlodeCG;
    [SerializeField] Animation anim;    

    // 실행되는동안 텍스트의 출력을 막는용도
    public static bool isFinished = true;
    public static bool isChanged = false;

    public IEnumerator AppearSlide(string p_SlideName)
    {
        Sprite t_sprite = Resources.Load<Sprite>("Slide_Image/" + p_SlideName);
        if (t_sprite != null)
        {
            img_SlodeCG.gameObject.SetActive(true);
            img_SlodeCG.sprite = t_sprite;
            anim.Play("Appear");
        }
        else
        {
            Debug.LogError(p_SlideName + "에 해당하는 이미지 파일이 없습니다.");
        }

        yield return new WaitForSeconds(0.5f);

        isFinished = true;
    }
    public IEnumerator DisappearSlide()
    {
        anim.Play("Disappear");
        yield return new WaitForSeconds(0.5f);
        img_SlodeCG.gameObject.SetActive(false);

        isFinished = true;
    }

    // 이게 아마 메인
    public IEnumerator ChangeSlide(string p_SlideName)
    {
        isFinished = false;
        StartCoroutine(DisappearSlide());
        yield return new WaitUntil(() => isFinished);

        isFinished = false;
        StartCoroutine(AppearSlide(p_SlideName));
        yield return new WaitUntil(() => isFinished);

        isChanged = true;
    }
}
