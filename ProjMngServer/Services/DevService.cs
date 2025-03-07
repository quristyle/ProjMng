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

namespace ProjMngServer.Services {
  public class DevService : IDevService {


    private readonly IConfiguration _configuration;

    public DevService(IConfiguration configuration) {
      _configuration = configuration;
    }


    public IEnumerable<dynamic> DevExecuteQuery(Devsqlresp dsr, Dictionary<string, string> param) {
      //var connectionString = _configuration.GetConnectionString(dsr.DbConnectionString);

if(dsr.Dsl_type == "POSTGRESQL" ){




      using (IDbConnection db = new NpgsqlConnection(dsr.DbConnectionString)) {

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

          Debug.WriteLine("exsit prames run  " );
          return db.Query(sql: dsr.Dsl_query, param : parameters);
        }
        else {
          Debug.WriteLine("not exsit prames  ");
          return db.Query(sql: dsr.Dsl_query);
        }

      }
}
      else{


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

          Debug.WriteLine("exsit prames run  " );
          return db.Query(sql: dsr.Dsl_query, param : parameters);
        }
        else {
          Debug.WriteLine("not exsit prames  ");
          return db.Query(sql: dsr.Dsl_query);
        }

      }

      }



    }





    public static Dictionary<string, string> db_constring = new Dictionary<string, string>();

    string mssqlConstrFormat = @"Server={0},{1};Database={2};User Id={3};Password={4};{5}";
    //Server=mitddns02.iptime.org,14344;Database=mitERP_HANJU;User Id=sa;Password=mit0104!;TrustServerCertificate=True
    string postgresqlConstrFormat = @"Host={0};Port={1};Database={2};Username={3};Password={4}";
    //Host=jsini.co.kr;Port=15432;Database=jsini;Username=jsini;Password=jsini
    string mysqlConstrFormat = @"Server={0},{1};Database={2};User Id={3};Password={4};{5}";

    Devsqlresp GetQuery(string dsl_type = "MSSQL", string dsl_cd = "proclist", string db_nick= "hanju_dev") {
      if (string.IsNullOrEmpty(dsl_type)) dsl_type = "MSSQL";
      if (string.IsNullOrEmpty(dsl_cd)) dsl_cd = "proclist";
      if (string.IsNullOrEmpty(db_nick)) db_nick = "hanju_dev";

      Debug.WriteLine("dsl_type: " + dsl_type);
      Debug.WriteLine("dsl_cd: " + dsl_cd);
      Debug.WriteLine("db_nick: " + db_nick);

      var connectionString = _configuration.GetConnectionString("jsini");

      Debug.WriteLine("connectionString: " + connectionString);

      string query = @"
select d.* 
  from projmng.devsqlresp d 
 where dsl_type = '"+ dsl_type + @"'
   and dsl_cd = '"+ dsl_cd + @"'
";
      Devsqlresp dsr = null;
      using (IDbConnection db = new NpgsqlConnection(connectionString)) {
        dsr= db.Query< Devsqlresp>(sql: query).ToList().FirstOrDefault();

        Debug.WriteLine("dsr.Dsl_query: " + dsr.Dsl_query);
        Debug.WriteLine("dsr.Dsl_cd: " + dsr.Dsl_cd);
        Debug.WriteLine("dsr.Dsl_type: " + dsr.Dsl_type);


      }

      dsr.DbConnectionString = GetConstring(db_nick);
      Debug.WriteLine("dsr.DbConnectionString: " + dsr.DbConnectionString);

      return dsr;
    }



string db_nick_key = "@db_nick";
string dbConQuery = @"
select db_ip, db_port, db_database, db_id, db_pwd, db_cert, db_comm, db_nick, db_type
  from projmng.devdbinfo d 
 where db_nick = @db_nick
";
    string GetConstring(string db_nick) {

      string result = db_constring.TryGetValue(db_nick, out var dbValue) ? dbValue.ToString() : string.Empty;

      if (string.IsNullOrEmpty(result)) { // 없으면 가져온다.

        var connectionString = _configuration.GetConnectionString("jsini");
        using (IDbConnection db = new NpgsqlConnection(connectionString)) {

          var parameters = new DynamicParameters();
          parameters.Add(db_nick_key, db_nick);
          DbInfo dbinfo = db.Query<DbInfo>(sql: dbConQuery, param: parameters).ToList().FirstOrDefault();
          switch (dbinfo.Db_type) {
            case "MSSQL":
              result = string.Format(mssqlConstrFormat, dbinfo.Db_ip, dbinfo.Db_port, dbinfo.Db_database, dbinfo.Db_id, dbinfo.Db_pwd, dbinfo.Db_cert);
              break;
            case "POSTGRESQL":
              result = string.Format(postgresqlConstrFormat, dbinfo.Db_ip, dbinfo.Db_port, dbinfo.Db_database, dbinfo.Db_id, dbinfo.Db_pwd);
              break;
            case "MYSQL":
              result = string.Format(mysqlConstrFormat, dbinfo.Db_ip, dbinfo.Db_port, dbinfo.Db_database, dbinfo.Db_id, dbinfo.Db_pwd);
              break;
          }
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
      string dbNick =           param.TryGetValue("dbnick", out var dbNickValue) && dbNickValue != null ? dbNickValue.ToString() : string.Empty;
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
