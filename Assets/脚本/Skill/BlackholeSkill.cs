using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlackholeSkill : Skill,ISaveManager
{
    [SerializeField] private UI_SkillTreeSlot blackHoleUnlockButton;
    public bool blackholeUnlocked;// { get; private set; }
    [SerializeField] private int amountOfAttacks;
   [SerializeField] private float cloneCooldown;
   [SerializeField] private float blackholeDuration;
   [Space]
   [SerializeField] private GameObject blackHolePrefab;
   [SerializeField] private float maxSize;
   [SerializeField] private float growSpeed;
   [SerializeField] private float shrinkSpeed;
    BlackHoleController currentBlackhole;

    protected override void Start()
    {
        base.Start();

        blackHoleUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockBlackhole);
    }



    private void UnlockBlackhole()
    {
        if (blackHoleUnlockButton.unlocked)
            blackholeUnlocked = true;

    }
    public override bool CanUseSkill()
    {
        return base.CanUseSkill();
    }

    public override void UseSkill()
    {
        base.UseSkill();
        GameObject newBlackHole = Instantiate(blackHolePrefab, player.transform.position, Quaternion.identity);
        currentBlackhole = newBlackHole.GetComponent<BlackHoleController>();
        currentBlackhole.SetupBlackhole(maxSize, growSpeed, shrinkSpeed, amountOfAttacks, cloneCooldown, blackholeDuration);


        AudioManager.instance.PlaySFX(3, player.transform);
        AudioManager.instance.PlaySFX(6, player.transform);
    }

   

    protected override void Update()
    {
        base.Update();
    }

    public bool SkillCompleted()
    {
        if (!currentBlackhole)
            return false;


        if (currentBlackhole.playerCanExitState)
        {
            currentBlackhole = null;
            return true;
        }


        return false;
    }
    public float GetBlackholeRadius()
    {
        return maxSize / 2;
    }
    protected override void CheckUnlock()
    {
        base.CheckUnlock();
        UnlockBlackhole();
    }

    // ���������ؼ��ܽ���״̬
    public void LoadData(GameData _data)
    {
        if (_data.skillTree.TryGetValue(blackHoleUnlockButton.name, out bool blackholeUnlocked))
            blackHoleUnlockButton.unlocked = blackholeUnlocked;

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
        _data.skillTree[blackHoleUnlockButton.name] = blackHoleUnlockButton.unlocked;
    }
}
