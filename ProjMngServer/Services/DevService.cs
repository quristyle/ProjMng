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
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using System.Dynamic;
using System.Runtime.Intrinsics.Arm;
using System.Reflection.Metadata;
using static System.Net.WebRequestMethods;
using Microsoft.Extensions.Primitives;

namespace ProjMngServer.Services;
public class DevService : BaseService {

  public DevService(IConfiguration configuration) { _configuration = configuration; }


  public ResultInfo<dynamic> GetDataQuery(string dbNick, string query) {

    ResultInfo<dynamic> ri = new ResultInfo<dynamic>();

    DateTime sdt = DateTime.Now;

    DbInfo di = GetDbInfo(dbNick);

    IDbConnection db = null;
    IDataReader rdr = null;
    try {

      if (di.Db_type == "POSTGRESQL") {
        db = new NpgsqlConnection(di.ToConnectionString());
      }
      else if (di.Db_type == "MSSQL") {
        db = new SqlConnection(di.ToConnectionString());
      }



      rdr = db.ExecuteReader(sql: query);

      var resultList = new List<dynamic>();

      while (rdr.Read()) {

        var expandoObject = new ExpandoObject() as IDictionary<string, object>;
        string nm = "";
        object oval = null;
        //string empty = null;
        for (int i = 0; i < rdr.FieldCount; i++) {
          nm = rdr.GetName(i);
          oval = rdr.GetValue(i);

          // oval 값이 Dbnull 인 경우 json 으로 {} 넘어 간다... 이를 클라이언트에서 처리시 잘못하면 parse error 가 난다.
          if (oval.GetType() == typeof(System.DBNull)) {
            expandoObject.Add(nm, null);
          }
          else {
            expandoObject.Add(nm, oval);
          }
        }
        resultList.Add(expandoObject);
      }



      ri.Cols = GetColumns(rdr);

      ri.Data = resultList;










    }
    catch (Exception e) {
      ri.Code = -99;
      ri.Message = e.Message;
    }
    finally {
      if (rdr != null) { rdr.Close(); rdr.Dispose(); }
      if (db != null) { db.Close(); db.Dispose(); }
    }

    return ri;


  }

  public ResultInfo<dynamic> GetData(Dictionary<string, string> param) {

    //ResultInfo<dynamic> ri = new ResultInfo<dynamic>();

    DateTime sdt = DateTime.Now;
    string dbtype = param.TryGetValue("db", out var dbValue) && dbValue != null ? dbValue.ToString() : string.Empty;
    string stp = param.TryGetValue("stp", out var stpValue) && stpValue != null ? stpValue.ToString() : string.Empty;
    string sta = param.TryGetValue("sta", out var staValue) && staValue != null ? staValue.ToString() : string.Empty;
    string sob = param.TryGetValue("sob", out var sobValue) && sobValue != null ? sobValue.ToString() : string.Empty;
    string proj = param.TryGetValue("proj", out var projValue) && projValue != null ? projValue.ToString() : string.Empty;
    string dbNick = param.TryGetValue("dbnick", out var dbNickValue) && dbNickValue != null ? dbNickValue.ToString() : string.Empty;
    string sva = param.TryGetValue("sva", out var svaValue) && svaValue != null ? svaValue.ToString() : string.Empty;

    DateTime spdt = DateTime.Now;
    DateTime epdt = DateTime.Now;

    ResultInfo<dynamic> ri = DevExecuteQuery(dbNick, stp, param);
    epdt = DateTime.Now;
    GetRes(ref ri, param, sdt, spdt, epdt);

    return ri;
  }


  public ResultInfo<dynamic> DevExecuteQuery(string dbNick, string stp, Dictionary<string, string> param) {

    ResultInfo<dynamic> ri = new ResultInfo<dynamic>();

    // 디비정보 가져오기
    DbInfo di = GetDbInfo(dbNick);

    // 디비의 쿼리 정보 가져오기
    Devsqlresp dsr = GetDsr(di, stp);



    if (dsr == null) {
      ri.Code = -88;
      ri.Message = $" dbtype {di.Db_type} 의 {stp} 가 정의 되지 않았습니다.";
      return ri;
    }


    IDbConnection db = null;
    IDataReader rdr = null;
    try {

      if (dsr.Dsl_type == "POSTGRESQL") {
        db = new NpgsqlConnection(di.ToConnectionString());
      }
      else if (dsr.Dsl_type == "MSSQL") {
        db = new SqlConnection(di.ToConnectionString());
      }

      param["schema"] = di.Db_schema;

      string directString = ChangeQueryDirectQuery(dsr.Dsl_query, param);

      DynamicParameters parameters = GetParams(directString, param);

      rdr = db.ExecuteReader(sql: directString, param: parameters);

      var resultList = new List<dynamic>();

      while (rdr.Read()) {

        var expandoObject = new ExpandoObject() as IDictionary<string, object>;
        string nm = "";
        object oval = null;
        //string empty = null;
        for (int i = 0; i < rdr.FieldCount; i++) {
          nm = rdr.GetName(i);
          oval = rdr.GetValue(i);

          // oval 값이 Dbnull 인 경우 json 으로 {} 넘어 간다... 이를 클라이언트에서 처리시 잘못하면 parse error 가 난다.
          if (oval.GetType() == typeof(System.DBNull)) {
            expandoObject.Add(nm, null);
          }
          else {
            expandoObject.Add(nm, oval);
          }
        }
        resultList.Add(expandoObject);
      }



      ri.Cols = GetColumns(rdr);

      ri.Data = resultList;


    }
    catch (Exception e) {
      ri.Code = -99;
      ri.Message = e.Message;
    }
    finally {
      if (rdr != null) { rdr.Close(); rdr.Dispose(); }
      if (db != null) { db.Close(); db.Dispose(); }
    }

    return ri;


  }


  /// <summary> 디비에 맞는 시스템 쿼리 가져 오기 </summary>
  private Devsqlresp GetDsr(DbInfo di, string dsrKey) {
    Devsqlresp result = null;
    foreach (var dsr in AppData.DsrInfos) {
      if (dsr.Dsl_type == di.Db_type && dsr.Dsl_cd == dsrKey) {
        result = dsr;
        break;
      }
    }
    if (result == null) {
      result = GetDevsqlresp(di.Db_type, dsrKey);
      if (result != null) {
        AppData.DsrInfos.Add(result);
      }
    }

    return result;
  }




  private Devsqlresp GetDsr(string dbNick, string dsrKey) {

    // 디비정보 가져오기
    DbInfo di = GetDbInfo(dbNick);

    Devsqlresp result = null;
    foreach (var dsr in AppData.DsrInfos) {
      if (dsr.Dsl_type == di.Db_type && dsr.Dsl_cd == dsrKey) {
        result = dsr;
        break;
      }
    }
    if (result == null) {
      result = GetDevsqlresp(di.Db_type, dsrKey);
      AppData.DsrInfos.Add(result);
    }

    return result;
  }


  /// <summary> db 에서 dsr 정보를 가져온다. </summary>
  Devsqlresp GetDevsqlresp(string dsl_type, string dsl_cd) {
    var connectionString = _configuration.GetConnectionString("jsini");

    var parameters = new DynamicParameters();
    parameters.Add("@dsl_type", dsl_type);
    parameters.Add("@dsl_cd", dsl_cd);

    Devsqlresp dsr = null;
    using (IDbConnection db = new NpgsqlConnection(connectionString)) {
      dsr = db.Query<Devsqlresp>(sql: ConstInfo.dbVsqlResp, parameters).ToList().FirstOrDefault();
    }

    return dsr;
  }



  /// <summary>
  /// $key 값은 문자열을 바꾼다. 다이렉트 쿼리 문자열 $ 를 바꾼다.
  /// </summary>
  /// <param name="dsl_query"></param>
  /// <param name="param"></param>
  /// <returns></returns>
  string ChangeQueryDirectQuery(string dsl_query, Dictionary<string, string> param) {
    String result = dsl_query;
    // $key 값은 문자열을 바꾼다. 다이렉트 쿼리 문자열 $
    var _dd_matches = Regex.Matches(dsl_query, @"\$\w+");
    string[] _dd_kkk = _dd_matches.Select(m => m.Value).Distinct().ToArray();

    if (_dd_kkk != null && _dd_kkk.Length > 0) {

      foreach (string str in _dd_kkk) {
        string key = str.Replace("$", "");
        if (param.TryGetValue(key, out var strValue) && strValue != null) {
          result = result.Replace(str, strValue);
        }
      }

    }
    return result;
  }



  DynamicParameters GetParams(string directString, Dictionary<string, string> param) {


    DynamicParameters parameters = new DynamicParameters();

    // @ 값은 파라미터를 등록한다.
    var matches = Regex.Matches(directString, @"@\w+");
    string[] kkk = matches.Select(m => m.Value).Distinct().ToArray();


    if (kkk != null && kkk.Length > 0) {
      foreach (string str in kkk) {
        parameters.Add(str.Replace("@", ""), param.TryGetValue(str.Replace("@", ""), out var strValue) && strValue != null ? strValue.ToString() : string.Empty);
      }
    }

    return parameters;

  }

}