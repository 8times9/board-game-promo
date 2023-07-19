drop table if exists Post
drop table if exists Email
drop procedure if exists spAddPost

create table Post (
PostID int Identity(1,1) primary key not null,
Date datetime not null,
Text nvarchar(1000) not null)
go

create procedure spAddPost @Text nvarchar(1000)
as
insert into Post(Date, Text) values (CURRENT_TIMESTAMP, @Text)
go
 
create table Email(
EmailID int Identity(1,1) primary key not null,
EmailAddress varchar(100))

DROP PROCEDURE if exists spAddEmail
go
CREATE PROCEDURE spAddEmail
@EmailAddress varchar(30)
as
BEGIN
insert into Emails
(EmailAddress)
VALUES
(@EmailAddress)
END
GO

 DROP INDEX if exists uiEmail on Emails
go
CREATE UNIQUE INDEX uiEmail 
on Emails (EmailAddress)
go

--Admin login creation--

DROP TABLE if exists UserRoleMaster
go

DROP TABLE if exists Account
go


 create table Account(
 AccountID int not null identity (1,1) primary key,
 Username varchar(100) not null,
 Password varchar(100) not null,
 PasswordSalt varchar(100) not null
)

DROP PROCEDURE if exists spAddAccount
go

CREATE PROCEDURE spAddAccount
@Username varchar(20), 
@Password varchar(100)
as
begin
	DECLARE @randomText uniqueidentifier = NewID()
	DECLARE @Salt as Char(100)
	SET @Salt = HASHBYTES('SHA2_256',CONVERT(Char(100), @randomText))
	-- this creates a random text string, inputs it into SHA256 algorithm, and saves it as our salt variable

	DECLARE @PasswordSalted as varbinary(max)
	SET @PasswordSalted = HASHBYTES('SHA2_256',CONCAT(@Salt, @Password))
	-- this combines our salt and user password then inputs it into SHA 256 algorithm

	INSERT INTO Account
	(Username, Password, PasswordSalt)
	VALUES
	(@Username, @PasswordSalted, @Salt)
end
go

 exec spAddAccount 'tableknight', 'semifinal-overwrite-hardened'
 go

CREATE UNIQUE INDEX uiUsername 
on Account(Username)
go

DROP PROCEDURE if exists spValidateUser
go

create procedure spValidateUser
@Username varchar(20),
@Password varchar(100)
as 
begin
	select AccountID, Username
	from Account
	where Username = @Username and
		Password = HASHBYTES('SHA2_256', CONCAT(PasswordSalt, @Password))
end
go