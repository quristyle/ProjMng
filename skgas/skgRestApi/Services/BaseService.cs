using Dapper;
using Npgsql;
using skgRestApi.Models;
using System.Data;
using System.Dynamic;
using System.Runtime.Intrinsics.Arm;

namespace skgRestApi.Services;
public class BaseService {

  protected IConfiguration _configuration;


  /// <summary>
  /// 포로시저의 파라미터정보를 리턴
  /// </summary>
  /// <param name="db"></param>
  /// <param name="schema_name"></param>
  /// <param name="procedureName"></param>
  /// <returns></returns>
  protected IEnumerable<dynamic> ProcParams(IDbConnection db, string schema_name, string procedureName) {
    string getProcParamsQuery = $@"
                SELECT
                    p.parameter_name,
                    p.data_type,
                    p.specific_name,
                    p.parameter_mode
                FROM
                    information_schema.parameters p
                WHERE 1=1
                    -- p.specific_schema = '{schema_name}' 
                    and p.specific_name ~ ('^{procedureName.ToLower()}(_[0-9]+)?$')
                ORDER BY
                    p.ordinal_position;
            ";

    return db.Query(getProcParamsQuery);
  }


  /// <summary> 되돌려줄 response dictionary </summary>
  //protected void GetRes<T>(ref ResultInfo<T> ri, IDictionary<string, string> param
  //  , DateTime sdt, DateTime spdt, DateTime epdt
  //  ) {

  //  var rcnt = 0;

  //  ri.Res = new Dictionary<string, object>(){
  //        { "p", param },
  //        { "sdt", sdt.ToString("yyyy.MM.dd HH:mm:ss") },
  //        { "edt", DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss") },
  //        { "dtgap", (DateTime.Now-sdt).TotalSeconds },
  //        { "spdt", spdt.ToString("yyyy.MM.dd HH:mm:ss") },
  //        { "epdt", epdt.ToString("yyyy.MM.dd HH:mm:ss") },
  //        { "tot_sgap", (epdt-sdt).TotalSeconds },
  //        { "tot_mgap", (epdt-sdt).Milliseconds },
  //        { "cnt", rcnt },
  //      };

  //}

  public static List<Dictionary<string, object>> ConvertToListOfDictionaries(IEnumerable<dynamic> data) {
  
      var result = new List<Dictionary<string, object>>();
    if (data == null) return result;

    foreach (var item in data) {
      var dict = new Dictionary<string, object>();
      if (item is IDictionary<string, string> stringDict) {
        foreach (var kvp in stringDict) {
          dict[kvp.Key] = kvp.Value;
        }
      }
      else if (item is IDictionary<string, object> objectDict) {
        foreach (var kvp in objectDict) {
          dict[kvp.Key] = kvp.Value;
        }
      }
      result.Add(dict);
    }
    return result;
  }






  /// <summary>
  /// datareader 에서 컬럼명과 데이터타입을 dictionary로 리턴
  /// </summary>
  /// <param name="idr"></param>
  /// <returns></returns>
  //protected Dictionary<string, string> GetColumns(IDataReader idr) {

  //  var expandoObject = new Dictionary<string, string>();
  //  var schemaTable = idr.GetSchemaTable();
  //  if (schemaTable != null) {

  //    foreach (DataRow row in schemaTable.Rows) {
  //      string columnName = row["ColumnName"].ToString();
  //      string dataType = row["DataType"].ToString();
  //      expandoObject.Add(columnName, dataType);
  //    }
  //  }
  //  return expandoObject;
  //}






}

public static class ResponseUtil {

  /// <summary>
  /// datareader 에서 컬럼명과 데이터타입을 dictionary로 리턴
  /// </summary>
  /// <param name="idr"></param>
  /// <returns></returns>
  public static Dictionary<string, string> GetColumns(IDataReader idr) {

    var expandoObject = new Dictionary<string, string>();
    var schemaTable = idr.GetSchemaTable();
    if (schemaTable != null) {

      foreach (DataRow row in schemaTable.Rows) {
        string columnName = row["ColumnName"].ToString();
        string dataType = row["DataType"].ToString();
        expandoObject.Add(columnName, dataType);
      }
    }
    return expandoObject;
  }
}

