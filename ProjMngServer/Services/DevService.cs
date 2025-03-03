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


    public IEnumerable<dynamic> HanDevExecuteQuery(string query, object[] paramArr = null) {
      var connectionString = _configuration.GetConnectionString("hanju_dev");

      using (IDbConnection db = new SqlConnection(connectionString)) {
        if(paramArr == null) {
          return db.Query(sql: query);
        }
        else {
          return db.Query(sql: query, param: paramArr);

        }
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

      switch (stp) {
        case "proclist":


          query = @"
SELECT ROUTINE_NAME AS [ProcedureName]
     , case when charindex('Description', ROUTINE_DEFINITION) > 0
             and charindex('Author', ROUTINE_DEFINITION) > 0
             and charindex('Author', ROUTINE_DEFINITION) - ( charindex('Description', ROUTINE_DEFINITION)+12 ) >= 0
            then trim(replace(replace(replace(replace(replace(substring(
		       ROUTINE_DEFINITION, 
		       charindex('Description', ROUTINE_DEFINITION)+12
		       , charindex('Author', ROUTINE_DEFINITION) - ( charindex('Description', ROUTINE_DEFINITION)+12 )
		       ), 
		       '//'+char(13), ''), char(9), ''), '*', ''), '  ', ''), char(10), ''))
            else ''
            end as description
     , ROUTINE_DEFINITION
  FROM INFORMATION_SCHEMA.ROUTINES 
 WHERE ROUTINE_TYPE = 'PROCEDURE'
";


          break;
        case "procInfo":
          query = @"
SELECT ROUTINE_NAME AS [ProcedureName] 
     , ROUTINE_DEFINITION
FROM INFORMATION_SCHEMA.ROUTINES 
WHERE ROUTINE_TYPE = 'PROCEDURE' 
and ROUTINE_NAME = '"+ sva + @"'
";
          break;



          break;
        case "tablelist":
          query = @"
with x as (
 select * from information_schema.tables a
  -- wehre a.table_type = 'BASE TABLE'
 ) select table_name as tableName
        , ( SELECT value FROM ::fn_listextendedproperty (NULL, 'schema', table_schema, 'table', table_name, default, default) ) as Description
     from x
    where table_type = 'BASE TABLE'
      and table_name like '%" + sva + @"%'
    order by table_name
";
          break;


          break;
        case "columnsOftable":
          query = @"
select a.name, b.name as ColunmName, c.value as Description
  from sysobjects a
    inner join sys.columns b
   on a.id = b.object_id
       left join  sys.extended_properties  c  
   on c.major_id = b.object_id and c.minor_id = b.column_id 
 where a.xtype = 'U' --and c.value is not null
 AND A.NAME = '" + sva + @"'
 order by 1,2
";
          break;







      }

     var aaa = HanDevExecuteQuery(query);

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
