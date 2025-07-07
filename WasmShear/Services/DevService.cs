using ProjModel;
using System.Net.Http.Json;
//using static Microsoft.CodeAnalysis.CSharp.SyntaxTokenParser;

namespace WasmShear.Services;

public class DevService : BaseService {

  public DevService(IHttpClientFactory httpClientFactory, AppData appData) 
    : base(httpClientFactory.CreateClient("jsini"), appData) { }

  public async Task<ResultInfo<T>> GetList<T>(RequestDto rd) {

    return await GetData<T>(rd, TargetDevUrl);

  }



  

  public async Task<ResultInfo<T>> GetListQuery<T>(string db_nick, string query) {


    CheckUserServerUrlChanged();



    ResultInfo<T> result = null;






    HttpResponseMessage response = null;
    Dictionary<string, string> dic = new Dictionary<string, string>() { { "query", query }, { "db_nick", db_nick } };


    if (AbsoluteUrl == null ) {
      response = await _httpClient.PostAsJsonAsync("api/Dev/sql", dic, System.Text.Json.JsonSerializerOptions.Default);
    }
    else {
      response = await _httpClient.PostAsJsonAsync(new Uri(AbsoluteUrl, "api/Dev/sql"), dic, System.Text.Json.JsonSerializerOptions.Default);
    }





    //var response = await _httpClient.PostAsJsonAsync("api/Dev/sql", new Dictionary<string, string>() { { "query", query }, { "db_nick", db_nick } }
    //, System.Text.Json.JsonSerializerOptions.Default);

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

