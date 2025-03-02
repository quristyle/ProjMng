using System.Data;
using System.Collections.Generic;
using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;

namespace ProjMngServer.Services {
  public class DevService : IDevService {


    private readonly IConfiguration _configuration;

    public DevService(IConfiguration configuration) {
      _configuration = configuration;
    }


    public IEnumerable<dynamic> ExecuteQuery(string query, string connectionStringName) {
      var connectionString = _configuration.GetConnectionString(connectionStringName);

      using (IDbConnection db = new SqlConnection(connectionString)) {
        return db.Query(query);
      }
    }

    public Dictionary<string, object> GetData(Dictionary<string, object> param) {

      // 조회목적      [stp]: srch type .. db 관련, code 관련, file 관련, etc
      // 조회대상      [sta]: db, project code, doc 
      // 조회대상 세부  [sob]: table, proc, view, func, columns
      // 조회조건      [sva]: 조건값

      string stp = param.TryGetValue("stp", out var stpValue) && stpValue != null ? stpValue.ToString() : string.Empty;
      string sta = param.TryGetValue("sta", out var staValue) && staValue != null ? staValue.ToString() : string.Empty;
      string sob = param.TryGetValue("sob", out var sobValue) && sobValue != null ? sobValue.ToString() : string.Empty;
      string sva = param.TryGetValue("sva", out var svaValue) && svaValue != null ? svaValue.ToString() : string.Empty;



      string query = @"
SELECT ROUTINE_NAME AS [ProcedureName], ROUTINE_DEFINITION
FROM INFORMATION_SCHEMA.ROUTINES 
WHERE ROUTINE_TYPE = 'PROCEDURE'
";

      //and ROUTINE_NAME like @p0

     var aaa =  ExecuteQuery(query, "hanju_dev");



      var data = new Dictionary<string, object>      {
                { "Rec", param },
                { "result", aaa  }
            };

      return data;
    }
  }

  interface IDevService {
    Dictionary<string, object> GetData(Dictionary<string, object> parameters); 
    //IEnumerable<dynamic> ExecuteQuery(string query, string connectionStringName);

  }

}
