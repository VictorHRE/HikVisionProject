create table devices
(
    id               int identity
        constraint PK_devices
            primary key,
    name             nvarchar(200) collate SQL_Latin1_General_CP1_CI_AS not null,
    ipAddress        nvarchar(15) collate SQL_Latin1_General_CP1_CI_AS  not null,
    deviceMacAddress nvarchar(max) collate SQL_Latin1_General_CP1_CI_AS not null,
    username         nvarchar(20) collate SQL_Latin1_General_CP1_CI_AS  not null,
    password         nvarchar(200) collate SQL_Latin1_General_CP1_CI_AS not null,
    port             int                                                not null,
    status           nvarchar(max) collate SQL_Latin1_General_CP1_CI_AS not null,
    createdAt        datetime2                                          not null,
    updatedAt        datetime2                                          not null
)
go


create table employeeAttendances
(
    Id             int identity,
    employeeNumber nvarchar(450) collate SQL_Latin1_General_CP1_CI_AS not null,
    attendanceType nvarchar(max) collate SQL_Latin1_General_CP1_CI_AS not null,
    time           datetime2                                          not null,
    constraint PK_employeeAttendances
        primary key (Id, employeeNumber)
)
go


create table employees
(
    Id             int identity
        constraint PK_employees
            primary key,
    identification nvarchar(450) collate SQL_Latin1_General_CP1_CI_AS not null,
    Name           nvarchar(200) collate SQL_Latin1_General_CP1_CI_AS not null,
    lastName       nvarchar(200) collate SQL_Latin1_General_CP1_CI_AS not null,
    position       nvarchar(100) collate SQL_Latin1_General_CP1_CI_AS,
    phone          nvarchar(20) collate SQL_Latin1_General_CP1_CI_AS  not null,
    email          nvarchar(200) collate SQL_Latin1_General_CP1_CI_AS,
    branchId       int                                                not null,
    status         nvarchar(max) collate SQL_Latin1_General_CP1_CI_AS not null,
    userType       nvarchar(max) collate SQL_Latin1_General_CP1_CI_AS,
    gender         nvarchar(max) collate SQL_Latin1_General_CP1_CI_AS,
    beginTime      datetime2                                          not null,
    endTime        datetime2                                          not null,
    birthDate      datetime2                                          not null,
    createdAt      datetime2                                          not null
)
go

create unique index IX_employees_identification
    on employees (identification)
go

create table eventLogs
(
    Id                     int identity
        constraint PK_eventLogs
            primary key,
    data                   nvarchar(max) collate SQL_Latin1_General_CP1_CI_AS,
    eventType              nvarchar(max) collate SQL_Latin1_General_CP1_CI_AS,
    createdAt              datetime2                                                       not null,
    IdStoreHQ              nvarchar(max) default N'0' collate SQL_Latin1_General_CP1_CI_AS not null,
    EmployeeIdentification nvarchar(max) default N'-' collate SQL_Latin1_General_CP1_CI_AS not null
)
go

