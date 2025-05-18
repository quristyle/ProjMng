
using ProjModel;
using WasmShear.Commons;

namespace WasmShear.Services;

public class JsiniService : BaseService {

  public JsiniService(IHttpClientFactory httpClientFactory) : base(httpClientFactory.CreateClient("jsini")) { }
  public async Task<ResultInfo<T>> GetList<T>(string proc_name, IDictionary<string, string> dic, string proc_type = "srch", bool isFast = false) {

    if (string.IsNullOrEmpty(proc_name) || proc_name.Length < 3) {
      // 규칙위반
    }
    string calltype = proc_name.Substring(0, 2).ToLower();
    string targetUrl = TargetUrl;
    if (calltype == "sp") {
      targetUrl = TargetUrl;
      if (isFast) {
        targetUrl = TargetUrlFast;
      }
    }
    else if (calltype == "md") { targetUrl = TargetUrlMedia; }

    Dictionary<string, string> req = new Dictionary<string, string>() {
      { "req_cname", proc_name }
      , { "req_type", proc_type }
      , { "req_sdt", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") }
      , { "req_isFast", isFast.ToString() }
    };

    var reqDic = WasmUtil.JoinDictionaries(dic, req);

    return await GetData<T>(proc_name, reqDic, targetUrl);
  }


  public async Task<Member> Login(string id, string pw) {

    var dic = new Dictionary<string, string>() {
      { "userid", id }
      , { "pwd", pw }
    };
    var result = await GetList<Member>("sp_proj_login", dic);
    if (result.Code == 0) {
      return result.Data.FirstOrDefault();
    }
    return null; 
  }

}
