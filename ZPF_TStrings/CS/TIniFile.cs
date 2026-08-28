
using System;
using System.Globalization;


namespace ZPF
{
   /// <summary>
   /// TIniFile stores and retrieves application-specific information and settings from INI files.
   /// 
   /// 03/06/06 - ME  - Bugfix: public TIniFile( string IniFileName ) - IniFileName != null
   /// 22/08/06 - ME  - Bugfix: filter Comments ( '#' ) in ReadSectionValues
   /// 17/01/07 - ME  - TrimEnd() on string string values
   /// 18/01/07 - ME  - Add: ReadSectionValues( Section, SubSection, Strings )
   /// 10/04/07 - ME  - Bugfix: ReadSectionValues
   /// 18/06/07 - ME  - Add: WriteSectionValues
   /// 11/11/12 - ME  - Add: NETFX_CORE --> Windows 8 RT:
   /// 19/12/12 - ME  - Bugfix: NETFX_CORE --> Windows 8 RT:
   /// 21/03/13 - ME  - Add: ReadLong, WriteColor & ReadColor
   /// 26/06/14 - ME  - Bugfix: ReadString: Strings.IgnoreSPC = true;
   /// 01/12/14 - ME  - Bugfix: await ... PCL ...
   /// 08/08/16 - ME  - Add: void WriteDateTime(string Section, string Ident, DateTime Value)
   /// 08/08/16 - ME  - Add: DateTime ReadDateTime(string Section, string Ident, DateTime Default)
   /// 02/09/16 - ME  - Bugfix: ReadDateTime
   /// 14/12/17 - ME  - Code review
   /// 26/10/18 - ME  - ReadSections(TStrings ) --> TStrings ReadSections()
   /// 
   /// 2005..2018 ZePocketForge.com, SAS ZPF, ZeProgramFactory
   /// </summary>

   public class TIniFile
   {
      private string FFileName;
      private TStrings FSections;

      /// <summary>
      /// TIniFile creates a new TIniFile object.
      /// </summary>
      /// <param name="IniFileName">On Windows, most INI files are stored in 
      /// the \WINDOWS directory. To work with an INI file in another location, 
      /// specify the full path name of the file in IniFileName.</param>

      public TIniFile(string FileName)
      {
         FSections = new TStrings();

         if ((FileName != null) && (FileName.Length > 0))
         {
            FFileName = FileName;
            LoadValues();
         }
         else
         {
            FFileName = "";
         };
      }

      ~TIniFile()
      {
         Clear();
         FFileName = "";
      }

      /******************************************************************************/
      //void SetFileName( const wstring IniFileName );
      //__declspec(property ( get=FFileName, put=SetFileName )) wstring IniFileName[];

      /******************************************************************************/
      /// <summary>
      /// Erases all data from the INI file in memory.
      /// </summary>

      public void Clear()
      {
         if (FSections != null)
         {
            while (FSections.Count > 0)
            {
               EraseSection(FSections[0]);
            };
         };
      }

      /// <summary>
      /// Flushes buffered INI file data to disk.
      /// </summary>

      public bool UpdateFile()
      {
         TStrings IniFile = new TStrings();
         TStrings Section;

         for (int i = 0; i < FSections.Count; i++)
         {
            IniFile.Add("[" + FSections[i] + "]");

            Section = (FSections.GetObject(i) as TStrings);

            for (int j = 0; j < Section.Count; j++)
            {
               IniFile.Add(Section[j]);
            };

            IniFile.Add("");
         };

         IniFile.SaveToFile(FFileName, System.Text.Encoding.Unicode);

         return true;
      }

      /// <summary>
      /// Deletes a specified entry from the .ini file.
      /// </summary>
      /// <param name="Section">Section is string containing the name of a .ini file section.</param>
      /// <param name="Ident">Ident is a string containing the name of the key to remove.</param>

      public void DeleteKey(string Section, string Ident)
      {
      }

      /******************************************************************************/
      /// <summary>
      /// Erases an entire section of an INI file.
      /// </summary>
      /// <param name="Section">Section identifies the INI file section to remove.</param>

      public void EraseSection(string Section)
      {
         int i;
         TStrings Strings;

         i = FSections.IndexOf(Section);

         if (i >= 0)
         {
            Strings = (FSections.GetObject(i) as TStrings);
            Strings.Clear();

            //delete Strings;

            FSections.Delete(i);
         };
      }

      void ReadSection(string Section, TStrings Strings)
      {
      }

      public TStrings ReadSections()
      {
         TStrings Result = new TStrings();
         Result.Text = FSections.Text;
         return Result;
      }

      /// <summary>
      /// Reads the values from all keys within a section of an INI file into a string list.
      /// </summary>
      /// <param name="Section">Section identifies the section in the file from which to read key values. </param>
      /// <param name="Strings">Strings is the string list object into which to read the values.</param>

      public void ReadSectionValues(string Section, TStrings Strings)
      {
         TStrings CurrentSection;

         Strings.Clear();

         if (Strings != null)
         {
            for (int i = 0; i < FSections.Count; i++)
            {
               //if( FSections[ i ] == Section )
               if (FSections[i].ToUpper() == Section.ToUpper())
               {
                  CurrentSection = (FSections.GetObject(i) as TStrings);

                  for (int j = 0; j < CurrentSection.Count; j++)
                  {
                     string st = CurrentSection[j].TrimEnd();

                     if ((st.Length > 0)
                           && (st[0] != '#')
                           && (st[0] != '\n'))
                     {
                        Strings.Add(st);
                     };
                  };

                  i = FSections.Count;
               };
            };
         };
      }

      public TStrings ReadSectionValues(string Section)
      {
         TStrings Strings = new TStrings();

         ReadSectionValues(Section, Strings);

         return Strings;
      }

      public void ReadSectionValues(string Section, string SubSection, TStrings Strings)
      {
         ReadSectionValues(Section, Strings);

         TStrings SubStrings = new TStrings();
         ReadSectionValues(Section + "." + SubSection, SubStrings);

         for (int i = 0; i < SubStrings.Count; i++)
         {
            Strings[SubStrings.Names(i)] = SubStrings.ValueFromIndex(i);
         };
      }

      public TStrings AddSection(string Section)
      {
         TStrings Result;

         Result = new TStrings();
         FSections.AddObject(Section, Result);

         return (Result);
      }

      /******************************************************************************/
      /// <summary>
      /// Retrieves a string value from an INI file. 
      /// </summary>
      /// <param name="Section">Section identifies the section in the file that contains the desired key.</param>
      /// <param name="Ident">Ident is the name of the key from which to retrieve the value.</param>
      /// <param name="Default">Default is the string value to return if the: Section or the Key does 
      /// not exist. Or the data value for the key is not assigned.</param>
      /// <returns></returns>

      public string ReadString(string Section, string Ident, string Default)
      {
         string Result;
         int i;
         TStrings Strings;

         Result = Default;
         i = FSections.IndexOf(Section);

         if (i >= 0)
         {
            Strings = (FSections.GetObject(i) as TStrings);
            Strings.IgnoreSPC = true;
            i = Strings.IndexOfName(Ident);

            if (i >= 0)
            {
               Result = Strings[Ident].TrimEnd();
            };
         };

         return (Result);
      }

      /// <summary>
      /// Writes a string value to an INI file.
      /// </summary>
      /// <param name="Section">Section identifies the section in the file that contain the key to which to write.</param>
      /// <param name="Ident">Ident is the name of the key for which to set a value.</param>
      /// <param name="Value">Value is the string value to write.</param>

      public void WriteString(string Section, string Ident, string Value)
      {
         int i;
         TStrings Strings;

         i = FSections.IndexOf(Section);

         if (i < 0)
         {
            AddSection(Section);
            i = FSections.IndexOf(Section);
         };

         if (i >= 0)
         {
            Strings = (FSections.GetObject(i) as TStrings);
            Strings[Ident] = Value;
         };
      }

      /// <summary>
      /// Writes a TString value to an section of INI file.
      /// </summary>
      /// <param name="Section">Section identifies the section in the file that contain the key to which to write.</param>
      /// <param name="Ident">Ident is the name of the key for which to set a value.</param>
      /// <param name="Values">Values is the TStrings value to write.</param>

      public void WriteSectionValues(string Section, TStrings Values)
      {
         int i;
         TStrings Strings;

         i = FSections.IndexOf(Section);

         if (i < 0)
         {
            AddSection(Section);
            i = FSections.IndexOf(Section);
         };

         if (i >= 0)
         {
            Strings = (FSections.GetObject(i) as TStrings);
            Strings.Text = Values.Text;
         };
      }

      /******************************************************************************/
      /// <summary>
      /// Retrieves an integer value from an ini file. 
      /// </summary>
      /// <param name="Section">Section identifies the section in the file that contains the desired key. </param>
      /// <param name="Ident">Ident is the name of the key from which to retrieve the value.</param>
      /// <param name="Default">Default is the integer value to return if the Section or the Key does 
      /// not exist. Or the Data value for the key is not assigned.</param>
      /// <returns></returns>

      public int ReadInteger(string Section, string Ident, int Default)
      {
         int Result;
         string st;

         Result = Default;
         st = ReadString(Section, Ident, "");

         if (st.Length > 0)
         {
            if (st.StartsWith("0x"))
            {
               st = st.Substring(2);
               Result = int.Parse(st, System.Globalization.NumberStyles.HexNumber, null);
            }
            else
            {
               Result = System.Convert.ToInt32(st);
            };
         };

         return (Result);
      }

      /// <summary>
      /// Writes an integer value to an ini file.
      /// </summary>
      /// <param name="Section">Section identifies the section in the file that contain the key to which to write.</param>
      /// <param name="Ident">Ident is the name of the key for which to set a value.</param>
      /// <param name="Value">Value is the integer value to write.</param>

      public void WriteInteger(string Section, string Ident, int Value)
      {
         WriteString(Section, Ident, Value.ToString());
      }

      public DateTime ReadDateTime(string Section, string Ident, DateTime Default)
      {
         DateTime Result = Default;

         string st = ReadString(Section, Ident, "");

         if (st.Length > 0)
         {
            try
            {
               try
               {
                  CultureInfo culture = new CultureInfo("fr-FR");
                  Result = Convert.ToDateTime(st, culture);
               }
               catch
               {
                  Result = System.Convert.ToDateTime(st);
               };
            }
            catch
            {
               Result = Default;
            };
         };

         return (Result);
      }

      public void WriteDateTime(string Section, string Ident, DateTime Value)
      {
         WriteString(Section, Ident, Value.ToString("dd/MM/yyyy HH:mm:ss"));
      }

      public long ReadLong(string Section, string Ident, long Default)
      {
         long Result;
         string st;

         Result = Default;
         st = ReadString(Section, Ident, "");

         if (st.Length > 0)
         {
            if (st.StartsWith("0x"))
            {
               st = st.Substring(2);
               Result = long.Parse(st, System.Globalization.NumberStyles.HexNumber, null);
            }
            else
            {
               Result = System.Convert.ToInt64(st);
            };
         };

         return (Result);
      }

      public void WriteLong(string Section, string Ident, long Value)
      {
         WriteString(Section, Ident, Value.ToString());
      }

      /******************************************************************************/

      public bool ReadBool(string Section, string Ident, bool Default)
      {
         bool Result;
         string st;

         Result = Default;
         st = ReadString(Section, Ident, "");

         if (st.Length > 0)
         {
            Result = (st != "0"); // System.Convert.ToBoolean( st );
         };

         return (Result);
      }

      public void WriteBool(string Section, string Ident, bool Value)
      {
         if (Value)
         {
            WriteString(Section, Ident, "1");
         }
         else
         {
            WriteString(Section, Ident, "0");
         };
      }

      /******************************************************************************/
      /// <summary>
      /// Contains the name of the ini file from which to read and to which to write information.
      /// </summary>

      public string FileName
      {
         set
         {
            if (FFileName != value)
            {
               FSections.Clear();

               if (value.Length > 0)
               {
                  FFileName = value;
                  LoadValues();
               }
               else
               {
                  FFileName = "";
               };
            };
         }
         get
         {
            return FFileName;
         }
      }

      /******************************************************************************/

      public void LoadValues()
      {
         TStrings List = new TStrings();
         string st;
         TStrings Strings;

         if ((FFileName.Length > 0))
         {

            List.LoadFromFile(FFileName, System.Text.Encoding.UTF8);

            Clear();
            Strings = null;

            for (int i = 0; i < List.Count; i++)
            {
               st = List[i];

               if ((st.Length > 0) && (st[0] != ';'))   // ? Comment
               {
                  if ((st[0] == '[')
                     && (st[st.Length - 1] == ']'))           // ? Section
                  {
                     Strings = AddSection(st.Substring(1, st.Length - 2));
                  }
                  else
                  {
                     if (Strings != null) Strings.Add(st);
                  };
               };
            };
         }
         else
         {
            Clear();
         };
      }

      /******************************************************************************/
   }
}
