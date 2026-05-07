# MyUnity2DRpgGAME

Unity 2D 横版动作 RPG 游戏。

## 技术栈

- Unity 2021.3
- C#
- 2D URP

## 项目结构

```
Assets/
├── 脚本/           # C# 脚本
│   ├── 基础/        # GameManager, Checkpoint, Entity
│   ├── Player/      # 玩家状态机
│   ├── enemy/       # 敌人 AI (骷髅, 史莱姆, 弓箭手, 小鬼, 寂静领主)
│   ├── Skill/       # 技能系统 (剑, 闪避, 格挡, 黑洞, 水晶, 克隆)
│   ├── Item/        # 装备与物品
│   ├── Save/        # JSON 加密存档
│   ├── UI/          # UI 系统
│   ├── Stats/       # 属性数值
│   ├── Effects/     # 视觉特效
│   └── Manager/     # Audio 管理
├── Scenes/          # 场景
├── Graphics/        # 美术资源
└── Aldio/           # 音频
```

## 功能

- **玩家**: 闲置、移动、跳跃、冲刺、攀墙、连招攻击、反击、瞄准投掷
- **敌人**: 史莱姆、骷髅、弓箭手、小鬼、寂静领主 (Boss)
- **技能树**: 剑技、闪避、格挡、黑洞、水晶、克隆、冰火、雷击
- **背包**: 装备、消耗品、仓库、合成
- **存档**: 本地 JSON 加密
- **音频**: BGM 与音效混音器

## 运行

1. Unity Hub 打开项目（推荐 2021.3 LTS 及以上）
2. `File → Build Settings` 确认 MainMenu 和 MainScene 已添加
3. 打开 `Assets/Scenes/MainMenu.unity`，点击 Play
