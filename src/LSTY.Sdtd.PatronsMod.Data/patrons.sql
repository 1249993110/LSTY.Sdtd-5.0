PRAGMA FOREIGN_KEYS = ON;			--启用外键
--Player
CREATE TABLE IF NOT EXISTS T_Player(
	SteamId TEXT PRIMARY KEY,		--SteamId
	CreatedDate TIMESTAMP DEFAULT (DATETIME(CURRENT_TIMESTAMP,'LOCALTIME')),
	EntityId INTEGER,				--实体Id
	Name INTEGER,					--玩家名称
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
	IsHome INTEGER,					--是否是Home类型
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
	GoodsName TEXT NOT NULL,		--商品名称
	BuyCmd TEXT NOT NULL,			--购买命令
	Content TEXT NOT NULL,			--内容 物品/方块/实体/指令
	Amount INTEGER,					--数量
	Quality INTEGER,				--品质
	Price INTEGER,					--售价
	ContentType TEXT NOT NULL,		--商品类型
	FOREIGN KEY(ContentType) REFERENCES T_ContentTypes(Name) ON DELETE CASCADE
);
--创建索引
CREATE UNIQUE INDEX IF NOT EXISTS Index_Goods ON T_Goods(BuyCmd);

--奖品
CREATE TABLE IF NOT EXISTS T_Lottery(
	Id TEXT PRIMARY KEY,			--唯一ID	
	CreatedDate TIMESTAMP DEFAULT (DATETIME(CURRENT_TIMESTAMP,'LOCALTIME')),
	LotteryName TEXT NOT NULL,		--奖品名称
	Content TEXT NOT NULL,			--内容 物品/方块/实体/指令/积分
	Amount INTEGER,					--数量
	Quality INTEGER,				--品质
	ContentType TEXT NOT NULL,		--奖品类型
	FOREIGN KEY(ContentType) REFERENCES T_ContentTypes(Name) ON DELETE CASCADE
);

--聊天日志
CREATE TABLE IF NOT EXISTS T_ChatLog(
	Id INTEGER PRIMARY KEY AUTOINCREMENT,
	CreatedDate TIMESTAMP DEFAULT (DATETIME(CURRENT_TIMESTAMP,'LOCALTIME')),
	SteamId TEXT,
	EntityId INTEGER,
	ChatType TEXT,
	Message TEXT
);
--创建索引
CREATE INDEX IF NOT EXISTS Index_ChatLog ON T_ChatLog(SteamId);
CREATE INDEX IF NOT EXISTS Index_ChatLog1 ON T_ChatLog(EntityId);

--反作弊日志
CREATE TABLE IF NOT EXISTS T_AntiCheatLog(
	Id INTEGER PRIMARY KEY AUTOINCREMENT,
	SteamId TEXT,
	CreatedDate TIMESTAMP DEFAULT (DATETIME(CURRENT_TIMESTAMP,'LOCALTIME')),
	Message TEXT
);