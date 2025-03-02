using Microsoft.JSInterop;
using Newtonsoft.Json;

namespace ProjMngWasm.Services {
  public class SessionStorageService : ISessionStorageService {


    private readonly IJSRuntime _jsRuntime;

    public SessionStorageService(IJSRuntime jsRuntime) {
      this._jsRuntime = jsRuntime;
    }

    public async Task<T?> GetItemAsync<T>(string name) {
      try {
        var data = await _jsRuntime.InvokeAsync<string>("sessionStorage.getItem", name);

        if (data == null)
          return default;

        return JsonConvert.DeserializeObject<T>(data);

      }
      catch {
        return default;
      }
    }


    public async Task SetItemAsync(string name, object value) {
      await _jsRuntime.InvokeVoidAsync("sessionStorage.setItem", name, JsonConvert.SerializeObject(value));
    }

    
    public async Task RemoveItemAsync(string name) {
      await _jsRuntime.InvokeVoidAsync("sessionStorage.removeItem", name);
    }

  }

  public interface ISessionStorageService {

    Task<T?> GetItemAsync<T>(string name);

    Task SetItemAsync(string name, object value);

    Task RemoveItemAsync(string name);

  }
}
