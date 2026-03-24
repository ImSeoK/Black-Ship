using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class VestUI : MonoBehaviour
{
    [Header("파우치 버튼")]
    public Button pouchMag;
    public Button pouchMed;
    public Button pouchUtilL;
    public Button pouchUtilR;
    public Button pouchGrenade;
    public Button pouchHolster;
    public Button pouchBack;

    [Header("베스트오버레이")]
    public Image vestOverlay;

    [Header("참조")]
    public BelongingUI belongingUI;

    [Header("하이라이트")]
    public Color selectedTint = new Color(1f, 0.85f, 0.4f, 1f); // Inspector에서 조절
    private Color normalTint = Color.white;

    private Button currentSelected;
    private Dictionary<Button, string> pouchMap;
    private Dictionary<Button, Color> originalColors = new Dictionary<Button, Color>();

    void Awake()
    {
        pouchMap = new Dictionary<Button, string>
        {
            { pouchMag,     "mag"     },
            { pouchMed,     "med"     },
            { pouchUtilL,   "util_l"  },
            { pouchUtilR,   "util_r"  },
            { pouchGrenade, "grenade" },
            { pouchHolster, "holster" },
            { pouchBack,    "back"    },
        };

        foreach (var pair in pouchMap)
        {
            Image img = pair.Key.GetComponentInChildren<Image>();
            if (img != null)
                originalColors[pair.Key] = img.color;
        }

        foreach (var pair in pouchMap)
        {
            Button btn = pair.Key;
            string id = pair.Value;
            btn.onClick.AddListener(() => OnPouchClicked(btn, id));
        }

        // 오버레이 초기화
        if (vestOverlay != null)
            vestOverlay.fillAmount = 0f;
    }

    void Start()
    {
        OnPouchClicked(pouchMag, "mag");
    }

    void OnPouchClicked(Button btn, string pouchID)
    {
        if (currentSelected != null)
            SetHighlight(currentSelected, false);

        currentSelected = btn;
        SetHighlight(btn, true);

        belongingUI.ShowPouch(pouchID);
    }

    void SetHighlight(Button btn, bool on)
    {
        Image img = btn.GetComponentInChildren<Image>();
        if (img == null) return;

        img.color = on ? selectedTint : normalTint;
    }

    public void UpdateOverlay()
    {
        if (InventoryData.Instance == null) return;
        vestOverlay.fillAmount = InventoryData.Instance.GetWeightRatio();
    }
}