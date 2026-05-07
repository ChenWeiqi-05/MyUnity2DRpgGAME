using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using UnityEngine;
using UnityEngine.UI;

public class CloneSkill : Skill, ISaveManager
{
    [Header("Clone info")]
    [SerializeField] private float attackMultiplier;
    [SerializeField] private GameObject clonePrefab;
    [SerializeField] private float cloneDuration;
    [Space]

    [Header("Clone attack")]
    [SerializeField] private UI_SkillTreeSlot cloneAttackUnlockButton;
    [SerializeField] private float cloneAttackMultiplier;
    [SerializeField] private bool canAttack;

    [Header("Aggresive clone")]
    [SerializeField] private UI_SkillTreeSlot aggresiveCloneUnlockButton;
    [SerializeField] private float aggresiveCloneAttackMultiplier;
    public bool canApplyOnHitEffect { get; private set; }

    [Header("Multiple clone")]
    [SerializeField] private UI_SkillTreeSlot multipleUnlockButton;
    [SerializeField] private float multiCloneAttackMultiplier;
    [SerializeField] private bool canDuplicateClone;
    [SerializeField] private float chanceToDuplicate;
    [Header("Crystal instead of clone")]
    [SerializeField] private UI_SkillTreeSlot crystalInseadUnlockButton;
    public bool crystalInseadOfClone;
    protected override void Start()
    {
        base.Start();


        cloneAttackUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockCloneAttack);
        aggresiveCloneUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockAggresiveClone);
        multipleUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockMultiClone);
        crystalInseadUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockCrystalInstead);
    }
    public void CreateClone(Transform _clonePosition, Vector3 _offset)
    {
        if (crystalInseadOfClone)
        {
            SkillManager.instance.crystal.CreateCrystal();
            return;
        }

        GameObject newClone = Instantiate(clonePrefab);

        //Debug.Log($"Playerλ��: {_clonePosition.position}, ��¡��λ��: {newClone.transform.position}");
        newClone.GetComponent<CloneSkillController>().SetupClone(_clonePosition, cloneDuration, canAttack, _offset,  canDuplicateClone, chanceToDuplicate, player, attackMultiplier);//FindClosestEnemy(newClone.transform)
    }

    #region ����������
    protected override void CheckUnlock()
    {
        UnlockCloneAttack();
        UnlockAggresiveClone();
        UnlockMultiClone();
        UnlockCrystalInstead();
    }

    private void UnlockCloneAttack()
    {
        if (cloneAttackUnlockButton.unlocked)
        {
            canAttack = true;
            attackMultiplier = cloneAttackMultiplier;
        }
    }

    private void UnlockAggresiveClone()
    {
        if (aggresiveCloneUnlockButton.unlocked)
        {
            canApplyOnHitEffect = true;
            attackMultiplier = aggresiveCloneAttackMultiplier;
        }
    }

    private void UnlockMultiClone()
    {
        if (multipleUnlockButton.unlocked)
        {
            canDuplicateClone = true;
            attackMultiplier = multiCloneAttackMultiplier;
        }
    }

    private void UnlockCrystalInstead()
    {
        if (crystalInseadUnlockButton.unlocked)
        {
            crystalInseadOfClone = true;
        }
    }


    #endregion
    public void CreateCloneWithDelay(Transform _enemyTransform)
    {
        //if (canCreateCloneOnCounterAttack)
        StartCoroutine(CloneDelayCorotine(_enemyTransform, new Vector3(2 * player.facingDir, 0)));
       // CreateClone(_enemyTransform,new Vector3(2*player.facingDir,0));
    }
    private IEnumerator CloneDelayCorotine(Transform _trasnform, Vector3 _offset)
    {
        yield return new WaitForSeconds(.4f);
        CreateClone(_trasnform, _offset);
    }

    public void LoadData(GameData _data)
    {
        // �Ӵ浵����ÿ�����ܲ۵Ľ���״̬���ò�λ������ΪΨһ����
        if (_data.skillTree.TryGetValue(cloneAttackUnlockButton.name, out bool attackUnlocked))
            cloneAttackUnlockButton.unlocked = attackUnlocked;

        if (_data.skillTree.TryGetValue(aggresiveCloneUnlockButton.name, out bool aggresiveUnlocked))
            aggresiveCloneUnlockButton.unlocked = aggresiveUnlocked;

        if (_data.skillTree.TryGetValue(multipleUnlockButton.name, out bool multiUnlocked))
            multipleUnlockButton.unlocked = multiUnlocked;

        if (_data.skillTree.TryGetValue(crystalInseadUnlockButton.name, out bool crystalUnlocked))
            crystalInseadUnlockButton.unlocked = crystalUnlocked;

        StartCoroutine(DelayCheckUnlock()); // 延迟调用以确保player已设置
    }

    private IEnumerator DelayCheckUnlock()
    {
        yield return null; // 等待一帧
        CheckUnlock(); // 应用解锁效果
    }

    // ���漼�ܽ���״̬
    public void SaveData(ref GameData _data)
    {
        // ��ÿ�����ܲ۵Ľ���״̬����浵
        _data.skillTree[cloneAttackUnlockButton.name] = cloneAttackUnlockButton.unlocked;
        _data.skillTree[aggresiveCloneUnlockButton.name] = aggresiveCloneUnlockButton.unlocked;
        _data.skillTree[multipleUnlockButton.name] = multipleUnlockButton.unlocked;
        _data.skillTree[crystalInseadUnlockButton.name] = crystalInseadUnlockButton.unlocked;
    }

}
