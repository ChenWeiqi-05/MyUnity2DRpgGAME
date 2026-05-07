$word = New-Object -ComObject Word.Application
$word.Visible = $false
$doc = $word.Documents.Add()

$sel = $word.Selection

# Title
$sel.Font.Name = 'Microsoft YaHei'
$sel.Font.Size = 22
$sel.Font.Bold = $true
$sel.ParagraphFormat.Alignment = 1
$sel.TypeText("RPG")
$sel.TypeText([char]32)
$sel.TypeText("游戏存档系统 - 完整类图")
$sel.TypeParagraph()

# Subtitle
$sel.Font.Size = 10
$sel.Font.Bold = $false
$sel.Font.ColorIndex = 15
$sel.ParagraphFormat.Alignment = 1
$sel.TypeText("生成日期: 2026-05-05")
$sel.TypeParagraph()
$sel.TypeParagraph()

# === Helper functions ===
function Write-Heading($text, $level) {
    $script:sel.Font.Name = 'Microsoft YaHei'
    $script:sel.Font.ColorIndex = 0
    $script:sel.ParagraphFormat.Alignment = 0
    if ($level -eq 1) { $script:sel.Font.Size = 16; $script:sel.Font.Bold = $true }
    elseif ($level -eq 2) { $script:sel.Font.Size = 13; $script:sel.Font.Bold = $true }
    else { $script:sel.Font.Size = 11; $script:sel.Font.Bold = $true }
    $script:sel.TypeText($text)
    $script:sel.TypeParagraph()
}

function Write-Code($lines) {
    $script:sel.Font.Name = 'Consolas'
    $script:sel.Font.Size = 9
    $script:sel.Font.Bold = $false
    foreach ($l in $lines) {
        $script:sel.TypeText("  " + $l)
        $script:sel.TypeParagraph()
    }
    $script:sel.TypeParagraph()
}

function Write-Body($text) {
    $script:sel.Font.Name = 'Microsoft YaHei'
    $script:sel.Font.Size = 10.5
    $script:sel.Font.Bold = $false
    $script:sel.TypeText($text)
    $script:sel.TypeParagraph()
}

function Write-Table($headers, $rows) {
    $hc = $headers.Count
    $rc = $rows.Count
    $end = $doc.Content.End - 1
    $t = $doc.Tables.Add($doc.Range($end, $end), ($rc + 1), $hc)
    $t.Borders.Enable = $true
    for ($i = 0; $i -lt $hc; $i++) {
        $t.Cell(1, $i + 1).Range.Text = $headers[$i]
        $t.Cell(1, $i + 1).Range.Font.Bold = $true
        $t.Cell(1, $i + 1).Range.Font.Name = 'Microsoft YaHei'
        $t.Cell(1, $i + 1).Range.Font.Size = 9
        $t.Cell(1, $i + 1).Shading.BackgroundPatternColor = 15132390
    }
    for ($r = 0; $r -lt $rc; $r++) {
        for ($c = 0; $c -lt $hc; $c++) {
            $t.Cell($r + 2, $c + 1).Range.Text = $rows[$r][$c]
            $t.Cell($r + 2, $c + 1).Range.Font.Name = 'Consolas'
            $t.Cell($r + 2, $c + 1).Range.Font.Size = 8
        }
    }
    $sel = $word.Selection
    $sel.Start = $doc.Content.End
    $sel.TypeParagraph()
}

# ============ CONTENT ============

Write-Heading "一、核心接口与类" 1
Write-Body ""

Write-Heading "1.1 ISaveManager (接口)" 2
Write-Code @(
    "public interface ISaveManager {",
    "    void LoadData(GameData _data);",
    "    void SaveData(ref GameData _data);",
    "}"
)

Write-Heading "1.2 SaveManager (单例 MonoBehaviour)" 2
Write-Code @(
    "public class SaveManager : MonoBehaviour {",
    "    public static SaveManager instance;",
    "    [SerializeField] string fileName;",
    "    [SerializeField] bool encryptData;",
    "    GameData gameData;",
    "    List<ISaveManager> saveManagers;",
    "    FileDataHandler dataHandler;",
    "",
    "    void Awake()   -> 单例 + DontDestroyOnLoad + 初始化 dataHandler",
    "    void Start()   -> FindAllSaveManagers() + LoadGame()",
    "    void NewGame() -> 创建空 GameData",
    "    void LoadGame()-> dataHandler.Load() -> 遍历 ISaveManager.LoadData()",
    "    void SaveGame()-> FindAllSaveManagers() -> 遍历 ISaveManager.SaveData() -> dataHandler.Save()",
    "    bool HasSavedData() -> 检查存档文件是否存在",
    "    void OnApplicationQuit() -> 自动保存",
    "}"
)

Write-Heading "1.3 GameData (数据容器)" 2
Write-Code @(
    "[System.Serializable]",
    "public class GameData {",
    "    int currency;                                       // 金币",
    "    SerializableDictionary<string,bool> skillTree;      // 技能树解锁",
    "    SerializableDictionary<string,int>  inventory;      // 背包物品",
    "    List<string> equipmentId;                           // 已装备",
    "    SerializableDictionary<string,bool> checkpoints;    // 检查点",
    "    string closestCheckpointId;                         // 最近检查点",
    "    float lostCurrencyX, lostCurrencyY;                 // 掉落货币位置",
    "    int lostCurrencyAmount;                             // 掉落货币数量",
    "    SerializableDictionary<string,float> volumeSettings;// 音量设置",
    "}"
)

Write-Heading "1.4 FileDataHandler (文件I/O)" 2
Write-Code @(
    "public class FileDataHandler {",
    "    string dataDirPath  = Application.persistentDataPath;",
    "    string dataFileName = `"data.alexdev`";",
    "    bool encryptData    = false;",
    "    string codeWord     = `"alexdev`";  // 未启用",
    "",
    "    void Save(GameData) -> JsonUtility.ToJson -> 写入文件",
    "    GameData Load()    -> 读文件 -> JsonUtility.FromJson",
    "    void Delete()       -> File.Delete",
    "}"
)

# ============ SECTION 2 ============
Write-Heading "二、ISaveManager 实现类 (12个)" 1
Write-Body ""

$h2 = @("类名", "保存的数据", "LoadData 特殊处理")
$r2 = @(
    @("GameManager", "checkpoints / closestCheckpointId / lostCurrency", "协程延迟0.1秒加载; 恢复玩家位置; 生成掉落货币预制体"),
    @("PlayerManager", "currency (金币)", "直接赋值"),
    @("Inventory", "inventory / equipmentId / stash", "先清空再填充"),
    @("UI", "volumeSettings (音量)", "遍历滑动条组件设置值"),
    @("UI_SkillTreeSlot", "单个技能槽解锁状态", "每个技能槽独立保存/加载"),
    @("SwordSkill", "剑技能树解锁状态", "遍历技能字典"),
    @("ParrySkill", "格挡技能树解锁状态", ""),
    @("DodgeSkill", "闪避技能树解锁状态", ""),
    @("DashSkill", "冲刺技能树解锁状态", ""),
    @("CrystalSkill", "水晶技能树解锁状态", ""),
    @("BlackholeSkill", "黑洞技能解锁状态", ""),
    @("CloneSkill", "分身技能树解锁状态", "")
)
Write-Table $h2 $r2

# ============ SECTION 3 ============
Write-Heading "三、存档触发点 (7个)" 1
Write-Body ""

$h3 = @("触发源", "调用方法", "时机", "场景")
$r3 = @(
    @("UI_MainMenu", "ExitGame()", "主菜单点击退出按钮", "MainMenu"),
    @("UI (游戏内)", "SaveAndExitGame()", "选项面板点击保存退出", "MainScene"),
    @("GameManager", "RestartScene()", "角色死亡后重启场景", "MainScene"),
    @("UI", "RestartGameButton()", "通关面板点击重新开始", "MainScene"),
    @("PlayerStats", "Die()", "角色死亡时立即保存", "MainScene"),
    @("PlayerItemDrop", "GenerateDrop()", "掉落物品后保存", "MainScene"),
    @("SaveManager", "OnApplicationQuit()", "程序关闭时自动保存", "全局")
)
Write-Table $h3 $r3

# ============ SECTION 4 ============
Write-Heading "四、数据流向" 1
Write-Body ""

Write-Heading "4.1 SaveGame() 保存流程" 2
Write-Code @(
    "SaveManager.SaveGame()",
    "  |-- FindAllSaveManagers()       // 重新扫描场景中所有 ISaveManager",
    "  |-- foreach ISaveManager:",
    "  |     saveManager.SaveData(ref gameData)",
    "  |     |-- GameManager   -> checkpoints + lostCurrency",
    "  |     |-- PlayerManager -> currency",
    "  |     |-- Inventory     -> inventory + equipmentId",
    "  |     |-- UI            -> volumeSettings",
    "  |     |-- Skill slots   -> skillTree",
    "  |-- dataHandler.Save(gameData)",
    "       |-- JsonUtility.ToJson -> 写入文件"
)

Write-Heading "4.2 LoadGame() 加载流程" 2
Write-Code @(
    "SaveManager.LoadGame()",
    "  |-- dataHandler.Load()          // 读文件 -> JsonUtility.FromJson",
    "  |-- null 检查所有字段 (skillTree/inventory/checkpoints...)",
    "  |-- foreach ISaveManager:",
    "       saveManager.LoadData(gameData)",
    "       |-- GameManager   -> 激活检查点 + 移动玩家 + 生成掉落货币",
    "       |-- PlayerManager -> 恢复 currency",
    "       |-- Inventory     -> 恢复背包和装备",
    "       |-- UI            -> 恢复音量滑块",
    "       |-- Skill slots   -> 恢复技能解锁状态"
)

# ============ SECTION 5 ============
Write-Heading "五、类关系总结" 1
Write-Body ""

Write-Heading "调用关系" 2
Write-Code @(
    "UI_MainMenu --------> SaveManager.instance.SaveGame()",
    "UI          --------> SaveManager.instance.SaveGame()",
    "PlayerStats --------> SaveManager.instance.SaveGame()",
    "PlayerItemDrop -----> SaveManager.instance.SaveGame()",
    "GameManager --------> SaveManager.instance.SaveGame()"
)

Write-Heading "所有权/依赖" 2
Write-Code @(
    "SaveManager --拥有--> GameData",
    "SaveManager --使用--> FileDataHandler",
    "SaveManager --依赖--> ISaveManager (遍历所有实现)",
    "FileDataHandler -读写-> GameData (via JSON)"
)

Write-Heading "实现关系 (12个ISaveManager实现)" 2
Write-Code @(
    "GameManager / PlayerManager / Inventory / UI / UI_SkillTreeSlot",
    "SwordSkill / ParrySkill / DodgeSkill / DashSkill",
    "CrystalSkill / BlackholeSkill / CloneSkill"
)

# ============ SECTION 6 ============
Write-Heading "六、存档文件信息" 1
Write-Body ""

Write-Code @(
    "存档路径: C:\Users\Admin\AppData\LocalLow\DefaultCompany\RPG\data.alexdev",
    "文件格式: JSON 明文",
    "序列化方式: JsonUtility.ToJson / JsonUtility.FromJson",
    "加密状态: 未启用 (encryptData = false, codeWord = `"alexdev`" 已注释)"
)

# Save
$desktop = [Environment]::GetFolderPath('Desktop')
$savePath = $desktop + '\RPG_存档系统_类图.docx'
$doc.SaveAs([ref]$savePath, [ref]16)
$doc.Close()
$word.Quit()

[System.Runtime.InteropServices.Marshal]::ReleaseComObject($word) | Out-Null
Write-Output "DONE: $savePath"
