using BlazorMonaco.Editor;
using ProjModel;

namespace ProjMngWasm.Commons;
public class WasmUtil {

  // 복사하기
  public static List<CommonCode> DeepCopy(List<CommonCode> original) {
    return original.Select(item => new CommonCode {
      Code = item.Code,
      Name = item.Name,
      Desc = item.Desc,
      Others = new Dictionary<string, string>(item.Others)
    }).ToList();
  }


  /// <summary>
  /// dic 결합
  /// </summary>
  /// <param name="dict1"></param>
  /// <param name="dict2"></param>
  /// <returns></returns>
  public static Dictionary<string, string> JoinDictionaries(IDictionary<string, object> dict1, Dictionary<string, string> dict2) {
    
    var d1 = JoinConvert(dict1);

    var result = JoinDictionaries(d1, dict2);

    return result;
  }
  public static Dictionary<string, string> JoinDictionaries(Dictionary<string, string> dict1, IDictionary<string, object> dict2) {

    var d1 = JoinConvert(dict2);

    var result = JoinDictionaries(dict1, d1);

    return result;
  }

  /// <summary>
  /// dic 변환 obj to string
  /// </summary>
  /// <param name="dict1"></param>
  /// <returns></returns>
  public static Dictionary<string, string> JoinConvert(IDictionary<string, object> dict1) {
    var result = new Dictionary<string, string>();

    foreach (var c in dict1) {
      if (c.Value == null) continue;
        if (c.Value.GetType() == typeof(DateTime)) {

          result[c.Key] = ((DateTime)dict1[c.Key]).ToString("yyyyMMdd");

        }
        else {
          result[c.Key] = c.Value.ToString();
        }

    }

    return result;
  }


  public static Dictionary<string, string> JoinDictionaries(IDictionary<string, string> dict1, Dictionary<string, string> dict2) {
    var result = new Dictionary<string, string>();

    var allKeys = dict1.Keys.Union(dict2.Keys);

    foreach (var key in allKeys) {
      if (dict1.ContainsKey(key)) {
        result[key] = dict1[key] + "";
      }
      else if (dict2.ContainsKey(key)) {
        result[key] = dict2[key];
      }
    }

    return result;
  }


  public static Type GetType (string clsStr) {
    var nullableTypes = new Dictionary<string, string> {
        { "System.DateTime", "System.Nullable`1[System.DateTime]" },
        { "System.Int32", "System.Nullable`1[System.Int32]" },
        { "System.Boolean", "System.Nullable`1[System.Boolean]" },
        { "System.Byte", "System.Nullable`1[System.Byte]" },
        { "System.Char", "System.Nullable`1[System.Char]" },
        { "System.Decimal", "System.Nullable`1[System.Decimal]" },
        { "System.Double", "System.Nullable`1[System.Double]" },
        { "System.Single", "System.Nullable`1[System.Single]" }, // float
        { "System.Int64", "System.Nullable`1[System.Int64]" }, // long
        { "System.SByte", "System.Nullable`1[System.SByte]" },
        { "System.Int16", "System.Nullable`1[System.Int16]" }, // short
        { "System.UInt32", "System.Nullable`1[System.UInt32]" },
        { "System.UInt64", "System.Nullable`1[System.UInt64]" },
        { "System.UInt16", "System.Nullable`1[System.UInt16]" }
    };

    if (nullableTypes.ContainsKey(clsStr)) {
      return Type.GetType(nullableTypes[clsStr]);

      //return Type.GetType(nullableTypes[clsStr] + ", System.Private.CoreLib");
    }
    else if (clsStr == "System.String") {
      return typeof(string);
    }
    else {
      return Type.GetType(clsStr);
    }
  }


}
