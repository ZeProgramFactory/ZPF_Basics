using System;

namespace ZPF.Graphics
{
   /// <summary>
   /// Based on ColorHelper.cs, ...
   /// 
   /// 2017..2020 ZePocketForge.com, SAS ZPF
   /// </summary>
   public class Color
   {
      public static Color Default
      {
         get { return new Color(1, 1, 1, 1); }
      }

      public Color()
      {
         // Black
         _Alpha = 1.0m;
         _Red = 0.0m;
         _Green = 0.0m;
         _Blue = 0.0m;
      }

      public Color(byte red, byte green, byte blue)
      {
         _Alpha = 1.0m;
         _Red = red / 255.0M;
         _Green = green / 255.0M;
         _Blue = blue / 255.0M;
      }

      public Color(decimal red, decimal green, decimal blue)
      {
         _Alpha = 1.0m;
         _Red = red;
         _Green = green;
         _Blue = blue;
      }

      public Color(byte alpha, byte red, byte green, byte blue)
      {
         Alpha = alpha / 255.0M;
         _Red = red / 255.0M;
         _Green = green / 255.0M;
         _Blue = blue / 255.0M;
      }

      public Color(decimal alpha, decimal red, decimal green, decimal blue)
      {
         Alpha = alpha;
         _Red = red;
         _Green = green;
         _Blue = blue;
      }


      /// <summary>
      /// Gets the alpha component of the color.
      /// </summary>
      public decimal Alpha
      {
         get => _Alpha;
         set
         {
            _Alpha = value;

            if (_Alpha > 1)
            {
               _Alpha = value / 255;
            };
         }
      }
      decimal _Alpha = 0;

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 

      static uint ToHex(char c)
      {
         ushort x = (ushort)c;
         if (x >= '0' && x <= '9')
            return (uint)(x - '0');

         x |= 0x20;
         if (x >= 'a' && x <= 'f')
            return (uint)(x - 'a' + 10);
         return 0;
      }

      static uint ToHexD(char c)
      {
         var j = ToHex(c);
         return (j << 4) | j;
      }

      public static Color Parse(string hex)
      {
         // Undefined
         if (hex.Length < 3) return Default;

         int idx = (hex[0] == '#') ? 1 : 0;

         switch (hex.Length - idx)
         {
            case 3: //#rgb => ffrrggbb
               var t1 = ToHexD(hex[idx++]);
               var t2 = ToHexD(hex[idx++]);
               var t3 = ToHexD(hex[idx]);

               return new Color((byte)t1, (byte)t2, (byte)t3);

            case 4: //#argb => aarrggbb
               var f1 = ToHexD(hex[idx++]);
               var f2 = ToHexD(hex[idx++]);
               var f3 = ToHexD(hex[idx++]);
               var f4 = ToHexD(hex[idx]);
               return new Color((byte)f1, (byte)f2, (byte)f3, (byte)f4);

            case 6: //#rrggbb => ffrrggbb
               return new Color((byte)(ToHex(hex[idx++]) << 4 | ToHex(hex[idx++])),
                     (byte)(ToHex(hex[idx++]) << 4 | ToHex(hex[idx++])),
                     (byte)(ToHex(hex[idx++]) << 4 | ToHex(hex[idx])));

            case 8: //#aarrggbb
               var a1 = ToHex(hex[idx++]) << 4 | ToHex(hex[idx++]);

               return new Color((byte)a1, (byte)(ToHex(hex[idx++]) << 4 | ToHex(hex[idx++])),
                     (byte)(ToHex(hex[idx++]) << 4 | ToHex(hex[idx++])),
                     (byte)(ToHex(hex[idx++]) << 4 | ToHex(hex[idx])));

            default: //everything else will result in unexpected results
               return Default;
         }
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 

      public string ToHex(byte NbDigits = 8)
      {
         if (NbDigits == 6)
         {
            return $"{(byte)(_Red * 255):x2}{(byte)(_Green * 255):x2}{(byte)(_Blue * 255):x2}";
         }
         else
         {
            return $"{(byte)(_Alpha * 255):x2}{(byte)(_Red * 255):x2}{(byte)(_Green * 255):x2}{(byte)(_Blue * 255):x2}";
         };
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 

      /// <summary>
      /// Gets the red component of the color.
      /// </summary>
      public decimal Red { get => _Red; set => _Red = value; }
      decimal _Red = 0;

      /// <summary>
      /// Gets the green component of the color.
      /// </summary>
      public decimal Green { get => _Green; set => _Green = value; }
      decimal _Green = 0;

      /// <summary>
      /// Gets the blue component of the color.
      /// </summary>
      public decimal Blue { get => _Blue; set => _Blue = value; }
      decimal _Blue = 0;


      ////
      //// Summary:
      ////     Converts the hexadecimal string representation of a color to its SkiaSharp.SKColor
      ////     equivalent.
      ////
      //// Parameters:
      ////   hexString:
      ////     The hexadecimal string representation of a color.
      ////
      //// Returns:
      ////     The new SkiaSharp.SKColor instance.
      ////
      //// Remarks:
      ////     This method can parse a string in the forms with or without a preceding '#' character:
      ////     AARRGGB, RRGGBB, ARGB, RGB.
      //public static SKColor Parse(string hexString);

      /// <summary>
      /// Returns a new color based on this current instance, but with the new alpha channel value.
      /// </summary>
      /// <param name="alpha">The new alpha component.</param>
      /// <returns></returns>
      public Color WithAlpha(byte alpha)
      {
         return new ZPF.Graphics.Color((byte)(alpha / 255), Red, Green, Blue);
      }

      /// <summary>
      /// Returns a new color based on this current instance, but with the new alpha channel value.
      /// </summary>
      /// <param name="alpha">The new alpha component.</param>
      /// <returns></returns>
      public Color WithAlpha(decimal alpha)
      {
         return new ZPF.Graphics.Color(alpha, Red, Green, Blue);
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 

      public override string ToString()
      {
         return ToHex();
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 
   }
}
