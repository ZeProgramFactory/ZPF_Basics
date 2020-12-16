
using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace ZPF
{
   /// <summary>
   /// TStrings is the base class for objects that represent a list of strings.
   /// 
   /// 20/04/06 - ME  - Add: public string this[ int Index ] ... set
   /// 01/08/06 - ME  - Share rights with Tiki Labs
   /// 15/08/06 - ME  - Bugfix: public string Names( int Index );
   /// 16/08/06 - ME  - Bugfix: LoadFromFile
   /// 25/08/06 - ME  - Bugfix: public string this[ string Name ]
   /// 06/10/06 - ME  - Code review 
   /// 31/01/07 - ME  - Bugfix: Names: if Count=0 
   /// 12/02/07 - ME  - Bugfix: IndexOfName( string Name ) "\R\N"
   ///                          this[ string Name ]
   /// 29/05/07 - ME  - Add: _IgnoreSPC
   /// 17/09/09 - ME  - Add: HTML
   /// 12/12/11 - ME  - Add: Add basic support for Windows Phone
   /// 12/12/10 - ME  - Add: Constructor to class TStringsItem
   /// 01/01/11 - ME  - Add: Add file support for Windows Phone ( IsolatedStorage ) LoadFromFile
   /// 24/03/11 - ME  - Bugfix: Insert(int Index, string st) - if( Index == 0 && Count == 0 )
   /// 18/02/12 - ME  - Add: Add file support for Windows Phone ( IsolatedStorage ) SaveToFile
   /// 11/11/12 - ME  - Add: NETFX_CORE --> Windows 8 RT: LoadFromFile, SaveToFile, Clear, TStrings()
   /// 19/12/12 - ME  - Update: public TStrings(), public void Clear() --> WP8
   /// 10/01/12 - ME  - Bugfix: IndexOfName if there is no value (--> no = )
   /// 29/03/13 - ME  - Bugfix: WP: Add(string st)
   /// 21/12/13 - ME  - Add: Push / Pop object
   /// 03/11/14 - ME  - Add: Stream SaveToStream()
   /// 12/11/14 - ME  - Add: public void Sort()
   /// 11/01/15 - ME  - Add: LoadFromFile: MaxSize
   /// 06/02/15 - ME  - Add: Reengeniering: Conditional compilation: Load / SaveSteam
   /// 10/09/15 - ME  - Error(string Msg, int Data) if (Debugger.IsAttached) Break();
   /// 18/04/16 - ME  - Add: bool Rename(string OldName, string NewName);
   /// 31/10/16 - ME  - Add: void AddStrings( TStrings );
   /// 12/02/17 - ME  - Add: int Add(string name, string value)
   /// 22/02/17 - ME  - BugFix: public async Task<bool> LoadFromFile(FileName, Encoding, MaxSize ) file in subfolder
   /// 22/02/17 - ME  - BugFix: public async Task<bool> SaveToFile(FileName, Encoding) file in subfolder
   /// 05/06/17 - ME  - Add: public string this[int Index1, int Index]
   /// 05/06/17 - ME  - Add: public string this[string Name, int Index]
   /// 27/08/17 - ME  - BugFix (Q&D): TStrings FromJSon(string JSON)
   /// 18/09/17 - ME  - BugFix: TStrings FromJSon(string JSON)
   /// 14/12/17 - ME  - Code review
   /// 01/04/18 - ME  - Add: Innerlist.Insert
   /// 21/05/18 - ME  - NETSTANDARD1_4
   /// 23/05/18 - ME  - Bugfix: Insert(int Index, string st)
   /// 20/09/18 - ME  - Bugfix: Text
   /// 
   /// 2005..2018 ZePocketForge.com, SAS ZPF
   /// </summary>

   public class TStrings
   {
      class TStringsItem
      {
         public string FString;
         public Object FObject = null;

         public TStringsItem()
         {
         }

         public TStringsItem(string st)
         {
            FString = st;
         }

         public override string ToString()
         {
            return FString;
         }
      };

      //string SSortedListError    = "Operation not allowed on sorted string list";
      //string SDuplicateString    = "String list does not allow duplicates";
      string SListIndexError = "List index out of bounds ({0})";
      //string SListCapacityError  = "List capacity out of bounds ({0})";
      //string SListCountError     = "List count out of bounds ({0})";

      const int NEW_SIZE = 16;
      Array InnerList = null;

      public TStrings()
      {
         _IgnoreSPC = false;
         InnerList = Array.CreateInstance(Type.GetType(typeof(TStringsItem).AssemblyQualifiedName), NEW_SIZE);
      }

      ~TStrings()
      {
         Clear();
      }

      public void Clear()
      {
         InnerList = Array.CreateInstance(Type.GetType(typeof(TStringsItem).AssemblyQualifiedName), NEW_SIZE);
         _Count = 0;
      }

      int _Count = 0;

      public int Count
      {
         get
         {
            return _Count;
         }
      }

      /******************************************************************************/

      public void Sort()
      {
         bool flag = true;
         string sTemp;
         object oTemp;

         // sorting an array
         for (int i = 1; (i <= (this.Count - 1)) && flag; i++)
         {
            flag = false;

            for (int j = 0; j < (this.Count - 1); j++)
            {
               if (this[j + 1].CompareTo(this[j]) < 0)
               {
                  sTemp = this[j];
                  oTemp = this.GetObject(j);

                  this[j] = this[j + 1];
                  this.SetObject(j, this.GetObject(j + 1));

                  this[j + 1] = sTemp;
                  this.SetObject(j + 1, oTemp);

                  flag = true;
               }
            }
         }
      }

      /******************************************************************************/

      private void Check(int Index)
      {
         if ((Index < 0) || (Index >= _Count))
         {
            Error(SListIndexError, Index);
         }
      }

      /******************************************************************************/

      public string Get(int Index)
      {
         Check(Index);
         return (InnerList.GetValue(Index) as TStringsItem).ToString();
      }

      /// <summary>
      /// Call AddStrings to add the strings from another TStrings object to the list. If both the source and destination TStrings objects support objects associated with their strings, references to the associated objects will be added as well.  
      /// </summary>
      /// <param name="strings"></param>
      public void AddStrings(TStrings strings)
      {
         for (int i = 0; i < strings.Count; i++)
         {
            int ind = this.Add(strings[i]);

            if (strings.GetObject(i) != null)
            {
               this.SetObject(ind, strings.GetObject(i));
            };
         };
      }

      void Put(int Index, string S)
      {
         Check(Index);

         TStringsItem si = InnerList.GetValue(Index) as TStringsItem;
         si.FString = S;
         InnerList.SetValue(si, Index);
      }

      /******************************************************************************/

      /// <summary>
      /// Returns the object associated with the string at a specified index. 
      /// </summary>
      /// <param name="Index">Index is the index of the string with which the object is associated.</param>
      /// <returns></returns>
      public Object GetObject(int Index)
      {
         Check(Index);

         return (InnerList.GetValue(Index) as TStringsItem).FObject;
      }

      public bool Rename(string OldName, string NewName)
      {
         if (string.IsNullOrEmpty(OldName)) return false;
         if (string.IsNullOrEmpty(NewName)) return false;

         int Ind = IndexOfName(OldName);

         if (Ind > -1)
         {
            this[Ind] = NewName + "=" + this.ValueFromIndex(Ind);
            return true;
         };

         return false;
      }

      public Object GetObject(string Name)
      {
         Object Result;
         int i;

         i = IndexOfName(Name);

         if (i >= 0)
         {
            Result = GetObject(i);
         }
         else
         {
            Result = null;
         };

         return (Result);
      }

      /// <summary>
      /// Changes the object associated with the string at a specified index.
      /// </summary>
      /// <param name="Index"></param>
      /// <param name="Obj"></param>
      public void SetObject(int Index, Object Obj)
      {
         Check(Index);

         TStringsItem si = InnerList.GetValue(Index) as TStringsItem;
         si.FObject = Obj;
         InnerList.SetValue(si, Index);
      }

      /// <summary>
      /// Adds a string to the list, and associates an object with the string. 
      /// </summary>
      /// <param name="Value"></param>
      /// <param name="Obj"></param>
      /// <returns></returns>

      public int AddObject(string Value, Object Obj)
      {
         int Result;

         Result = Add(Value);
         TStringsItem si = InnerList.GetValue(Result) as TStringsItem;
         si.FObject = Obj;
         InnerList.SetValue(si, Result);

         return (Result);
      }

      /******************************************************************************/

      public int Add(string name, string value)
      {
         if (string.IsNullOrEmpty(name))
         {
            return -1;
         };

         string format = (string.IsNullOrEmpty(value) ? "{0}" : "{0}={1}");

         return Add(string.Format(format, name, value));
      }

      /// <summary>
      /// Adds a string at the end of the list.
      /// </summary>
      /// <param name="st"></param>
      /// <returns>Returns the index of the new string.</returns>

      public int Add(string st)
      {
         TStringsItem StringItem = new TStringsItem();
         StringItem.FString = st;
         int Result = -1;

         if (_Count + 1 > InnerList.Length)
         {
            Array tmp = Array.CreateInstance(Type.GetType(typeof(TStringsItem).AssemblyQualifiedName), InnerList.Length + NEW_SIZE);

            InnerList.CopyTo(tmp, 0);
            InnerList = tmp;
         }

         InnerList.SetValue(new TStringsItem(st), _Count);
         Result = _Count;

         _Count += 1;

         return (Result);
      }

      /// <summary>
      /// Inserts a string at a specified position. If Index is 0, the string is inserted 
      /// at the beginning of the list. If Index is 1, the string is put in the second 
      /// position of the list, and so on.
      /// </summary>
      /// <param name="Index"></param>
      /// <param name="st"></param>

      public void Insert(int Index, string st)
      {
         if (Index == 0 && Count == 0)
         {
            Add(st);
         }
         else
         {
            Check(Index);

            Add(st);

            for (int i = Count - 1; i > Index; i--)
            {
               this[i] = this[i - 1];
               this.SetObject(i, this.GetObject(i - 1));
            };

            this[Index] = st;
            this.SetObject(Index, null);
         };
      }

      /******************************************************************************/
      /// <summary>
      /// Returns the position of a string in the list.
      /// </summary>
      /// <param name="st"></param>
      /// <returns>IndexOf returns the 0-based index of the string. Thus, if S 
      /// matches the first string in the list, IndexOf returns 0, if S is the 
      /// second string, IndexOf returns 1, and so on. If the string is not in 
      /// the string list, IndexOf returns -1.</returns>
      public int IndexOf(string st)
      {
         int Result;

         for (Result = 0; Result < this.Count; Result++)
         {
            if (Get(Result).ToUpper() == st.ToUpper())
            {
               return (Result);
            }
         }

         return (-1);
      }

      /// <summary>
      /// Returns the position of the first name-value pair with the specified name.
      /// </summary>
      /// <param name="Name"></param>
      /// <returns>Locates the first occurrence of a name-value pair where the name 
      /// part is equal to the Name parameter or differs only in case. IndexOfName 
      /// returns the 0-based index of the string. If no string in the list has the 
      /// indicated name, IndexOfName returns -1.</returns>
      public int IndexOfName(string Name)
      {
         int pos, Result;
         string st;

         for (Result = 0; Result < this.Count; Result++)
         {
            st = Get(Result);
            pos = st.IndexOf("=");

            if ((pos != -1) && (st.Substring(0, pos).ToUpper().Replace(@"\R\N", "\r\n") == Name.ToUpper()))
            {
               return (Result);
            }
            else
            {
               if (_IgnoreSPC)
               {
                  st = st.Trim();
                  pos = st.IndexOf("=");

                  if ((pos != -1) && (st.Substring(0, pos).Trim().ToUpper().Replace(@"\R\N", "\r\n") == Name.ToUpper()))
                  {
                     return (Result);
                  };
               }
               else
               {
                  pos = st.IndexOf("=");

                  if (pos == -1)
                  {
                     if (Name.ToUpper() == st.ToUpper())
                     {
                        return (Result);
                     };
                  }
                  else
                  {
                     if ((pos != -1) && (st.Substring(0, pos).ToUpper().Replace(@"\R\N", "\r\n") == Name.ToUpper()))
                     {
                        return (Result);
                     };
                  };
               };
            };
         };

         Result = -1;

         return (Result);
      }

      /******************************************************************************/
      /// <summary>
      /// Deletes a specified string from the list.
      /// </summary>
      /// <param name="Index"></param>

      public void Delete(int Index)
      {
         Check(Index);

         if (Index < this.Count)
         {
            for (int i = Index; i < _Count - 1; i++)
            {
               InnerList.SetValue(InnerList.GetValue(i + 1), i);
            };

            _Count -= 1;
         };
      }

      /******************************************************************************/
      /// <summary>
      /// Fills the list with the lines of text in a specified file.
      /// </summary>
      /// <param name="IniFileName"></param>
      public void LoadFromFile(string FileName, System.Text.Encoding Encoding)
      {
         Clear();

         try
         {
            using (var stream = new FileStream(FileName, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
               StreamReader sr;

               if (Encoding == null)
               {
                  sr = new StreamReader(stream);
               }
               else
               {
                  sr = new StreamReader(stream, (System.Text.Encoding)(Encoding));
               };

               string astringLine;

               while ((astringLine = sr.ReadLine()) != null)
               {
                  Add(astringLine);
               };
            };
         }
         catch (Exception ex)
         {
            System.Diagnostics.Debug.WriteLine(String.Format("{0}: {1}", "TStrings.LoadFromFile( ... )", ex.Message));
         }
      }

      public bool LoadFromFile(string FileName)
      {
         //STD LoadFromFile(FileName, System.Text.Encoding.Default);
         LoadFromFile(FileName, null);
         return true;
      }


      /******************************************************************************/
      /// <summary>
      /// Saves the strings in the list to the specified file.
      /// </summary>
      /// <param name="IniFileName"></param>
      /// 

      public void SaveToFile(string FileName)
      {
         SaveToFile(FileName, System.Text.Encoding.Unicode);
      }


      /// <summary>
      /// Saves the strings in the list to the specified file.
      /// </summary>
      /// <param name="fileName"></param>
      /// <param name="encoding"></param>
      public void SaveToFile(string fileName, System.Text.Encoding encoding)
      {
         if (fileName == "") return;

         using (var stream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.Write))
         {
            using (StreamWriter asw = new StreamWriter(stream, encoding))
            {
               for (int i = 0; i < this.Count; i++)
               {
                  asw.WriteLine(Get(i));
               }
            };
         };

         //StringBuilder text = new StringBuilder();
         //for (int i = 0; i < this.Count; i++)
         //{
         //   text.Append(Get(i));
         //}
         //System.IO.File.WriteAllText(fileName, text.ToString(), encoding);
      }


      /******************************************************************************/

      public Stream SaveToStream()
      {
         return new MemoryStream(System.Text.Encoding.UTF8.GetBytes(this.Text));
      }

      /******************************************************************************/

      public void Push(string Value)
      {
         Add(Value);
      }

      public void PushObject(string Value, Object Obj)
      {
         Push(Value);
         SetObject(Count - 1, Obj);
      }

      public string Pop()
      {
         string Result;

         Result = Get(this.Count - 1);
         Delete(this.Count - 1);

         return (Result);
      }

      public Object PopObject()
      {
         if (this.Count > 0)
         {
            Object Result = GetObject(this.Count - 1);
            Delete(this.Count - 1);

            return Result;
         }
         else
         {
            return null;
         };
      }

      /******************************************************************************/

      void Error(string Msg, int Data)
      {
         if (Debugger.IsAttached)
         {
            Debugger.Break();
         }

         throw new Exception(string.Format(Msg, Data));
      }

      /******************************************************************************/

      public string this[int Index]
      {
         get
         {
            return Get(Index);
         }
         set
         {
            Put(Index, value);
         }
      }

      public string this[string Name]
      {
         get
         {
            string Result;
            int i;

            i = IndexOfName(Name);

            if (i >= 0)
            {
               Result = Get(i).Replace("\\r\\n", "\r\n");
               Result = Result.Substring(Result.IndexOf("=") + 1);
            }
            else
            {
               Result = "";
            };

            return (Result);
         }
         set
         {
            int i;

            i = IndexOfName(Name);

            if ((value != null) && (value.Length != 0))
            {
               if (i < 0)
               {
                  Add(Name + "=" + value);
               }
               else
               {
                  Put(i, Name + "=" + value);
               };
            }
            else
            {
               if (i >= 0)
                  Delete(i);
            };
         }
      }

      public string this[int Index1, int Index]
      {
         get
         {
            Check(Index1);

            var o = GetObject(Index1) as TStrings;

            if (o != null)
            {
               return o[Index];
            }
            else
            {
               return "";
            };
         }
         set
         {
            Check(Index1);

            if (Index == 0)
            {
               Put(Index1, value);
            };

            if ((GetObject(Index1) as TStrings) != null)
            {
               var t = (GetObject(Index1) as TStrings);
               for (int ii = t.Count; ii <= Index; ii++) t.Add("");
               t[Index] = value;
            }
            else
            {
               TStrings t = new TStrings();
               for (int ii = t.Count; ii <= Index; ii++) t.Add("");
               t[Index] = value;

               SetObject(Index1, t);
            };
         }
      }

      public string this[string Name, int Index]
      {
         get
         {
            string Result;
            int i;

            i = IndexOfName(Name);

            if (i >= 0)
            {
               var o = GetObject(i) as TStrings;

               if (o != null)
               {
                  return o[Index];
               }
               else
               {
                  return "";
               };
            }
            else
            {
               Result = "";
            };

            return (Result);
         }
         set
         {
            int i;

            i = IndexOfName(Name);

            if (i < 0)
            {
               if (Index == 0)
               {
                  i = Add(Name + "=" + value);
               }
               else
               {
                  i = Add(Name + "=");
               };

               TStrings t = new TStrings();
               for (int ii = t.Count; ii <= Index; ii++) t.Add("");
               t[Index] = value;

               SetObject(i, t);
            }
            else
            {
               if (Index == 0)
               {
                  Put(i, Name + "=" + value);
               };

               if ((GetObject(i) as TStrings) != null)
               {
                  var t = (GetObject(i) as TStrings);
                  for (int ii = t.Count; ii <= Index; ii++) t.Add("");
                  t[Index] = value;
               }
               else
               {
                  TStrings t = new TStrings();
                  for (int ii = t.Count; ii <= Index; ii++) t.Add("");
                  t[Index] = value;

                  SetObject(i, t);
               };
            };
         }
      }

      /******************************************************************************/
      /// <summary>
      /// Indicates the name part of strings that are name-value pairs.
      /// </summary>
      /// <param name="Index"></param>
      /// <returns></returns>

      public string Names(int Index)
      {
         string Result;

         Check(Index);

         if (this.Count == 0)
         {
            Result = "";
         }
         else
         {
            if (Index >= 0)
            {
               Result = Get(Index);

               int Ind = Result.IndexOf("=");

               if (Ind != -1)
               {
                  Result = Result.Substring(0, Ind);
               }
               ;
            }
            else
            {
               Result = "";
            }
            ;
         }
         ;

         return (Result);
      }

      /******************************************************************************/
      /// <summary>
      /// Lists the strings in the TStrings object as a single string with 
      /// the individual strings delimited by carriage returns and line feeds.
      /// </summary>

      public string Text
      {
         get
         {
            string Result = "";

            for (int i = 0; i < this.Count; i++)
            {
               if (i == 0)
               {
                  Result = this[i];
               }
               else
               {
                  Result = Result + "\n" + this[i];
               };
            };

            return (Result);
         }
         set
         {
            int Ind = 0;

            this.Clear();

            Ind = value.IndexOf("\n");

            while (Ind != -1)
            {
               var st = value.Substring(0, Ind);
               if (!string.IsNullOrEmpty(st) && st.EndsWith("\r"))
               {
                  st = st.Substring(0, st.Length - 1);
               };

               this.Add(st);
               value = value.Substring(Ind + 1, value.Length - Ind - 1);
               Ind = value.IndexOf("\n");
            };

            if (value.Length > 0)
            {
               this.Add(value);
            }
         }
      }

      /******************************************************************************/
      /// <summary>
      /// Lists the strings in the TStrings object as a single string with 
      /// the individual strings delimited by <br />.
      /// </summary>

      public string HTML
      {
         get
         {
            string Result = "";

            for (int i = 0; i < this.Count; i++)
            {
               if (i == 0)
               {
                  Result = this[i];
               }
               else
               {
                  Result = Result + "<br />" + this[i];
               }
               ;
            }
            ;

            return (Result);
         }
      }

      /******************************************************************************/

      public string HTMLTable(string ClassName = "")
      {
         string Result = (string.IsNullOrEmpty(ClassName) ? "<table>" : string.Format("<table class='{0}'>", ClassName));

         for (int i = 0; i < this.Count; i++)
         {
            if (string.IsNullOrEmpty(ClassName))
            { Result += string.Format("<tr><th>{0}</th><td>{1}</td></tr>", Names(i), ValueFromIndex(i)); }
            else
            { Result += string.Format("<tr class='##'><th class='##'>{0}</th><td class='##'>{1}</td></tr>", Names(i), ValueFromIndex(i)).Replace("##", ClassName); }
         };

         Result += "</table>";

         return (Result);
      }

      /******************************************************************************/
      /// <summary>
      /// Represents the value part of a string with a given index, on strings 
      /// that are name-value pairs.
      /// </summary>

      public string ValueFromIndex(int Index)
      {
         string Result = Get(Index);
         int pos = Result.IndexOf("=");

         if (pos != -1)
         {
            Result = Result.Substring(pos + 1);
         };

         return Result;
      }

      /******************************************************************************/

      public int Find(string Pattern)
      {
         int Result = -1;

         for (int i = 0; i < this.Count; i++)
         {
            string st = Get(i);

            if (st.StartsWith(Pattern))
            {
               return (i);
            }
         };

         return Result;
      }

      /******************************************************************************/

      private bool _IgnoreSPC;

      public bool IgnoreSPC
      {
         get { return _IgnoreSPC; }
         set { _IgnoreSPC = value; }
      }

      /******************************************************************************/
      //TStrings Dico = TStrings.FromJSon("[{\"1\":\"Log\"}, {\"2\":\"Warning\"}, {\"3\":\"Error\"}, {\"4\":\"Critical\"} ]");
      //TStrings Dico = TStrings.FromJSon("[{\"1\" :\"Log\"}, {\"2\": \"Warning\"}, {\"3\" : \"Error\"}, { \"4\":\"Critical\" } ]");
      //TStrings Dico = TStrings.FromJSon("[{\"1\":11 }, {\"2\":22}, {\"3\": 33}, {\"4\": 44 } ]");

      /// <summary>
      /// Creates a new TStrings from JSON ...
      /// <code>
      /// TStrings Dico = TStrings.FromJSon("[{\"1\":\"Log\"}, {\"2\":\"Warning\"}, {\"3\":\"Error\"}, {\"4\":\"Critical\"} ]");
      /// </code>
      /// </summary>
      /// <param name="JSON"></param>
      /// <returns></returns>
      public static TStrings FromJSon(string JSON)
      {
         JSON = LineUpJSON(JSON);

         TStrings Result = new TStrings();

         JSON = JSON.Replace("},", "\n");

         JSON = JSON.Replace("\":\"", "=");

         JSON = JSON.Replace("\":", "=");
         JSON = JSON.Replace("\"\n", "\n");

         JSON = JSON.Replace("\n{\"", "\n");
         JSON = JSON.Replace("\n {\"", "\n");

         JSON = JSON.TrimStart(new char[] { '[', '{', '"', ' ' });
         JSON = JSON.TrimEnd(new char[] { ']', '}', ' ', '"' });

         JSON = JSON.Replace("\",\"", "\n");

         Result.Text = JSON;

         return Result;
      }

      public static string LineUpJSON(string JSON)
      {
         string Result = "";

         if (string.IsNullOrEmpty(JSON))
         {
            return JSON;
         };

         // - - -  - - - 

         int QuotationMark = 0;

         for (int i = 0; i < JSON.Length; i++)
         {
            switch (JSON[i])
            {
               case '\"':
                  QuotationMark++;
                  Result += JSON[i];
                  break;

               case ' ':
                  if (IsEven(QuotationMark))
                  {
                     // nothing to do
                  }
                  else
                  {
                     Result += JSON[i];
                  };
                  break;

               case '\r':
               case '\n':
                  if (IsOdd(QuotationMark))
                  {
                     Result += JSON[i];
                  }
                  break;

               default:
                  Result += JSON[i];
                  break;
            };
         };

         // - - -  - - - 

         return Result;
      }

      #region math helper
      public static bool IsEven(int value)
      {
         return value % 2 == 0;
      }

      public static bool IsOdd(int value)
      {
         return value % 2 != 0;
      }
      #endregion

      /******************************************************************************/

      public static TStrings FromJSonValue(string Text)
      {
         TStrings Result = new TStrings();

         if (Text != "" && Text != "[]")
         {
            if ((Text[0] == '[') && (Text[Text.Length - 1] == ']'))
            {
               Text = Text.Substring(1, Text.Length - 2);
            };

            if ((Text[0] == '{') && (Text[Text.Length - 1] == '}'))
            {
               Text = Text.Substring(1, Text.Length - 2);
            };

            Text = Text.Replace("\",", "\"\n");

            Result.Text = Text;

            for (int i = 0; i < Result.Count; i++)
            {
               if (Result.ValueFromIndex(i).StartsWith("\"") && Result.ValueFromIndex(i).EndsWith("\""))
               {
                  Result[i] = Result.Names(i) + "=" + Result.ValueFromIndex(i).Substring(1, Result.ValueFromIndex(i).Length - 2);
               };
            };
         };

         return Result;
      }
   }
}
