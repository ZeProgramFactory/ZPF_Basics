using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json; // Install-Package System.Text.Json 
using System.Xml.Serialization;
using ZPF.AT;

namespace ZPF
{
   public static class wsHelper
   {
      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

      public static string wsServer = "";
      public static string wsServerDoc = "";
      public static HttpClient _httpClient = null;
      public static TStrings Logs = new TStrings();

      public static bool Init()
      {
         if (_httpClient != null) return true;

#if __WASM__
         var innerHandler = new Uno.UI.Wasm.WasmHttpHandler();
#else
         var innerHandler = new HttpClientHandler();
#endif

         _httpClient = new HttpClient(innerHandler);

         return _httpClient != null;
      }


      public static Uri CalcURI(string function)
      {
         if (string.IsNullOrEmpty(wsServer) && ! function.ToUpper().StartsWith("HTTP") )
         {
            return null;
         };

         if (string.IsNullOrEmpty(function))
         {
            return null;
         };

         if (!string.IsNullOrEmpty(wsServer) && !wsServer.EndsWith("/"))
         {
            wsServer = wsServer + '/';
         };

         if (function.StartsWith("/"))
         {
            function = function.Substring(1);
         };

         string URL = wsServer + function;

         return new Uri(URL);
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

      public static string LastError { get; set; }
      public static string LastData { get; set; }
      public static TimeSpan LastTripDuration { get; set; }
      public static long LastReceivedDataSize { get; set; }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

      /// <summary>
      /// HTTP Get - Retrieve data - OK
      /// </summary>
      /// <param name="Function"></param>
      /// <param name="basicAuth"></param>
      /// <returns></returns>
      public static async Task<string> wGet(string Function, string basicAuth = "")
      {
         Uri uri = CalcURI(Function);

         if (uri == null)
         {
            return null;
         };

         return await wGet(uri, basicAuth);
      }

      /// <summary>
      /// HTTP Get - Retrieve data - OK
      /// </summary>
      /// <param name="Function"></param>
      /// <param name="basicAuth"></param>
      /// <returns></returns>
      public static async Task<T> wGet<T>(string Function, string basicAuth = "", bool Zipped = false)
      {
         Uri uri = CalcURI(Function);

         if (uri == null)
         {
            return default(T);
         };

         var data = await wGet(uri, basicAuth, Zipped: Zipped);

         if (string.IsNullOrEmpty(data))
         {
            return default(T);
         }
         else
         {
            try
            {
               return JsonSerializer.Deserialize<T>(data);
            }
            catch (Exception ex)
            {
               LastError = ex.Message;

               return default(T);
            };
         };
      }

      /// <summary>
      /// HTTP Get - Retrieve data - OK
      /// </summary>
      /// <param name="Function"></param>
      /// <param name="basicAuth"></param>
      /// <returns></returns>
      public static async Task<T> wGet<T>(Uri uri, string basicAuth = "")
      {
         var data = await wGet(uri, basicAuth);
         return JsonSerializer.Deserialize<T>(data);
      }

      /// <summary>
      /// HTTP Get - Retrieve data - OK
      /// </summary>
      /// <param name="uri"></param>
      /// <param name="basicAuth"></param>
      /// <returns></returns>
      public static async Task<string> wGet(Uri uri, string basicAuth = "", bool Zipped = false)
      {
         Init();

         LastError = "";
         LastData = "";

         try
         {
#if __WASM__
#else
            if (!string.IsNullOrEmpty(basicAuth))
            {
               byte[] byteArray = Encoding.ASCII.GetBytes(basicAuth);
               _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
            };
#endif

            if (Zipped)
            {
               //               _httpClient.Timeout = TimeSpan.FromMinutes(3);

               var dt = DateTime.Now;
               var st = await _httpClient.GetStringAsync(uri);

               LastTripDuration = (DateTime.Now - dt);
               LastError = st;

               byte[] r1 = Convert.FromBase64String(st);
               LastReceivedDataSize = r1.Length;

               //byte[] r1 = await _httpClient.GetByteArrayAsync(uri);

               //var fn = @"C:\Users\zepro\AppData\Local\Packages\aba65081-bf10-4bf0-bc21-2ec02c1f514e_fa6wnk0zd4sjp\LocalState\Dummy.zip";
               //r1 = System.IO.File.ReadAllBytes(fn);

               LastData = ZIPHelper.Unzip(r1);
            }
            else
            {
               var dt = DateTime.Now;

               LastData = await _httpClient.GetStringAsync(uri);

               LastTripDuration = (DateTime.Now - dt);
               LastReceivedDataSize = LastData.Length;
            };
         }
         catch (Exception ex)
         {
            LastError = ex.Message;
            Debug.WriteLine(ex.Message);

            LastData = null;
         };

         return LastData;
      }

       // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

      public static async Task<T> xGet<T>(string Function, string basicAuth = "")
      {
         var DT = DateTime.Now;
         Uri uri = CalcURI(Function);

         if (uri == null)
         {
            return default(T);
         };

         var xmlString = await wGet(uri, basicAuth);

         if (string.IsNullOrEmpty(xmlString))
         {
            ZPF.AT.Log.Write(ZPF.AT.ErrorLevel.Info, $"xGet {Function} = null");
            ZPF.AT.Log.Write(ZPF.AT.ErrorLevel.Info, $"[{LastError}]");

            return default(T);
         }
         else
         {
            try
            {
               ZPF.AT.Log.Write(ZPF.AT.ErrorLevel.Log, $"xGet {Function} {(DateTime.Now-DT).TotalMilliseconds:0.0}ms" );
              
                  XmlSerializer serializer = new XmlSerializer(typeof(T));
                  StringReader stringReader = new StringReader(xmlString);
                  var t = (T)serializer.Deserialize(stringReader);
                  return t;
               
               
            }
            catch (Exception ex)
            {
               var at = new AuditTrail(ex, AuditTrail.TextFormat.TxtEx)
               {
                  Level = ErrorLevel.Error,
                  DataIn = Function,
               };

               ZPF.AT.Log.Write(at);

               LastError = ex.Message + "\n\n" + xmlString;
               return default(T);
            };
         };
      }

      

      public static async Task<T> xGet<T>(Uri uri, string basicAuth = "")
      {
         string xmlString = "";

         try
         {
            xmlString = await wGet(uri, basicAuth);

            //XmlSerializer serializer = new XmlSerializer(typeof(T), new XmlRootAttribute("Param"));
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            StringReader stringReader = new StringReader(xmlString);

            return (T)serializer.Deserialize(stringReader);
         }
         catch (Exception ex)
         {
            LastError = ex.Message + "\n\n" + xmlString;
            Debug.WriteLine(ex.Message);

            return default(T);
         };
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

      public static async Task<Stream> wGetStream(Uri uri)
      {
         Init();

         LastError = "";

         try
         {
            _httpClient.Timeout = TimeSpan.FromSeconds(20);

            var dt = DateTime.Now;

            byte[] byteArray = await _httpClient.GetByteArrayAsync(uri);

            LastTripDuration = (DateTime.Now - dt);
            LastReceivedDataSize = byteArray.Length;

            return new System.IO.MemoryStream(byteArray);
         }
         catch (Exception ex)
         {
            LastError = ex.Message;
            Debug.WriteLine(ex.Message);
         };

         return null;
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

      #region NotYetTested
      public static async Task<bool> wGetDownload(string Function, string FilePath)
      {
         Uri uri = CalcURI(Function);

         if (uri == null)
         {
            LastError = "uri = null";
            return false;
         };

         return await wGetDownload(uri, FilePath);
      }

      /// <summary>
      /// not tested
      /// </summary>
      /// <param name="uri"></param>
      /// <param name="FilePath"></param>
      /// <returns></returns>
      public static async Task<bool> wGetDownload(Uri uri, string FilePath)
      {
         Init();

         LastError = "";

         try
         {
            var dt = DateTime.Now;

            _httpClient.Timeout = TimeSpan.FromSeconds(20);
            byte[] byteArray = await _httpClient.GetByteArrayAsync(uri);

            LastTripDuration = (DateTime.Now - dt);
            LastReceivedDataSize = byteArray.Length;
#if XF
            ZPF.XF.Basics.Current.FileIO.WriteStream(new System.IO.MemoryStream(byteArray), FilePath);
#else
            throw new NotImplementedException();
#endif

            return true;
         }
         catch (Exception ex)
         {
            LastError = ex.Message;
            Debug.WriteLine(ex.Message);
         };

         return false;
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

      /// <summary>
      /// 
      /// </summary>
      /// <param name="Function"></param>
      /// <param name="oData"></param>
      /// <returns></returns>
      public static async Task<bool> wPost(string Function, object oData)
      {
         Uri uri = CalcURI(Function);

         if (uri == null)
         {
            LastError = "uri = null";
            return false;
         };

         Logs.Add($"(0) oData={oData != null} { oData.GetType() }");
         var json = JsonSerializer.Serialize(oData, new JsonSerializerOptions { WriteIndented=true, });

         return await wPost(uri, json);
      }

      public static async Task<Stream> wPost_S(string Function, object oData)
      {
         Uri uri = CalcURI(Function);

         if (uri == null)
         {
            LastError = "uri = null";
            return null;
         };

         return await wPost_Stream(uri, oData);
      }

      /// <summary>
      /// not tested
      /// </summary>
      /// <param name="uri"></param>
      /// <param name="oData"></param>
      /// <returns></returns>
      public static async Task<bool> wPost(string Function, string json)
      {
         LastError = "";

         Uri uri = CalcURI(Function);

         if (uri == null)
         {
            LastError = "uri = null";
            return false;
         };

         try
         {
            Logs.Add($"(1) txt={json}");

            return await wPost(uri, json);
         }
         catch (Exception ex)
         {
            LastError = ex.Message;
            return false;
         };
      }

      /// <summary>
      /// not tested
      /// </summary>
      /// <param name="uri"></param>
      /// <param name="oData"></param>
      /// <returns></returns>
      public static async Task<Stream> wPost_Stream(Uri uri, object oData)
      {
         LastError = "";

         try
         {
            var json = JsonSerializer.Serialize(oData, new JsonSerializerOptions { WriteIndented = true, });
            return await wPost_Stream(uri, json);
         }
         catch (Exception ex)
         {
            LastError = ex.Message;
            return null;
         };
      }

      public static async Task<string> wPost_String(string Function, string json)
      {
         LastError = "";

         Uri uri = CalcURI(Function);

         if (uri == null)
         {
            LastError = "uri = null";
            return null;
         };

         try
         {
            Logs.Add($"(1) wPost_String");

            return await wPost_String(uri, json);
         }
         catch (Exception ex)
         {
            LastError = ex.Message;
            return null;
         };
      }

      public static async Task<string> wPost_String(string Function, object oData)
      {
         LastError = "";

         Uri uri = CalcURI(Function);

         if (uri == null)
         {
            LastError = "uri = null";
            return null;
         };

         try
         {
            Logs.Add($"(1) wPost_String");

            return await wPost_String(uri, oData);
         }
         catch (Exception ex)
         {
            LastError = ex.Message;
            return null;
         };
      }


      public static async Task<string> wPost_String(Uri uri, object oData)
      {
         LastError = "";

         try
         {
            var json = JsonSerializer.Serialize(oData, new JsonSerializerOptions { WriteIndented = true, });
            return await wPost_String(uri, json);
         }
         catch (Exception ex)
         {
            LastError = ex.Message;
            return null;
         };
      }

      /// <summary>
      /// not tested
      /// </summary>
      /// <param name="uri"></param>
      /// <param name="json"></param>
      /// <param name="basicAuth"></param>
      /// <returns></returns>
      public static async Task<bool> wPost(Uri uri, string json, string basicAuth = "")
      {
         LastError = "";
         LastData = "";

         try
         {

#if __WASM__
            var dlg = new Windows.UI.Popups.MessageDialog($"¤¤ Post:\n {json.Length}\n {json}");
            await dlg.ShowAsync();
#else
            if (!string.IsNullOrEmpty(basicAuth))
            {
               byte[] byteArray = Encoding.ASCII.GetBytes(basicAuth);
               _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
            };
#endif
            _httpClient.BaseAddress = uri;

            var x = new StringContent(json, Encoding.Unicode);
            x.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            //var response = await _httpClient.PostAsync("", x);
            var response = await _httpClient.PostAsync(uri.ToString(), x);

            response.EnsureSuccessStatusCode();

            LastData = response.StatusCode.ToString();

            Logs.Add($"(2) StatusCode={LastData}");
         }
         catch (Exception ex)
         {
            LastError = ex.Message;
            Debug.WriteLine(ex.Message);

            return false;
         };

         return true;
      }

      public static async Task<Stream> wPost_Stream(Uri uri, string json, string basicAuth = "")
      {
         LastError = "";
         string data = "";

         try
         {

#if __WASM__
#else
            if (!string.IsNullOrEmpty(basicAuth))
            {
               byte[] byteArray = Encoding.ASCII.GetBytes(basicAuth);
               _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
            };
#endif
            //httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            _httpClient.BaseAddress = uri;

            var x = new StringContent(json, Encoding.Unicode);
            //var x = new StringContent(json, Encoding.UTF8);
            x.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var response = await _httpClient.PostAsync("", x);

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStreamAsync();
         }
         catch (Exception ex)
         {
            LastError = ex.Message;
            Debug.WriteLine(ex.Message);

            return null;
         };

         return null;
      }

      public static async Task<string> wPost_String(Uri uri, string json, string basicAuth = "")
      {
         LastError = "";
         LastData = "";

         try
         {

#if __WASM__
#else
            if (!string.IsNullOrEmpty(basicAuth))
            {
               byte[] byteArray = Encoding.ASCII.GetBytes(basicAuth);
               _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
            };
#endif

            var x = new StringContent(json, Encoding.BigEndianUnicode);
            x.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var response = await _httpClient.PostAsync(uri, x);

            response.EnsureSuccessStatusCode();

            LastData = await response.Content.ReadAsStringAsync();
            return LastData;
         }
         catch (Exception ex)
         {
            Log.Write(new AuditTrail(ex)
            {
                Application= "wsHelper.wPost_String1",
                DataOut=json,
            });

            LastError = ex.Message;
            Debug.WriteLine(ex.Message);

            return null;
         };
      }
      
      #endregion

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -
   }
}

