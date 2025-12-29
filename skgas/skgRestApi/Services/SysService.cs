using Dapper;
using Npgsql;
using skgRestApi.Models;
using System.Data;
using System.Dynamic;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;

namespace skgRestApi.Services;

public class SysService : BaseService {

  private readonly IMemoryCache _cache;
  private static CancellationTokenSource _resetCacheToken = new CancellationTokenSource();

  public SysService(IConfiguration configuration, IMemoryCache cache) { _configuration = configuration; _cache = cache; }

  public async Task<ResultInfo<dynamic>> GetDataAsync(string procedureName, IDictionary<string, string> param) {

    ResultInfo<dynamic> ri = new ResultInfo<dynamic>();

    if (string.IsNullOrWhiteSpace(procedureName)) { }
    else {
      // appsettings.json 설정 사용 권장 (없을 경우 하드코딩 값 사용)
      var connectionString = _configuration.GetConnectionString("ghub") 
                             ?? @"Host=jin114.co.kr;Port=31015;Database=ghub;Username=ghub;Password=ghub;Search Path=ghub";

      var builder = new NpgsqlConnectionStringBuilder(connectionString);
      string schema_name = builder.SearchPath ?? string.Empty;

      List<dynamic> procParams;
      var parameters = new DynamicParameters();

      // try {
      using (var db = new NpgsqlConnection(connectionString)) {
        await db.OpenAsync();

        string cacheKey = $"ProcParams_{procedureName}";
        if (!_cache.TryGetValue(cacheKey, out procParams)) {
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

          procParams = (await db.QueryAsync(getProcParamsQuery)).ToList();

          var cacheEntryOptions = new MemoryCacheEntryOptions()
              .SetAbsoluteExpiration(TimeSpan.FromMinutes(60))
              .AddExpirationToken(new CancellationChangeToken(_resetCacheToken.Token));

          _cache.Set(cacheKey, procParams, cacheEntryOptions);
        }

        if (procParams.Count > 0) {

          string outCursorParamName = null;

          using (var tran = await db.BeginTransactionAsync()) {

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
                  object paramValue = null;
                  if (param != null) {
                    paramValue = param.TryGetValue(paramKey, out var value) && value != null ? value.ToString() : null;
                  }
                  parameters.Add(paramName, paramValue, DbType.String);

                }
              }
            }

            await db.ExecuteAsync(sql: schema_name + "." + procedureName, param: parameters, commandType: CommandType.StoredProcedure, transaction: tran);

            // out cursor 처리
            if (!string.IsNullOrEmpty(outCursorParamName)) {

              var cursor = parameters.Get<string>(outCursorParamName);

              if (cursor != null) {

                using (var cmd = new NpgsqlCommand($"FETCH ALL IN \"{cursor}\"", db)) // db를 NpgsqlConnection으로 캐스팅
                {
                  cmd.Transaction = tran;
                  using (var rdr = await cmd.ExecuteReaderAsync()) {

                  var resultList = new List<dynamic>();
                  //var resultList2 = new List<dynamic>();


                  if (rdr.HasRows) {
                    while (await rdr.ReadAsync()) {

                      var expandoObject = new ExpandoObject() as IDictionary<string, object>;
                      string nm = "";
                      object oval = null;
                      string empty = null;
                      for (int i = 0; i < rdr.FieldCount; i++) {
                        nm = rdr.GetName(i);
                        oval = rdr.GetValue(i);

                        // oval 값이 Dbnull 인 경우 json 으로 {} 넘어 간다... 이를 클라이언트에서 처리시 잘못하면 parse error 가 난다.
                        if (oval.GetType() == typeof(System.DBNull)) {
                          expandoObject.Add(nm, empty);
                        }
                        else {
                          expandoObject.Add(nm, oval);
                        }
                      }
                      resultList.Add(expandoObject);
                    }

                  }

                  ri.Data = resultList;
                  ri.Cols = ResponseUtil.GetColumns(rdr);

                }
                }

              }
            }
            else { // out cursor 가 없는 경우, 그냥 쿼리 실행 해서 결과가 있으면 넣어준다.
              if (!procParams.Any()) { //프로시저의 파라미터가 없는 경우에만
                IEnumerable<dynamic> aaa = await db.QueryAsync<dynamic>(sql: schema_name + "." + procedureName, param: parameters, commandType: CommandType.StoredProcedure, transaction: tran);
                ri.Data = aaa.ToList();
              }
            }

            await tran.CommitAsync();



          }



        }



      }


    }

    return ri;
  }

// 캐시에 담겨 있는 모든 프로시저 파라미터 정보를 삭제한다.
public void ClearCache(){
    if (_resetCacheToken != null && !_resetCacheToken.IsCancellationRequested && _resetCacheToken.Token.CanBeCanceled) {
        _resetCacheToken.Cancel();
        _resetCacheToken.Dispose();
    }
    _resetCacheToken = new CancellationTokenSource();
}

}