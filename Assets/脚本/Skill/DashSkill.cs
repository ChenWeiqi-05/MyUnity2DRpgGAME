using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class DashSkill : Skill,ISaveManager
{
    [Header("Dash")]
    [SerializeField] private UI_SkillTreeSlot dashUnlockButton;
    public bool dashUnlocked{ get; private set; }

    [Header("Clone on dash")]
    [SerializeField] private UI_SkillTreeSlot cloneOnDashUnlockButton;
    public bool cloneOnDashUnlocked{ get; private set; }

    [Header("Clone on arrival")]
    [SerializeField] private UI_SkillTreeSlot cloneOnArrivalUnlockButton;
    public bool cloneOnArrivalUnlocked{ get; private set; }
    public override void UseSkill()
    {
        base.UseSkill();
        


    }
    protected override void Start()
    {
        base.Start();

        dashUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockDash);
        cloneOnDashUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockCloneOnDash);
        cloneOnArrivalUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockCloneOnArrival);
    }

    protected override void CheckUnlock()
    {
        UnlockDash();
        UnlockCloneOnDash();
        UnlockCloneOnArrival();
    }

    private void UnlockDash()
    {
        if (dashUnlockButton.unlocked)
            dashUnlocked = true;
    }

    private void UnlockCloneOnDash()
    {
        if (cloneOnDashUnlockButton.unlocked)
            cloneOnDashUnlocked = true;
    }

    private void UnlockCloneOnArrival()
    {
        if (cloneOnArrivalUnlockButton.unlocked)
            cloneOnArrivalUnlocked = true;
    }


    public void CloneOnDash()
    {
        if (cloneOnDashUnlocked)
       SkillManager.instance.clone.CreateClone(player.transform, Vector3.zero);
    }
    public void CloneOnArrival()
    {
        if (cloneOnArrivalUnlocked)
       SkillManager.instance.clone.CreateClone(player.transform, Vector3.zero);
    
    }

    // ���������ؼ��ܽ���״̬
    public void LoadData(GameData _data)
    {
        if (_data.skillTree.TryGetValue(dashUnlockButton.name, out bool dashUnlocked))
            dashUnlockButton.unlocked = dashUnlocked;

        if (_data.skillTree.TryGetValue(cloneOnDashUnlockButton.name, out bool cloneDashUnlocked))
            cloneOnDashUnlockButton.unlocked = cloneDashUnlocked;

        if (_data.skillTree.TryGetValue(cloneOnArrivalUnlockButton.name, out bool cloneArrivalUnlocked))
            cloneOnArrivalUnlockButton.unlocked = cloneArrivalUnlocked;

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
        _data.skillTree[dashUnlockButton.name] = dashUnlockButton.unlocked;
        _data.skillTree[cloneOnDashUnlockButton.name] = cloneOnDashUnlockButton.unlocked;
        _data.skillTree[cloneOnArrivalUnlockButton.name] = cloneOnArrivalUnlockButton.unlocked;
    }
}
