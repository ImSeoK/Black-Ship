using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class AnimationEventData
{
    public string eventName;
    public UnityEvent onEvent;
}

public class AnimationEventHandler : MonoBehaviour
{
    [Header("애니메이션 이벤트 목록")]
    public AnimationEventData[] events;

    public void TriggerEvent(string eventName)
    {
        foreach (AnimationEventData data in events)
        {
            if (data.eventName == eventName)
            {
                data.onEvent?.Invoke();
                return;
            }
        }

        Debug.LogWarning($"이벤트 '{eventName}'을 찾을 수 없습니다.");
    }
}