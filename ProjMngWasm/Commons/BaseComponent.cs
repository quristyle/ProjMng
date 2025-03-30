using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using ProjMngWasm.Commons;
using ProjMngWasm.Services;
using ProjModel;
using Radzen;

namespace ProjMngWasm;

public class BaseComponent : ComponentBase {

  [Inject] private JsiniService? JsiniService { get; set; }
  [Inject] protected UMSService? UmsService { get; set; }
  [Inject] protected DevService? DevService { get; set; }
  [Inject] protected FuneralService? FuneralService { get; set; }
  [Inject] protected SysService? SysService { get; set; }
  [Inject]  protected IJSRuntime? JSRuntime { get; set; }
  [Inject] protected NavigationManager? NavigationManager { get; set; }
  [Inject] protected AppData? appData { get; set; }
  [Inject] protected NotificationService? NotificationService { get; set; }


  protected async Task<ResultInfo<T>> DbCont<T>(string proc_name, Dictionary<string, string> dic, bool isFast = false) {
    return await DbCont<T>(proc_name, dic, "srch", isFast);
  }
  private async Task<ResultInfo<T>> DbCont<T>(string proc_name, Dictionary<string, string> dic, string proc_type="srch",  bool isFast = false) {
    if (string.IsNullOrWhiteSpace(proc_name) || !proc_name.StartsWith("sp_") || proc_name.Length < 6) {
      Notify(NotificationSeverity.Warning, "Error Message", "규칙 위반", 5000);
      return new ResultInfo<T> {
        Code = -1,
        Message = "Invalid procedure name"
      };
    }

    var data = await JsiniService.GetList<T>(proc_name, dic, proc_type, isFast);

    if (data.Code < 0) {
      Notify(NotificationSeverity.Error, "Error Message", data.Message, 50000, true);
    }

    return data;
  }


  public async Task<ResultInfo<T>> SysCont<T>(string proc_name, Dictionary<string, string> dic, string proc_type = "srch", bool isFast = false) {

    var data = await SysService.GetList<T>(proc_name, dic, proc_type, isFast);

    if (data.Code < 0) {
      Notify(NotificationSeverity.Error, "Error Message", data.Message, 50000, true);
    }

    return data;
  }


  public async Task<ResultInfo<T>> MdCont<T>(string md_name, Dictionary<string, string> dic, string proc_type = "srch", bool isFast = false) {
    if (string.IsNullOrWhiteSpace(md_name) || !md_name.StartsWith("md_") || md_name.Length < 6) {
      Notify(NotificationSeverity.Warning, "Error Message", "규칙 위반", 5000);
      return null;
    }

    var data = await JsiniService.GetList<T>(md_name, dic, proc_type, isFast);

    if (data.Code < 0) {
      Notify(NotificationSeverity.Error, "Error Message", data.Message, 50000, true);
    }

    return data;
  }

  protected async Task<ResultInfo<T>> DbSave<T>(string proc_name, IDictionary<string, object> dic, bool isFast = false) {
    //var req = WasmUtil.JoinDictionaries(dic, new Dictionary<string, string>() {  });

    var req = WasmUtil.JoinConvert(dic);

    return await DbSave<T>( proc_name, req, isFast);
  }
  protected async Task<ResultInfo<T>> DbSave<T>(string proc_name, Dictionary<string, string> dic, bool isFast = false) {    
    return await DbCont<T>( proc_name, dic, "save", isFast);
  }
  protected async Task<ResultInfo<T>> DbDelete<T>(string proc_name, Dictionary<string, string> dic, bool isFast = false) {
    return await DbCont<T>(proc_name, dic, "delete", isFast);
  }

  protected async Task<ResultInfo<T>> JsCont<T>(string action_name, Dictionary<string, string> dic) {

    var data = await DevService.GetList<T>(action_name, dic);

    if (data.Code < 0) {
      Notify(NotificationSeverity.Error, "Error Message", data.Message, 50000, true);
    }
   
    return data;
  }
  protected async Task<ResultInfo<T>> JsContQuery<T>(CommonCode db, string query) {

    var data = await DevService.GetListQuery<T>(db.Others["db_nick"], query);

    if (data.Code < 0) {
      Notify(NotificationSeverity.Error, "Error Message", $"다이렉트 쿼리 준비:{data.Message}", 50000, true);
    }

    return data;
  }




  protected object GetDicValue(IDictionary<string, object> args, string key) {
    var value = args.FirstOrDefault(kvp => kvp.Key.Equals(key, StringComparison.OrdinalIgnoreCase)).Value;
    return value;
  }

  protected string GetDicValueStr(IDictionary<string, object> args, string key) {
    var value = GetDicValue(args, key)?.ToString();
    return value;
  }



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

}