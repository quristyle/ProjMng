using System.Text.RegularExpressions;

namespace ProjMngServer;
public class BlazorUtil {

  private static string ExtractValue(string content, string key, string empStr) {
    string pattern = $@"{key}\s*:\s*(?<value>[^\n\r*]+)";
    Match match = Regex.Match(content, pattern);
    return match.Success ? match.Groups["value"].Value.Trim() : empStr;
  }








  public static string RemovePath(string input, string pathToRemove) {
    // Normalize the path to remove by replacing backslashes with forward slashes and making it case-insensitive
    string normalizedPath = Regex.Escape(pathToRemove.Replace("\\", "/"));

    // Create a regex pattern to match the normalized path in a case-insensitive manner
    string pattern = "(?i)" + normalizedPath.Replace("/", "[/\\\\]");

    // Replace the matched path with an empty string
    return Regex.Replace(input, pattern, "", RegexOptions.IgnoreCase);
  }



  public static string RemoveNonAlphabeticLeadingChar(string input) {
    return Regex.Replace(input, "^[^a-zA-Z]+", "");
  }







  //blazor 파일을 읽어서 메뉴를 만들어 주는 함수
  public static List<Dictionary<string, string>> GetBlazorMenuList(string basePath, string projNamespace, string pageRoot, string pagePattern) {

   List<Dictionary<string, string>> aaa = new List<Dictionary<string, string>>();

   // string basePath = @"c:\projects\ProjMng";
   // string projNamespace = @"ProjMngWasm";
   // string pageRoot = @"Pages";
    string folderPath = basePath + @"\" + projNamespace + @"\";
    string searchPattern = "*.razor";
    //string pagePattern = "@page\\s+\"(?<url>[^\"]+)\"";
    //string namePattern = "@\\* description :\\s*(?<title>[^*]+)\\s*\\*@";


    /* description : 프로젝트 상태 정보를 제공
* title : 모니터링
* sort : 30
* credt : 2021-09-01
* author : quristyle
*/


    try {
      foreach (string file in Directory.GetFiles(folderPath, searchPattern, SearchOption.AllDirectories)) {
        string content = File.ReadAllText(file);
        Match match = Regex.Match(content, pagePattern);

        if (match.Success) {
          string url = match.Groups["url"].Value;
          Dictionary<string, string> binfo = new Dictionary<string, string>();
          binfo.Add("name", Path.GetFileNameWithoutExtension(file));

            string dir = Path.GetDirectoryName(file);
          string rurl = RemovePath(dir, basePath);
          rurl = RemovePath(rurl, projNamespace);
          rurl = RemovePath(rurl, pageRoot);
          rurl = RemovePath(rurl, "/");
          binfo.Add("dir", rurl);

          string filepath = Path.GetFullPath(file);
          rurl = RemovePath(filepath, basePath);
          rurl = RemovePath(rurl, ".razor");
          rurl = RemovePath(rurl, "..");

          binfo.Add("fullname", RemoveNonAlphabeticLeadingChar(rurl.Replace("\\", ".")));



          binfo.Add("url", url);
          binfo.Add("description", ExtractValue(content, "description", Path.GetFileNameWithoutExtension(file) ));
          binfo.Add("title", ExtractValue(content, "title", ExtractValue(content, "화면명", url)));
          binfo.Add("sort", ExtractValue(content, "sort", "999") ) ;
          binfo.Add("credt", ExtractValue(content, "credt", ExtractValue(content, "작성일자", string.Empty)));
          binfo.Add("author", ExtractValue(content, "author", ExtractValue(content, "작성자명", string.Empty)));
          aaa.Add(binfo);



          /*
      * 작성자명 : 김지수
      * 작성일자 : 25-02-27
      * 최종수정 : 25-02-27
      * 화면명 : 계측기별 Peak값 조회
      * 프로시저명 : : P_HMI_DAY_USEQTY_PEAK, P_HMI_TIME_USEQTY_PEAK, P_HMI_MIN_UEEQTY_PEAK, P_HMI_PEAK_VALUE_RST
      */



        }
      }
    }
    catch (Exception ex) {
      Console.WriteLine("오류 발생: " + ex.Message);
    }
    return aaa;
  }
}