using UnityEngine;

public enum PrologueTriggerActionType
{
    ShowRadioMessage,   // 무전 수신 메시지 표시
    ShowTutorialHint,   // 튜토리얼 힌트 텍스트 변경
    HideTutorialHint,   // 튜토리얼 힌트 숨김
    CompleteTransfer,   // 이송 구간 완료 신호 → 다음 컷씬으로 진행
    PlayBGM,            // BGM 변경
    FadeOut,
}

[System.Serializable]
public class PrologueTriggerAction
{
    public PrologueTriggerActionType actionType;

    [Tooltip("ShowRadioMessage: 무전 발신자 이름 (예: 지원팀, 본부)")]
    public string callerName;

    [Tooltip("ShowRadioMessage / ShowTutorialHint: 표시할 텍스트")]
    [TextArea(2, 4)]
    public string messageText;

    [Tooltip("ShowRadioMessage: false면 자동 숨김 안 함")]
    public bool autoHide = true;

    [Tooltip("PlayBGM: 재생할 오디오 클립")]
    public AudioClip bgmClip;
}

/// <summary>
/// 플레이어가 콜라이더에 진입하면 프롤로그 이벤트를 발동합니다.
/// 3구간(이송 구간) 경로 곳곳에 배치하여 무전·힌트·구간 완료 등을 제어하세요.
/// </summary>
public class PrologueTriggerZone : MonoBehaviour
{
    [Header("트리거 설정")]
    [Tooltip("진입 시 순서대로 실행할 액션 목록")]
    public PrologueTriggerAction[] actions;

    [Tooltip("true면 한 번 발동 후 재발동 안 함 (권장)")]
    public bool triggerOnce = true;

    [Header("에디터 시각화")]
    public Color gizmoColor = new Color(0f, 1f, 0.5f, 0.3f);

    private bool hasTriggered = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (triggerOnce && hasTriggered) return;
        if (!other.CompareTag("Player")) return;

        hasTriggered = true;
        ExecuteActions();
    }

    void ExecuteActions()
    {
        if (actions == null || actions.Length == 0) return;

        foreach (var action in actions)
        {
            switch (action.actionType)
            {
                case PrologueTriggerActionType.ShowRadioMessage:
                    RadioTutorialUI.Instance?.ShowRadioMessage(
                        action.callerName,
                        action.messageText,
                        action.autoHide);
                    break;

                case PrologueTriggerActionType.ShowTutorialHint:
                    RadioTutorialUI.Instance?.ShowTutorialHint(action.messageText);
                    break;

                case PrologueTriggerActionType.HideTutorialHint:
                    RadioTutorialUI.Instance?.HideTutorialHint();
                    break;

                case PrologueTriggerActionType.CompleteTransfer:
                    PrologueManager.Instance?.CompleteTransferSection();
                    break;

                case PrologueTriggerActionType.PlayBGM:
                    if (action.bgmClip != null)
                        AudioManager.Instance?.PlayBGM(action.bgmClip, true);
                    break;

                case PrologueTriggerActionType.FadeOut:
                    ScreenFadeUI.Instance?.FadeOut();
                    break;
            }
        }
    }

    // ================================================================
    // 에디터 시각화
    // ================================================================

    void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;
        Gizmos.matrix = transform.localToWorldMatrix;

        Collider2D col = GetComponent<Collider2D>();
        if (col is BoxCollider2D box)
        {
            Gizmos.DrawCube(box.offset, box.size);
            Gizmos.color = new Color(gizmoColor.r, gizmoColor.g, gizmoColor.b, 1f);
            Gizmos.DrawWireCube(box.offset, box.size);
        }
        else if (col is CircleCollider2D circle)
        {
            // 원형 콜라이더일 경우 구체로 시각화
            Gizmos.DrawSphere(circle.offset, circle.radius);
        }
    }
}
