using SkiaSharp;

namespace ZPF
{
   public static class ColorExtensionsSkia
   {
      public static SKColor ToSKColor(this ZPF.Color color)
      {
         try
         {
            return new SKColor((byte)(color.Red * 255), (byte)(color.Green * 255), (byte)(color.Blue * 255), (byte)(color.Alpha * 255));
         }
         catch
         {
         };

         return SKColors.Gray;
      }

      public static ZPF.Color ToColor(this SKColor color)
      {
         return new Color(color.Alpha / 255, color.Red / 255, color.Green / 255, color.Blue / 255);
      }

   }
}
