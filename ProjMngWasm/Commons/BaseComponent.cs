using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using ProjMngWasm.Commons;
using ProjModel;
using Radzen;
using System.Data;
using WasmShear;
using WasmShear.Commons;
using WasmShear.Services;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ProjMngWasm;

public class BaseComponent : CommonComponent {

  [Inject] protected NotificationService? NotificationService { get; set; }
  [Inject] protected AppProjData? appProjData { get; set; }
  [Inject] protected DialogService DialogService { get; set; }
  [Inject] protected ContextMenuService ContextMenuService { get; set; }



  protected Dictionary<string, string> GetProjDbParams(CommonCode dbType) {

    return new Dictionary<string, string>() {
  {"db",dbType?.Others["db_type"]??string.Empty},
  {"dbnick",dbType?.Others["db_nick"]??string.Empty},
  {"schema",dbType?.Others["db_schema"]??string.Empty},
  {"db_rid",dbType?.Code??string.Empty},

  };
  }


  protected async Task<ResultInfo<T>> DbCont<T>(string proc_name, Dictionary<string, string> dic, bool isFast = false, bool isServerFix = false) {
    return await DbCont<T>(proc_name, dic, "srch", isFast, isServerFix);
  }

  protected async Task<ResultInfo<T>> DbCont<T>(string proc_name, Dictionary<string, string> dic, string proc_type, bool isFast = false, bool isServerFix = false) {
    return await DbCont_h<T>(proc_name, dic, proc_type, isFast, isServerFix);
  }

  private async Task<ResultInfo<T>> DbCont_h<T>(string proc_name, Dictionary<string, string> dic, string proc_type="srch",  bool isFast = false, bool isServerFix = false) {
    if (string.IsNullOrWhiteSpace(proc_name) || !proc_name.StartsWith("sp_") || proc_name.Length < 6) {
      Notify(NotificationSeverity.Warning, "Error Message", "규칙 위반", 5000);
      return new ResultInfo<T> {
        Code = -1,
        Message = "Invalid procedure name"
      };
    }
    if ( dic == null ) { dic = new(); }
    RequestDto rd = new RequestDto() {
      ProcName = proc_name      ,
      ProcType = proc_type      ,
      IsFast = isFast      ,
      MainParam = dic      ,
      SSUserId = appData.User?.UserId ?? ""      ,
      Start = DateTime.Now
    };

    //var data = await jsiniService.GetList<T>(proc_name, dic, proc_type, isFast);

    ResultInfo<T> data = null;
    if (isServerFix) {
      data = await jsiniService.GetList<T>(rd, "api/Proj/sys");
    }
    else {
      data = await jsiniService.GetList<T>(rd);
    }
      //var data = await jsiniService.GetList<T>(rd);

    if (data == null) {
      Notify(NotificationSeverity.Warning, "Warning Message", $"{proc_name} Data is null", 50000, true);
    }
    else if (data.Code < 0) {
      Notify(NotificationSeverity.Error, "Error Message", data.Message, 50000, true);
    }

    return data;
  }


  private async Task<ResultInfo<T>> DbCont<T>(RequestDto rd) {
    if (string.IsNullOrWhiteSpace(rd.ProcName) || !rd.ProcName.StartsWith("sp_") || rd.ProcName.Length < 6) {
      Notify(NotificationSeverity.Warning, "Error Message", "규칙 위반", 5000);
      return new ResultInfo<T> {
        Code = -1,
        Message = "Invalid procedure name"
      };
    }

    rd.Start = DateTime.Now;

    //var data = await jsiniService.GetList<T>(proc_name, dic, proc_type, isFast);
    var data = await jsiniService.GetList<T>(rd);

    if (data == null) {
      Notify(NotificationSeverity.Warning, "Warning Message", $"{rd.ProcName} Data is null", 50000, true);
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

    RequestDto rd = new RequestDto() { ProcName= md_name , MainParam = dic, ProcType=proc_type, IsFast=isFast};

    var data = await jsiniService.GetList<T>(rd);

    if (data.Code < 0) {
      Notify(NotificationSeverity.Error, "Error Message", data.Message, 50000, true);
    }

    return data;
  }

  /*
  protected async Task<ResultInfo<T>> DbSave<T>(string proc_name, IDictionary<string, object> dic, bool isFast = false, bool isServerFix = false) {
    //var req = WasmUtil.JoinDictionaries(dic, new Dictionary<string, string>() {  });

    var req = WasmUtil.JoinConvert(dic);

    return await DbSave<T>(proc_name, req, isFast, isServerFix);
  }
  protected async Task<ResultInfo<T>> DbSave<T>(string proc_name, IDictionary<string, object> dic, ResultInfo<Dictionary<string, object>> data, bool isFast = false, bool isServerFix = false) {
    //var req = WasmUtil.JoinDictionaries(dic, new Dictionary<string, string>() {  });

    var req = WasmUtil.JoinConvert(dic);

    return await DbSave<T>(proc_name, req, isFast, isServerFix);
  }
  protected async Task<ResultInfo<T>> DbSave<T>(string proc_name, Dictionary<string, string> dic, bool isFast = false, bool isServerFix = false) {
    var data = await DbCont<T>( proc_name, dic, "save", isFast, isServerFix);
    Notify(data);
    return data;
  }
  */

  protected List<Dictionary<string, object>> GetChangeData(ResultInfo<Dictionary<string, object>> ri) {
    var data = ri.Data;
    List<Dictionary<string, object>> result = new();
    if (data != null) {
      foreach (var dic in data) {
        if (dic.TryGetValue("quri_ischange", out var isChangeObj)) {
          bool isChange = false;
          if (isChangeObj is bool b)
            isChange = b;
          else if (isChangeObj is string s)
            isChange = s.Equals("true", StringComparison.OrdinalIgnoreCase);

          if (isChange) {
            // 복사본 생성 후 quri_ischange 제거
            var newDic = new Dictionary<string, object>(dic);
            newDic.Remove("quri_ischange");
            result.Add(newDic);

            // 원본의 quri_ischange 제거
            dic.Remove("quri_ischange");
            // 원본의 quri_ischange도 false로 변경(필요시)
            // dic["quri_ischange"] = false;
          }
        }
      }
    }
    return result;
  }


  protected async Task<ResultInfo<T>> DbSave<T>(string proc_name, Dictionary<string, string> dic, ResultInfo<Dictionary<string, object>> data, bool isFast = false) {

    List<Dictionary<string, object>> mlist = GetChangeData(data);
    if (mlist == null || mlist.Count <= 0) {
      NotificationService.Notify(new NotificationMessage {
        Severity = NotificationSeverity.Warning,
        Summary = "알림",
        Detail = "수정대상이 존재하지 않습니다.",
       // Duration = duration,
        ShowProgress = true,
        Payload = DateTime.Now
      });
      return new ResultInfo<T>() { Code=-77, Message="수정대상이 존재하지 않습니다." } ;
    }
    else {
      RequestDto rd = new RequestDto() { ProcName = proc_name, MainParam = dic, ProcType = "save", IsFast = isFast, MultyData = mlist };

      var res = await DbCont<T>(rd);
      Notify(res);
      return res;
    }
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

    RequestDto rd = new RequestDto() { ProcName = action_name, MainParam = dic };
    var data = await devService.GetList<T>(rd);

    if (data.Code < 0) {
      Notify(NotificationSeverity.Error, "Error Message", data.Message, 10000, true);
    }
   
    return data;
  }



  protected async Task<ResultInfo<T>> JsProcDbReturn<T>(string action_name, CommonCode dbType, Dictionary<string, string> dic = null) {

    var param = GetProjDbParams(dbType);

    if (dic == null) dic = new Dictionary<string, string>() { };

    var dic_a = WasmUtil.JoinDictionaries(param, dic);

    RequestDto rd = new RequestDto() { ProcName = action_name, IsProjDb = true, MainParam = dic_a };

    var data = await devService.GetList<T>(rd);

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

  protected async Task<ResultInfo<T>> JsContQuery<T>(string db_nick, string query) {

    var data = await devService.GetListQuery<T>(db_nick, query);

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


    RequestDto rd = new RequestDto() { ProcName = "sp_projCommon", MainParam = new Dictionary<string, string>() {
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
    return (value?.ToString() == svalue) ;
  }







}