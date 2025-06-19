using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using ProjModel;
using System.Net.Http.Json;

namespace WasmShear.Services;

public class BaseService  {

  protected string AuthToken { get; set; } = "ums_token";
  protected string AuthTokenKey { get; set; } = "Bearer";
  protected string DataPath { get; set; } = "data";
  protected string ColumPath { get; set; } = "cols";
  protected string RecPath { get; set; } = "rec";
  protected string FirstDataRow { get; set; } = "data[0]";

  protected readonly HttpClient _httpClient;

  protected const string TargetUrl = "api/Proj";
  protected const string TargetDevUrl = "api/Dev";
  protected const string TargetUrlFast = "api/Proj/fast";
  protected const string TargetUrlMedia = "api/Media";

  public BaseService(HttpClient httpClient) {
    _httpClient = httpClient;
  }

  /// <summary>
  /// 응답 string ResultInfo 객체로 변환
  /// </summary>
  /// <typeparam name="T"></typeparam>
  /// <param name="responseString"></param>
  /// <returns></returns>
  protected ResultInfo<T> GetRi<T>(string responseString) {

    ResultInfo<T> result = null;
    if (!string.IsNullOrWhiteSpace(responseString)) {

      //JObject jobj = JObject.Parse(responseString);
      //result = jobj.ToObject<ResultInfo<T>>();


      JObject jobj = JObject.Parse(responseString);
      var settings = new JsonSerializerSettings {
        ContractResolver = new DefaultContractResolver {
          NamingStrategy = new DefaultNamingStrategy() // 대소문자 구분 없이 매핑
        }
      };
      result = jobj.ToObject<ResultInfo<T>>(JsonSerializer.Create(settings));




      /*

      JToken jtcol = jobj.SelectToken(ColumPath);
      JToken jtrec = jobj.SelectToken(RecPath);
      JToken jt = jobj.SelectToken(DataPath);

      if (jtrec != null) {
        var rec = jtrec.ToObject<Dictionary<string, object>>();
        result.Res = rec;
      }
      if (jtcol != null) {
        var col = jtcol.ToObject<Dictionary<string, string>>();
        result.Cols = col;
      }
      if (jt != null) {
        
        var dt = jt.ToObject<List<T>>();

        try {
          result.Code = int.Parse(jobj["code"].ToString());
          result.Message = jobj["msg"].ToString();

        }
        catch (Exception ee) {
          result.Message = ee.Message; 
        }

        result.Data = dt;
      }
      else {

        result.Code = -98;
        result.Message = $"dataPath({DataPath}) 에서 해당하는 데이터가 없습니다.";
        Console.WriteLine($"dataPath({DataPath}) 에서 해당하는 데이터가 없습니다.");
      }


*/

    }
    else {
      SetResultCode<T>(result, -97, $"응답 실패, 응답이 비어 있습니다");
    }
    return result;
  }

  protected void SetResultCode<T>(ResultInfo<T> ri, int code, string message) {
    if (ri == null) {
      ri = new ResultInfo<T>();
    }
    ri.Code = code;
    ri.Message = message;
  }


  public enum HttpCallType {
    Get, Post, Put, Delete, DeleteAll, PostJson, PutJson, DeleteJson
  }


  /// <summary>
  /// rest api 호출후 데이터 받아옴.
  /// </summary>
  /// <typeparam name="T"></typeparam>
  /// <param name="dic"></param>
  /// <param name="targetUrl"></param>
  /// <returns></returns>
  protected async Task<ResultInfo<T>> GetData<T>(string stpStr,   IDictionary<string, string> dic, string targetUrl = TargetUrl, HttpCallType hctype = HttpCallType.PostJson) {

    ResultInfo<T> result = null;
    dic["stp"] = stpStr;

    try {
      HttpResponseMessage response = null;

      switch (hctype) {
        case HttpCallType.Get:
          response = await _httpClient.GetAsync(targetUrl);
          break;
        default:
          response = await _httpClient.PostAsJsonAsync(targetUrl, dic, System.Text.Json.JsonSerializerOptions.Default);
          break;
      }

      if (response == null) {
        SetResultCode<T>(result, -1001, $"호출타입정의 확인 필요 : {hctype.ToString()}");
      }
      
      //요청 성공 여부 확인
      response.EnsureSuccessStatusCode();

      if (response.StatusCode == System.Net.HttpStatusCode.OK) {
        string responseString = await response.Content.ReadAsStringAsync();

        result = GetRi<T>(responseString);

      }
      else {
        SetResultCode<T>(result, -96, $"응답 실패, 응답코드 : {response.StatusCode}");
      }
    }
    catch (HttpRequestException ex) {
      SetResultCode<T>(result, -95, $"HTTP 요청 실패: {ex.Message}");
    }
    catch (JsonException ex) {
      SetResultCode<T>(result, -94, $"JSON 파싱 실패: {ex.Message}");
    }
    catch (Exception ex) {
      SetResultCode<T>(result, -93, $"예외 발생: {ex.Message}");
    }
    return result;
  }



  /*
  protected void ChangeDataType(Dictionary<string, string> cols, object obj) {
    List<Dictionary<string, object?>> data = (List<Dictionary<string, object?>>)obj;

    // 변환할 컬럼과 타입을 미리 캐싱
    var intColumns = cols.Where(c => c.Value == "System.Int32").Select(c => c.Key).ToList();

    // 병렬 처리를 사용하여 데이터 변환
    Parallel.ForEach(data, d => {
      foreach (var ckey in intColumns) {
        if (d.TryGetValue(ckey, out var value) && value != null && int.TryParse(value.ToString(), out int result)) {
          d[ckey] = result;
        }
        else {
          d[ckey] = 0; // 기본값을 설정하거나 다른 처리를 할 수 있습니다.
        }
      }
    });
  }
  */


}



