using UnityEngine;

public class CutsceneController : MonoBehaviour
{
    public void ShowDialogue1()
    {
        if (DialogueUI.Instance != null)
        {
            DialogueUI.Instance.ShowDialogue("주인공", new string[] {
                "뭐지 이 거대한 수류탄은",
                "?!",
                "호흡이 느껴진다",
                "빨리 쉘터로 데려가야겠어."
            });
        }
    }

    public void ShowDialogue2()
    {
        if (DialogueUI.Instance != null)
        {
            DialogueUI.Instance.ShowDialogue("주인공", new string[] {
                "읏차"
            });
        }
    }

    public void EndCutscene()
    {
        if (StatsManager.Instance != null)
        {
            StatsManager.Instance.PickUpBaby(false); // false = 들고 다니지 않음
        }

        StartCoroutine(EndCutsceneWithFade());
    }

    System.Collections.IEnumerator EndCutsceneWithFade()
    {
        // 대사 끝날 때까지 대기
        yield return new WaitUntil(() => !DialogueUI.Instance.IsDialogueActive());

        // 잠깐 대기
        yield return new WaitForSecondsRealtime(0.5f);

        // 시간 복원
        Time.timeScale = 1f;

        // BaseCamp로 이동 (LoadScene 안에 Fade 있음)
        if (LoadingManager.Instance != null)
        {
            LoadingManager.Instance.LoadScene("BaseCamp", "DefaultSpawn");
        }
    }
}