using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class DodgeSkill : Skill, ISaveManager
{
    [Header("Dodge")]
    [SerializeField] private UI_SkillTreeSlot unlockDodgeButton;
    [SerializeField] private int evasionAmount;
    public bool dodgeUnlocked;

    [Header("Mirage dodge")]
    [SerializeField] private UI_SkillTreeSlot unlockMirageDodge;
    public bool dodgeMirageUnlocked;


    protected override void Start()
    {
        base.Start();

        unlockDodgeButton.GetComponent<Button>().onClick.AddListener(UnlockDodge);
        unlockMirageDodge.GetComponent<Button>().onClick.AddListener(UnlockMirageDodge);
    }

    protected override void CheckUnlock()
    {
        UnlockDodge();
        UnlockMirageDodge();
    }

    private void UnlockDodge()
    {
        if (unlockDodgeButton.unlocked && !dodgeUnlocked)
        {
            player.stats.evasion.AddModifier(evasionAmount);
            Inventory.instance.UpdateStatsUI();
            dodgeUnlocked = true;
            
        }
    }

    private void UnlockMirageDodge()
    {
        if (unlockMirageDodge.unlocked)
            dodgeMirageUnlocked = true;
    }

    public void CreateMirageOnDodge()
    {
        if (dodgeMirageUnlocked)
            SkillManager.instance.clone.CreateClone(player.transform, new Vector3(2 * player.facingDir,0));
    }

    // ���������ؼ��ܽ���״̬
    public void LoadData(GameData _data)
    {
        if (_data.skillTree.TryGetValue(unlockDodgeButton.name, out bool dodgeUnlocked))
            unlockDodgeButton.unlocked = dodgeUnlocked;

        if (_data.skillTree.TryGetValue(unlockMirageDodge.name, out bool mirageUnlocked))
            unlockMirageDodge.unlocked = mirageUnlocked;

        StartCoroutine(DelayCheckUnlock()); // 延迟调用以确保player已设置
    }

    private IEnumerator DelayCheckUnlock()
    {
        yield return null; // 等待一帧
        CheckUnlock(); // 应用解锁效果
    }

    // ���������漼�ܽ���״̬
    public void SaveData(ref GameData _data)
    {
        _data.skillTree[unlockDodgeButton.name] = unlockDodgeButton.unlocked;
        _data.skillTree[unlockMirageDodge.name] = unlockMirageDodge.unlocked;
    }
}
