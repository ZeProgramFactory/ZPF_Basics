using System;
using ZPF;
using ZPF.SQL;

namespace ZPF.AT
{
   public class AuditTrail_App
   {
      #region SQLCreate

      public static readonly string PostScript_SQLite = @"
";

      public static readonly string PostScript_MSSQL = @"
";

      public static readonly string PostScript_MySQL = @"
";

      public static readonly string PostScript_PGSQL = @"
";

      #endregion

      // - - -  - - - 

      public AuditTrail_App()
      {
      }

      [DB_Attributes.PrimaryKey(true)]
      public Int64 PK { get; set; }

      [DB_Attributes.Index()]
      [DB_Attributes.MaxLength(128)]
      [DB_Attributes.IsMandatory]
      public string DeviceID { get; set; }

      [DB_Attributes.IsMandatory]
      [DB_Attributes.Index()]
      public DateTime LogDate { get; set; }

      [DB_Attributes.MaxLength(16)]
      public string RemoteAddr { get; set; }

      [DB_Attributes.MaxLength(64)]
      public string Manufacturer { get; set; }

      [DB_Attributes.MaxLength(64)]
      public string DeviceName { get; set; }

      [DB_Attributes.MaxLength(32)]
      public string OSVersion { get; set; }

      [DB_Attributes.Index()]
      [DB_Attributes.MaxLength(32)]
      public string Application { get; set; }

      [DB_Attributes.Index()]
      [DB_Attributes.MaxLength(16)]
      public string AppVersion { get; set; }

      [DB_Attributes.MaxLength(1024)]
      public string Data { get; set; }

      public Int64 Tag { get; set; }
   }
}

