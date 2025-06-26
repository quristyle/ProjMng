
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using ProjModel;

namespace WasmShear;
public class AppData {
  public IDictionary<string, List<CommonCode>> GlobalDic { get; set; } = new Dictionary<string, List<CommonCode>>();


  public event Action? UserChanged;

  private Member _user;
  public Member User {
    get => _user;
    set {
      if (_user != value) {
        _user = value;
        UserChanged?.Invoke();
      }
    }
  }

  // session 값 비교를 통해 로그인 여부를 판단
  public bool IsLogin { get; set; }


  [Inject] protected IJSRuntime? JSRuntime { get; set; }

  public async Task<bool> AuthCheck() {


    // 인증체크
    if (User == null) {


      try {
        string json = await JSRuntime.InvokeAsync<string>("localStorage.getItem", "userInfo");
        if (!string.IsNullOrEmpty(json)) {
          Member user = JsonConvert.DeserializeObject<Member>(json);
          
        }
      }
      catch (Exception eee) {
      }

      return (User != null);
    }
    else {
      return true;
      // 인증된 사용자
      // appData.User = user;
      // appData.IsLogin = true;
    }


  }



}
public class Member {
  public string FirstName { get; set; }
  public string LastName { get; set; }
  public string Password { get; set; }
  public string Email { get; set; }
  public string Phone_num { get; set; }
  public string Address { get; set; }
  public string City { get; set; }
  public string State { get; set; }
  public string Zip { get; set; }
  public string Date { get; set; }
  public string EmpId { get; set; }
  public string UserId { get; set; }
  public string SessionId { get; set; }
  public string User_photo { get; set; }
  public string Remark { get; set; }
  public string Theme { get; set; } = "standard-dark";




}