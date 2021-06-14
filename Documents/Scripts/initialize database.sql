USE [LSTY_Sdtd]
GO

--用户表
CREATE TABLE T_User(
	Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWSEQUENTIALID(),	--GUID
	CreatedDate DATETIME NOT NULL DEFAULT GETDATE(),			--创建日期
	DisplayName NVARCHAR(32) NOT NULL,							--显示名称
	LastLoginTime DATETIME,										--上次登录时间
	LastLoginIpAddress VARCHAR(32),								--上次登录Ip
);
GO

--标准账户表
CREATE TABLE T_StandardAccount(
	Fk_UserId UNIQUEIDENTIFIER PRIMARY KEY FOREIGN KEY REFERENCES T_User(Id),	--用户Id
	CreatedDate DATETIME NOT NULL DEFAULT GETDATE(),			--创建日期
	AccountName VARCHAR (32) NOT NULL,							--账户名
	PasswordHash VARCHAR(64) NOT NULL,							--密码哈希值
	PasswordSalt VARCHAR(64) NOT NULL,							--密码盐值
);
--创建索引
CREATE UNIQUE INDEX Index_AccountName ON T_StandardAccount(AccountName);
GO

--Email账户表
CREATE TABLE T_EmailAccount(
	Fk_UserId UNIQUEIDENTIFIER PRIMARY KEY FOREIGN KEY REFERENCES T_User(Id),	--用户Id
	CreatedDate DATETIME NOT NULL DEFAULT GETDATE(),			--创建日期
	Email VARCHAR(128) NOT NULL,								--Email用户身份的标识
);
--创建索引
CREATE UNIQUE INDEX Index_Email ON T_EmailAccount(Email);
GO

--QQ账户表
CREATE TABLE T_QQAccount(
	Fk_UserId UNIQUEIDENTIFIER PRIMARY KEY FOREIGN KEY REFERENCES T_User(Id),	--用户Id
	CreatedDate DATETIME NOT NULL DEFAULT GETDATE(),			--创建日期
	OpenId VARCHAR(64) NOT NULL,								--QQ用户身份的标识
);
--创建索引
CREATE UNIQUE INDEX Index_OpenId ON T_QQAccount(OpenId);
GO

--角色表
CREATE TABLE T_Role(
	Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWSEQUENTIALID(),	--角色Id
	CreatedDate DATETIME NOT NULL DEFAULT GETDATE(),			--创建日期
	Name NVARCHAR(32) NOT NULL,									--角色名称
	Description NVARCHAR(512)									--说明
);
--创建索引
CREATE UNIQUE INDEX Index_Name ON T_Role(Name);
GO

INSERT INTO T_Role(Name,Description) VALUES('BlacklistUser','黑名单用户');
INSERT INTO T_Role(Name,Description) VALUES('NormalUser','正常用户');
INSERT INTO T_Role(Name,Description) VALUES('Administrator','管理员');
GO

--用户角色表
CREATE TABLE T_UserRole(
	Fk_UserId UNIQUEIDENTIFIER FOREIGN KEY REFERENCES T_User(Id),	--用户Id
	Fk_RoleId UNIQUEIDENTIFIER FOREIGN KEY REFERENCES T_Role(Id),	--角色Id
	CreatedDate DATETIME NOT NULL DEFAULT GETDATE(),				--创建日期
	PRIMARY KEY(Fk_UserId,Fk_RoleId)
);
GO

--VIP用户表
CREATE TABLE T_VipInfo(
	Fk_UserId UNIQUEIDENTIFIER PRIMARY KEY FOREIGN KEY REFERENCES T_User(Id),	--用户Id
	CreatedDate DATETIME NOT NULL DEFAULT GETDATE(),				--创建日期
	ExpiryDate DATETIME NOT NULL DEFAULT GETDATE(),					--使用期限
	MaxInstanceCount INT NOT NULL DEFAULT 0,						--最大实例个数
	SecretKey VARCHAR(64) NOT NULL,									--加密的强随机字符串
);
--创建索引
CREATE UNIQUE INDEX Index_SecretKey ON T_VipInfo(SecretKey);
GO

--活跃客户端表
CREATE TABLE T_ActiveClient(
	Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWSEQUENTIALID(),		--GUID
	CreatedDate DATETIME NOT NULL DEFAULT GETDATE(),				--创建日期
	Fk_UserId UNIQUEIDENTIFIER FOREIGN KEY REFERENCES T_User(Id),	--用户Id
	DeviceId VARCHAR(64) NOT NULL,									--客户设备Id，由mac地址和进程Id生成
	IpAddress VARCHAR(32) NOT NULL,									--Ip地址
	Version VARCHAR(64) NOT NULL,									--客户端版本
);
--创建索引
CREATE UNIQUE INDEX Index_Fk_UserId ON T_ActiveClient(Fk_UserId);
CREATE UNIQUE INDEX Index_DeviceId ON T_ActiveClient(DeviceId);
GO

--创建视图
CREATE VIEW V_User AS
SELECT u.Id AS UserId,u.DisplayName,u.LastLoginTime,u.LastLoginIpAddress,sa.AccountName,
ea.Email,rol.Id AS RoleId,rol.[Name] AS RoleName,vip.ExpiryDate,vip.MaxInstanceCount,vip.SecretId FROM T_User AS u
LEFT JOIN T_StandardAccount AS sa ON u.Id=sa.Fk_UserId
LEFT JOIN T_EmailAccount AS ea ON u.Id=ea.Fk_UserId
LEFT JOIN T_UserRole AS urole ON u.Id=urole.Fk_UserId
LEFT JOIN T_Role AS rol ON urole.Fk_RoleId=rol.Id
LEFT JOIN T_VipInfo AS vip ON u.Id=vip.Fk_UserId;
GO

--创建视图
CREATE VIEW V_ActiveClient AS
SELECT ac.Id,ac.CreatedDate,ac.Fk_UserId AS UserId,ac.DeviceId,ac.IpAddress,ac.[Version],
u.DisplayName,sa.AccountName,
ea.Email,vip.ExpiryDate,vip.MaxInstanceCount FROM T_ActiveClient AS ac
LEFT JOIN T_User AS u ON u.Id=ac.Fk_UserId
LEFT JOIN T_StandardAccount AS sa ON u.Id=sa.Fk_UserId
LEFT JOIN T_EmailAccount AS ea ON u.Id=ea.Fk_UserId
LEFT JOIN T_VipInfo AS vip ON u.Id=vip.Fk_UserId;
GO

--Jwt RefreshToken表
CREATE TABLE T_RefreshToken(
	Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWSEQUENTIALID(),		--GUID
	CreatedDate DATETIME NOT NULL DEFAULT GETDATE(),				--创建日期
	Fk_UserId UNIQUEIDENTIFIER FOREIGN KEY REFERENCES T_User(Id),	--用户Id
	Token VARCHAR(512) NOT NULL,									--Refresh Token
	JwtId VARCHAR(512) UNIQUE NOT NULL,								--使用 JwtId 映射到对应的 token
	IsUsed BIT NOT NULL DEFAULT 0,									--如果已经使用过它，我们不想使用相同的 refresh token 生成新的 JWT token
	IsRevorked BIT NOT NULL DEFAULT 0,								--是否出于安全原因已将其撤销
	ExpiryDate DATETIME NOT NULL DEFAULT GETDATE(),					--Refresh Token 的生命周期很长，可以长达数月
);
--创建索引
CREATE UNIQUE INDEX Index_Fk_UserId ON T_RefreshToken(Fk_UserId);
CREATE UNIQUE INDEX Index_Token ON T_RefreshToken(Token);
GO

--菜单表
CREATE TABLE T_Menu(
	Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWSEQUENTIALID(),		--GUID
	CreatedDate DATETIME NOT NULL DEFAULT GETDATE(),				--创建日期
	ParentId UNIQUEIDENTIFIER ,										--父菜单Id
	Name NVARCHAR(512) NOT NULL,									--菜单名
	Icon VARCHAR(512) NOT NULL,										--菜单图标
	Url VARCHAR(512) NOT NULL,										--Url
	Sort INT NOT NULL,												--排序
	IsEnabled BIT NOT NULL,											--是否启用
	Description NVARCHAR(512)										--说明
);
GO

--角色菜单表
CREATE TABLE T_RoleMenu(	
	Fk_RoleId UNIQUEIDENTIFIER FOREIGN KEY REFERENCES T_Role(Id),	--角色Id
	Fk_MenuId UNIQUEIDENTIFIER FOREIGN KEY REFERENCES T_Menu(Id),	--菜单Id
	CreatedDate DATETIME NOT NULL DEFAULT GETDATE(),				--创建日期
	PRIMARY KEY(Fk_RoleId,Fk_MenuId)
);
GO

--许可表
CREATE TABLE T_Permission(
	Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWSEQUENTIALID(),		--GUID
	CreatedDate DATETIME NOT NULL DEFAULT GETDATE(),				--创建日期
	PathStartsSegments VARCHAR(512) NOT NULL,						--路由开始部分
	IsEnabled BIT NOT NULL,											--是否启用
	Description NVARCHAR(512)										--说明
);
--创建索引
CREATE UNIQUE INDEX Index_PathStartsSegments ON T_Permission(PathStartsSegments);
GO

--角色许可表
CREATE TABLE T_RolePermission(	
	Fk_RoleId UNIQUEIDENTIFIER FOREIGN KEY REFERENCES T_Role(Id),				--角色Id
	Fk_PermissionId UNIQUEIDENTIFIER FOREIGN KEY REFERENCES T_Permission(Id),	--许可Id
	CreatedDate DATETIME NOT NULL DEFAULT GETDATE(),							--创建日期
	PRIMARY KEY(Fk_RoleId,Fk_PermissionId)
);
GO