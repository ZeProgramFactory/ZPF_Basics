using System;

namespace ZPF;

/// <summary>
///  Represents the version information of an build
/// </summary>
public interface IVersionInfo
{
   /// <summary>
   /// Represents the version number as string
   /// </summary>
   public string sVersion { get; set; }

   /// <summary>
   /// Represents the version number
   /// </summary>
   public Version Version { get; }

   /// <summary>
   /// Represents the build date part of build timestamp as string
   /// </summary>
   public string BuildOn { get; set; }

   public int RevisionNumber { get; }
}
