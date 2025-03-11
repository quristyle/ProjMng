using Dapper;
using Npgsql;
using System.Data;
using System.Dynamic;
using System.Linq.Expressions;

namespace ProjMngServer.Services;


public static class DataReaderExtensions{
    public static IEnumerable<IDictionary<string, object>> ToDictionaries(this NpgsqlDataReader reader)    {
        while (reader.Read())        {
            yield return reader.ToDictionary();
        }
    }

    public static IDictionary<string, object> ToDictionary(this NpgsqlDataReader reader)    {
        var dictionary = new ExpandoObject() as IDictionary<string, object>;
        for (int i = 0; i < reader.FieldCount; i++)
        {
            dictionary.Add(reader.GetName(i), reader.GetValue(i));
        }
        return dictionary;
    }


  public static IEnumerable<dynamic> ToDynamicEnumerable(this NpgsqlDataReader reader) {
    while (reader.Read()) {
      yield return reader.ToDynamic();
    }
  }

  public static dynamic ToDynamic(this NpgsqlDataReader reader) {
    var expandoObject = new ExpandoObject() as IDictionary<string, object>;
    for (int i = 0; i < reader.FieldCount; i++) {
      expandoObject.Add(reader.GetName(i), reader.GetValue(i));
    }
    return expandoObject;
  }


}

public class BaseService {

}


public class ProjService : IProjService {

  private readonly IConfiguration _configuration;

  public ProjService(IConfiguration configuration) { _configuration = configuration; }

  public Dictionary<string, object> GetData(Dictionary<string, string> param) {

    DateTime sdt = DateTime.Now;
    DateTime spdt = DateTime.Now;
    DateTime epdt = DateTime.Now;
    int rcnt = 0;
    int errcode = 0;
    string message = string.Empty;


    string procedureName = param.TryGetValue("stp", out var stpValue) && stpValue != null ? stpValue.ToString().ToLower() : string.Empty;

    IEnumerable<dynamic> aaa = Enumerable.Empty<dynamic>(); // Initialize with an empty collection
                                                            //Dictionary<string, object> jjj = null;//new Dictionary<string, object>();

    IDictionary<string, object> bbb = new Dictionary<string, object>();
    if (string.IsNullOrWhiteSpace(procedureName)) {
    }
    else {
      var connectionString = _configuration.GetConnectionString("jsini");

      //var consDic =
      var consDic = connectionString
     .Split(';', StringSplitOptions.RemoveEmptyEntries)
     .Select(part => part.Split('=', 2))
     .Where(part => part.Length == 2)
     .ToDictionary(sp => sp[0].Trim(), sp => sp[1].Trim());

      string schema_name = consDic.TryGetValue("SearchPath", out var schemaValue) && schemaValue != null ? schemaValue.ToString() : string.Empty;

      //SearchPath=

      IEnumerable<dynamic> procParams;
      var parameters = new DynamicParameters();

      //  using (NpgsqlConnection db = new NpgsqlConnection(connectionString)) {

      try {
        using (IDbConnection db = new NpgsqlConnection(connectionString)) {


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
                    and p.specific_name ~ ('^{procedureName}(_[0-9]+)?$')
                ORDER BY
                    p.ordinal_position;
            ";

          procParams = db.Query(getProcParamsQuery);

          if (procParams.ToList().Count <= 0) {
            errcode = -1;
            message = $"{procedureName} 정보를 가져오지 못했습니다.";
          }


          spdt = DateTime.Now;

          if (errcode >= 0) {

            string outCursorParamName = null;


            db.Open();
            using (var tran = db.BeginTransaction()) {



              // 프로시저 파라미터 구성
              if (procParams.Any()) { // 프로시저의 파라미터가 존재하는 경우만 처리.
                foreach (var p in procParams) {
                  string paramName = p.parameter_name;
                  string paramKey = paramName.StartsWith("p_") ? paramName.Substring(2, paramName.Length - 2) : paramName;

                  // check parameter mode
                  string parameterMode = p.parameter_mode.ToString().ToUpper();
                  if (parameterMode == "INOUT" && p.data_type.ToString() == "refcursor") {
                    outCursorParamName = paramName;





                    parameters.Add(paramName, dbType: DbType.Object, direction: ParameterDirection.Output); // Output refcursor
                  }
                  else {
                    // param 에 일치하는 key 가 있는지 확인 하고, 있다면 값을 가져오고, 없으면 null.
                    // param 에 일치하는 key 가 없다면, null 로 설정된다.
                    //object paramValue = param.TryGetValue(paramKey, out var value) && value != null ? value.ToString() : DBNull.Value;
                    object paramValue = param.TryGetValue(paramKey, out var value) && value != null ? value.ToString() : null;
                    //if (paramValue == null) {
                    //  paramValue = param.TryGetValue("p_" + paramKey, out var value2) && value2 != null ? value2.ToString() : null;
                    //}
                    //else if (paramValue == null) {
                    //  paramValue = param.TryGetValue("in_" + paramKey, out var value2) && value2 != null ? value2.ToString() : null;
                    //}
                    parameters.Add(paramName, paramValue, DbType.String);




                  }
                }
              }




              db.Execute(sql: schema_name + "." + procedureName, param: parameters, commandType: CommandType.StoredProcedure);


              // out cursor 처리
              if (!string.IsNullOrEmpty(outCursorParamName)) {


                //var cursor = ncxxx.Parameters.TryGetValue(outCursorParamName, out NpgsqlParameter AAFFBB)?AAFFBB:null;

                var cursor = parameters.Get<string>(outCursorParamName);

                if (cursor != null) {

                  // 새로운 NpgsqlCommand 생성 (동일한 커넥션 사용)
                  // using (var cmd = new NpgsqlCommand($"FETCH ALL IN \"{cursor.Value}\"", db)) // db를 NpgsqlConnection으로 캐스팅
                  using (var cmd = new NpgsqlCommand($"FETCH ALL IN \"{cursor}\"", db as NpgsqlConnection)) // db를 NpgsqlConnection으로 캐스팅
                  using (var rdr = cmd.ExecuteReader()) {


                    var expandoObject2 = new ExpandoObject() as IDictionary<string, object>;
                    var resultList = new List<dynamic>();
                    var resultList2 = new List<dynamic>();
                    while (rdr.Read()) {
                      rcnt++;

                      var expandoObject = new ExpandoObject() as IDictionary<string, object>;
                      //var expandoObject2 = new ExpandoObject() as IDictionary<string, object>;
                      string nm = "";
                      object oval = null;
                      for (int i = 0; i < rdr.FieldCount; i++) {
                        nm = rdr.GetName(i);
                        oval = rdr.GetValue(i);
                        if (rcnt == 1) {
                          expandoObject2.Add(nm, rdr.GetFieldType(i).ToString());
                        }

                        Console.WriteLine("oval :" + oval.GetType());
                        Console.WriteLine("oval Name :" + oval.GetType().Name);

                        if (oval.GetType() == typeof(System.DBNull)) {
                          //expandoObject.Add(nm, null);
                          expandoObject.Add(nm, string.Empty);
                        }
                        else {
                          expandoObject.Add(nm, oval);
                        }
                      }
                      resultList.Add(expandoObject);
                    }
                    aaa = resultList;
                    bbb = expandoObject2;

                  }

                }
              }
              else { // out cursor 가 없는 경우, 그냥 쿼리 실행 해서 결과가 있으면 넣어준다.
                if (!procParams.Any()) { //프로시저의 파라미터가 없는 경우에만
                  aaa = db.Query<dynamic>(sql: schema_name + "." + procedureName, param: parameters, commandType: CommandType.StoredProcedure);
                }
              }



              tran.Commit();


              epdt = DateTime.Now;



            }



          }



        }


      }
      catch (Exception ee) {
        errcode = -99;
        message = ee.Message;
      }
      finally {
      }
    }
      DateTime edt = DateTime.Now;

    var data = new Dictionary<string, object> {
            { "rec", new Dictionary<string, object>(){ { "code", errcode },
            { "msg", message },
            { "p", param },
                                                       { "sdt", sdt.ToString("yyyy.MM.dd HH:mm:ss") },
                                                       { "edt", edt.ToString("yyyy.MM.dd HH:mm:ss") },
                                                       { "dtgap", (edt-sdt).TotalSeconds },
                                                       { "spdt", spdt.ToString("yyyy.MM.dd HH:mm:ss") },
                                                       { "epdt", epdt.ToString("yyyy.MM.dd HH:mm:ss") },
                                                       { "dtp_gap", (edt-sdt).TotalSeconds },
                                                       { "tot_sgap", (epdt-sdt).TotalSeconds },
                                                       { "tot_mgap", (epdt-sdt).Milliseconds },
                                                       { "cnt", rcnt },
                                                     }
            },
            { "cols", bbb },
            { "data", aaa }
        };

    return data;
  }
}

interface IProjService {
  Dictionary<string, object> GetData(Dictionary<string, string> parameters);
}