using System;
using ZPF;


namespace ZPF.AT
{
   public class AuditTrail_Appli
   {
      #region SQLCreate

      public static readonly string SQLCreate_SQLite =
@"
CREATE TABLE IF NOT EXISTS AuditTrail_Appli(
   PK                INTEGER PRIMARY KEY AUTOINCREMENT, 
	DeviceID          varchar(110) NOT NULL,
   LogDate           DATETIME     not null,
	RemoteAddr        varchar(16)  NULL,
	Manufacturer      varchar(64)  NULL,
	DeviceName        varchar(64)  NULL,
	OSVersion         varchar(32)  NULL,
	Application       varchar(32)  NULL,
	AppVersion        varchar(16)  NULL,
	Tag               varchar(64)  NULL,
	Data              text         NULL
);

CREATE INDEX AuditTrail_Appli_01 ON AuditTrail_Appli (DeviceID);
CREATE INDEX AuditTrail_Appli_02 ON AuditTrail_Appli (LogDate);
CREATE INDEX AuditTrail_Appli_03 ON AuditTrail_Appli (Application);
CREATE INDEX AuditTrail_Appli_04 ON AuditTrail_Appli (AppVersion);
";

      public static readonly string SQLCreate_MSSQL =
@"

CREATE TABLE [dbo].[AuditTrail_Appli](
	PK           bigint       IDENTITY(1000,1) NOT NULL,
	DeviceID     varchar(110) NOT NULL,
	LogDate      datetime     DEFAULT (getdate()),
	RemoteAddr   varchar(16)  NULL,
	Manufacturer varchar(64)  NULL,
	DeviceName   varchar(64)  NULL,
	OSVersion    varchar(32)  NULL,
	Application  varchar(32)  NULL,
	AppVersion   varchar(16)  NULL,
	Tag          varchar(64)  NULL,
	Data         text         NULL,
 CONSTRAINT [PK_AuditTrail_Appli] PRIMARY KEY CLUSTERED 
(
	[PK] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY];

CREATE INDEX [IX_AuditTrail_Appli_Tag]   ON [AuditTrail_Appli]([DeviceID] ASC);
CREATE INDEX [IX_AuditTrail_Appli_TS]    ON [AuditTrail_Appli]([LogDate] ASC);
CREATE INDEX [IX_AuditTrail_Appli_LEVEL] ON [AuditTrail_Appli]([Application] ASC);
CREATE INDEX [IX_AuditTrail_Appli_APP]   ON [AuditTrail_Appli]([AppVersion] ASC);
";

      public static readonly string SQLCreate_MySQL =
@"
";

      #endregion

      // - - -  - - - 

      public  AuditTrail_Appli()
      {
      }
   }
}

