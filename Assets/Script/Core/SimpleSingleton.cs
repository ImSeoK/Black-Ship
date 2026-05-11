using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 오브젝트 이름 기반 DontDestroyOnLoad 싱글톤.
/// 같은 이름의 오브젝트가 중복 생성되면 나중 것을 파괴합니다.
/// </summary>
public class SimpleSingleton : MonoBehaviour
{
    // 씬 전체 탐색(FindObjectsByType) 없이 O(1)로 중복 체크
    private static readonly HashSet<string> registered = new HashSet<string>();

    void Awake()
    {
        if (registered.Contains(gameObject.name))
        {
            Destroy(gameObject);
            return;
        }

        registered.Add(gameObject.name);
        DontDestroyOnLoad(gameObject);
    }

    void OnDestroy()
    {
        registered.Remove(gameObject.name);
    }
}
