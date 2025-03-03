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

namespace ProjMngServer.Services {
  public class DevService : IDevService {


    private readonly IConfiguration _configuration;

    public DevService(IConfiguration configuration) {
      _configuration = configuration;
    }


    public IEnumerable<dynamic> DevExecuteQuery(Devsqlresp dsr, Dictionary<string, string> param) {
      //var connectionString = _configuration.GetConnectionString(dsr.DbConnectionString);

      using (IDbConnection db = new SqlConnection(dsr.DbConnectionString)) {


        var matches = Regex.Matches(dsr.Dsl_query, @"@\w+");
        string[] kkk = matches.Select(m => m.Value).ToArray();
        if (kkk != null && kkk.Length > 0) {
          Dictionary<string,string> dic = new Dictionary<string,string>(); 

          //var dic = new { DbNick = "" };

          foreach (string str in kkk) {
            dic.Add(str.Replace("@", ""),  (param.TryGetValue(str.Replace("@", ""), out var strValue) && strValue != null ? strValue.ToString() : string.Empty)     );
          }


          var parameters = new DynamicParameters();
          foreach (string str in kkk) {
            parameters.Add(str.Replace("@", ""), param.TryGetValue(str.Replace("@", ""), out var strValue) && strValue != null ? strValue.ToString() : string.Empty);
          }

          return db.Query(sql: dsr.Dsl_query, param : parameters);
        }
        else {
          return db.Query(sql: dsr.Dsl_query);
        }

      }
    }

    public static Dictionary<string, string> db_constring = new Dictionary<string, string>();

    Devsqlresp GetQuery(string dsl_type = "MSSQL", string dsl_cd = "proclist", string db_nick= "hanju_dev") {
      if (string.IsNullOrEmpty(dsl_type)) dsl_type = "MSSQL";
      if (string.IsNullOrEmpty(dsl_cd)) dsl_cd = "proclist";
      if (string.IsNullOrEmpty(db_nick)) db_nick = "hanju_dev";
      var connectionString = _configuration.GetConnectionString("jsini");
      string query = @"
select d.* 
  from projmng.devsqlresp d 
 where dsl_type = '"+ dsl_type + @"'
   and dsl_cd = '"+ dsl_cd + @"'
";
      Devsqlresp dsr = null;
      using (IDbConnection db = new NpgsqlConnection(connectionString)) {
        dsr= db.Query< Devsqlresp>(sql: query).ToList()[0];
      }

      dsr.DbConnectionString = GetConstring(db_nick);

      return dsr;
    }

    string GetConstring(string db_nick) {

      string result = db_constring.TryGetValue(db_nick, out var dbValue) ? dbValue.ToString() : string.Empty;


      if (string.IsNullOrEmpty(result)) {

        var connectionString = _configuration.GetConnectionString("jsini");
        string query = @"
select 'Server=mitddns02.iptime.org,14344;Database=mitERP_HANJU;User Id=sa;Password=mit0104!;TrustServerCertificate=True' as constring 
  from projmng.devdbinfo d 
 where db_nick = '" + db_nick + @"'
";
        using (IDbConnection db = new NpgsqlConnection(connectionString)) {
          result = db.Query<string>(sql: query).ToList()[0];
          db_constring[db_nick] = result;
        }
      }

      return result;
    }


    public Dictionary<string, object> GetData(Dictionary<string, string> param) {

      // 조회목적      [stp]: srch type .. db 관련, code 관련, file 관련, etc
      // 조회대상      [sta]: db, project code, doc 
      // 조회대상 세부  [sob]: table, proc, view, func, columns
      // 조회조건      [sva]: 조건값

      string db = param.TryGetValue("db", out var dbValue) && dbValue != null ? dbValue.ToString() : string.Empty;
      string stp = param.TryGetValue("stp", out var stpValue) && stpValue != null ? stpValue.ToString() : string.Empty;
      string sta = param.TryGetValue("sta", out var staValue) && staValue != null ? staValue.ToString() : string.Empty;
      string sob =   param.TryGetValue("sob", out var sobValue) && sobValue != null ? sobValue.ToString() : string.Empty;
      string proj =              param.TryGetValue("proj", out var projValue) && projValue != null ? projValue.ToString() : string.Empty;
      string dbNick =           param.TryGetValue("dbnick", out var dbNickValue) && projValue != null ? dbNickValue.ToString() : string.Empty;
      string sva =          param.TryGetValue("sva", out var svaValue) && svaValue != null ? svaValue.ToString() : string.Empty;


      Devsqlresp dsr = GetQuery(db, stp, dbNick);
      dsr.Proj = proj;

      var aaa = DevExecuteQuery(dsr, param);

      var data = new Dictionary<string, object>      {
                { "Rec", param },
                { "result", aaa  }
            };

      return data;
    }
  }

  interface IDevService {
    Dictionary<string, object> GetData(Dictionary<string, string> parameters); 
    //IEnumerable<dynamic> ExecuteQuery(string query, string connectionStringName);

  }

}
