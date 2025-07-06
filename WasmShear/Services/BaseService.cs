using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using ProjModel;
using System.Net.Http.Json;

namespace WasmShear.Services;

public class BaseService {

  protected string AuthToken { get; set; } = "ums_token";
  protected string AuthTokenKey { get; set; } = "Bearer";
  protected string DataPath { get; set; } = "data";
  protected string ColumPath { get; set; } = "cols";
  protected string RecPath { get; set; } = "rec";
  protected string FirstDataRow { get; set; } = "data[0]";

  protected HttpClient _httpClient;
  protected AppData _appData; // AppData 필드 추가

  public const string TargetBaseUrl = "api/Proj/sys";
  protected const string TargetUrl = "api/Proj";
  protected const string TargetDevUrl = "api/Dev";
  protected const string TargetUrlFast = "api/Proj/fast";
  protected const string TargetUrlMedia = "api/Media";


  string UserServerUrl { get; set; }
  Uri AbsoluteUrl { get; set; }

  public BaseService(HttpClient httpClient, AppData appData) {
    _httpClient = httpClient;
    _appData = appData;

    //if (string.IsNullOrEmpty(_appData.ActiveServerUrl)) {
    //  Console.WriteLine($" BaseService init 111111111111111111111 _appData.ActiveServerUrl : {UserServerUrl} : {_appData.ActiveServerUrl}");
    //  _appData.ActiveServerUrl = _httpClient.BaseAddress.Host;
    //  Console.WriteLine($" BaseService init 222222222222222222 _appData.ActiveServerUrl : {UserServerUrl} : {_appData.ActiveServerUrl}");
    //}

    //Console.WriteLine($" BaseService init _appData.UserServerUrl : {UserServerUrl} : {_appData.User?.UserServerUrl}");
    //if ( !string.IsNullOrWhiteSpace( _appData.User?.UserServerUrl )) {
    //  UserServerUrl = _appData.User?.UserServerUrl;
    //  Console.WriteLine($" BaseService init _appData.UserServerUrl in ............. : {_appData.User?.UserServerUrl}");
    //}

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

      JObject jobj = JObject.Parse(responseString);
      var settings = new JsonSerializerSettings {
        ContractResolver = new DefaultContractResolver {
          NamingStrategy = new DefaultNamingStrategy() // 대소문자 구분 없이 매핑
        }
      };
      result = jobj.ToObject<ResultInfo<T>>(JsonSerializer.Create(settings));

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

  private string? _previousUserServerUrl;

  private void CheckUserServerUrlChanged() {
    var currentUrl = _appData.User?.UserServerUrl;

    if (_previousUserServerUrl == currentUrl)
      return; // 변경되지 않았으면 무시

    _previousUserServerUrl = currentUrl;

    if (string.IsNullOrWhiteSpace(currentUrl)) {
      AbsoluteUrl = null;
      _appData.ActiveServerUrl = "default Server";
    }
    else {
      try {
        AbsoluteUrl = new Uri(currentUrl);
        _appData.ActiveServerUrl = AbsoluteUrl.Host;
      }
      catch {
        AbsoluteUrl = null;
        _appData.ActiveServerUrl = "default Server";
      }
    }

  }




  protected async Task<ResultInfo<T>> GetData<T>(RequestDto rd, string targetUrl = TargetUrl, HttpCallType hctype = HttpCallType.PostJson) {

    
    Console.WriteLine($" 시이작. UserServerUrl : {UserServerUrl},  ActiveServerUrl : {_appData.ActiveServerUrl},  _appData.User?.UserServerUrl : {_appData.User?.UserServerUrl} ");

    CheckUserServerUrlChanged();

    Console.WriteLine($" 끄으엇. UserServerUrl : {UserServerUrl},  ActiveServerUrl : {_appData.ActiveServerUrl},  _appData.User?.UserServerUrl : {_appData.User?.UserServerUrl} ");



    /*
    if ( !string.IsNullOrWhiteSpace(_appData.User?.UserServerUrl) && AbsoluteUrl == null) {
      // UserServerUrl = _appData.User?.UserServerUrl;
      // _httpClient .BaseAddress = new Uri(UserServerUrl);
      // _httpClient = new HttpClient { BaseAddress = new Uri(UserServerUrl) };

      AbsoluteUrl = new Uri(_appData.User?.UserServerUrl);
      _appData.ActiveServerUrl = AbsoluteUrl.Host;
      Console.WriteLine($"한번만 발생하자 11111111111111111 : {_appData.ActiveServerUrl} ...  ");
    }
    else if (string.IsNullOrWhiteSpace(_appData.User?.UserServerUrl) && AbsoluteUrl != null) {
      AbsoluteUrl = null;
      _appData.ActiveServerUrl = "defalut server";
      Console.WriteLine($"한번만 발생하자 222222222222222 : {_appData.ActiveServerUrl} ...  ");
    }
    else if (!string.IsNullOrWhiteSpace(_appData.User?.UserServerUrl)) {
      AbsoluteUrl = new Uri(_appData.User?.UserServerUrl);
      _appData.ActiveServerUrl = AbsoluteUrl.Host;
      Console.WriteLine($"한번만 발생하자 33333333333333333 : {_appData.ActiveServerUrl} ...  ");
    }
    else if (string.IsNullOrWhiteSpace(_appData.User?.UserServerUrl)) {
      AbsoluteUrl = null;
      _appData.ActiveServerUrl = "defalut server";
      Console.WriteLine($"한번만 발생하자 444444444444 : {_appData.ActiveServerUrl} ...  ");
    }
    */
    ResultInfo<T> result = null;

    try {


      HttpResponseMessage response = null;

      switch (hctype) {
        case HttpCallType.Get:
          response = await _httpClient.GetAsync(targetUrl);
          break;
        default:

          if (AbsoluteUrl == null || targetUrl == "api/Proj/login" || targetUrl == "api/Proj/sys") {
            response = await _httpClient.PostAsJsonAsync(targetUrl, rd, System.Text.Json.JsonSerializerOptions.Default);
          }
          else {
            response = await _httpClient.PostAsJsonAsync(new Uri(AbsoluteUrl, targetUrl), rd, System.Text.Json.JsonSerializerOptions.Default);
          }

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



}