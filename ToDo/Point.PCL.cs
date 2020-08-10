namespace ZPF.PCL
{
   public class Point
   {
      public Point(int X, int Y)
      {
         this._X = X;
         this._Y = Y;
      }

      private int _X;

      public int X
      {
         get { return _X; }
         set { _X = value; }
      }

      private int _Y;

      public int Y
      {
         get { return _Y; }
         set { _Y = value; }
      }
   }

   public class PointF
   {
      public PointF(double X, double Y)
      {
         this._X = X;
         this._Y = Y;
      }

      private double _X;

      public double X
      {
         get { return _X; }
         set { _X = value; }
      }

      private double _Y;

      public double Y
      {
         get { return _Y; }
         set { _Y = value; }
      }
   }
}

