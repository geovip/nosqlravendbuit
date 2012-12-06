use master
drop database UITSQLDB
go
create database UITSQLDB
go
use UITSQLDB
go

create table [User]
(
	Id varchar(36) primary key,
	UserName nvarchar(100),
	[Password] nvarchar(100),
	Email nvarchar(100),
	FullName nvarchar(100),
	CreateDate datetime,
)

create table UserGroup
(
	Id varchar(36) primary key,
	UserId varchar(36),
	GroupId varchar(36),
	JoinDate datetime,
	IsApprove varchar(20),
	RoleId varchar(36),
)

create table [Role]
(
	Id varchar(36) primary key,
	RoleName nvarchar(50)
)

create table [Group]
(
	Id varchar(36) primary key,
	GroupName ntext,
	[Description] ntext,
	CreateDate datetime,
	CreateBy varchar(36),
	IsPublic bit
)

create table Topic
(
	Id varchar(36) primary key,
	TopicName ntext,
	Content ntext,
	CreateBy varchar(36),
	GroupId varchar(36),
	CreateDate datetime,
	LastModified datetime,
	NumberOfView int,
	NumberOfComment int
)

create table Comment
(
	Id varchar(36) primary key,
	TopicId varchar(36),
	Content ntext,
	ParentContent ntext,
	CreateBy varchar(36),
	CreateDate datetime,
	IsDeleted bit
)
go
alter table UserGroup add constraint fk_usergroup_user foreign key(UserId) references [User](Id)
go
alter table UserGroup add constraint fk_usergroup_role foreign key(RoleId) references [Role](Id)
go
alter table UserGroup add constraint fk_usergroup_group foreign key(GroupId) references [Group](Id)
go
alter table [Group] add constraint fk_group_user foreign key(CreateBy) references [User](Id)
go
alter table Topic add constraint fk_topic_user foreign key(CreateBy) references [User](Id)
go
alter table Topic add constraint fk_topic_group foreign key(GroupId) references [Group](Id)
go
alter table Comment add constraint fk_comment_user foreign key(CreateBy) references [User](Id)
go
alter table Comment add constraint fk_comment_topic foreign key(TopicId) references Topic(Id)