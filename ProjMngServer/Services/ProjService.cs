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
}


public class ProjService : IProjService {

    private readonly IConfiguration _configuration;

    public ProjService(IConfiguration configuration) {        _configuration = configuration;    }

    public Dictionary<string, object> GetData(Dictionary<string, string> param) {
        string procedureName = param.TryGetValue("stp", out var stpValue) && stpValue != null ? stpValue.ToString() : string.Empty;
        var connectionString = _configuration.GetConnectionString("jsini");

        IEnumerable<dynamic> aaa = Enumerable.Empty<dynamic>(); // Initialize with an empty collection
        Dictionary<string, object> jjj = null;//new Dictionary<string, object>();

DataTable ddddttt= null;



IEnumerable<dynamic> procParams;
            var parameters = new DynamicParameters();

        using (NpgsqlConnection db = new NpgsqlConnection(connectionString)) {


db.Open();
  var tran =  db.BeginTransaction();



            // 프로시저 파라미터 정보 얻기 (수정된 정규식 사용)
            string getProcParamsQuery = $@"
                SELECT
                    p.parameter_name,
                    p.data_type,
                    p.specific_name,
                    p.parameter_mode
                FROM
                    information_schema.parameters p
                WHERE 1=1
                    -- p.specific_schema = 'projmng' AND -- 스키마 확인 (projmng)
                    and p.specific_name ~ ('^{procedureName}(_[0-9]+)?$')   -- 정확히 일치하는 이름으로 검색. sp_projdblist2 와 sp_projdblist 구분 가능.
                ORDER BY
                    p.ordinal_position;
            ";


            procParams = db.Query(getProcParamsQuery);

        
            string outCursorParamName = null;
            string realProcedureName = procedureName; // 프로시저의 실제이름을 저장.



NpgsqlCommand ncxxx = new NpgsqlCommand();
ncxxx.Connection = db;
ncxxx.CommandText = "projmng." + procedureName;
ncxxx.CommandType = CommandType.StoredProcedure;



            // 프로시저 파라미터 구성
            if (procParams.Any()) { // 프로시저의 파라미터가 존재하는 경우만 처리.
                foreach (var p in procParams) {
                    string paramName = p.parameter_name;
                    string paramKey = paramName;
                    realProcedureName = p.specific_name; // 프로시저의 실제 이름을 저장.
                    
                    // check parameter mode
                    string parameterMode = p.parameter_mode.ToString().ToUpper();
                    if (parameterMode == "INOUT" && p.data_type.ToString() == "refcursor") {
                        outCursorParamName = paramName;
NpgsqlParameter x = new NpgsqlParameter();

x.ParameterName = paramName;
x.NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Refcursor;
x.Direction = ParameterDirection.InputOutput;
x.Value = DBNull.Value;
ncxxx.Parameters.Add(x);



                       // parameters.Add(paramName, dbType: DbType.Object, direction: ParameterDirection.Output); // Output refcursor
                    }
                    else {
                        // param 에 일치하는 key 가 있는지 확인 하고, 있다면 값을 가져오고, 없으면 null.
                         // param 에 일치하는 key 가 없다면, null 로 설정된다.
                        object paramValue = param.TryGetValue(paramKey, out var value) && value != null ? value.ToString() : DBNull.Value;
                      //  parameters.Add(paramName, paramValue);
NpgsqlParameter x = new NpgsqlParameter();

x.ParameterName = paramName;
//x.NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Refcursor;
//x.Direction = ParameterDirection.InputOutput;
x.Value = paramValue;
ncxxx.Parameters.Add(x);


                    }
                }
            }

            

            // 프로시저 실행
          //  try {
              

ncxxx.ExecuteNonQuery();




              //  db.Execute(sql: "projmng." + procedureName, param: parameters, commandType: CommandType.StoredProcedure);

                

//db.CreateCommand();


               // out cursor 처리
                if (!string.IsNullOrEmpty(outCursorParamName)) {


   var cursor = ncxxx.Parameters.TryGetValue(outCursorParamName, out NpgsqlParameter AAFFBB)?AAFFBB:null;

                 //   var cursor = parameters.Get<string>(outCursorParamName);

                    if (cursor != null) {


//NpgsqlCommand nc = new NpgsqlCommand($"FETCH ALL IN \"{cursor}\"", db);


//ncxxx.Parameters.Clear();
//ncxxx.CommandText = $"FETCH ALL IN \"{cursor.Value}\"";
//ncxxx.CommandType = CommandType.TableDirect;

// NpgsqlDataReader dr = ncxxx.ExecuteReader();



// while (dr.Read())
// {
//     // do what you want with data, convert this to json or...
//     Console.WriteLine(dr[0]);

// //var ddddddd =  dr[0].ToDictionaries();

// }
// dr.Close();


// string aaaa = "";






                      // 새로운 NpgsqlCommand 생성 (동일한 커넥션 사용)
                       using (var cmd = new NpgsqlCommand($"FETCH ALL IN \"{cursor.Value}\"", db )) // db를 NpgsqlConnection으로 캐스팅
                         using (var rdr = cmd.ExecuteReader()) {


                         //IDbCommand cmd = db.CreateCommand();

                        //  IDbCommand cmd = db.CreateCommand();
                        //  cmd.CommandText = $"FETCH ALL IN \"{cursor}\"";
                        //  var rdr = cmd.ExecuteReader();

                        //     if( rdr != null){

                             //aaa = rdr.ToDictionary(); // this is the key!
                         //   jjj =  Enumerable.Range(0, rdr.FieldCount).ToDictionary(rdr.GetName, rdr.GetValue);

 ddddttt = new DataTable();
ddddttt.Load(rdr);


jjj = new Dictionary<string, object>();
foreach(DataRow dr in ddddttt.Rows){
    jjj.Add(dr[0]+"",dr[1]+"");
}

                        // aaa=  rdr.ToDictionaries();// .AsAsyncEnumerable<ThirdPartyFoo>(  (int x, DateTime y, int z) => new ThirdPartyFoo(…));

                        //     }


                         //var rdr = cmd.ExecuteReader();

                            //if( rdr != null){




                            // aaa = rdr.Cast<IDataRecord>().Select(record => {
                            //     var row = new System.Dynamic.ExpandoObject() as IDictionary<string, object>;
                            //     for (int i = 0; i < record.FieldCount; i++) {
                            //         row.Add(record.GetName(i), record.GetValue(i));
                            //     }
                            //     return row as dynamic;
                            // });


                            //}
                        }
                      
                    }
                } else { // out cursor 가 없는 경우, 그냥 쿼리 실행 해서 결과가 있으면 넣어준다.
                  if (!procParams.Any()){ //프로시저의 파라미터가 없는 경우에만
                    aaa = db.Query<dynamic>(sql: "projmng." + procedureName,param:parameters, commandType: CommandType.StoredProcedure);
                  }
                }
            // }
            // catch (Exception ex) {
            //     Debug.WriteLine($"Error executing stored procedure: {ex.Message}");
            //     aaa = Enumerable.Empty<dynamic>();
            // }


//db.EnlistTransaction(tran);

//db.EnlistTransaction(true);

  tran.Commit();

//db.Close();

        }

        var data = new Dictionary<string, object> {
            { "Rec", param },
            { "result", aaa },
            { "result2", jjj },
        };

        return data;
    }
}

interface IProjService {
    Dictionary<string, object> GetData(Dictionary<string, string> parameters);
}
