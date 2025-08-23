using System;

namespace ZPF
{
   public interface IVersionInfo
   {
      public string sVersion { get; }
      public Version Version { get; }

      public string BuildOn { get; }
      public int RevisionNumber { get; }
   }
}
