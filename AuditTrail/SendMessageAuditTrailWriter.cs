using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace ZPF.AT;

public class SendMessageAuditTrailWriter : IAuditTrailWriter
{
   private readonly string outputFile;
   private readonly bool DebugOutput;

   /// <summary>
   /// 
   /// </summary>
   /// <param name="outputFile"></param>
   /// <param name="DebugOutput">if or if not System.Diagnostics.Debug.WriteLine</param>
   public SendMessageAuditTrailWriter(string outputFile, bool DebugOutput = true)
   {
      this.outputFile = outputFile ?? throw new ArgumentNullException(nameof(outputFile));
      this.DebugOutput = DebugOutput;
   }

   public void Clean(AuditTrailViewModel sender)
   {
      try
      {
         TStrings Lines = new TStrings();

         Lines.LoadFromFile(outputFile);

         while (Lines.Count > sender.MaxLines)
         {
            Lines.Delete(0);
         }

         Lines.SaveToFile(outputFile, System.Text.Encoding.ASCII);
      }
      catch (Exception ex)
      {
         Debug.WriteLine(ex.Message);

         if (Debugger.IsAttached)
         {
            Debugger.Break();
         }
      }
   }

   public ObservableCollection<AuditTrail> LoadAuditTrail(AuditTrailViewModel sender, bool Filtered = true, long MaxRecords = 500)
   {
      return sender.Logs;
   }

   public void WriteLine(AuditTrailViewModel sender, AuditTrail message)
   {
      try
      {
         string Line = sender.FormatLine(message);

         if (DebugOutput) System.Diagnostics.Debug.WriteLine(Line);

         File.AppendAllLines(outputFile, new[] { Line });

         if ((message.Level == ErrorLevel.Critical) && (string.IsNullOrEmpty(message.DataOut)))
         {
            File.AppendAllLines(outputFile, new[] { message.DataOut + Environment.NewLine });
         }

         // - - -  - - - 

         IntPtr hwnd = FindWindow("MauiMessageReceiverWindow", null);

         if (hwnd == IntPtr.Zero)
         {
            //StatusLabel.Text = "Receiver not found";
            return;
         }

         Line = System.Text.Json.JsonSerializer.Serialize(message);

         SendString(hwnd, Line);
      }
      catch (Exception ex)
      {
         System.Diagnostics.Debug.WriteLine("AuditTrailViewModel: " + ex.Message);
      }
   }

   public long Begin(AuditTrailViewModel sender, AuditTrail message)
   {
      WriteLine(sender, message);
      return 1;
   }

   public void End(AuditTrailViewModel sender, long parent, ErrorLevel errorLevel, string message, string dataOutType, string dataOut)
   {
      AuditTrail at = new AuditTrail { Level = errorLevel, Message = message, DataOutType = dataOutType, DataOut = dataOut };
      WriteLine(sender, at);
   }


   private void SendString(IntPtr hwnd, string message)
   {
      byte[] bytes = System.Text.Encoding.Unicode.GetBytes(message + "\0");

      IntPtr buffer = Marshal.AllocHGlobal(bytes.Length);
      Marshal.Copy(bytes, 0, buffer, bytes.Length);

      COPYDATASTRUCT cds = new()
      {
         dwData = IntPtr.Zero,
         cbData = bytes.Length,
         lpData = buffer
      };

      IntPtr ptrCds = Marshal.AllocHGlobal(Marshal.SizeOf(cds));
      Marshal.StructureToPtr(cds, ptrCds, false);

      SendMessage(hwnd, WM_COPYDATA, IntPtr.Zero, ptrCds);

      Marshal.FreeHGlobal(buffer);
      Marshal.FreeHGlobal(ptrCds);
   }

   private const int WM_COPYDATA = 0x004A;

   [StructLayout(LayoutKind.Sequential)]
   public struct COPYDATASTRUCT
   {
      public IntPtr dwData;
      public int cbData;
      public IntPtr lpData;
   }

   [DllImport("user32.dll", CharSet = CharSet.Unicode)]
   private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

   [DllImport("user32.dll")]
   private static extern IntPtr SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);
}
