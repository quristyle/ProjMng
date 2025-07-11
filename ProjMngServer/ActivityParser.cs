

namespace ProjMngServer;

using ProjModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

public class ActivityParser {

  //glue xml 읽고 추출
  public static List<ActivityInfo> ParseActivityFiles(string rootPath) {
    var activityList = new List<ActivityInfo>();
    var xmlFiles = Directory.GetFiles(rootPath, "*-service.xml", SearchOption.AllDirectories);

    foreach (var file in xmlFiles) {
      try {
        XDocument doc = XDocument.Load(file);
        var serviceElement = doc.Root;

        // <service> 루트 확인
        if (serviceElement == null || serviceElement.Name.LocalName != "service")
          continue;

        XNamespace ns = serviceElement.Name.Namespace;
        string serviceName = serviceElement.Attribute("name")?.Value ?? "";

        // Router activity 찾기
        var router = serviceElement.Elements(ns + "activity")
                                   .FirstOrDefault(e => e.Attribute("name")?.Value == "Router");
        if (router == null) continue;

        var transitions = router.Elements(ns + "transition");

        foreach (var transition in transitions) {
          string transitionName = transition.Attribute("name")?.Value ?? "";
          string transitionValue = transition.Attribute("value")?.Value ?? "";

          var procedureNames = new List<string>();
          var resultKeys = new List<string>();

          string dao = "";
          //string resultKey = "";
          string activityClass = "";

          string currentActivityName = transitionValue;

          // 다음 activity를 따라가며 프로시저 체인 및 result-key 체인 구성
          while (!string.Equals(currentActivityName, "end", StringComparison.OrdinalIgnoreCase)) {
            var activity = serviceElement.Elements(ns + "activity")
                                         .FirstOrDefault(e => e.Attribute("name")?.Value == currentActivityName);
            if (activity == null)
              break;

            string procName = activity.Elements(ns + "property")
                                      .FirstOrDefault(p => p.Attribute("name")?.Value == "procedure-name")
                                      ?.Attribute("value")?.Value ?? "";

            if (!string.IsNullOrWhiteSpace(procName))
              procedureNames.Add(procName);

            string resKey = activity.Elements(ns + "property")
                                    .FirstOrDefault(p => p.Attribute("name")?.Value == "result-key")
                                    ?.Attribute("value")?.Value ?? "";

            if (!string.IsNullOrWhiteSpace(resKey))
              resultKeys.Add(resKey);

            // dao, activityClass는 첫 활동 기준
            if (string.IsNullOrEmpty(dao)) {
              dao = activity.Elements(ns + "property")
                            .FirstOrDefault(p => p.Attribute("name")?.Value == "dao")
                            ?.Attribute("value")?.Value ?? "";
            }

            if (string.IsNullOrEmpty(activityClass)) {
              activityClass = activity.Attribute("class")?.Value ?? "";
            }

            // 다음 activity 이름 탐색
            var nextTransition = activity.Elements(ns + "transition").FirstOrDefault();
            currentActivityName = nextTransition?.Attribute("value")?.Value ?? "end";
          }

          activityList.Add(new ActivityInfo {
            ServiceName = serviceName,
            TransitionName = transitionName,
            TransitionValue = transitionValue,
            Dao = dao,
            ProcedureName = string.Join(" -> ", procedureNames),
            ResultKey = string.Join(" -> ", resultKeys),
            Activity = activityClass
          });
        }
      }
      catch (Exception ex) {
        Console.WriteLine($"[ERROR] {file}: {ex.Message}");
      }
    }

    return activityList;
  }



  public static List<SrcFileInfo> ParseSrcFiles(string rootPath, string extend) {
    var activityList = new List<SrcFileInfo>();
    var jspFiles = Directory.GetFiles(rootPath, "*."+ extend, SearchOption.AllDirectories);

    foreach (var file in jspFiles) {
      try {

        FileInfo fi = new FileInfo(file);



        string relativePath = Path.GetRelativePath(rootPath, fi.FullName);
        string[] parts = relativePath.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        string firstDir = parts.Length > 0 ? parts[0] : "";


        string fileNameOnly = Path.GetFileNameWithoutExtension(fi.Name);

        activityList.Add(new SrcFileInfo {
          GubunDir = firstDir,
          FullPath = fi.FullName,
          FileName = fileNameOnly,
          FileNameNExtend = fi.Name,
          Extend = fi.Extension,
          CreateDate = fi.CreationTime,
          ModifyDate = fi.LastWriteTime,
          LastDate = fi.LastAccessTime
        });
      }
      catch (Exception ex) {
        Console.WriteLine($"[ERROR] {file}: {ex.Message}");
      }
    }

    return activityList;
  }




}
