using Cysharp.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

public class MysteryBoxCollect : NetworkBehaviour, ICollectable
{
    [Header("References")]
    [SerializeField] MysteryBoxSkillSO[] _mysteryBoxSkills;
    [SerializeField] Animator _animator;
    [SerializeField] BoxCollider _collider;

    [Header("Settings")]
    [SerializeField] float _respawnTimer;

    public void Collect(PlayerSkillController playerSkillController)
    {
        if (playerSkillController.HasSkill()) return;

        MysteryBoxSkillSO skill = GetRandomSkill();
        SkillsUi.Instance.SetSkill(skill.SkillName, skill.SkillIcon);
        playerSkillController.SetSkillSetup(skill);

        CollectRpc();
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void CollectRpc()
    {
        AnimateCollection();
        ReSpawn().Forget();
    }

    private void AnimateCollection()
    {
        _collider.enabled = false;
        _animator.SetTrigger(Consts.BoxAnimations.IS_COLLECTED);
    }

    private async UniTaskVoid ReSpawn()
    {
        await UniTask.Delay((int)(_respawnTimer * 1000));
        _collider.enabled = true;
        _animator.SetTrigger(Consts.BoxAnimations.IS_RESPAWNED);
    }

    private MysteryBoxSkillSO GetRandomSkill()
    {
        int index = Random.Range(0, _mysteryBoxSkills.Length);
        return _mysteryBoxSkills[index];
    }
}