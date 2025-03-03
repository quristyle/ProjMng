using Newtonsoft.Json.Linq;
using System.Net.Http.Json;
using System.Text.Json;

namespace ProjMngWasm.Services;

public class UMSService : IUMSService {


protected string AuthToken {get;set;} = "ums_token";
protected string AuthTokenKey {get;set;} = "Bearer";
protected string DataPath {get;set;} = "DataSet.dt0.data";
protected string FirstDataRow {get;set;} = "$..data[0]";

  protected readonly HttpClient _httpClient;
  protected readonly ISessionStorageService _sess;

  public UMSService(HttpClient httpClient , ISessionStorageService sess ) {
	_httpClient = httpClient;
	_sess = sess;
}

  public async Task<IEnumerable<IDictionary<string, object>>> GetList(string path, Dictionary<string, object> dic) {
    Req req = ServiceUtil.CreateReq<Req>(path, dic);
    return await PostMethodList("api/QueryService/ExecuteRequestAsync", req);
}


private async Task SetAuthorizationHeader()  {
      _httpClient.DefaultRequestHeaders.Authorization = null; // 헤더 초기화
      string tk = await _sess.GetItemAsync<string>(AuthToken);
      if (!string.IsNullOrEmpty(tk))      {
          _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(AuthTokenKey, tk);
      }
}

private async Task<IEnumerable<IDictionary<string, object>>> PostMethodList<T>(string url, T content){
    try    {
        // 토큰 가져오기 및 헤더 설정
        await SetAuthorizationHeader();

        HttpResponseMessage response = await _httpClient.PostAsJsonAsync(url, content, JsonSerializerOptions.Default);
        
        // 401 Unauthorized 응답 처리 (토큰 만료)
        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)        {
            Console.WriteLine($"토큰이 만료되었습니다. 재발급을 시도합니다.");
            // 로그인 재요청
            await ReLogin();
            
            // 재요청 전 헤더 초기화 및 토큰 설정
            await SetAuthorizationHeader();
            // 재요청
            response = await _httpClient.PostAsJsonAsync(url, content, JsonSerializerOptions.Default);
        }

        //요청 성공 여부 확인
        response.EnsureSuccessStatusCode();


        if (response.StatusCode == System.Net.HttpStatusCode.OK)        {
            string responseString = await response.Content.ReadAsStringAsync();

            // 응답 문자열 확인
            if (!string.IsNullOrWhiteSpace(responseString))            {
                JObject jobj = JObject.Parse(responseString);
                JToken jt = jobj.SelectToken(DataPath);

                if (jt != null)                {
                    return jt.ToObject<List<IDictionary<string, object>>>();
                }
                else                {
                    Console.WriteLine($"dataPath({DataPath}) 에서 해당하는 데이터가 없습니다.");
                }
            }
            else            {
                Console.WriteLine("응답이 비어 있습니다.");
            }
        }else{
          Console.WriteLine($"응답 실패, 응답코드 : {response.StatusCode}");
        }
    }
    catch (HttpRequestException ex)    {
        Console.WriteLine($"HTTP 요청 실패: {ex.Message}");
    }
    catch (JsonException ex)    {
        Console.WriteLine($"JSON 파싱 실패: {ex.Message}");
    }
    catch (Exception ex)    {
        Console.WriteLine($"예외 발생: {ex.Message}");
    }
    return Enumerable.Empty<IDictionary<string, object>>();
}

private async Task ReLogin(){
    // 1. 로그인 요청에 필요한 파라미터들을 명확하게 정의합니다.
    Dictionary<string, object> loginParams = new Dictionary<string, object>()    {
        { "USER_ID", "super" },
        { "PASSWORD", "super" },
        { "PASSWORD_C", "super" },
        { "IS_HAN", "H" }
    };

    try    {
        // 2. LoginReq 타입으로 생성
        Req req = ServiceUtil.CreateReq<Req>("SP_USER_LOGIN_SELECT", loginParams);
        
        string token = await PostMethodStr("api/Account/Login", req, "TOKEN");

        if (!string.IsNullOrEmpty(token))        {
            await _sess.SetItemAsync(AuthToken, token);
        }
        else{
            Console.WriteLine($"로그인 실패: 토큰이 없습니다.");
        }
    }
    catch (Exception ex)    {
        // 4. 예외 처리
        Console.WriteLine($"로그인 실패: {ex.Message}");
        // 필요에 따라 사용자에게 오류 메시지를 표시하거나, 추가적인 로깅을 수행할 수 있습니다.
    }
}



  private async Task<string> PostMethodStr<T>(string url, T content, string key)  {
      string tokenStr = string.Empty;
      try      {
          // 요청 전 헤더 초기화 (토큰 제거)
          _httpClient.DefaultRequestHeaders.Authorization = null;

          // 토큰 가져오기
          string tk = await _sess.GetItemAsync<string>(AuthToken); 

          // 토큰이 존재하면 헤더에 추가
          if (!string.IsNullOrEmpty(tk))          {
              _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(AuthTokenKey, tk);
          }

          HttpResponseMessage response = await _httpClient.PostAsJsonAsync(url, content, JsonSerializerOptions.Default);

          // 요청 성공 확인
          response.EnsureSuccessStatusCode();

          // 상태 코드가 OK가 아니면, 오류 메시지를 출력
          if (response.StatusCode != System.Net.HttpStatusCode.OK)          {
              Console.WriteLine($"응답 실패, 응답코드 : {response.StatusCode}");
              return ""; 
          }

          string responseString = await response.Content.ReadAsStringAsync();

          if (!string.IsNullOrWhiteSpace(responseString))          {
            Newtonsoft.Json.Linq.JObject jobj = Newtonsoft.Json.Linq.JObject.Parse(responseString);

            var token = jobj.SelectToken(FirstDataRow+"."+key);

            tokenStr = token?.ToString() ?? string.Empty; 
          } else {
              Console.WriteLine($"응답이 비어 있습니다."); 
          }
      }
      catch (HttpRequestException ex)      {
          Console.WriteLine($"HTTP 요청 실패: {ex.Message}");
      }
      catch (JsonException ex)      {
          Console.WriteLine($"JSON 파싱 실패: {ex.Message}");
      }
      catch (Exception ex)      {
          Console.WriteLine($"예외 발생: {ex.Message}");
      }

      return tokenStr;
  }




}



public interface IUMSService {
  Task<IEnumerable<IDictionary<string, object>>> GetList(string path, Dictionary<string, object> dic);
}

public interface IReq{

}
  public class Req : IReq {
    public string QueryName { get; set; }
    public List<QueryParameter> QueryParameters { get; set; } = new List<QueryParameter>();
    public bool ReturnQueryParameter { get; set; }
  }

  public class QueryParameter {
    public string ParameterName { get; set; }
    public object ParameterValue { get; set; }
    public int ParameterDirection { get; set; } = 1;
    public string Prefix { get; set; } = "IN_";
  }