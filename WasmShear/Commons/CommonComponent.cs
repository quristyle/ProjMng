﻿
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ProjModel;
using WasmShear;
using WasmShear.Commons;
using WasmShear.Services;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WasmShear.Commons;

public class CommonComponent : ComponentBase {

  [Inject] protected JsiniService? jsiniService { get; set; }
  [Inject] protected UMSService? umsService { get; set; }
  [Inject] protected DevService? devService { get; set; }
  [Inject] protected FuneralService? funeralService { get; set; }
  [Inject] protected SysService? sysService { get; set; }
  [Inject] protected IJSRuntime? jsRuntime { get; set; }
  [Inject] protected NavigationManager? navigationManager { get; set; }
  [Inject] protected AppData? appData { get; set; }


  public async Task<bool> AuthCheck() {


    // 인증체크
    if (appData.User == null) {


      try {
        string json = await jsRuntime.InvokeAsync<string>("localStorage.getItem", "userInfo");
        if (!string.IsNullOrEmpty(json)) {
          Member user = JsonConvert.DeserializeObject<Member>(json);
          appData.User = user;
        }
      }
      catch (Exception eee) {
      }

      return (appData.User != null);
    }
    else {
      return true;
      // 인증된 사용자
      // appData.User = user;
      // appData.IsLogin = true;
    }


  }

  protected async Task<ResultInfo<T>> DbCont<T>(string proc_name, Dictionary<string, string> dic, bool isFast = false, bool isServerFix = false) {
    return await DbCont<T>(proc_name, dic, "srch", isFast, isServerFix);
  }
  private async Task<ResultInfo<T>> DbCont<T>(string proc_name, Dictionary<string, string> dic, string proc_type="srch",  bool isFast = false, bool isServerFix = false) {
    if (string.IsNullOrWhiteSpace(proc_name) || !proc_name.StartsWith("sp_") || proc_name.Length < 6) {
      //Notify(NotificationSeverity.Warning, "Error Message", "규칙 위반", 5000);
      return new ResultInfo<T> {
        Code = -1,
        Message = "Invalid procedure name"
      };
    }

    RequestDto rd = new RequestDto() { 
    ProcName = proc_name
    , ProcType = proc_type
    , IsFast = isFast
    , MainParam = dic
    };

    //var data = await jsiniService.GetList<T>(rd);


    ResultInfo<T> data = null;
    if (isServerFix) {
      data = await jsiniService.GetList<T>(rd, "api/Proj/sys");
    }
    else {
      data = await jsiniService.GetList<T>(rd);
    }




    if (data.Code < 0) {
      //Notify(NotificationSeverity.Error, "Error Message", data.Message, 50000, true);
    }

    return data;
  }


  public async Task<ResultInfo<T>> SysCont<T>(string proc_name, Dictionary<string, string> dic, string proc_type = "srch", bool isFast = false) {

    var data = await sysService.GetList<T>(proc_name, dic, proc_type, isFast);

    if (data.Code < 0) {
      //Notify(NotificationSeverity.Error, "Error Message", data.Message, 50000, true);
    }

    return data;
  }


  public async Task<ResultInfo<T>> MdCont<T>(string md_name, Dictionary<string, string> dic, string proc_type = "srch", bool isFast = false) {
    if (string.IsNullOrWhiteSpace(md_name) || !md_name.StartsWith("md_") || md_name.Length < 6) {
      //Notify(NotificationSeverity.Warning, "Error Message", "규칙 위반", 5000);
      return null;
    }

    RequestDto rd = new RequestDto() {
      ProcName = md_name
    ,      ProcType = proc_type
    ,      IsFast = isFast
    ,      MainParam = dic
    };


    var data = await jsiniService.GetList<T>(rd);

    if (data.Code < 0) {
      //Notify(NotificationSeverity.Error, "Error Message", data.Message, 50000, true);
    }

    return data;
  }

  protected async Task<ResultInfo<T>> DbSave<T>(string proc_name, IDictionary<string, object> dic, bool isFast = false, bool isServerFix = false) {
    //var req = WasmUtil.JoinDictionaries(dic, new Dictionary<string, string>() {  });

    var req = WasmUtil.JoinConvert(dic);

    return await DbSave<T>( proc_name, req, isFast, isServerFix);
  }
  protected async Task<ResultInfo<T>> DbSave<T>(string proc_name, Dictionary<string, string> dic, bool isFast = false, bool isServerFix = false) {
    var data = await DbCont<T>( proc_name, dic, "save", isFast, isServerFix);
    Notify(data);
    return data;
  }


  protected async Task<ResultInfo<T>> DbDelete<T>(string proc_name, IDictionary<string, object> dic, bool isFast = false) {
    var req = WasmUtil.JoinConvert(dic);
    return await DbDelete<T>(proc_name, req, isFast);
  }


  protected async Task<ResultInfo<T>> DbDelete<T>(string proc_name, Dictionary<string, string> dic, bool isFast = false) {

    return await DbCont<T>(proc_name, dic, "delete", isFast);
  }


  protected async Task<ResultInfo<T>> JsCont<T>(string action_name, Dictionary<string, string> dic  ) {

    if (dic == null) dic = new Dictionary<string, string>() { };


    RequestDto rd = new RequestDto() {
      ProcName = action_name
    ,      MainParam = dic
    };


    var data = await devService.GetList<T>(rd);

    if (data.Code < 0) {
      //Notify(NotificationSeverity.Error, "Error Message", data.Message, 10000, true);
    }
   
    return data;
  }
  //protected async Task<ResultInfo<T>> JsContQuery<T>(CommonCode db, string query) {

  //  var data = await devService.GetListQuery<T>(db.Others["db_nick"], query);

  //  if (data.Code < 0) {
  //    Notify(NotificationSeverity.Error, "Error Message", $"다이렉트 쿼리 준비:{data.Message}", 50000, true);
  //  }

  //  return data;
  //}


  public async Task Login(string id, string pw) {


    var user =  await jsiniService.Login(id, pw);
    if (user != null) {
      appData.User = user;
      appData.IsLogin = true;
      appData.ActiveServerUrl = null;
      string json = JsonConvert.SerializeObject(user);

      await jsRuntime.InvokeVoidAsync("localStorage.setItem", "userInfo", json);
      navigationManager.NavigateTo("/");
    }
    else {
      //Notify(NotificationSeverity.Error, "Error Message", "로그인 실패", 50000, true);
    }
  }


  protected async Task LoadInfo() {
    try {
      string json = await jsRuntime.InvokeAsync<string>("localStorage.getItem", "userInfo");
      if (!string.IsNullOrEmpty(json)) {
        Member user = JsonConvert.DeserializeObject<Member>(json);
        appData.User = user;
      }
    }
    catch (Exception eee) {
      Console.WriteLine($"Error loading user info: {eee.Message}");
    }
  }

  protected async Task SaveUserInfo() {
    var user = appData.User;

    await DbSave<Dictionary<string, object>>("sp_dev_user_exec_all", new Dictionary<string, string>() {
      { "prop_type", "LASTPAGE" },
      { "user_id", user.UserId },
      { "page", user.Last_page },
      { "page_nm", user.Last_page_nm },
      { "theme", user.Theme },
      { "page_yn", user.Last_page_yn.ToString() },
      { "sideauto_close", user.SideBarAutoClose.ToString() },
      { "serverurl", user.UserServerUrl },

    }, false, true);
    appData.ActiveServerUrl = user.UserServerUrl;

    string json = JsonConvert.SerializeObject(user);

    await jsRuntime.InvokeVoidAsync("localStorage.setItem", "userInfo", json);

  }


  protected async Task Logout() {
    appData.User = null;
    appData.IsLogin = false;

    await jsRuntime.InvokeVoidAsync("localStorage.removeItem", "userInfo");
    navigationManager.NavigateTo("/");
  }


  protected object GetDicValue(IDictionary<string, object> args, string key) {
    var value = args.FirstOrDefault(kvp => kvp.Key.Equals(key, StringComparison.OrdinalIgnoreCase)).Value;
    return value;
  }

  protected string GetDicValueStr(IDictionary<string, object> args, string key) {
    var value = GetDicValue(args, key)?.ToString();
    return value;
  }







  protected void Notify<T>(ResultInfo<T> data ) {
    /*
    NotificationSeverity ns = NotificationSeverity.Success;
    int waitTime = 2;
    if (data.Code < 0) {
      waitTime = 50;
      ns = NotificationSeverity.Error;
    }
    Notify(ns, "Message", data.Message, waitTime * 1000);
    */
  }

  /*
protected void Notify(NotificationSeverity severity, string summary, string detail, int duration, bool isProgress=false) {

 NotificationService.Notify(new NotificationMessage {
   Severity = severity,
   Summary = summary,
   Detail = detail,
   Duration = duration,
   ShowProgress = isProgress,
   Payload = DateTime.Now
 });
}


 */






  protected async Task<List<CommonCode>> GetCommon(string _codeId, string _key) {


    RequestDto rd = new RequestDto() {
      ProcName = "sp_projCommon"
    ,
      MainParam = new Dictionary<string, string>() {
                { "code_id", _codeId },
                { "etc0", _key }
            }
    };



    var data = await jsiniService.GetList<Dictionary<string, string>>(rd);


    List<CommonCode> dic = new List<CommonCode>();
    foreach (var d in data.Data) {

      dic.Add(

        new CommonCode() {
          Code = d["code"],
          Name = d["name"],
          Desc = d["desc"],
          Others = d,
        }

        );
    }

    return dic;


  }






  protected string DicValue(IDictionary<string, object> args, string key) {
    var value = args.FirstOrDefault(kvp => kvp.Key.Equals(key, StringComparison.OrdinalIgnoreCase)).Value;
    return value?.ToString() ?? string.Empty;
  }



  protected bool DicIsSameValue(IDictionary<string, object> args, string key, string svalue) {
    var value = args.FirstOrDefault(kvp => kvp.Key.Equals(key, StringComparison.OrdinalIgnoreCase)).Value;
    return (value?.ToString() == svalue);
  }










}