

namespace ProjMngServer;

using ProjModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

public class ActivityParser {
  public static List<ActivityInfo> ParseActivityFiles(string rootPath) {
    var activityList = new List<ActivityInfo>();

    var xmlFiles = Directory.GetFiles(rootPath, "*.xml", SearchOption.AllDirectories);

    foreach (var file in xmlFiles) {
      try {
        XDocument doc = XDocument.Load(file);

        XNamespace ns = "http://www.poscoict.com/glueframework/service";
        var serviceElement = doc.Root;
        if (serviceElement == null) continue;

        string serviceName = serviceElement.Attribute("name")?.Value ?? "";

        // Router 안에 있는 transition 들 가져오기
        var router = serviceElement.Elements(ns + "activity")
                                   .FirstOrDefault(e => e.Attribute("name")?.Value == "Router");

        if (router == null) continue;

        var transitions = router.Elements(ns + "transition");

        foreach (var transition in transitions) {
          string transitionName = transition.Attribute("name")?.Value ?? "";
          string transitionValue = transition.Attribute("value")?.Value ?? "";

          // transition value 와 일치하는 activity 찾기
          var activity = serviceElement.Elements(ns + "activity")
                                       .FirstOrDefault(e => e.Attribute("name")?.Value == transitionValue);
          if (activity == null) continue;

          string activityClass = activity.Attribute("class")?.Value ?? "";
          string dao = activity.Elements(ns + "property")
                               .FirstOrDefault(p => p.Attribute("name")?.Value == "dao")
                               ?.Attribute("value")?.Value ?? "";

          string procedureName = activity.Elements(ns + "property")
                                         .FirstOrDefault(p => p.Attribute("name")?.Value == "procedure-name")
                                         ?.Attribute("value")?.Value ?? "";

          string resultKey = activity.Elements(ns + "property")
                                     .FirstOrDefault(p => p.Attribute("name")?.Value == "result-key")
                                     ?.Attribute("value")?.Value ?? "";

          activityList.Add(new ActivityInfo {
            ServiceName = serviceName,
            TransitionName = transitionName,
            TransitionValue = transitionValue,
            Dao = dao,
            ProcedureName = procedureName,
            ResultKey = resultKey,
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
}
