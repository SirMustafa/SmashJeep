using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillsUi : MonoBehaviour
{
    public static SkillsUi Instance;

    [Header("Skill References")]
    [SerializeField] private Image _skillImg;
    [SerializeField] private TextMeshProUGUI _skillNameTxt;
    [SerializeField] private TextMeshProUGUI _timerTxt;
    [SerializeField] private Transform _timerParentTransform;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        SetSkillToNone();
    }

    public void SetSkill(string skillName, Sprite skillSprite)
    {
        _skillImg.gameObject.SetActive(true);
        _skillNameTxt.text = skillName;
        _skillImg.sprite = skillSprite;
    }

    public void SetSkillToNone()
    {
        _skillImg.gameObject.SetActive(false);
        _skillNameTxt.text = string.Empty;
    }
}