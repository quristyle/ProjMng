
using ProjModel;

namespace WasmShear;
public class WasmUtil {

  /// <summary>
  /// 구분된 값으로 구분해서 마지막 문자값 가져오기
  /// </summary>
  /// <param name="input"></param>
  /// <returns></returns>
  public static string ExtractLastSegment(string input, char c = '.') {
    int lastIndex = input.LastIndexOf(c);
    if (lastIndex >= 0 && lastIndex < input.Length - 1) {
      return input.Substring(lastIndex + 1);
    }
    return input;
  }

  /// <summary>
  /// dic 비교
  /// </summary>
  /// <param name="dic1"></param>
  /// <param name="dic2"></param>
  /// <returns></returns>
  public static bool AreDictionariesEqual(Dictionary<string, string> dic1, Dictionary<string, string> dic2) {
    if (dic1 == null && dic2 == null) return true;
    if (dic1 == null || dic2 == null) return false;
    if (dic1.Count != dic2.Count) return false;

    foreach (var kvp in dic1) {
      if (!dic2.TryGetValue(kvp.Key, out var value) || kvp.Value != value) {
        return false;
      }
    }

    return true;
  }


  // 복사하기
  public static List<CommonCode> DeepCopy(List<CommonCode> original) {
    return original.Select(item => new CommonCode {
      Code = item.Code,
      Name = item.Name,
      Desc = item.Desc,
      Others = new Dictionary<string, string>(item.Others)
    }).ToList();
  }


  /// <summary>
  /// dic 결합
  /// </summary>
  /// <param name="dict1"></param>
  /// <param name="dict2"></param>
  /// <returns></returns>
  public static Dictionary<string, string> JoinDictionaries(IDictionary<string, object> dict1, Dictionary<string, string> dict2) {
    
    var d1 = JoinConvert(dict1);

    var result = JoinDictionaries(d1, dict2);

    return result;
  }
  public static Dictionary<string, string> JoinDictionaries(Dictionary<string, string> dict1, IDictionary<string, object> dict2) {

    var d1 = JoinConvert(dict2);

    var result = JoinDictionaries(dict1, d1);

    return result;
  }

  /// <summary>
  /// dic 변환 obj to string
  /// </summary>
  /// <param name="dict1"></param>
  /// <returns></returns>
  public static Dictionary<string, string> JoinConvert(IDictionary<string, object> dict1) {
    var result = new Dictionary<string, string>();

    foreach (var c in dict1) {
      if (c.Value == null) continue;
        if (c.Value.GetType() == typeof(DateTime)) {

          result[c.Key] = ((DateTime)dict1[c.Key]).ToString("yyyyMMdd");

        }
        else {
          result[c.Key] = c.Value.ToString();
        }

    }

    return result;
  }


  public static Dictionary<string, string> JoinDictionaries(IDictionary<string, string> dict1, Dictionary<string, string> dict2) {
    var result = new Dictionary<string, string>();

    var allKeys = dict1.Keys.Union(dict2.Keys);

    foreach (var key in allKeys) {
      if (dict1.ContainsKey(key)) {
        result[key] = dict1[key] + "";
      }
      else if (dict2.ContainsKey(key)) {
        result[key] = dict2[key];
      }
    }

    return result;
  }


  public static Type GetType (string clsStr) {
    var nullableTypes = new Dictionary<string, string> {
        { "System.DateTime", "System.Nullable`1[System.DateTime]" },
        { "System.Int32", "System.Nullable`1[System.Int32]" },
        { "System.Boolean", "System.Nullable`1[System.Boolean]" },
        { "System.Byte", "System.Nullable`1[System.Byte]" },
        { "System.Char", "System.Nullable`1[System.Char]" },
        { "System.Decimal", "System.Nullable`1[System.Decimal]" },
        { "System.Double", "System.Nullable`1[System.Double]" },
        { "System.Single", "System.Nullable`1[System.Single]" }, // float
        { "System.Int64", "System.Nullable`1[System.Int64]" }, // long
        { "System.SByte", "System.Nullable`1[System.SByte]" },
        { "System.Int16", "System.Nullable`1[System.Int16]" }, // short
        { "System.UInt32", "System.Nullable`1[System.UInt32]" },
        { "System.UInt64", "System.Nullable`1[System.UInt64]" },
        { "System.UInt16", "System.Nullable`1[System.UInt16]" }
    };

    if (nullableTypes.ContainsKey(clsStr)) {
      return Type.GetType(nullableTypes[clsStr]);

      //return Type.GetType(nullableTypes[clsStr] + ", System.Private.CoreLib");
    }
    else if (clsStr == "System.String") {
      return typeof(string);
    }
    else {
      return Type.GetType(clsStr);
    }
  }






  public static string PostgresqlProcMakeBase =
@"
/* description : 해당 테이블 또는 프로그램명 등..
 * writer      : user
 * date        : quri_date_quri
 * update      : user
 * update date : quri_date_quri
 * update desc : 변경 사항
 * version     : 1.0
 * version desc : 최초 작성 .. db type 의 기본 코드 입니다. 프로젝트에 맞게 등록해서 사용을 추천 합니다.
 */
CREATE OR REPLACE PROCEDURE quri_schema_quri.sp_quri_TableName_quri_exec(
  IN p_srch character varying
quri_in_cols_quri
  , IN p_req_type character varying
  , INOUT p_cur refcursor
)
 LANGUAGE plpgsql
AS $procedure$

declare

	BEGIN

    if p_req_type = 'save' then

      if nvl(p_quri_keycol_quri, '') = '' then

	      insert into quri_schema_quri.quri_TableName_quri
	      ( quri_cols_quri )
	      values
	      ( quri_p_cols_quri )
	      ;

      else

	      update quri_schema_quri.quri_TableName_quri
	         set quri_up_cols_quri
	       where quri_keycol_quri = p_quri_keycol_quri
	      ;

      end if;

    end if;

      open p_cur for

      select quri_cols_comment_quri
        from quri_schema_quri.quri_TableName_quri a
       where 1=1
         and ( ( nvl(p_quri_keycol_quri, '') = '' and 1=1  )
             or  ( nvl(p_quri_keycol_quri, '') != '' and a.quri_keycol_quri = p_quri_keycol_quri )
             )

      ;


	END;

$procedure$
;


";


  public static string MssqlProcMakeBase =
@"
/* description : 해당 테이블 또는 프로그램명 등..
 * writer      : user
 * date        : quri_date_quri
 * update      : user
 * update date : quri_date_quri
 * update desc : 변경 사항
 * version     : 1.0
 * version desc : 최초 작성 .. db type 의 기본 코드 입니다. 프로젝트에 맞게 등록해서 사용을 추천 합니다.
 */

CREATE PROCEDURE quri_schema_quri.sp_quri_TableName_quri_exec
(
	@p_srch		nvarchar(30)
quri_in_cols_quri
	, @p_req_type	nvarchar(30)
)
AS

BEGIN
    -- 선언부
	DECLARE @xxxx nvarchar(MAX)

	--IF @p_req_type = 'save'
	--BEGIN
			-- insert, update
	--END
		
      select quri_cols_comment_quri
        from quri_schema_quri.quri_TableName_quri a
       where 1=1
         and ( ( ifnull(@p_quri_keycol_quri, '') = '' and 1=1  )
             or  ( ifnull(@p_quri_keycol_quri, '') != '' and a.quri_keycol_quri = @p_quri_keycol_quri )
             )

END

";


}

