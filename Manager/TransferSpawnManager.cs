using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Location 
{
    public string name;
    public Transform tf_Spawn;
}

public class TransferSpawnManager : MonoBehaviour
{
    [SerializeField] Location[] locations;
    Dictionary<string, Transform> locationDic = new Dictionary<string, Transform>();

    public static bool spawnTiming = false;

    // ※ 약간 이런느낌 dictionary를 location의 사이즈만큼 찾아줌
    void Start()
    {
        for (int i = 0; i < locations.Length; i++)
        {
            locationDic.Add(locations[i].name, locations[i].tf_Spawn);
        }

        if (spawnTiming)
        {
            TransferManager theTM = FindObjectOfType<TransferManager>();
            // ※ 일단 임시변수에 값저장하는거 많이함
            // 키값을 가져왔음 첫씬의 door 에 있는 속성값
            string t_LocationName = theTM.GetLocationName();
            Transform t_Spawn = locationDic[t_LocationName];
            PlayerController.instance.transform.position = t_Spawn.position;
            PlayerController.instance.transform.rotation = t_Spawn.rotation;
            // 모르겠다 난 그냥 다 때려박을듯
            Camera.main.transform.localPosition = new Vector3(0, 1, 0);
            Camera.main.transform.localEulerAngles = Vector3.zero;
            PlayerController.instance.Reset();

            StartCoroutine(theTM.Done());
            spawnTiming = false;
        }
    }


}
