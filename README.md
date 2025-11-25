# OmniGauge

Duckov 游戏数据分析与物品管理MOD，提供实时伤害统计、任务追踪和物品浏览功能。

## 功能特性

### 伤害统计
- 实时追踪游戏内造成的伤害数据
- 区分普通伤害与暴击伤害
- 自动统计总伤害、最高伤害、命中次数、平均伤害
- 计算每秒伤害(DPS)和暴击率
- 按武器/伤害来源分类统计
- 支持多种格式导出：CSV、JSON、HTML报告、精美图片

### 游戏任务
- 读取并展示游戏内所有任务
- 显示任务进度和完成状态
- 支持任务搜索和筛选

### 物品浏览
- 浏览游戏内所有物品
- 按类别筛选物品
- 支持物品名称搜索
- 快速给予物品功能
- 批量添加物品支持

## 使用方法

1. 将编译后的DLL文件放入游戏MOD目录
2. 启动游戏后按 `T` 键打开/关闭面板
3. 按 `R` 键刷新游戏数据

## 快捷键

| 按键 | 功能 |
|------|------|
| T | 打开/关闭OmniGauge面板 |
| R | 刷新游戏任务和物品数据 |

## 导出功能

伤害数据支持以下导出格式：

- **CSV**: 包含完整的伤害日志和统计摘要，可用Excel打开
- **JSON**: 结构化数据，便于程序处理
- **HTML**: 精美的网页报告，包含图表和详细统计
- **图片**: 生成可分享的统计图片

## 技术说明

- 基于 Duckov Modding API 开发
- 使用 Harmony 进行游戏方法Hook
- Web界面基于嵌入式HTTP服务器
- 前端使用 Chart.js 绑制图表

## 编译要求

- .NET Framework / .NET Standard 2.1
- 引用 duckovAPI 库
- 引用 0Harmony 库

## 项目结构

```
├── Class1.cs                 # MOD入口，初始化各系统
├── Systems/
│   ├── DamageTracker.cs      # 伤害数据追踪器
│   ├── RealDamageTracker.cs  # 实际伤害Hook处理
│   ├── GameHooks.cs          # 游戏方法Hook
│   ├── GameTaskReader.cs     # 游戏任务读取
│   ├── TaskManager.cs        # 任务管理
│   ├── ItemReader.cs         # 物品数据读取
│   ├── SimpleWebView.cs      # Web服务器和界面
│   └── WebUIContent.cs       # Web界面HTML/CSS/JS
├── Assets/                   # 资源文件
└── ReleaseFiles/             # 发布配置
```

## 许可证

MIT License
