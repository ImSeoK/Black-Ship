using UnityEngine;

public class SimpleSingleton : MonoBehaviour
{
    void Awake()
    {
        // 같은 이름의 오브젝트가 이미 있는지 확인
        string myName = gameObject.name;
        GameObject[] allObjects = FindObjectsByType<GameObject>(FindObjectsSortMode.None);

        int count = 0;
        foreach (GameObject obj in allObjects)
        {
            // DontDestroyOnLoad 영역이거나 현재 씬에 같은 이름
            if (obj.name == myName)
            {
                count++;
                if (count > 1)
                {
                    // 중복 발견 - 나중에 생성된 것(this) 삭제
                    Destroy(gameObject);
                    return;
                }
            }
        }

        // 중복 없으면 유지
        DontDestroyOnLoad(gameObject);
    }
}