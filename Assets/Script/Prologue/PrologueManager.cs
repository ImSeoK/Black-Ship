using UnityEngine;
using UnityEngine.Playables;
using System.Collections;

/// <summary>
/// 프롤로그 씬 전체 흐름을 순서대로 제어합니다.
/// 컷씬 재생 → 직접 조작 구간 → 컷씬 → BaseCamp 전환까지 담당.
/// </summary>
public class PrologueManager : MonoBehaviour
{
    public static PrologueManager Instance;

    [Header("1구간 - 암실 의뢰 접촉 (컷씬 + 대화)")]
    public PlayableDirector darkRoomDirector;
    public CutsceneData darkRoomData;

    [Header("2구간 - 약소국 쉘터 외부 인계 (컷씬 + 대화)")]
    public PlayableDirector shelterDirector;
    public CutsceneData shelterData;

    [Header("4구간 - 병력 습격 (컷씬)")]
    public PlayableDirector ambushDirector;
    public CutsceneData ambushData;

    [Header("5구간 - 능력 발현 + UTO 타이틀 (컷씬)")]
    public PlayableDirector abilityRevealDirector;
    public CutsceneData abilityRevealData;

    [Header("6구간 - 현재 복귀 (컷씬 + 대화)")]
    public PlayableDirector returnDirector;
    public CutsceneData returnData;

    [Header("플레이어 오브젝트 (3구간부터 활성화)")]
    [Tooltip("씬에 배치된 플레이어 오브젝트. 컷씬 구간에서는 비활성화 상태로 시작합니다.")]
    public GameObject playerObject;

    [Header("씬 전환 (프롤로그 완료 후)")]
    public string nextSceneName = "BaseCamp";
    public string nextSpawnPoint = "PrologueReturn";

    [Header("개발용 - 특정 구간부터 시작")]
    public bool skipToSection = false;
    [Range(1, 6)]
    public int skipTargetSection = 3;

    // 3구간 완료 플래그 (PrologueTriggerZone에서 set)
    private bool transferSectionDone = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        // 플레이어는 처음에 비활성화 (1·2구간은 순수 컷씬)
        if (playerObject != null)
            playerObject.SetActive(false);

        if (skipToSection)
            StartCoroutine(RunFromSection(skipTargetSection));
        else
            StartCoroutine(RunPrologue());
    }

    // ================================================================
    // 메인 시퀀스
    // ================================================================

    IEnumerator RunPrologue()
    {
        yield return Section1_DarkRoom();
        yield return Section2_ShelterHandover();
        yield return Section3_TransferRoute();
        yield return Section4_MilitaryAmbush();
        yield return Section5_AbilityReveal();
        yield return Section6_Return();
        yield return LoadNextScene();
    }

    /// <summary>개발용: 지정 구간부터 실행</summary>
    IEnumerator RunFromSection(int section)
    {
        // 플레이어가 필요한 구간이면 미리 활성화
        if (section >= 3 && playerObject != null)
            playerObject.SetActive(true);

        if (section <= 1) yield return Section1_DarkRoom();
        if (section <= 2) yield return Section2_ShelterHandover();
        if (section <= 3) yield return Section3_TransferRoute();
        if (section <= 4) yield return Section4_MilitaryAmbush();
        if (section <= 5) yield return Section5_AbilityReveal();
        if (section <= 6) yield return Section6_Return();
        yield return LoadNextScene();
    }

    // ================================================================
    // 구간별 코루틴
    // ================================================================

    IEnumerator Section1_DarkRoom()
    {
        Debug.Log("[Prologue] 1구간: 암실 의뢰 접촉");
        yield return PlayCutscene(darkRoomDirector, darkRoomData);
    }

    IEnumerator Section2_ShelterHandover()
    {
        Debug.Log("[Prologue] 2구간: 약소국 쉘터 외부 인계");
        yield return PlayCutscene(shelterDirector, shelterData);
    }

    IEnumerator Section3_TransferRoute()
    {
        Debug.Log("[Prologue] 3구간: 이송 구간 시작");

        // 플레이어 활성화
        if (playerObject != null)
            playerObject.SetActive(true);

        // 초기 튜토리얼 힌트 표시
        RadioTutorialUI.Instance?.ShowInitialHints();

        // PrologueTriggerZone의 CompleteTransfer 액션이 호출될 때까지 대기
        transferSectionDone = false;
        yield return new WaitUntil(() => transferSectionDone);

        RadioTutorialUI.Instance?.HideAll();

        Debug.Log("[Prologue] 3구간: 완료");
    }

    IEnumerator Section4_MilitaryAmbush()
    {
        Debug.Log("[Prologue] 4구간: 병력 습격");
        // CutsceneManager가 플레이어 컴포넌트를 자동으로 비활성화/복원
        yield return PlayCutscene(ambushDirector, ambushData);
    }

    IEnumerator Section5_AbilityReveal()
    {
        Debug.Log("[Prologue] 5구간: 능력 발현 + UTO 타이틀");
        yield return PlayCutscene(abilityRevealDirector, abilityRevealData);

        // Timeline 종료 후 UTO 타이틀 로고 연출
        if (UTOTitleUI.Instance != null)
        {
            bool titleDone = false;
            UTOTitleUI.Instance.PlayTitleReveal(() => titleDone = true);
            yield return new WaitUntil(() => titleDone);
        }
    }

    IEnumerator Section6_Return()
    {
        Debug.Log("[Prologue] 6구간: 현재 복귀");
        yield return PlayCutscene(returnDirector, returnData);
        // 컷씬 종료 후 CutsceneManager가 플레이어 컴포넌트를 자동 복원
    }

    IEnumerator LoadNextScene()
    {
        Debug.Log($"[Prologue] 완료 → {nextSceneName} 전환");

        if (LoadingManager.Instance != null)
        {
            LoadingManager.Instance.LoadScene(nextSceneName, nextSpawnPoint);
        }
        else
        {
            Debug.LogWarning("[PrologueManager] LoadingManager 없음 → 직접 씬 로드");
            UnityEngine.SceneManagement.SceneManager.LoadScene(nextSceneName);
        }

        yield break;
    }

    // ================================================================
    // 유틸리티
    // ================================================================

    /// <summary>컷씬 재생 후 완료까지 대기</summary>
    IEnumerator PlayCutscene(PlayableDirector director, CutsceneData data)
    {
        if (director == null || data == null)
        {
            Debug.LogWarning("[PrologueManager] Director 또는 CutsceneData 미할당 → 구간 스킵");
            yield break;
        }

        if (CutsceneManager.Instance == null)
        {
            Debug.LogWarning("[PrologueManager] CutsceneManager 없음 → 구간 스킵");
            yield break;
        }

        bool done = false;
        CutsceneManager.Instance.Play(director, data, () => done = true);
        yield return new WaitUntil(() => done);
    }

    /// <summary>
    /// PrologueTriggerZone의 CompleteTransfer 액션에서 호출.
    /// 3구간 이송 구간을 완료 처리합니다.
    /// </summary>
    public void CompleteTransferSection()
    {
        Debug.Log("[PrologueManager] 이송 구간 완료 신호 수신");
        transferSectionDone = true;
    }
}
