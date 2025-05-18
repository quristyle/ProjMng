
using System.Web;
using WasmShear.Commons;

namespace WasmShear.Services;

public class FuneralService : BaseService {

  public FuneralService(HttpClient httpClient) : base(httpClient) {
    RecPath = "result";
  }

  public async Task<IEnumerable<IDictionary<string, object>>> GetList(string path, Dictionary<string, object> dic) {

    var query = HttpUtility.ParseQueryString(string.Empty);
    query["p"] = path;
    foreach (var item in dic) {
      query[item.Key] = item.Value.ToString();
    }
    string queryString = query.ToString();
    string url = $"fr3.jsp?{queryString}";

    var dicc = WasmUtil.JoinConvert(dic);
    var res = await GetData<IDictionary<string, object>>(path, dicc, url, HttpCallType.Get);

    return res.Data;
  }

}