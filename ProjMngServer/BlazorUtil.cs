using System.Text.RegularExpressions;

namespace ProjMngServer;
public class BlazorUtil {

  private static string ExtractValue(string content, string key, string empStr) {
    string pattern = $@"{key}\s*:\s*(?<value>[^\n\r*]+)";
    Match match = Regex.Match(content, pattern);
    return match.Success ? match.Groups["value"].Value.Trim() : empStr;
  }

  //blazor 파일을 읽어서 메뉴를 만들어 주는 함수
  public static IEnumerable<dynamic> GetBlazorMenuList() {

    List<Dictionary<string, string>> aaa = new List<Dictionary<string, string>>();

    string basePath = @"c:\projects\ProjMng";
    string projNamespace = @"ProjMngWasm";
    string pageRoot = @"Pages";
    string folderPath = basePath + @"\" + projNamespace + @"\";
    string searchPattern = "*.razor";
    string pagePattern = "@page\\s+\"(?<url>[^\"]+)\"";
    //string namePattern = "@\\* description :\\s*(?<title>[^*]+)\\s*\\*@";

    try {
      foreach (string file in Directory.GetFiles(folderPath, searchPattern, SearchOption.AllDirectories)) {
        string content = File.ReadAllText(file);
        Match match = Regex.Match(content, pagePattern);

        if (match.Success) {
          string url = match.Groups["url"].Value;
          Dictionary<string, string> binfo = new Dictionary<string, string>();
          binfo.Add("name", Path.GetFileNameWithoutExtension(file));
          binfo.Add("fullname", Path.GetFullPath(file).Replace(basePath, "").Replace(@"\", ".").Replace(@"..", ".").Replace(@".razor", ".").Trim('.'));
          binfo.Add("dir", Path.GetDirectoryName(file).Replace(basePath, "").Replace(projNamespace, "").Replace(pageRoot, "").Replace(@"\\", @"\").Replace(@"\\", @"\").Replace(@"\\", @"\").Replace(@"\\", @"\").Replace(@"\\", @"\").Replace(@"\\", @"\").Replace(@"\\", @"\"));
          binfo.Add("url", url);
          binfo.Add("description", ExtractValue(content, "description", Path.GetFileNameWithoutExtension(file) ));
          binfo.Add("title", ExtractValue(content, "title", url));
          binfo.Add("sort", ExtractValue(content, "sort", "999") ) ;
          binfo.Add("credt", ExtractValue(content, "credt", "non date"));
          binfo.Add("author", ExtractValue(content, "author", "anybody"));
          aaa.Add(binfo);
        }
      }
    }
    catch (Exception ex) {
      Console.WriteLine("오류 발생: " + ex.Message);
    }
    return aaa;
  }
}