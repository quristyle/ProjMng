using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using ProjMngWasm.Commons;
using WasmShear.Services;
using ProjModel;
using Radzen;
using WasmShear;
using WasmShear.Commons;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ProjMngWasm;

public class BaseComponent : CommonComponent {

  [Inject] protected NotificationService? NotificationService { get; set; }
  [Inject] protected AppProjData? appProjData { get; set; }
  [Inject] protected DialogService DialogService { get; set; }
  [Inject] protected ContextMenuService ContextMenuService { get; set; }


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

    var data = await jsiniService.GetList<T>(proc_name, dic, proc_type, isFast);

    if(data == null) {
      Notify(NotificationSeverity.Warning, "Warning Message", $"{proc_name} Data is null", 50000, true);
    }
    else if (data.Code < 0) {
      Notify(NotificationSeverity.Error, "Error Message", data.Message, 50000, true);
    }

    return data;
  }


  public async Task<ResultInfo<T>> SysCont<T>(string proc_name, Dictionary<string, string> dic, string proc_type = "srch", bool isFast = false) {

    var data = await sysService.GetList<T>(proc_name, dic, proc_type, isFast);

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

    var data = await jsiniService.GetList<T>(md_name, dic, proc_type, isFast);

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
    var data = await DbCont<T>( proc_name, dic, "save", isFast);
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

    var data = await devService.GetList<T>(action_name, dic);

    if (data.Code < 0) {
      Notify(NotificationSeverity.Error, "Error Message", data.Message, 10000, true);
    }
   
    return data;
  }
  protected async Task<ResultInfo<T>> JsContQuery<T>(CommonCode db, string query) {

    var data = await devService.GetListQuery<T>(db.Others["db_nick"], query);

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







  protected void Notify<T>(ResultInfo<T> data ) {

    NotificationSeverity ns = NotificationSeverity.Success;
    int waitTime = 2;
    if (data.Code < 0) {
      waitTime = 50;
      ns = NotificationSeverity.Error;
    }
    Notify(ns, "Message", data.Message, waitTime * 1000);
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








  protected async Task<List<CommonCode>> GetCommon(string _codeId, string _key) {

    var data = await jsiniService.GetList<Dictionary<string, string>>("sp_projCommon", new Dictionary<string, string>() {
                { "code_id", _codeId },
                { "etc0", _key }
            });


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
    return (value?.ToString() == svalue) ;
  }







}