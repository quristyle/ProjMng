
using ProjModel;
using WasmShear.Commons;

namespace WasmShear.Services;

public class SysService : BaseService {

  public SysService(IHttpClientFactory httpClientFactory) : base(httpClientFactory.CreateClient("jsini")) { }
  public async Task<ResultInfo<T>> GetList<T>(string proc_name, IDictionary<string, string> dic, string proc_type = "srch", bool isFast = false) {

    if (string.IsNullOrEmpty(proc_name) || proc_name.Length < 3) {
      // 규칙위반
    }
    string calltype = proc_name.Substring(0, 2).ToLower();
    string targetUrl = "api/Sys";

    Dictionary<string, string> req = new Dictionary<string, string>() {
      { "req_cname", proc_name }
      , { "req_type", proc_type }
      , { "req_sdt", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") }
      , { "req_isFast", isFast.ToString() }
    };

    var reqDic = WasmUtil.JoinDictionaries(dic, req);

    return await GetData<T>(proc_name, reqDic, targetUrl);
  }

}
