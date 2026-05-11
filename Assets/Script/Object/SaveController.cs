using System.Collections;
using UnityEngine;

public class SaveController : InteractableObject
{

    [Header("세이브 포인트 ID")]
    public string radioID;

    private const string LastSavedKey = "LastSavedRadioID";
    private bool saved = false;
    private Animator anim;

    [Header("개발용")]
    public bool resetOnStart = false;


    protected override void Start()
    {
        if (resetOnStart)
            PlayerPrefs.DeleteKey(LastSavedKey);
        base.Start();
        anim = GetComponent<Animator>();

        if (PlayerPrefs.GetString(LastSavedKey, "") == radioID)
        {
            saved = true;
            interactable = false;
            anim?.SetBool("IsActivated", true);
        }
    }

    public override void Interact()
    {
        if (saved) return;
        saved = true;
        interactable = false;
        anim?.SetBool("IsActivated", true);
        Save();
    }

    public void Save()
    {
        // 다른 라디오 전부 초기화
        foreach (var radio in FindObjectsByType<SaveController>(FindObjectsSortMode.None))
        {
            Debug.Log($"radio: {radio.radioID}, this: {this.radioID}, same: {radio == this}");
            if (radio != this)
            {
                radio.saved = false;
                radio.interactable = true;
                if (radio.anim != null)
                {
                    radio.anim.SetBool("IsActivated", false);
                    radio.anim.Play("Entry", 0, 0f); // Entry로 강제 리셋
                }
            }
        }

        PlayerPrefs.SetString(LastSavedKey, radioID);

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            PlayerPrefs.SetFloat("SavePosX", player.transform.position.x);
            PlayerPrefs.SetFloat("SavePosY", player.transform.position.y);
        }

        PlayerPrefs.SetString("SaveScene", UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        PlayerStats.Instance?.SaveState();
        PlayerPrefs.Save();
        Debug.Log("[SaveController] 저장 완료");
    }

    public void Load()
    {
        PlayerStats.Instance?.LoadState();
    }
}