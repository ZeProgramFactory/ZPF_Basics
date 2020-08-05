
using System;
using ZPF.SQL;

namespace ZPF
{
   public class NameValue
   {
      public NameValue()
      {
      }

      public NameValue(string name, string value)
      {
         this.Name = name;
         this.Value = value;
         this.Tag = null;
      }

      public string Name { get; set; }
      public string Value { get; set; }

      public object Tag { get; set; }

      // - - -  - - - 

      public NameValue Copy()
      {
         return new NameValue { Name=Name, Value=Value, Tag=Tag };
      }

      // - - -  - - - 

      public override string ToString()
      {
         return $"{Name}=(string){Value}";
      }
   }


   public class NameValue_UInt64
   {
      public NameValue_UInt64()
      {
      }

      public NameValue_UInt64(string name, UInt64 value)
      {
         this.Name = name;
         this.Value = value;
      }

      public string Name { get; set; }
      public UInt64 Value { get; set; }

      public object Tag { get; set; }

      // - - -  - - - 

      public override string ToString()
      {
         return $"{Name}=(UInt64){Value}";
      }
   }


   public class NameValue_Int64
   {
      public NameValue_Int64()
      {
      }

      public NameValue_Int64(string name, Int64 value)
      {
         this.Name = name;
         this.Value = value;
      }

      public string Name { get; set; }

      [DB_Attributes.IgnoreIntType]
      public Int64 Value { get; set; }

      public object Tag { get; set; }

      // - - -  - - - 

      public override string ToString()
      {
         return $"{Name}=(Int64){Value}";
      }
   }


   public class NameValue_Decimal
   {
      public NameValue_Decimal()
      {
      }

      public NameValue_Decimal(string name, Decimal value)
      {
         this.Name = name;
         this.Value = value;
      }

      public string Name { get; set; }
      public Decimal Value { get; set; }

      public object Tag { get; set; }

      // - - -  - - - 

      public override string ToString()
      {
         return $"{Name}=(Decimal){Value}";
      }
   }


   public class NameValue_Double
   {
      public NameValue_Double()
      {
      }

      public NameValue_Double(string name, Double value)
      {
         this.Name = name;
         this.Value = value;
      }

      public string Name { get; set; }
      public Double Value { get; set; }

      public object Tag { get; set; }

      // - - -  - - - 

      public override string ToString()
      {
         return $"{Name}=(Double){Value}";
      }
   }

   public class NameValue_DateTime
   {
      public NameValue_DateTime()
      {
      }

      public NameValue_DateTime(string name, DateTime dt)
      {
         this.Name = name;
         this.Value = dt;
      }

      public string Name { get; set; }
      public DateTime Value { get; set; }

      public object Tag { get; set; }

      // - - -  - - - 

      public override string ToString()
      {
         return $"{Name} (DateTime) ({Value.ToString()})";
      }
   }

   public class NameValue_Point
   {
      public NameValue_Point()
      {
      }

      public NameValue_Point(string name, Double X, Double Y)
      {
         this.Name = name;
         this.X = X;
         this.Y = Y;
      }

      public string Name { get; set; }
      public Double X { get; set; }
      public Double Y { get; set; }

      public object Tag { get; set; }

      // - - -  - - - 

      public override string ToString()
      {
         return $"{Name} (Double x,y) ({X},{Y})";
      }
   }

   public class NameValue_Coord
   {
      public NameValue_Coord()
      {
      }

      public NameValue_Coord(string name, Double Lat, Double Lon)
      {
         this.Name = name;
         this.Lat = Lat;
         this.Lon = Lon;
      }

      public string Name { get; set; }
      public Double Lat { get; set; }
      public Double Lon { get; set; }

      public object Tag { get; set; } = null;

      // - - -  - - - 

      public override string ToString()
      {
         return $"{Name} (Double Lat,Lon) ({Lat},{Lon})";
      }
   }
}
