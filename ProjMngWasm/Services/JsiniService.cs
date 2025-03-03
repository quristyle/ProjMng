using Newtonsoft.Json.Linq;
using System.Net.Http.Json;
using System.Text.Json;

namespace ProjMngWasm.Services;



public class JsiniService : IJsiniService {


protected string AuthToken {get;set;} = "ums_token";
protected string AuthTokenKey {get;set;} = "Bearer";
protected string DataPath {get;set;} = "result";
protected string FirstDataRow {get;set;} = "$..data[0]";

  protected readonly HttpClient _httpClient;
  protected readonly ISessionStorageService _sess;

  public JsiniService(HttpClient httpClient , ISessionStorageService sess ) {
	_httpClient = httpClient;
	_sess = sess;
}

  void SetDic(string path, ref Dictionary<string, string> dic) {

    dic.Add("stp", path);
    dic.Add("sta", "");
    dic.Add("sob", "");
  }

  public async Task<IEnumerable<T>> GetList<T>(string path, Dictionary<string, string> dic) {



    SetDic( path, ref dic);



    try {
      HttpResponseMessage response = await _httpClient.PostAsJsonAsync("api/Dev", dic, JsonSerializerOptions.Default);
      //요청 성공 여부 확인
      response.EnsureSuccessStatusCode();

      if (response.StatusCode == System.Net.HttpStatusCode.OK) {
        string responseString = await response.Content.ReadAsStringAsync();

        // 응답 문자열 확인
        if (!string.IsNullOrWhiteSpace(responseString)) {
          JObject jobj = JObject.Parse(responseString);
          JToken jt = jobj.SelectToken(DataPath);

          if (jt != null) {
            return jt.ToObject<List<T>>();
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
    return Enumerable.Empty<T>();
  }

  public async Task<T> Get<T>(string path, Dictionary<string, string> dic) {

    SetDic(path, ref dic);

    try {
      HttpResponseMessage response = await _httpClient.PostAsJsonAsync("api/Dev", dic, JsonSerializerOptions.Default);
      //요청 성공 여부 확인
      response.EnsureSuccessStatusCode();

      if (response.StatusCode == System.Net.HttpStatusCode.OK) {
        string responseString = await response.Content.ReadAsStringAsync();

        // 응답 문자열 확인
        if (!string.IsNullOrWhiteSpace(responseString)) {
          JObject jobj = JObject.Parse(responseString);
          JToken jt = jobj.SelectToken(DataPath);

          if (jt != null) {
            return jt.ToObject<List<T>>()[0];
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
    return default(T);
  }


}



public interface IJsiniService {
  Task<IEnumerable<T>> GetList<T>(string path, Dictionary<string, string> dic);
  Task<T> Get<T>(string path, Dictionary<string, string> dic);
}