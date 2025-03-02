using System;
using System.Data;
using ProjMngWasm.Models;
using System;
using System.Data;
using ProjMngWasm.Models;
using Newtonsoft.Json.Linq;
using ProjMngWasm.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System;
using System.Data;
using ProjMngWasm.Models;
using System;
using System.Data;
using ProjMngWasm.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text;
using System.Web;


namespace ProjMngWasm.Services;



public class FuneralService : IFuneralService {

 
protected string DataPath {get;set;} = "data";

  protected readonly HttpClient _httpClient;
  protected readonly ISessionStorageService _sess;

  public FuneralService(HttpClient httpClient , ISessionStorageService sess ) {
	_httpClient = httpClient;
	_sess = sess;
}



  public async Task<IEnumerable<IDictionary<string, object>>> GetList(string path, Dictionary<string, object> dic) {
    
    try    {

        var query = HttpUtility.ParseQueryString(string.Empty);
            query["p"] = path;
            foreach (var item in dic)            {
                query[item.Key] = item.Value.ToString();
            }
            string queryString = query.ToString();
            string url = $"fr3.jsp?{queryString}";

HttpResponseMessage response = await _httpClient.GetAsync(url);
        //요청 성공 여부 확인
        response.EnsureSuccessStatusCode();

        
            string responseString = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(responseString))            {
                Console.WriteLine("응답이 비어 있습니다.");
                return Enumerable.Empty<IDictionary<string, object>>();
            }

            JObject jobj = JObject.Parse(responseString);
            JToken jt = jobj.SelectToken(DataPath);

            if (jt != null)            {
                return jt.ToObject<List<IDictionary<string, object>>>();
            }
            else            {
                Console.WriteLine($"dataPath({DataPath}) 에서 해당하는 데이터가 없습니다.");
                return Enumerable.Empty<IDictionary<string, object>>();
            }
        }
        catch (HttpRequestException ex)        {
            Console.WriteLine($"HTTP 요청 실패: {ex.Message}");
            throw; // rethrow to allow upper levels to handle if necessary
        }
        catch (JsonException ex)        {
            Console.WriteLine($"JSON 파싱 실패: {ex.Message}");
            throw; // rethrow
        }
        catch (Exception ex)        {
            Console.WriteLine($"예외 발생: {ex.Message}");
            throw;
        }
    //return Enumerable.Empty<IDictionary<string, object>>();
}

 

}


public interface IFuneralService {
  Task<IEnumerable<IDictionary<string, object>>> GetList(string path, Dictionary<string, object> dic);
}


