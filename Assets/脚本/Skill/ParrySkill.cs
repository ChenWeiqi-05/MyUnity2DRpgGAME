using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class ParrySkill : Skill,ISaveManager
{

    [Header("Parry")]
    [SerializeField] private UI_SkillTreeSlot parryUnlockButton;
    public bool parryUnlocked{ get; private set; }

    [Header("Parry restore")]
    [SerializeField] private UI_SkillTreeSlot restoreUnlockButton;
    [Range(0f, 1f)]
    [SerializeField] private float restoreHealthPerentage;
    public bool restoreUnlocked{ get; private set; }

    [Header("Parry with mirage")]
    [SerializeField] private UI_SkillTreeSlot parryWithMirageUnlockButton;
    public bool parryWithMirageUnlocked{ get; private set; }

    public override void UseSkill()
    {
        base.UseSkill();


        if (restoreUnlocked)
        {
            int restoreAmount = Mathf.RoundToInt(player.stats.GetMaxHealthValue() * restoreHealthPerentage);
            player.stats.IncreaseHealthBy(restoreAmount);
        }

    }

    protected override void Start()
    {
        base.Start();

        parryUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockParry);
        restoreUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockParryRestore);
        parryWithMirageUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockParryWithMirage);
    }

    protected override void CheckUnlock()
    {
        UnlockParry();
        UnlockParryRestore();
        UnlockParryWithMirage();
    }
    private void UnlockParry()
    {
        if (parryUnlockButton.unlocked)
            parryUnlocked = true;
    }

    private void UnlockParryRestore()
    {
        if (restoreUnlockButton.unlocked)
            restoreUnlocked = true;
    }

    private void UnlockParryWithMirage()
    {
        if (parryWithMirageUnlockButton.unlocked)
            parryWithMirageUnlocked = true;
    }

    public void MakeMirageOnParry(Transform _respawnTransform)
    {
        if (parryWithMirageUnlocked)
            SkillManager.instance.clone.CreateCloneWithDelay(_respawnTransform);
    }

    public void LoadData(GameData _data)
    {
        if (_data.skillTree.TryGetValue(parryUnlockButton.name, out bool parryUnlocked))
            parryUnlockButton.unlocked = parryUnlocked;

        if (_data.skillTree.TryGetValue(restoreUnlockButton.name, out bool restoreUnlocked))
            restoreUnlockButton.unlocked = restoreUnlocked;

        if (_data.skillTree.TryGetValue(parryWithMirageUnlockButton.name, out bool mirageUnlocked))
            parryWithMirageUnlockButton.unlocked = mirageUnlocked;

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
        _data.skillTree[parryUnlockButton.name] = parryUnlockButton.unlocked;
        _data.skillTree[restoreUnlockButton.name] = restoreUnlockButton.unlocked;
        _data.skillTree[parryWithMirageUnlockButton.name] = parryWithMirageUnlockButton.unlocked;
    }
}
