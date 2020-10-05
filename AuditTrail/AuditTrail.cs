using System;
using System.Data;
using ZPF;
using ZPF.SQL;

namespace ZPF.AT
{
   public enum ErrorLevel { Log = 0, Info = 1, Error = 2, Critical = 3 }

   public class AuditTrail
   {
      #region SQLCreate
      public static readonly string SQLCreate_SQLite =
@"
CREATE TABLE IF NOT EXISTS AuditTrail(
   PK                INTEGER PRIMARY KEY AUTOINCREMENT, 
   TimeStamp         DATETIME,
   TimeStampApp      DATETIME,
   TimeStampDB       DATETIME,
   Parent            INTEGER, 
   IsBusiness        BOOLEAN default 0,
   Level             int,

   Application       varchar(32),
   Tag               VARCHAR(128) not null,
   Message           VARCHAR(1024) not null,

   Ticks             INTEGER, 
   [DataInType]      VARCHAR (16)  NULL,
   [DataIn]          TEXT           NULL,
   [DataOutType]     VARCHAR (16)  NULL,
   [DataOut]         TEXT           NULL,

   [TerminalID]      VARCHAR (128)  NULL,
   [TerminalIP]      VARCHAR (128)  NULL,
   [FKUser]          VARCHAR (128)  NULL,

   [ItemID]          VARCHAR (128)  NULL,
   [ItemType]        VARCHAR (64)   NULL,
   [FKExtern]        INTEGER
);

CREATE INDEX AuditTrail_01 ON AuditTrail (Tag);
CREATE INDEX AuditTrail_02 ON AuditTrail (TimeStamp);
CREATE INDEX AuditTrail_03 ON AuditTrail (Level);
CREATE INDEX AuditTrail_04 ON AuditTrail (Application);

CREATE VIEW [V_AuditTrail_Last100]
AS
SELECT TOP (100) PK, TimeStamp, [Level], Tag, Message, DataIn, DataOut, TerminalID, FKUser, ItemID, ItemType
FROM   AuditTrail
ORDER BY PK DESC;
";

      public static readonly string SQLCreate_MSSQL =
@"
CREATE TABLE [dbo].[AuditTrail] (
   [PK]             BIGINT         IDENTITY (1000, 1) NOT NULL,
   [TimeStamp]      DATETIME       DEFAULT (getdate()) NOT NULL,
   [TimeStampApp]   DATETIME       ,
   [TimeStampDB]    DATETIME       DEFAULT (getdate()),
   [Parent]         BIGINT         NULL,
   [IsBusiness]     BIT            DEFAULT ((0)) NULL,
   [Level]          INT            NOT NULL,

   [Application] VARCHAR (32)   NULL,
   [Tag]         VARCHAR (128)  NOT NULL,
   [Message]     VARCHAR (1024) NOT NULL,

   [Ticks]           BIGINT         NULL,
   [DataInType]      VARCHAR (16)  NULL,
   [DataIn]          TEXT           NULL,
   [DataOutType]     VARCHAR (16)  NULL,
   [DataOut]         TEXT           NULL,

   [TerminalID]  VARCHAR (128)  NULL,
   [TerminalIP]  VARCHAR (128)  NULL,
   [FKUser]      VARCHAR (128)  NULL,

   [ItemID]          VARCHAR (128)  NULL,
   [ItemType]        VARCHAR (64)   NULL,
   [FKExtern]        BIGINT,

    CONSTRAINT [PK_AuditTrail] PRIMARY KEY CLUSTERED ([PK] ASC)
);

CREATE INDEX [IX_AuditTrail_Tag]   ON [AuditTrail]([Tag] ASC);
CREATE INDEX [IX_AuditTrail_TS]    ON [AuditTrail]([TimeStamp] ASC);
CREATE INDEX [IX_AuditTrail_LEVEL] ON [AuditTrail]([Level] ASC);
CREATE INDEX [IX_AuditTrail_APP]   ON [AuditTrail]([Application] ASC);
  
EXECUTE('CREATE VIEW [V_AuditTrail_Last100]
AS
SELECT TOP (100) PK, TimeStamp, [Level], Tag, Message, DataIn, DataOut, TerminalID, FKUser, ItemID, ItemType
FROM   AuditTrail
ORDER BY PK DESC;');
";

      public static readonly string SQLCreate_MySQL =
@"
NOT UP TO DATE

CREATE TABLE `AuditTrail` (
	`PK` INT(11) NOT NULL AUTO_INCREMENT,
	`TimeStamp` DATETIME NOT NULL,
	`TimeStampApp` DATETIME,
	`TimeStampDB` DATETIME,
	`Level` INT(11) NOT NULL,
	`Tag` VARCHAR(128) NOT NULL,
	`Message` VARCHAR(1024) NOT NULL,
	`Data` TEXT NULL,
	`TerminalID` VARCHAR(128) NULL DEFAULT NULL,
	`FKUser` VARCHAR(128) NULL DEFAULT NULL,
	`ItemID` VARCHAR(128) NULL DEFAULT NULL,
	`ItemType` VARCHAR(128) NULL DEFAULT NULL,
	PRIMARY KEY (`PK`),
	INDEX `Tag` (`Tag`),
	INDEX `TimeStamp` (`TimeStamp`)
)
COLLATE='latin1_swedish_ci'
ENGINE=InnoDB;
";


#if DEVAO
     public static readonly string SQLCreate_PGSQL =
@"
CREATE TABLE public.audittrail (
   pk BIGSERIAL,
   ""timestamp"" TIMESTAMP WITHOUT TIME ZONE NOT NULL, --DEFAULT now()
   timestampapp TIMESTAMP WITHOUT TIME ZONE,
   timestampdb TIMESTAMP WITHOUT TIME ZONE DEFAULT now(),
   parent BIGINT,
   isbusiness BOOLEAN DEFAULT false,
   level INTEGER NOT NULL,
   tag VARCHAR(128) NOT NULL,
   application VARCHAR(32),
   message VARCHAR(1024) NOT NULL,
   ticks BIGINT,
   datain TEXT,
   dataintype VARCHAR(128),
   dataout TEXT,
   dataouttype VARCHAR(128),
   terminalid VARCHAR(128),
   terminalip VARCHAR(128),
   fkuser VARCHAR(128),
   itemid VARCHAR(128),
   itemtype VARCHAR(128),
   fkstructure BIGINT DEFAULT '-1'::integer,
   CONSTRAINT pk_audittrail PRIMARY KEY(pk)
) 
WITH(oids = true);

CREATE INDEX ix_audittrail_app ON public.audittrail
   USING btree(application);

CREATE INDEX ix_audittrail_level ON public.audittrail
   USING btree(level);

CREATE INDEX ix_audittrail_tag ON public.audittrail
   USING btree(tag);

CREATE INDEX ix_audittrail_ts ON public.audittrail
   USING btree(""timestamp"");
";
#else
      public static readonly string SQLCreate_PGSQL =
@"
CREATE TABLE public.audittrail (
   pk BIGSERIAL,
   ""timestamp"" TIMESTAMP WITHOUT TIME ZONE NOT NULL, --DEFAULT now()
   timestampapp TIMESTAMP WITHOUT TIME ZONE,
   timestampdb TIMESTAMP WITHOUT TIME ZONE DEFAULT now(),
   parent BIGINT,
   isbusiness BOOLEAN DEFAULT false,
   level INTEGER NOT NULL,
   tag VARCHAR(128) NOT NULL,
   application VARCHAR(32),
   message VARCHAR(1024) NOT NULL,
   ticks BIGINT,
   datain TEXT,
   dataintype VARCHAR(128),
   dataout TEXT,
   dataouttype VARCHAR(128),
   terminalid VARCHAR(128),
   terminalip VARCHAR(128),
   fkuser VARCHAR(128),
   itemid VARCHAR(128),
   itemtype VARCHAR(128),
   fkextern BIGINT DEFAULT '-1'::integer,
   CONSTRAINT pk_audittrail PRIMARY KEY(pk)
) 
WITH(oids = true);

CREATE INDEX ix_audittrail_app ON public.audittrail
   USING btree(application);

CREATE INDEX ix_audittrail_level ON public.audittrail
   USING btree(level);

CREATE INDEX ix_audittrail_tag ON public.audittrail
   USING btree(tag);

CREATE INDEX ix_audittrail_ts ON public.audittrail
   USING btree(""timestamp"");
";
#endif
      #endregion

      // - - -  - - - 

      public static TStrings Dico = TStrings.FromJSon("[{\"1\":\"Log\"}, {\"2\":\"Info\"}, {\"3\":\"Error\"}, {\"4\":\"Critical\"} ]");
      public enum TextFormat { Txt = 0, TxtEx = 1, HTML = 2 }

      // - - -  - - - 

      public AuditTrail()
      {
         TimeStamp = DateTime.Now;
         Level = ErrorLevel.Log;
         Tag = "";

#if DEVAO
         FKStructure = -1;
#else
         FKExtern = -1;
#endif

      }

      public AuditTrail(Exception ex, TextFormat textFormat = TextFormat.Txt)
      {
         TimeStamp = DateTime.Now;

         Level = ErrorLevel.Critical;
         Tag = "Exception";

#if DEVAO
         FKStructure = -1;
#else
         FKExtern = -1;
#endif

         if (ex != null)
         {
            Message = ex.Message;

            switch (textFormat)
            {
               default:
               case TextFormat.Txt:
                  DataOutType = "TXT";
                  DataOut = ex.Message + Environment.NewLine + ex.StackTrace + Environment.NewLine + Environment.NewLine + ex.Source;
                  break;

               case TextFormat.TxtEx:
                  DataOutType = "TXTEx";

                  DataOut = "!" + ex.Message + Environment.NewLine
                  + Environment.NewLine
                  + "#StackTrace" + Environment.NewLine
                  + ex.StackTrace + Environment.NewLine
                  + Environment.NewLine
                  + "#Source" + Environment.NewLine
                  + ex.Source;
                  break;

               case TextFormat.HTML:
                  DataOutType = "HTML";

                  TStrings html = new TStrings();

                  html.Add("<p style='font-family: Consolas,monospace; bgcolor=transparent;'>");
                  html.Add(string.Format("{0}</br>", "Message"));
                  html.Add(string.Format("<b>{0}</b></br></br>", ex.Message));

                  html.Add(string.Format("{0}</br>", "StackTrace"));
                  html.Add(string.Format("<b>{0}</b></br></br>", ex.StackTrace));

                  html.Add(string.Format("{0}</br>", "Source"));
                  html.Add(string.Format("<b>{0}</b></br></br>", ex.Source));
                  html.Add("</p>");

                  DataOut = html.Text;
                  break;
            };
         };
      }

      // - - -  - - - 

      [DB_Attributes.PrimaryKey]
      public Int64 PK { get; set; }

      public DateTime TimeStamp { get; set; }
      public DateTime TimeStampApp { get; set; }
      public DateTime TimeStampDB { get; set; }

      public ErrorLevel Level { get; set; }


      [DB_Attributes.Ignore]
      public string sLevel
      {
         get
         {
            switch (Level)
            {
               case ErrorLevel.Log: return Dico["1"];
               case ErrorLevel.Info: return Dico["2"];
               case ErrorLevel.Error: return Dico["3"];
               case ErrorLevel.Critical: return Dico["4"];
               default: return Dico["4"];
            };
         }
      }

      public Int64 Parent { get; set; }
      public bool IsBusiness { get; set; }
      public string Tag { get; set; }
      public string Application { get; set; }
      public string Message { get; set; }

      public Int64 Ticks { get; set; }

      [DB_Attributes.Ignore]
      public TimeSpan Duration
      {
         get => new TimeSpan(Ticks);
         set => Ticks = value.Ticks;
      }

      public string DataIn { get; set; }
      public string DataInType { get; set; }
      public string DataOut { get; set; }
      public string DataOutType { get; set; }

      public string TerminalID { get; set; }
      public string TerminalIP { get; set; }

      public string FKUser { get; set; }
      public string ItemID { get; set; }
      public string ItemType { get; set; }

#if DEVAO
      public Int64 FKStructure { get; set; }
#else
      public Int64 FKExtern { get; set; }
#endif
      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 

      [DB_Attributes.Ignore]
      public bool HasData { get { return !string.IsNullOrEmpty(DataIn) || !string.IsNullOrEmpty(DataOut); } }

      // - - -  - - - 

      [DB_Attributes.Ignore]
      public string Foreground
      {
         get
         {
            switch (Level)
            {
               case ErrorLevel.Log: return (Ticks > 0 ? "#FF000000" /*Brushes.Black*/ : "#FF708090" /*Brushes.SlateGray*/);
               case ErrorLevel.Info: return (Ticks > 0 ? "#FF000000" /*Brushes.Black*/ : "#FFFFFFFF" /*Brushes.White*/);
               default: return "#FFFFFFFF" /*Brushes.White*/;
            };
         }
      }

      [DB_Attributes.Ignore]
      public string Background
      {
         get
         {
            switch (Level)
            {
               case ErrorLevel.Log: return (Ticks > 0 ? "#FF008000" /*Brushes.Green*/ : "#FFF5F5F5" /*Brushes.WhiteSmoke*/);
               case ErrorLevel.Info: return (Ticks > 0 ? "#FF008000" /*Brushes.Green*/ : "#FFF5DEB3" /*Brushes.Wheat*/);
               case ErrorLevel.Error: return "#FFFA8072" /*Brushes.Salmon*/;
               case ErrorLevel.Critical: return "#FF8B0000" /*Brushes.DarkRed*/;
            };

            return "#FF800080" /*Brushes.Purple*/;
         }
      }

      // - - -  - - -

      public override string ToString()
      {
         return $"{TimeStamp.ToString("HH:mm:ss")} { (new String('*', (int)Level) + "----" ).Left(4) } {Tag.Left(10)} {Message}";
      }
   }
}
