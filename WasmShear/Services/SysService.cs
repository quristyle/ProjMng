
using ProjModel;
using System.IO;
using WasmShear.Commons;

namespace WasmShear.Services;

public class SysService : BaseService {

  public SysService(IHttpClientFactory httpClientFactory, AppData appData)
    : base(httpClientFactory.CreateClient("jsini"), appData) { }
  public async Task<ResultInfo<T>> GetList<T>(string proc_name, IDictionary<string, string> dic, string proc_type = "srch", bool isFast = false) {
    var dict = new Dictionary<string, string>(dic);



    RequestDto rd = _appData.CreateDto(proc_name, dict);
    rd.ProcType = proc_type;
    rd.IsFast = isFast;
    //rd.MultyData = mlist;




    //RequestDto rd = new RequestDto() { ProcType= proc_type, ProcName=proc_name, IsFast=isFast, MainParam= dict };

    string targetUrl = "api/Sys";

    return await GetData<T>(rd, targetUrl);
  }

}
