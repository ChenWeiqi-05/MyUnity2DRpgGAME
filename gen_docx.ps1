$ErrorActionPreference = "Stop"
$desktop = [Environment]::GetFolderPath('Desktop')
$outPath = "$desktop\RPG_存档系统_类图.docx"

# Temp working directory
$tmp = "$env:TEMP\docxgen_" + [Guid]::NewGuid().ToString('N')
New-Item -ItemType Directory -Path "$tmp\word\_rels" -Force | Out-Null
New-Item -ItemType Directory -Path "$tmp\_rels" -Force | Out-Null

# ============================================================
# [Content_Types].xml
# ============================================================
@'
<?xml version="1.0" encoding="UTF-8" standalone="yes"?>
<Types xmlns="http://schemas.openxmlformats.org/package/2006/content-types">
  <Default Extension="rels" ContentType="application/vnd.openxmlformats-package.relationships+xml"/>
  <Default Extension="xml" ContentType="application/xml"/>
  <Override PartName="/word/document.xml" ContentType="application/vnd.openxmlformats-officedocument.wordprocessingml.document.main+xml"/>
</Types>
'@ | Out-File -FilePath "$tmp\[Content_Types].xml" -Encoding UTF8

# ============================================================
# _rels/.rels
# ============================================================
@'
<?xml version="1.0" encoding="UTF-8" standalone="yes"?>
<Relationships xmlns="http://schemas.openxmlformats.org/package/2006/relationships">
  <Relationship Id="rId1" Type="http://schemas.openxmlformats.org/officeDocument/2006/relationships/officeDocument" Target="word/document.xml"/>
</Relationships>
'@ | Out-File -FilePath "$tmp\_rels\.rels" -Encoding UTF8

# ============================================================
# word/_rels/document.xml.rels
# ============================================================
@'
<?xml version="1.0" encoding="UTF-8" standalone="yes"?>
<Relationships xmlns="http://schemas.openxmlformats.org/package/2006/relationships">
</Relationships>
'@ | Out-File -FilePath "$tmp\word\_rels\document.xml.rels" -Encoding UTF8

# ============================================================
# word/document.xml  (main content)
# ============================================================
# Helper: escape XML special chars
function XmlEscape($s) {
    return $s.Replace('&', '&amp;').Replace('<', '&lt;').Replace('>', '&gt;').Replace('"', '&quot;').Replace("'", '&apos;')
}

# Build document XML piece by piece
$docXml = @'
<?xml version="1.0" encoding="UTF-8" standalone="yes"?>
<w:document xmlns:w="http://schemas.openxmlformats.org/wordprocessingml/2006/main"
            xmlns:r="http://schemas.openxmlformats.org/officeDocument/2006/relationships">
  <w:body>
'@

# ---- Helper functions to emit Word XML ----
function AddPara($text, $align, $fontSize, $bold, $fontName, $color) {
    $j = "both"
    if ($align -eq 'center') { $j = "center" }
    elseif ($align -eq 'left') { $j = "left" }
    $b = if ($bold) { '<w:b/>' } else { '' }
    $c = if ($color) { "<w:color w:val=`"$color`"/>" } else { '' }
    $t = [System.Security.SecurityElement]::Escape($text)
    return @"
  <w:p>
    <w:pPr><w:jc w:val="$j"/></w:pPr>
    <w:r>
      <w:rPr><w:rFonts w:ascii="$fontName" w:hAnsi="$fontName" w:eastAsia="$fontName"/><w:sz w:val="$fontSize"/>$b$c</w:rPr>
      <w:t xml:space="preserve">$t</w:t>
    </w:r>
  </w:p>
"@
}

function AddEmptyPara {
    return '<w:p><w:pPr><w:spacing w:before="60" w:after="60"/></w:pPr></w:p>'
}

function AddCodeLine($text) {
    $t = [System.Security.SecurityElement]::Escape($text)
    return @"
  <w:p>
    <w:pPr><w:spacing w:before="0" w:after="0" w:line="240" w:lineRule="auto"/></w:pPr>
    <w:r>
      <w:rPr><w:rFonts w:ascii="Consolas" w:hAnsi="Consolas"/><w:sz w:val="18"/></w:rPr>
      <w:t xml:space="preserve">  $t</w:t>
    </w:r>
  </w:p>
"@
}

function AddTable($headers, $rows) {
    $hc = $headers.Count
    $rc = $rows.Count
    $xml = @"
  <w:tbl>
    <w:tblPr>
      <w:tblW w:w="9000" w:type="dxa"/>
      <w:tblBorders>
        <w:top w:val="single" w:sz="4" w:space="0" w:color="auto"/>
        <w:left w:val="single" w:sz="4" w:space="0" w:color="auto"/>
        <w:bottom w:val="single" w:sz="4" w:space="0" w:color="auto"/>
        <w:right w:val="single" w:sz="4" w:space="0" w:color="auto"/>
        <w:insideH w:val="single" w:sz="4" w:space="0" w:color="auto"/>
        <w:insideV w:val="single" w:sz="4" w:space="0" w:color="auto"/>
      </w:tblBorders>
    </w:tblPr>
"@
    # Header row
    $xml += "    <w:tr><w:trPr><w:tblHeader/></w:trPr>"
    foreach ($h in $headers) {
        $t = [System.Security.SecurityElement]::Escape($h)
        $w = [math]::Floor(9000 / $hc)
        $xml += @"
      <w:tc>
        <w:tcPr><w:tcW w:w="$w" w:type="dxa"/><w:shd w:val="clear" w:color="auto" w:fill="D9D9D9"/></w:tcPr>
        <w:p><w:pPr><w:jc w:val="center"/></w:pPr>
          <w:r><w:rPr><w:rFonts w:ascii="Microsoft YaHei" w:hAnsi="Microsoft YaHei" w:eastAsia="Microsoft YaHei"/><w:sz w:val="18"/><w:b/></w:rPr><w:t xml:space="preserve">$t</w:t></w:r>
        </w:p>
      </w:tc>
"@
    }
    $xml += "</w:tr>"

    # Data rows
    foreach ($row in $rows) {
        $xml += "<w:tr>"
        foreach ($cell in $row) {
            $t = [System.Security.SecurityElement]::Escape($cell)
            $w = [math]::Floor(9000 / $hc)
            $xml += @"
      <w:tc>
        <w:tcPr><w:tcW w:w="$w" w:type="dxa"/></w:tcPr>
        <w:p><w:pPr><w:spacing w:before="0" w:after="0"/></w:pPr>
          <w:r><w:rPr><w:rFonts w:ascii="Consolas" w:hAnsi="Consolas"/><w:sz w:val="16"/></w:rPr><w:t xml:space="preserve">$t</w:t></w:r>
        </w:p>
      </w:tc>
"@
        }
        $xml += "</w:tr>"
    }
    $xml += "</w:tbl>"
    return $xml
}

# ============ BUILD DOCUMENT CONTENT ============

# Title
$docXml += AddPara 'RPG 游戏存档系统 - 完整类图' 'center' '44' $true 'Microsoft YaHei' ''
$docXml += AddPara '生成日期: 2026-05-05' 'center' '20' $false 'Microsoft YaHei' '808080'
$docXml += AddEmptyPara

# ======== Section 1 ========
$docXml += AddPara '一、核心接口与类' 'left' '32' $true 'Microsoft YaHei' ''
$docXml += AddEmptyPara

$docXml += AddPara '1.1 ISaveManager (接口)' 'left' '26' $true 'Microsoft YaHei' ''
$docXml += AddCodeLine 'public interface ISaveManager {'
$docXml += AddCodeLine '    void LoadData(GameData _data);'
$docXml += AddCodeLine '    void SaveData(ref GameData _data);'
$docXml += AddCodeLine '}'
$docXml += AddEmptyPara

$docXml += AddPara '1.2 SaveManager (单例 MonoBehaviour)' 'left' '26' $true 'Microsoft YaHei' ''
$docXml += AddCodeLine 'public class SaveManager : MonoBehaviour {'
$docXml += AddCodeLine '    public static SaveManager instance;'
$docXml += AddCodeLine '    [SerializeField] string fileName;'
$docXml += AddCodeLine '    [SerializeField] bool encryptData;'
$docXml += AddCodeLine '    GameData gameData;'
$docXml += AddCodeLine '    List<ISaveManager> saveManagers;'
$docXml += AddCodeLine '    FileDataHandler dataHandler;'
$docXml += AddCodeLine ''
$docXml += AddCodeLine '    void Awake()   -> 单例 + DontDestroyOnLoad + 初始化 dataHandler'
$docXml += AddCodeLine '    void Start()   -> FindAllSaveManagers() + LoadGame()'
$docXml += AddCodeLine '    void NewGame() -> 创建空 GameData'
$docXml += AddCodeLine '    void LoadGame()-> dataHandler.Load() -> 遍历 ISaveManager.LoadData()'
$docXml += AddCodeLine '    void SaveGame()-> FindAllSaveManagers() -> 遍历 ISaveManager.SaveData() -> dataHandler.Save()'
$docXml += AddCodeLine '    bool HasSavedData() -> 检查存档文件是否存在'
$docXml += AddCodeLine '    void OnApplicationQuit() -> 自动保存'
$docXml += AddCodeLine '}'
$docXml += AddEmptyPara

$docXml += AddPara '1.3 GameData (数据容器)' 'left' '26' $true 'Microsoft YaHei' ''
$docXml += AddCodeLine '[System.Serializable]'
$docXml += AddCodeLine 'public class GameData {'
$docXml += AddCodeLine '    int currency;                                       // 金币'
$docXml += AddCodeLine '    SerializableDictionary<string, bool> skillTree;      // 技能树解锁'
$docXml += AddCodeLine '    SerializableDictionary<string, int>  inventory;      // 背包物品'
$docXml += AddCodeLine '    List<string> equipmentId;                           // 已装备'
$docXml += AddCodeLine '    SerializableDictionary<string, bool> checkpoints;    // 检查点'
$docXml += AddCodeLine '    string closestCheckpointId;                         // 最近检查点'
$docXml += AddCodeLine '    float lostCurrencyX, lostCurrencyY;                 // 掉落货币位置'
$docXml += AddCodeLine '    int lostCurrencyAmount;                             // 掉落货币数量'
$docXml += AddCodeLine '    SerializableDictionary<string, float> volumeSettings;// 音量设置'
$docXml += AddCodeLine '}'
$docXml += AddEmptyPara

$docXml += AddPara '1.4 FileDataHandler (文件I/O)' 'left' '26' $true 'Microsoft YaHei' ''
$docXml += AddCodeLine 'public class FileDataHandler {'
$docXml += AddCodeLine '    string dataDirPath  = Application.persistentDataPath;'
$docXml += AddCodeLine '    string dataFileName = "data.alexdev";'
$docXml += AddCodeLine '    bool encryptData    = false;'
$docXml += AddCodeLine '    string codeWord     = "alexdev";  // 未启用'
$docXml += AddCodeLine ''
$docXml += AddCodeLine '    void Save(GameData) -> JsonUtility.ToJson -> 写入文件'
$docXml += AddCodeLine '    GameData Load()    -> 读文件 -> JsonUtility.FromJson'
$docXml += AddCodeLine '    void Delete()       -> File.Delete'
$docXml += AddCodeLine '}'
$docXml += AddEmptyPara

# ======== Section 2 ========
$docXml += AddPara '二、ISaveManager 实现类 (12个)' 'left' '32' $true 'Microsoft YaHei' ''
$docXml += AddEmptyPara

$h2 = @('类名', '保存的数据', 'LoadData 特殊处理')
$r2 = @(
    @('GameManager', 'checkpoints / closestCheckpointId / lostCurrency', '协程延迟0.1秒加载; 恢复玩家位置; 生成掉落货币预制体'),
    @('PlayerManager', 'currency (金币)', '直接赋值'),
    @('Inventory', 'inventory / equipmentId / stash', '先清空再填充'),
    @('UI', 'volumeSettings (音量)', '遍历滑动条组件设置值'),
    @('UI_SkillTreeSlot', '单个技能槽解锁状态', '每个技能槽独立保存/加载'),
    @('SwordSkill', '剑技能树解锁状态', '遍历技能字典'),
    @('ParrySkill', '格挡技能树解锁状态', ''),
    @('DodgeSkill', '闪避技能树解锁状态', ''),
    @('DashSkill', '冲刺技能树解锁状态', ''),
    @('CrystalSkill', '水晶技能树解锁状态', ''),
    @('BlackholeSkill', '黑洞技能解锁状态', ''),
    @('CloneSkill', '分身技能树解锁状态', '')
)
$docXml += AddTable $h2 $r2
$docXml += AddEmptyPara

# ======== Section 3 ========
$docXml += AddPara '三、存档触发点 (7个)' 'left' '32' $true 'Microsoft YaHei' ''
$docXml += AddEmptyPara

$h3 = @('触发源', '调用方法', '时机', '场景')
$r3 = @(
    @('UI_MainMenu', 'ExitGame()', '主菜单点击退出按钮', 'MainMenu'),
    @('UI (游戏内)', 'SaveAndExitGame()', '选项面板点击保存退出', 'MainScene'),
    @('GameManager', 'RestartScene()', '角色死亡后重启场景', 'MainScene'),
    @('UI', 'RestartGameButton()', '通关面板点击重新开始', 'MainScene'),
    @('PlayerStats', 'Die()', '角色死亡时立即保存', 'MainScene'),
    @('PlayerItemDrop', 'GenerateDrop()', '掉落物品后保存', 'MainScene'),
    @('SaveManager', 'OnApplicationQuit()', '程序关闭时自动保存', '全局')
)
$docXml += AddTable $h3 $r3
$docXml += AddEmptyPara

# ======== Section 4 ========
$docXml += AddPara '四、数据流向' 'left' '32' $true 'Microsoft YaHei' ''
$docXml += AddEmptyPara

$docXml += AddPara '4.1 SaveGame() 保存流程' 'left' '26' $true 'Microsoft YaHei' ''
$docXml += AddCodeLine 'SaveManager.SaveGame()'
$docXml += AddCodeLine '  |-- FindAllSaveManagers()       // 重新扫描场景中所有 ISaveManager'
$docXml += AddCodeLine '  |-- foreach ISaveManager:'
$docXml += AddCodeLine '  |     saveManager.SaveData(ref gameData)'
$docXml += AddCodeLine '  |     |-- GameManager   -> checkpoints + lostCurrency'
$docXml += AddCodeLine '  |     |-- PlayerManager -> currency'
$docXml += AddCodeLine '  |     |-- Inventory     -> inventory + equipmentId'
$docXml += AddCodeLine '  |     |-- UI            -> volumeSettings'
$docXml += AddCodeLine '  |     |-- Skill slots   -> skillTree'
$docXml += AddCodeLine '  |-- dataHandler.Save(gameData)'
$docXml += AddCodeLine '       |-- JsonUtility.ToJson -> 写入文件'
$docXml += AddEmptyPara

$docXml += AddPara '4.2 LoadGame() 加载流程' 'left' '26' $true 'Microsoft YaHei' ''
$docXml += AddCodeLine 'SaveManager.LoadGame()'
$docXml += AddCodeLine '  |-- dataHandler.Load()          // 读文件 -> JsonUtility.FromJson'
$docXml += AddCodeLine '  |-- null 检查所有字段 (skillTree/inventory/checkpoints...)'
$docXml += AddCodeLine '  |-- foreach ISaveManager:'
$docXml += AddCodeLine '       saveManager.LoadData(gameData)'
$docXml += AddCodeLine '       |-- GameManager   -> 激活检查点 + 移动玩家 + 生成掉落货币'
$docXml += AddCodeLine '       |-- PlayerManager -> 恢复 currency'
$docXml += AddCodeLine '       |-- Inventory     -> 恢复背包和装备'
$docXml += AddCodeLine '       |-- UI            -> 恢复音量滑块'
$docXml += AddCodeLine '       |-- Skill slots   -> 恢复技能解锁状态'
$docXml += AddEmptyPara

# ======== Section 5 ========
$docXml += AddPara '五、类关系总结' 'left' '32' $true 'Microsoft YaHei' ''
$docXml += AddEmptyPara

$docXml += AddPara '调用关系' 'left' '26' $true 'Microsoft YaHei' ''
$docXml += AddCodeLine 'UI_MainMenu --------> SaveManager.instance.SaveGame()'
$docXml += AddCodeLine 'UI          --------> SaveManager.instance.SaveGame()'
$docXml += AddCodeLine 'PlayerStats --------> SaveManager.instance.SaveGame()'
$docXml += AddCodeLine 'PlayerItemDrop -----> SaveManager.instance.SaveGame()'
$docXml += AddCodeLine 'GameManager --------> SaveManager.instance.SaveGame()'
$docXml += AddEmptyPara

$docXml += AddPara '所有权/依赖' 'left' '26' $true 'Microsoft YaHei' ''
$docXml += AddCodeLine 'SaveManager --拥有--> GameData'
$docXml += AddCodeLine 'SaveManager --使用--> FileDataHandler'
$docXml += AddCodeLine 'SaveManager --依赖--> ISaveManager (遍历所有实现)'
$docXml += AddCodeLine 'FileDataHandler -读写-> GameData (via JSON)'
$docXml += AddEmptyPara

$docXml += AddPara '实现关系 (12个ISaveManager实现)' 'left' '26' $true 'Microsoft YaHei' ''
$docXml += AddCodeLine 'GameManager / PlayerManager / Inventory / UI / UI_SkillTreeSlot'
$docXml += AddCodeLine 'SwordSkill / ParrySkill / DodgeSkill / DashSkill'
$docXml += AddCodeLine 'CrystalSkill / BlackholeSkill / CloneSkill'
$docXml += AddEmptyPara

# ======== Section 6 ========
$docXml += AddPara '六、存档文件信息' 'left' '32' $true 'Microsoft YaHei' ''
$docXml += AddEmptyPara

$docXml += AddCodeLine '存档路径: C:\Users\Admin\AppData\LocalLow\DefaultCompany\RPG\data.alexdev'
$docXml += AddCodeLine '文件格式: JSON 明文'
$docXml += AddCodeLine '序列化方式: JsonUtility.ToJson / JsonUtility.FromJson'
$docXml += AddCodeLine '加密状态: 未启用 (encryptData = false, codeWord = "alexdev" 已注释)'

# Close document
$docXml += @'
  </w:body>
</w:document>
'@

# Write document.xml
$docXml | Out-File -FilePath "$tmp\word\document.xml" -Encoding UTF8

# ============================================================
# ZIP to .docx
# ============================================================
$docxPath = $outPath
if (Test-Path $docxPath) { Remove-Item $docxPath -Force }

# Use .NET ZipFile (available in .NET 4.5+)
Add-Type -AssemblyName System.IO.Compression.FileSystem
[System.IO.Compression.ZipFile]::CreateFromDirectory($tmp, $docxPath, [System.IO.Compression.CompressionLevel]::Optimal, $false)

# Cleanup
Remove-Item $tmp -Recurse -Force

Write-Output "DONE: $docxPath"
