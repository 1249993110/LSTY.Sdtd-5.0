USE [LSTY_Sdtd]
GO

--用户表
CREATE TABLE T_User(
	Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWSEQUENTIALID(),--GUID
	CreatedDate DATETIME NOT NULL DEFAULT GETDATE(),	--创建日期
	DisplayName NVARCHAR(24) NOT NULL,					--显示名称
	LastLoginTime DATETIME,								--上次登录时间
	LastLoginIpAddress VARCHAR(50),						--上次登录Ip
);
--创建索引
CREATE UNIQUE INDEX Index_User ON T_User(UserID);
GO

--标准账户表
CREATE TABLE T_StandardAccount(
	AccountName
	GUID UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWSEQUENTIALID(),--GUID
	CreatedDate DATETIME NOT NULL DEFAULT GETDATE(),	--创建日期
	Fk_UserID INT FOREIGN KEY REFERENCES T_User(UserID)
		ON DELETE CASCADE
		ON UPDATE CASCADE,										--用户ID
	PasswordHash VARCHAR(50) NOT NULL,							--密码哈希值
	SecurityStamp VARCHAR(50) NOT NULL,							--安全戳
);
--创建索引
CREATE UNIQUE INDEX Index_StandardAccount ON T_StandardAccount(Fk_UserID);
GO

--Email账户表
CREATE TABLE T_EmailAccount(
	GUID UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWSEQUENTIALID(),	--GUID
	CreatedDate DATETIME NOT NULL DEFAULT GETDATE(),	--创建日期
	Fk_UserID INT FOREIGN KEY REFERENCES T_User(UserID)
		ON DELETE CASCADE
		ON UPDATE CASCADE,								--用户ID
	Email VARCHAR(50) UNIQUE NOT NULL,					--Email用户身份的标识
);
--创建索引
CREATE UNIQUE INDEX Index_EmailAccount ON T_EmailAccount(Fk_UserID);
CREATE UNIQUE INDEX Index1_EmailAccount ON T_EmailAccount(Email);
GO

--QQ账户表
CREATE TABLE T_QQAccount(
	GUID UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWSEQUENTIALID(),	--GUID
	CreatedDate DATETIME NOT NULL DEFAULT GETDATE(),	--创建日期
	Fk_UserID INT FOREIGN KEY REFERENCES T_User(UserID)
		ON DELETE CASCADE
		ON UPDATE CASCADE,								--用户ID
	OpenID VARCHAR(50) UNIQUE NOT NULL,					--QQ用户身份的标识
);
--创建索引
CREATE UNIQUE INDEX Index_QQAccount ON T_QQAccount(Fk_UserID);
CREATE UNIQUE INDEX Index1_QQAccount ON T_QQAccount(OpenID);
GO

--角色表
CREATE TABLE T_Role(
	GUID UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWSEQUENTIALID(),	--GUID
	CreatedDate DATETIME NOT NULL DEFAULT GETDATE(),	--创建日期
	RoleID INT DEFAULT(-1) UNIQUE NOT NULL,				--角色ID
	RoleName NVARCHAR(8),								--角色名称
);
--创建索引
CREATE UNIQUE INDEX Index_Role ON T_Role(RoleID);
GO

INSERT INTO T_Role(RoleID,RoleName) VALUES(-1,'黑名单用户');
INSERT INTO T_Role(RoleID,RoleName) VALUES(0,'超级管理员');
INSERT INTO T_Role(RoleID,RoleName) VALUES(1,'管理员');
INSERT INTO T_Role(RoleID,RoleName) VALUES(2,'优惠会员');
INSERT INTO T_Role(RoleID,RoleName) VALUES(3,'普通会员');
INSERT INTO T_Role(RoleID,RoleName) VALUES(4,'普通用户');
GO

--用户角色表
CREATE TABLE T_UserRole(
	GUID UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWSEQUENTIALID(),	--GUID
	CreatedDate DATETIME NOT NULL DEFAULT GETDATE(),	--创建日期
	Fk_UserID INT FOREIGN KEY REFERENCES T_User(UserID)
		ON DELETE CASCADE
		ON UPDATE CASCADE,								--用户ID
	Fk_RoleID INT FOREIGN KEY REFERENCES T_Role(RoleID)
		ON DELETE CASCADE
		ON UPDATE CASCADE,								--用户角色ID
);
--创建索引
CREATE UNIQUE INDEX Index_UserRole ON T_UserRole(Fk_UserID);
GO

--工具使用期限表
CREATE TABLE T_ST_Expiry(
	GUID UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWSEQUENTIALID(),	--GUID
	CreatedDate DATETIME NOT NULL DEFAULT GETDATE(),	--创建日期
	Fk_UserID INT FOREIGN KEY REFERENCES T_User(UserID)
		ON DELETE CASCADE
		ON UPDATE CASCADE,								--用户ID
	ExpiryTime DATETIME DEFAULT GETDATE(),				--使用期限
);
--创建索引
CREATE UNIQUE INDEX Index_ST_Expiry ON T_ST_Expiry(Fk_UserID);
GO

--在线用户表
CREATE TABLE T_OnlineUser(
	GUID UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWSEQUENTIALID(),	--GUID
	CreatedDate DATETIME NOT NULL DEFAULT GETDATE(),	--创建日期
	Fk_UserID INT FOREIGN KEY REFERENCES T_User(UserID)
		ON UPDATE CASCADE,								--用户ID
	DisplayName VARCHAR(24),							--显示名称
	LoginTime DATETIME,									--登录时间
	IpAddress VARCHAR(50),								--Ip地址
	ExpiryTime DATETIME DEFAULT GETDATE(),				--使用期限
	Fk_RoleID INT FOREIGN KEY REFERENCES T_Role(RoleID)
		ON UPDATE CASCADE,								--用户角色ID
	RoleName NVARCHAR(8),								--角色名称
	Version VARCHAR(50),								--客户版本
);
--创建索引
CREATE INDEX Index_OnlineUser ON T_OnlineUser(Fk_UserID);
GO

--创建视图
CREATE VIEW V_User AS
SELECT u.UserID,u.DisplayName,u.LastLoginTime,u.LastLoginIpAddress,
eA.Email,rol.RoleID,rol.RoleName,expiry.ExpiryTime FROM T_User AS u
LEFT JOIN T_EmailAccount AS eA ON u.UserID=eA.Fk_UserID
LEFT JOIN T_UserRole AS urole ON u.UserID=urole.Fk_UserID
LEFT JOIN T_Role AS rol ON urole.Fk_RoleID=rol.RoleID
LEFT JOIN T_ST_Expiry AS expiry ON u.UserID=expiry.Fk_UserID;
GO

--存储过程，插入多条与邮箱账户相关的数据
CREATE PROCEDURE SP_InsertEmailAccount
	@Email VARCHAR(50),			--Email
	@DisplayName VARCHAR(24),	--显示名称
	@PasswordHash VARCHAR(50),	--密码哈希值
	@SecurityStamp VARCHAR(50),	--安全戳
	@LastLoginTime DATETIME,	--上次登录时间
	@LastLoginIpAddress VARCHAR(50),--上次登录Ip
	@RoleID INT,				--用户角色ID
	@ExpiryTime DATETIME,		--使用期限
	
	@UserID INT OUT				--用户ID
AS
BEGIN
	BEGIN TRY
		BEGIN TRAN
			SET NOCOUNT ON;

			INSERT INTO T_User(DisplayName,LastLoginTime,LastLoginIpAddress)
			VALUES(@DisplayName,@LastLoginTime,@LastLoginIpAddress)
		
			select @UserID = @@Identity
			print(@UserID)

			INSERT INTO T_EmailAccount(Fk_UserID,Email)
			VALUES(@UserID,@Email)
		
			INSERT INTO T_StandardAccount(Fk_UserID,PasswordHash,SecurityStamp)
			VALUES(@UserID,@PasswordHash,@SecurityStamp)
		 
			INSERT INTO T_UserRole(Fk_UserID,Fk_RoleID)
			VALUES(@UserID,@RoleID)
			
			INSERT INTO T_ST_Expiry(Fk_UserID,ExpiryTime)
			VALUES(@UserID,@ExpiryTime)
			
		COMMIT TRAN
	End TRY
	BEGIN CATCH
		IF @@TRANCOUNT > 0			--判断有没有事务
			BEGIN
				ROLLBACK TRAN
			END
			
			DECLARE @ErrorMessage NVARCHAR(4000);
			DECLARE @ErrorSeverity INT;
			DECLARE @ErrorState INT;

			SELECT 
				@ErrorMessage = ERROR_MESSAGE(),
				@ErrorSeverity = ERROR_SEVERITY(),
				@ErrorState = ERROR_STATE();
			
			RAISERROR (@ErrorMessage,  -- Message text.
					   @ErrorSeverity, -- Severity.
					   @ErrorState     -- State.
					   );
	END CATCH
END;
GO