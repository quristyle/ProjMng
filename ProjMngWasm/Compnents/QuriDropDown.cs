using Microsoft.AspNetCore.Components;
using ProjMngWasm.Commons;
using ProjModel;
using Radzen.Blazor;
using System.Linq;
using System.Text.RegularExpressions;
using WasmShear;
using WasmShear.Services;

namespace ProjMngWasm.Compnents;

public class QuriDropDown<TValue> : RadzenDropDown<TValue> {

  [Inject] private JsiniService? jsiniService { get; set; }
  [Inject] AppProjData? appData { get; set; }

  [Parameter] public string? InitialCode { get; set; }

  public string? _codeId;
  //private string? _previousCodeId;
  [Parameter] public string? CodeId { get; set; }

  /// <summary> 전체 표기 여부 </summary>
  [Parameter] public bool IsAll { get; set; } = false;

  /// <summary> etc 에 값이 비었을때 메인코드 데이터를 호출 하지 않는다. </summary>
  [Parameter] public bool IsEtcFix { get; set; } = false;

  string? _etc0;
  [Parameter] public string? Etc0 { get; set; }

  [CascadingParameter(Name = "RegisterDropDown")] public Action<string>? RegisterDropDown { get; set; }
  [CascadingParameter(Name = "OnDropDownLoaded")] public Action<string>? OnParentDropDownLoaded { get; set; }


  


  protected override void OnInitialized() {
    base.OnInitialized();
    if (RegisterDropDown != null && !string.IsNullOrEmpty(CodeId))
      RegisterDropDown(CodeId);
  }

  // 데이터 로딩이 끝나는 시점에 호출
  private void NotifyParentLoaded() {
    if (OnParentDropDownLoaded != null && !string.IsNullOrEmpty(CodeId))
      OnParentDropDownLoaded(CodeId);
  }




  protected override async void OnParametersSet() {
    base.OnParametersSet();
    if (_codeId != CodeId) {
      _codeId = CodeId;
      await LoadItems();
    }

    if (_etc0 != Etc0) {
      _etc0 = Etc0;
      await LoadItems();
    }

  }

  //bool isFrist = true;
  async Task LoadItems() {

    if (IsEtcFix && string.IsNullOrEmpty(Etc0)) {
      // 데이터 로딩 완료 후 부모에 알림
      NotifyParentLoaded();
      return;
    }

    List<CommonCode> tmp = new List<CommonCode>();

    if (!string.IsNullOrEmpty(_codeId)) {
      // isRelod

      if (!string.IsNullOrWhiteSpace(_etc0) && appData.GlobalDic.TryGetValue(_codeId + "_etc0_" + _etc0, out var dictionary)) {


        Console.WriteLine($"isRelod LoadItems : {_codeId} etc0 : {_etc0}");

        // dictionary는 _codeId에 해당하는 Dictionary<string, string>입니다.
        tmp = WasmUtil.DeepCopy(dictionary);
      }
      else if (string.IsNullOrWhiteSpace(_etc0) && appData.GlobalDic.TryGetValue(_codeId, out var dictionary2)) {
        // dictionary는 _codeId에 해당하는 Dictionary<string, string>입니다.

        Console.WriteLine($"Finder LoadItems : {_codeId} etc0 : {_etc0}");

        tmp = WasmUtil.DeepCopy(dictionary2);
      }
      else {


        Console.WriteLine($"LoadData LoadItems : {_codeId} etc0 : {_etc0}");


        RequestDto rd = new RequestDto() {
          ProcName = "sp_projCommon",
          MainParam = new Dictionary<string, string>() {
                { "code_id", _codeId },
                { "etc0", Etc0 }
            }
        };



        // _codeId에 해당하는 딕셔너리가 없을 경우 처리
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

        if (!appData.GlobalDic.ContainsKey(_codeId)) {
          appData.GlobalDic.Add(_codeId, dic);
        }
        tmp = WasmUtil.DeepCopy(dic);

      }
    }
    else {
      tmp = new List<CommonCode>();
    }

    if (IsAll) {
      tmp.Insert(0, new CommonCode() { Code = "", Name = "All", Desc = "All items" });
    }

    Data = tmp.AsEnumerable();


    if (!string.IsNullOrEmpty(InitialCode)) {
      var match = tmp.FirstOrDefault(x => x.Code == InitialCode);
      if (match != null) {
        await ValueChanged.InvokeAsync((TValue)(object)match);
        //Value = (TValue)(object)match;
      }
      Console.WriteLine($"기본 선택자 선택 : {_codeId} InitialCode : {InitialCode}");
    }
    else if (!IsAll && tmp.Count > 0) {
      object obj = tmp[0];
      await ValueChanged.InvokeAsync((TValue)obj);
      //Value = (TValue)obj;
      Console.WriteLine($"첫번째 코드 선택 : {_codeId} etc0 : {_etc0}");
    }
    else if (IsAll) {
      object obj = tmp[0];
      await ValueChanged.InvokeAsync((TValue)obj);
      //Value = (TValue)obj;
      Console.WriteLine($"All 선택 : {_codeId} etc0 : {_etc0}");
    }



    //if (!IsAll && tmp.Count > 0) {
    //  object obj = tmp[0];
    //  await ValueChanged.InvokeAsync((TValue)obj); // 바인딩된 변수에 반영
    //}
    StateHasChanged(); // 다시 렌더링


    // 데이터 로딩 완료 후 부모에 알림
    NotifyParentLoaded();

  }



  //[Parameter] public CommonCode Value { get; set; }              // 바인딩 값
  //[Parameter] public EventCallback<CommonCode> ValueChanged { get; set; } // 변경 이벤트

}
