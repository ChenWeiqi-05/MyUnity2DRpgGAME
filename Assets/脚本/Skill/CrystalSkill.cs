using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CrystalSkill : Skill, ISaveManager
{
    [SerializeField] private float crystalDuration;
    [SerializeField] private GameObject crystalPrefab;
    private GameObject currentCrystal;
    [Header("Explosive crystal")]
    [SerializeField] private UI_SkillTreeSlot unlockExplosiveButton;
    [SerializeField] private float explisoveCooldown;
    [SerializeField] private bool canExplode;
    [Header("Crystal simple")]
    [SerializeField] private UI_SkillTreeSlot unlockCrystalButton;
    public bool crystalUnlocked { get; private set; }


    [Header("Moving crystal")]
    [SerializeField] private UI_SkillTreeSlot unlockMovingCrystalButton;
    [SerializeField] private bool canMoveToEnemy;
    [SerializeField] private float moveSpeed;

    [Header("Multi stacking crystal")]
    [SerializeField] private UI_SkillTreeSlot unlockMultiStackButton;
    [SerializeField] private bool canUseMultiStacks=false;
    [SerializeField] private int amountOfStacks;
    [SerializeField] private float multiStackCooldown;
    [SerializeField] private float useTimeWondow;
    [SerializeField] private List<GameObject> crystalLeft = new List<GameObject>();
    [Header("Crystal mirage")]
    [SerializeField] private UI_SkillTreeSlot unlockCloneInstaedButton;
    [SerializeField] private bool cloneInsteadOfCrystal;

    protected override void Start()
    {
        base.Start();

        unlockCrystalButton.GetComponent<Button>().onClick.AddListener(UnlockCrystal);
        unlockCloneInstaedButton.GetComponent<Button>().onClick.AddListener(UnlockCrystalMirage);
        unlockExplosiveButton.GetComponent<Button>().onClick.AddListener(UnlockExplosiveCrystal);
        unlockMovingCrystalButton.GetComponent<Button>().onClick.AddListener(UnlockMovingCrystal);
        unlockMultiStackButton.GetComponent<Button>().onClick.AddListener(UnlockMultiStack);

    }


    // ˮ��ϵ���ܽ���
    #region Unlock skill region

    protected override void CheckUnlock()
    {
        UnlockCrystal();
        UnlockCrystalMirage();
        UnlockExplosiveCrystal();
        UnlockMovingCrystal();
        UnlockMultiStack();


    }
    private void UnlockCrystal()
    {
        if (unlockCrystalButton.unlocked)
            crystalUnlocked = true;
    }

    private void UnlockCrystalMirage()
    {
        if (unlockCloneInstaedButton.unlocked)
            cloneInsteadOfCrystal = true;
    }

    private void UnlockExplosiveCrystal()
    {
        if (unlockExplosiveButton.unlocked)
        {
            canExplode = true;
            cooldown = explisoveCooldown;
        }
    }

    private void UnlockMovingCrystal()
    {
        if (unlockMovingCrystalButton.unlocked)
            canMoveToEnemy = true;
    }

    private void UnlockMultiStack()
    {
        if (unlockMultiStackButton.unlocked)
            canUseMultiStacks = true;
    }
    #endregion     // ����������

    public override void UseSkill()
    {
        base.UseSkill();

        if (CanUseMultiCrystal())
            return;

        if (currentCrystal == null)
        {
            //Debug.LogError("ˮ��δ����");
            CreateCrystal();
            
        }
        else
        {
            if (canMoveToEnemy)
                  return;
            else
            {
                //Debug.LogError("��λ");
                Vector2 playerPos = player.transform.position;
                player.transform.position = currentCrystal.transform.position;
                currentCrystal.transform.position = playerPos;

            }


            if (cloneInsteadOfCrystal)
            {
                SkillManager.instance.clone.CreateClone(currentCrystal.transform, Vector3.zero);
                Destroy(currentCrystal);
            }
            else
            {
                currentCrystal.GetComponent<CrystalSkillController>()?.FinishCrystal();
            }
        }

        
    }
    public void CreateCrystal()
    {
        currentCrystal = Instantiate(crystalPrefab, player.transform.position, Quaternion.identity);
        CrystalSkillController currentCystalScript = currentCrystal.GetComponent<CrystalSkillController>();


        currentCystalScript.SetupCrystal(crystalDuration, canExplode, canMoveToEnemy, moveSpeed, FindClosestEnemy(currentCrystal.transform), player);
    }
    public void CurrentCrystalChooseRandomTarget() => currentCrystal.GetComponent<CrystalSkillController>().ChooseRandomEnemy();

    private bool CanUseMultiCrystal()
    {
        if (canUseMultiStacks)
        {
            if (crystalLeft.Count > 0)
            {
                if (crystalLeft.Count == amountOfStacks)
                    Invoke("ResetAbility", useTimeWondow);

                cooldown = 0;
                GameObject crystalToSpawn = crystalLeft[crystalLeft.Count - 1];
                GameObject newCrystal = Instantiate(crystalToSpawn, player.transform.position, Quaternion.identity);

                crystalLeft.Remove(crystalToSpawn);

                newCrystal.GetComponent<CrystalSkillController>().
                    SetupCrystal(crystalDuration, canExplode, canMoveToEnemy, moveSpeed, FindClosestEnemy(newCrystal.transform), player);

                if (crystalLeft.Count <= 0)
                {
                    cooldown = multiStackCooldown;
                    RefilCrystal();
                }


                return true;

            }
        }


        return false;
    }
    private void RefilCrystal()
    {
        int amountToAdd = amountOfStacks - crystalLeft.Count;
        for (int i = 0; i < amountToAdd; i++)
        {
            crystalLeft.Add(crystalPrefab);
        }
    }
    private void ResetAbility()
    {
        if (cooldownTimer > 0)
            return;

        cooldownTimer = multiStackCooldown;

        RefilCrystal();
    }

    // ���������ؼ��ܽ���״̬
    public void LoadData(GameData _data)
    {
        if (_data.skillTree.TryGetValue(unlockExplosiveButton.name, out bool explosiveUnlocked))
            unlockExplosiveButton.unlocked = explosiveUnlocked;

        if (_data.skillTree.TryGetValue(unlockCrystalButton.name, out bool crystalUnlocked))
            unlockCrystalButton.unlocked = crystalUnlocked;

        if (_data.skillTree.TryGetValue(unlockMovingCrystalButton.name, out bool movingUnlocked))
            unlockMovingCrystalButton.unlocked = movingUnlocked;

        if (_data.skillTree.TryGetValue(unlockMultiStackButton.name, out bool multiStackUnlocked))
            unlockMultiStackButton.unlocked = multiStackUnlocked;

        if (_data.skillTree.TryGetValue(unlockCloneInstaedButton.name, out bool cloneUnlocked))
            unlockCloneInstaedButton.unlocked = cloneUnlocked;

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
        _data.skillTree[unlockExplosiveButton.name] = unlockExplosiveButton.unlocked;
        _data.skillTree[unlockCrystalButton.name] = unlockCrystalButton.unlocked;
        _data.skillTree[unlockMovingCrystalButton.name] = unlockMovingCrystalButton.unlocked;
        _data.skillTree[unlockMultiStackButton.name] = unlockMultiStackButton.unlocked;
        _data.skillTree[unlockCloneInstaedButton.name] = unlockCloneInstaedButton.unlocked;
    }
}
