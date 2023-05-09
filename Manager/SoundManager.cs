using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;
}
public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    [SerializeField] Sound[] effectSounds;
    [SerializeField] AudioSource[] effectPlayer;

    [SerializeField] Sound[] bgmSounds;
    [SerializeField] AudioSource bgmPlayer;

    [SerializeField] AudioSource voicePlayer;
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }

    private void PlayBGM(string p_name)
    {
        for (int i = 0; i < bgmSounds.Length; i++)
        {
            if(p_name == bgmSounds[i].name)
            {
                bgmPlayer.clip = bgmSounds[i].clip;
                bgmPlayer.Play();
                return;
            }
        }
        Debug.LogError(p_name + "에 해당하는 BGM이 없습니다.");
    }
    private void StopBGM()
    {
        bgmPlayer.Stop();
    }
    private void PauseBGM()
    {
        bgmPlayer.Pause();
    }
    private void UnPauseBGM()
    {
        bgmPlayer.UnPause();
    }
    private void PlayerEffectSound(string p_name)
    {
        for (int i = 0; i < effectSounds.Length; i++)
        {
            if(p_name == effectSounds[i].name)
            {
                for (int j = 0; j < effectPlayer.Length; j++)
                {
                    if(!effectPlayer[j].isPlaying)
                    {
                        effectPlayer[j].clip = effectSounds[i].clip;
                        effectPlayer[j].Play();
                        return;
                    }
                }
                Debug.LogError("모든 효과음 플레이어가 재생중입니다.");
                return;
            }
        }
        Debug.LogError(p_name + "에 해당하는 효과음 사운드가 없습니다.");
    }
    private void StopAllEffectSound()
    {
        for (int i = 0; i < effectPlayer.Length; i++)
            effectPlayer[i].Stop();
    }

    private void PlayVoiceSound(string p_name)
    {
        // ※ Resources 폴더의 해당 타입에 대한 값을 가져올때 이런방식도 씀
        AudioClip _clip = Resources.Load<AudioClip>("Sounds/Voice/" + p_name);
        if(_clip != null)
        {
            voicePlayer.clip = _clip;
            voicePlayer.Play();
        }
        else
            Debug.LogError(p_name + "에 해당하는 보이스 사운드가 없습니다.");
    }
    // 함수실행을 통합적으로 관리하는 함수
    // SoundPlay를 type에 따라서 case분기
    /// <summary>
    /// p_Type : 0 -> 브금 재생
    /// p_Type : 1 -> 효과음 재생
    /// p_Type : 2 -> 보이스 사운드 재생
    /// </summary>
    public void PlaySound(string p_name, int p_Type)
    {
        if (p_Type == 0) PlayBGM(p_name);
        else if (p_Type == 1) PlayerEffectSound(p_name);
        else if (p_Type == 2) PlayVoiceSound(p_name);
    }
}
