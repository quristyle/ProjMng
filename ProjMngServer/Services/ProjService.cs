using System.Data;
using System.Collections.Generic;
using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using System.Collections;
using ProjModel;
using System.Security.Cryptography;
using Npgsql;
using System.Text.RegularExpressions;
using System.Diagnostics;
using NpgsqlTypes;
using Npgsql;
using System.Text.RegularExpressions;
using System.Diagnostics;
using NpgsqlTypes;
using System.Dynamic;
using System.Linq;
using Microsoft.AspNetCore.SignalR.Protocol;
using System.Threading.Tasks;
using System.Transactions;

namespace ProjMngServer.Services;


public static class DataReaderExtensions
{
    public static IEnumerable<IDictionary<string, object>> ToDictionaries(this NpgsqlDataReader reader)
    {
        while (reader.Read())
        {
            yield return reader.ToDictionary();
        }
    }

    public static IDictionary<string, object> ToDictionary(this NpgsqlDataReader reader)
    {
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

    public ProjService(IConfiguration configuration) {        _configuration = configuration;    }

    public Dictionary<string, object> GetData(Dictionary<string, string> param) {

    DateTime sdt = DateTime.Now;
    DateTime spdt = DateTime.MinValue;
    DateTime epdt = DateTime.MinValue;
    int rcnt = 0;


    string procedureName = param.TryGetValue("stp", out var stpValue) && stpValue != null ? stpValue.ToString() : string.Empty;
        var connectionString = _configuration.GetConnectionString("jsini");

     //var consDic =
     var consDic = connectionString
    .Split(';', StringSplitOptions.RemoveEmptyEntries)
    .Select(part => part.Split('=', 2))
    .Where(part => part.Length == 2)
    .ToDictionary(sp => sp[0].Trim(), sp => sp[1].Trim());

    string schema_name = consDic.TryGetValue("SearchPath", out var schemaValue) && schemaValue != null ? schemaValue.ToString() : string.Empty;

    //SearchPath=

    IEnumerable<dynamic> aaa = Enumerable.Empty<dynamic>(); // Initialize with an empty collection
        Dictionary<string, object> jjj = null;//new Dictionary<string, object>();

IEnumerable<dynamic> procParams;
            var parameters = new DynamicParameters();

    //  using (NpgsqlConnection db = new NpgsqlConnection(connectionString)) {

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




      spdt = DateTime.Now;



      string outCursorParamName = null;


      db.Open();
      using (var tran = db.BeginTransaction()) {



        // 프로시저 파라미터 구성
        if (procParams.Any()) { // 프로시저의 파라미터가 존재하는 경우만 처리.
          foreach (var p in procParams) {
            string paramName = p.parameter_name;
            string paramKey = paramName;

            // check parameter mode
            string parameterMode = p.parameter_mode.ToString().ToUpper();
            if (parameterMode == "INOUT" && p.data_type.ToString() == "refcursor") {
              outCursorParamName = paramName;





              parameters.Add(paramName, dbType: DbType.Object, direction: ParameterDirection.Output); // Output refcursor
            } else {
              // param 에 일치하는 key 가 있는지 확인 하고, 있다면 값을 가져오고, 없으면 null.
              // param 에 일치하는 key 가 없다면, null 로 설정된다.
              //object paramValue = param.TryGetValue(paramKey, out var value) && value != null ? value.ToString() : DBNull.Value;
              object paramValue = param.TryGetValue(paramKey, out var value) && value != null ? value.ToString() : null;
              parameters.Add(paramName, paramValue);




            }
          }
        }






        db.Execute(sql: schema_name+"." + procedureName, param: parameters, commandType: CommandType.StoredProcedure);


        // out cursor 처리
        if (!string.IsNullOrEmpty(outCursorParamName)) {


          //var cursor = ncxxx.Parameters.TryGetValue(outCursorParamName, out NpgsqlParameter AAFFBB)?AAFFBB:null;

          var cursor = parameters.Get<string>(outCursorParamName);

          if (cursor != null) {


            // 새로운 NpgsqlCommand 생성 (동일한 커넥션 사용)
            // using (var cmd = new NpgsqlCommand($"FETCH ALL IN \"{cursor.Value}\"", db)) // db를 NpgsqlConnection으로 캐스팅
            using (var cmd = new NpgsqlCommand($"FETCH ALL IN \"{cursor}\"", db as NpgsqlConnection)) // db를 NpgsqlConnection으로 캐스팅
            using (var rdr = cmd.ExecuteReader()) {


              var resultList = new List<dynamic>();
              while (rdr.Read()) {
                rcnt++;
                var expandoObject = new ExpandoObject() as IDictionary<string, object>;
                for (int i = 0; i < rdr.FieldCount; i++) {
                  expandoObject.Add(rdr.GetName(i), rdr.GetValue(i));
                }
                resultList.Add(expandoObject);
              }
              aaa = resultList;

            }

          }
        } else { // out cursor 가 없는 경우, 그냥 쿼리 실행 해서 결과가 있으면 넣어준다.
          if (!procParams.Any()) { //프로시저의 파라미터가 없는 경우에만
            aaa = db.Query<dynamic>(sql: schema_name+"." + procedureName, param: parameters, commandType: CommandType.StoredProcedure);
          }
        }



        tran.Commit();


        epdt = DateTime.Now;



      }
    }

    DateTime edt = DateTime.Now;

    var data = new Dictionary<string, object> {
            { "rec", new Dictionary<string, object>(){ { "p", param },
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
            { "data", aaa }
        };

        return data;
    }
}

interface IProjService {
    Dictionary<string, object> GetData(Dictionary<string, string> parameters);
}
