using Unity.Netcode;
using UnityEngine;

public class PlayerSkillController : NetworkBehaviour
{
    [SerializeField] bool _hasSkilled;

    private MysteryBoxSkillSO _mysteryBoxSkill;
    private bool _isSkillUsed;

    private void Update()
    {
        if (!IsOwner) return;

        if (Input.GetKeyDown(KeyCode.Space) && !_isSkillUsed)
        {
            ActivateSkill();
            _isSkillUsed = true;
        }
    }

    public void SetSkillSetup(MysteryBoxSkillSO skill)
    {
        _mysteryBoxSkill = skill;
        _hasSkilled = true;
        _isSkillUsed = false;
    }

    public void ActivateSkill()
    {
        if (!HasSkill()) return;

        SkillsUi.Instance.SetSkillToNone();
        _hasSkilled = false;
        Debug.Log("Skill Used: " + _mysteryBoxSkill.SkillType);
    }

    public bool HasSkill()
    {
        return _hasSkilled;
    }
}