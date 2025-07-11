using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ProjModel;


public static class BaseModelExtensions {




  public static Dictionary<string, string> ToDictionary(this BaseModel wbs) {
    var dict = new Dictionary<string, string>();
    var props = wbs.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

    foreach (var prop in props) {
      var value = prop.GetValue(wbs);

      // DateTime 또는 DateTime? 처리
      if (prop.PropertyType == typeof(DateTime) || prop.PropertyType == typeof(DateTime?)) {
        var dt = value as DateTime?;
        dict[prop.Name.ToLower()] = dt.HasValue ? dt.Value.ToString("yyyy-MM-dd HH:mm:ss") : "";
      }
      //else if (prop.PropertyType == typeof(DateOnly) || prop.PropertyType == typeof(DateOnly?)) {
      //  var dt = value as DateOnly?;
      //  dict[prop.Name] = dt.HasValue ? dt.Value.ToString("yyyy-MM-dd") : "";
      //}
      else if (value != null) {
        dict[prop.Name.ToLower()] = value.ToString();
      }
      else {
        dict[prop.Name.ToLower()] = "";


      }
    }
    return dict;
  }


  public static string GetValue(this IDictionary<string, string> param, string pkey) {
    if (param.TryGetValue(pkey, out var dbValue) && dbValue != null)
      return dbValue.ToString().Trim();

    // 대소문자 무시하고 키 검색
    var key = param.Keys.FirstOrDefault(k => string.Equals(k, pkey, StringComparison.OrdinalIgnoreCase));
    if (key != null && param.TryGetValue(key, out dbValue) && dbValue != null)
      return dbValue.ToString().Trim();

    return string.Empty;
  }


  public static string GetValue(this IDictionary<string, object> param, string pkey) {
    //Console.WriteLine($" dicGetValue : {pkey}");
    if (param.TryGetValue(pkey, out var dbValue) && dbValue != null)
      return dbValue.ToString().Trim();

    // 대소문자 무시하고 키 검색
    var key = param.Keys.FirstOrDefault(k => string.Equals(k, pkey, StringComparison.OrdinalIgnoreCase));
    if (key != null && param.TryGetValue(key, out dbValue) && dbValue != null)
      return dbValue.ToString().Trim();

    return string.Empty;
  }



}


public static class ModelHelper {
  public static Dictionary<string, string> ToCols(Type modelType) {
    Dictionary<string, string> col = new Dictionary<string, string>();

    var properties = modelType.GetProperties();

    foreach (var prop in properties) {
      string name = prop.Name.ToLower();
      string type = prop.PropertyType.FullName;

      if (prop.PropertyType == typeof(DateTime?))
        type = "System.DateTime";

      col[name] = type;
    }

    return col;
  }

  // 제네릭 오버로드도 제공 가능
  public static Dictionary<string, string> ToCols<T>() where T : BaseModel {
    return ToCols(typeof(T));
  }
}


