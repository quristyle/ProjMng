using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System.Net.Http.Json;
using System.Text.Json;

namespace ProjMngWasm.Services;



public class JsiniService : IJsiniService {


protected string AuthToken {get;set;} = "ums_token";
protected string AuthTokenKey {get;set;} = "Bearer";
  protected string DataPath { get; set; } = "data";
  protected string ColumPath { get; set; } = "cols";

  protected readonly HttpClient _httpClient;
  protected readonly ISessionStorageService _sess;

  public JsiniService(HttpClient httpClient , ISessionStorageService sess ) {
	_httpClient = httpClient;
	_sess = sess;
}


  public async Task<ResultInfo<T>> GetList<T>(Dictionary<string, string> dic) {

    ResultInfo<T> result = new ResultInfo<T>();

    try {
      HttpResponseMessage response = await _httpClient.PostAsJsonAsync("api/Proj", dic, JsonSerializerOptions.Default);
      //요청 성공 여부 확인
      response.EnsureSuccessStatusCode();

      if (response.StatusCode == System.Net.HttpStatusCode.OK) {
        string responseString = await response.Content.ReadAsStringAsync();

        // 응답 문자열 확인
        if (!string.IsNullOrWhiteSpace(responseString)) {
          JObject jobj = JObject.Parse(responseString);
          JToken jtcol = jobj.SelectToken(ColumPath);
          JToken jt = jobj.SelectToken(DataPath);

          if (jt != null) {
            //return 

            var col = jtcol.ToObject<Dictionary<string,string>>();

            var dt = jt.ToObject<List<T>>();

            if ( typeof(T) == typeof(Dictionary<string, object>)) {
              ChangeDataType(col, dt);
            }


              result.Cols = col;
            result.Data = dt;
          }
          else {
            Console.WriteLine($"dataPath({DataPath}) 에서 해당하는 데이터가 없습니다.");
          }
        }
        else {
          Console.WriteLine("응답이 비어 있습니다.");
        }
      }
      else {
        Console.WriteLine($"응답 실패, 응답코드 : {response.StatusCode}");
      }
    }
    catch (HttpRequestException ex) {
      Console.WriteLine($"HTTP 요청 실패: {ex.Message}");
    }
    catch (JsonException ex) {
      Console.WriteLine($"JSON 파싱 실패: {ex.Message}");
    }
    catch (Exception ex) {
      Console.WriteLine($"예외 발생: {ex.Message}");
    }
    return result;// Enumerable.Empty<T>();
  }




  protected void ChangeDataType(Dictionary<string, string> cols, object obj) {

    List<Dictionary<string, object>> data = (List<Dictionary<string, object>>)obj;

    foreach (var c in cols) {
      if (c.Value == "System.Int32") {

        string ckey = c.Key;

        foreach (var d in data) {

          d[ckey] = int.Parse(d[ckey]?.ToString());

        }



      }
    }

  }


}





public interface IJsiniService {
  Task<ResultInfo<T>> GetList<T>(Dictionary<string, string> dic);  
}


public class ResultInfo<T> {
  public IDictionary<string,string> Cols { get; set; }
  public IEnumerable<T> Data { get; set; }
}