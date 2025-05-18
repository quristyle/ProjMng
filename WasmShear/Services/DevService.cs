using ProjModel;
using System.Net.Http.Json;
//using static Microsoft.CodeAnalysis.CSharp.SyntaxTokenParser;

namespace WasmShear.Services;

public class DevService : BaseService {

  public DevService(IHttpClientFactory httpClientFactory) : base(httpClientFactory.CreateClient("jsini")) { }

  public async Task<ResultInfo<T>> GetList<T>(string action_name, IDictionary<string, string> dic, string proc_type = "srch") {

    return await GetData<T>(action_name, dic, TargetDevUrl);

  }



  

  public async Task<ResultInfo<T>> GetListQuery<T>(string db_nick, string query) {

    ResultInfo<T> result = null;
    var response = await _httpClient.PostAsJsonAsync("api/Dev/sql", new Dictionary<string, string>() { { "query", query }, { "db_nick", db_nick } }
    , System.Text.Json.JsonSerializerOptions.Default);

    //요청 성공 여부 확인
    response.EnsureSuccessStatusCode();

    if (response.StatusCode == System.Net.HttpStatusCode.OK) {
      string responseString = await response.Content.ReadAsStringAsync();

      result = GetRi<T>(responseString);

    }
    else {
      SetResultCode<T>(result, -96, $"응답 실패, 응답코드 : {response.StatusCode}");
    }


    return result;

  }


}

