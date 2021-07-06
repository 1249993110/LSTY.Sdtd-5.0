PRAGMA FOREIGN_KEYS = ON;			--启用外键
--Player
CREATE TABLE IF NOT EXISTS T_Player(
	SteamId TEXT PRIMARY KEY,		--SteamId
	CreatedDate TIMESTAMP DEFAULT (DATETIME(CURRENT_TIMESTAMP,'LOCALTIME')),
	EntityId INTEGER,				--实体Id
	Name TEXT,						--玩家名称
	IP TEXT,						--IP地址
	TotalPlayTime REAL,				--总游戏时长（单位：分钟）
	LastOnline TIMESTAMP,			--上次在线
	LastPositionX INTEGER,			--X坐标
	LastPositionY INTEGER,			--Y坐标
	LastPositionZ INTEGER,			--Z坐标
	[Level] REAL,					--等级
	Score INTEGER,					--评分
	ZombieKills INTEGER,			--击杀僵尸
	PlayerKills INTEGER,			--击杀玩家
	Deaths INTEGER					--死亡次数
);

--Inventory
CREATE TABLE IF NOT EXISTS T_Inventory(
	SteamId TEXT PRIMARY KEY,		--SteamId
	CreatedDate TIMESTAMP DEFAULT (DATETIME(CURRENT_TIMESTAMP,'LOCALTIME')),
	Content TEXT					--Inventory Content
);

--积分信息
CREATE TABLE IF NOT EXISTS T_Points(
	SteamId TEXT PRIMARY KEY,		--SteamId
	CreatedDate TIMESTAMP DEFAULT (DATETIME(CURRENT_TIMESTAMP,'LOCALTIME')),
	[Count] INTEGER,				--拥有积分
	LastSignDay INTEGER				--上次签到天数
);

--公共回城点
CREATE TABLE IF NOT EXISTS T_CityPosition(
	Id TEXT PRIMARY KEY,			--唯一ID	
	CreatedDate TIMESTAMP DEFAULT (DATETIME(CURRENT_TIMESTAMP,'LOCALTIME')),
	CityName TEXT NOT NULL UNIQUE,	--城镇名称
	Command TEXT NOT NULL,			--传送命令
	PointsRequired INTEGER,			--传送花费积分
	Position TEXT					--三维坐标
);
--创建索引
CREATE UNIQUE INDEX IF NOT EXISTS Index_CityPosition ON T_CityPosition(Command);

--私人回城点
CREATE TABLE IF NOT EXISTS T_HomePosition(
	Id TEXT PRIMARY KEY,			--唯一ID
	CreatedDate TIMESTAMP DEFAULT (DATETIME(CURRENT_TIMESTAMP,'LOCALTIME')),
	HomeName TEXT NOT NULL,			--Home名称
	SteamId TEXT,					--SteamId
	Position TEXT					--三维坐标
);
--创建索引
CREATE INDEX IF NOT EXISTS Index_HomePosition ON T_HomePosition(SteamId);

--传送记录
CREATE TABLE IF NOT EXISTS T_TeleRecord(
	Id INTEGER PRIMARY KEY AUTOINCREMENT,--唯一ID
	SteamId TEXT,					--SteamId
	CreatedDate TIMESTAMP DEFAULT (DATETIME(CURRENT_TIMESTAMP,'LOCALTIME')),--传送日期
	TargetType TEXT,				--目标类型
	DestinationName TEXT,			--目的地名称
	Position TEXT					--三维坐标
);
--创建索引
CREATE INDEX IF NOT EXISTS Index_TeleRecord ON T_TeleRecord(SteamId);

--商品奖品类型
CREATE TABLE IF NOT EXISTS T_ContentTypes(
	Name TEXT PRIMARY KEY,		
	CreatedDate TIMESTAMP DEFAULT (DATETIME(CURRENT_TIMESTAMP,'LOCALTIME')),
	Description TEXT
);
INSERT INTO T_ContentTypes(Name,Description) SELECT 'Item','物品' WHERE NOT EXISTS(SELECT 1 FROM T_ContentTypes WHERE Name='Item');
INSERT INTO T_ContentTypes(Name,Description) SELECT 'Block','方块' WHERE NOT EXISTS(SELECT 1 FROM T_ContentTypes WHERE Name='Block');
INSERT INTO T_ContentTypes(Name,Description) SELECT 'Entity','实体' WHERE NOT EXISTS(SELECT 1 FROM T_ContentTypes WHERE Name='Entity');
INSERT INTO T_ContentTypes(Name,Description) SELECT 'Command','命令' WHERE NOT EXISTS(SELECT 1 FROM T_ContentTypes WHERE Name='Command');
INSERT INTO T_ContentTypes(Name,Description) SELECT 'Points','积分' WHERE NOT EXISTS(SELECT 1 FROM T_ContentTypes WHERE Name='Points');

--商品
CREATE TABLE IF NOT EXISTS T_Goods(
	Id TEXT PRIMARY KEY,			--唯一ID	
	CreatedDate TIMESTAMP DEFAULT (DATETIME(CURRENT_TIMESTAMP,'LOCALTIME')),
	Name TEXT NOT NULL,				--商品名称
	BuyCmd TEXT NOT NULL,			--购买命令
	Content TEXT NOT NULL,			--内容 物品/方块/实体/指令
	[Count] INTEGER,				--数量
	Quality INTEGER,				--品质
	Price INTEGER,					--售价
	ContentType TEXT NOT NULL,		--商品类型
	FOREIGN KEY(ContentType) REFERENCES T_ContentTypes(Name)
);
--创建索引
CREATE UNIQUE INDEX IF NOT EXISTS Index_Goods ON T_Goods(BuyCmd);

--奖品
CREATE TABLE IF NOT EXISTS T_Lottery(
	Id TEXT PRIMARY KEY,			--唯一ID	
	CreatedDate TIMESTAMP DEFAULT (DATETIME(CURRENT_TIMESTAMP,'LOCALTIME')),
	Name TEXT NOT NULL,				--奖品名称
	Content TEXT NOT NULL,			--内容 物品/方块/实体/指令/积分
	[Count] INTEGER,				--数量
	Quality INTEGER,				--品质
	ContentType TEXT NOT NULL,		--奖品类型
	FOREIGN KEY(ContentType) REFERENCES T_ContentTypes(Name)
);

--聊天日志
CREATE TABLE IF NOT EXISTS T_ChatLog(
	Id INTEGER PRIMARY KEY AUTOINCREMENT,
	CreatedDate TIMESTAMP DEFAULT (DATETIME(CURRENT_TIMESTAMP,'LOCALTIME')),
	SteamId TEXT,
	EntityId INTEGER,
	ChatType INTEGER,
	Message TEXT
);
--创建索引
CREATE INDEX IF NOT EXISTS Index_ChatLog ON T_ChatLog(SteamId);
CREATE INDEX IF NOT EXISTS Index_ChatLog1 ON T_ChatLog(EntityId);

CREATE VIEW IF NOT EXISTS V_ChatLog AS
SELECT _log.Id,_log.CreatedDate,_log.SteamId,_log.EntityId,_log.ChatType,_log.Message,player.Name as PlayerName FROM T_ChatLog AS _log LEFT JOIN T_Player AS player ON _log.SteamId = player.SteamId;

--反作弊日志
CREATE TABLE IF NOT EXISTS T_AntiCheatLog(
	Id INTEGER PRIMARY KEY AUTOINCREMENT,	
	CreatedDate TIMESTAMP DEFAULT (DATETIME(CURRENT_TIMESTAMP,'LOCALTIME')),
	SteamId TEXT,
	MessageEn TEXT,
	MessageZh TEXT
);

CREATE VIEW IF NOT EXISTS V_AntiCheatLog AS
SELECT _log.Id,_log.CreatedDate,_log.SteamId,_log.MessageEn,_log.MessageZh,player.Name as PlayerName FROM T_AntiCheatLog AS _log LEFT JOIN T_Player AS player ON _log.SteamId = player.SteamId;

--CDKey兑换
CREATE TABLE IF NOT EXISTS T_CDKey(
	Id INTEGER PRIMARY KEY AUTOINCREMENT,	
	CreatedDate TIMESTAMP DEFAULT (DATETIME(CURRENT_TIMESTAMP,'LOCALTIME')),
	[Key] TEXT NOT NULL,			--key
	LimitUseOnceEachPlayer INTEGER, --限制每个玩家仅能使用一次
	MaxExchangeCount INTEGER,		--最大兑换次数
	ExpiryDate TIMESTAMP,			--到期时间，小于或等于0则永远不会失效
	ItemName TEXT NOT NULL,			--兑换项名
	ItemContent TEXT NOT NULL,		--内容 物品/方块/实体/指令/积分
	ItemCount INTEGER,				--数量
	ItemQuality INTEGER,			--品质
	ContentType TEXT NOT NULL,		--兑换内容类型
	FOREIGN KEY(ContentType) REFERENCES T_ContentTypes(Name)
);
--创建索引
CREATE UNIQUE INDEX IF NOT EXISTS Index_Key ON T_CDKey([Key]);

--CDKey兑换记录
CREATE TABLE IF NOT EXISTS T_CDKeyExchangeLog(
	Id INTEGER PRIMARY KEY AUTOINCREMENT,	
	CreatedDate TIMESTAMP DEFAULT (DATETIME(CURRENT_TIMESTAMP,'LOCALTIME')),
	CDKey TEXT,
	SteamId TEXT,
	FOREIGN KEY(CDKey) REFERENCES T_CDKey([Key])
);
--创建索引
CREATE UNIQUE INDEX IF NOT EXISTS Index_1 ON T_CDKeyExchangeLog(CDKey,SteamId);

--成就奖励
CREATE TABLE IF NOT EXISTS T_AchievementReward(
	Id TEXT PRIMARY KEY,			--唯一ID	
	CreatedDate TIMESTAMP DEFAULT (DATETIME(CURRENT_TIMESTAMP,'LOCALTIME')),
	Name TEXT NOT NULL,				--名称
	TriggerVariable TEXT NOT NULL,	--触发变量
	TriggerRequiredCount INTEGER,	--触发数量
	RewardContent TEXT NOT NULL,	--内容 物品/方块/实体/指令/积分
	RewardCount INTEGER,			--数量
	RewardQuality INTEGER,			--品质
	ContentType TEXT NOT NULL,		--成就奖励类型
	FOREIGN KEY(ContentType) REFERENCES T_ContentTypes(Name)
);
--创建索引
CREATE UNIQUE INDEX IF NOT EXISTS Index_Name ON T_AchievementReward(Name);

--成就奖励记录
CREATE TABLE IF NOT EXISTS T_AchievementRewardLog(
	Id INTEGER PRIMARY KEY AUTOINCREMENT,	
	CreatedDate TIMESTAMP DEFAULT (DATETIME(CURRENT_TIMESTAMP,'LOCALTIME')),
	SteamId TEXT,
	AchievementName TEXT,
	FOREIGN KEY(AchievementName) REFERENCES T_AchievementReward(Name)
);
--创建索引
CREATE UNIQUE INDEX IF NOT EXISTS Index_1 ON T_AchievementRewardLog(AchievementName,SteamId);

--悬赏名单
CREATE TABLE IF NOT EXISTS T_KillReward(
	Id TEXT PRIMARY KEY,				--唯一ID
	CreatedDate TIMESTAMP DEFAULT (DATETIME(CURRENT_TIMESTAMP,'LOCALTIME')),
	SteamIdOrEntityName TEXT NOT NULL,	--SteamId或实体名称
	FriendlyName TEXT NOT NULL,			--友好名称
	RewardContent TEXT NOT NULL,		--内容 物品/方块/实体/指令/积分
	RewardCount INTEGER,				--数量
	RewardQuality INTEGER,				--品质
	ContentType TEXT NOT NULL,			--悬赏奖励类型
	SpawnedTips TEXT,					--生成提示
	KilledTips TEXT,					--击杀提示
	FOREIGN KEY(ContentType) REFERENCES T_ContentTypes(Name)
);
--创建索引
CREATE UNIQUE INDEX IF NOT EXISTS Index_SteamIdOrEntityName ON T_KillReward(SteamIdOrEntityName);
