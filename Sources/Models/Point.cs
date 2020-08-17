namespace ZPF
{
   public class PointI 
   {
      public PointI(int X, int Y)
      {
         this._X = X;
         this._Y = Y;
      }

      /// <summary>
      /// The horizontal position of the point.
      /// </summary>
      public int X
      {
         get { return _X; }
         set { _X = value; }
      }
      private int _X;

      /// <summary>
      /// The horizontal position of the point.
      /// </summary>
      public int Y
      {
         get { return _Y; }
         set { _Y = value; }
      }
      private int _Y;
   }


   public class PointD
   {
      public PointD(decimal X, decimal Y)
      {
         this._X = X;
         this._Y = Y;
      }

      /// <summary>
      /// The horizontal position of the point.
      /// </summary>
      public decimal X
      {
         get { return _X; }
         set { _X = value; }
      }
      private decimal _X;

      /// <summary>
      /// The vertical position of the point.
      /// </summary>
      public decimal Y
      {
         get { return _Y; }
         set { _Y = value; }
      }
      private decimal _Y;
   }


   public class PointF
   {
      public PointF(float X, float Y)
      {
         this._X = X;
         this._Y = Y;
      }

      /// <summary>
      /// The horizontal position of the point.
      /// </summary>
      public float X
      {
         get { return _X; }
         set { _X = value; }
      }
      private float _X;


      /// <summary>
      /// The vertical position of the point.
      /// </summary>
      public float Y
      {
         get { return _Y; }
         set { _Y = value; }
      }
      private float _Y;
   }
}

