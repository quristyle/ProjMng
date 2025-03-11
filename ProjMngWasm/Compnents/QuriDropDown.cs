using Microsoft.AspNetCore.Components;
using ProjMngWasm.Commons;
using ProjMngWasm.Services;
using ProjModel;
using Radzen.Blazor;

namespace ProjMngWasm.Compnents;

public class QuriDropDown<TValue> : RadzenDropDown<TValue> {

  [Inject] protected IJsiniService JsiniService { get; set; }
  [Inject]  AppData appData { get; set; }

  public string _codeId;
  private string _previousCodeId;
  [Parameter] public string CodeId { 
    get { return _codeId; } 
    set {
      if (_codeId != value) {
        _codeId = value;
        //LoadItems();
      }
    }
  }
  [Parameter] public bool IsAll { get; set; } = false;


  string _etc0;
  [Parameter] public string Etc0 { 
    get { return _etc0; } 
    set {
      if (_etc0 != value) {
        _etc0 = value;
        if (!string.IsNullOrWhiteSpace(value)) {
          //LoadItems(true);
        }
      }
    }
  }

  //Dictionary<string, string> codeList = new Dictionary<string, string>() { { "11", "quristyle all" },
  //{ "12", "quristyle wait" },
  //{ "13", "ui userAll complete" } ,
  //{ "14", "ui userAll wait" } ,
  //{ "15", "ui userAll " } ,
  //}; //No longer needed here

  protected override async Task OnInitializedAsync() {
    await base.OnInitializedAsync();
    //TextProperty = "Cd_Name";
    //ValueProperty = "Cd_Id";
    //TextProperty = "Cd_Name";
    //ValueProperty = "Cd_Id";



    //appData.GlobalDic.Add("projuser", codeList);
  }

  //protected override async void OnParametersSet() {
  //  base.OnParametersSet();
  // await LoadItems();
  //}



  protected override async void OnParametersSet() {
    base.OnParametersSet();
    if (_previousCodeId != _codeId) {
      _previousCodeId = _codeId;
      await LoadItems();
    }
  }

  //bool isFrist = true;
  async Task LoadItems(bool isRelod = false) {


    Console.WriteLine($"LoadItems : {_codeId} etc0 : {_etc0}");
    //if(!isFrist){
    //  isFrist = false;
    //  return;
    //}


    IEnumerable<CommonCode> tmp = new List<CommonCode>();

    if (!string.IsNullOrEmpty(_codeId)) {
      // isRelod

      if (!string.IsNullOrWhiteSpace(_etc0) && appData.GlobalDic.TryGetValue(_codeId+"_etc0_"+ _etc0, out var dictionary)) {


        Console.WriteLine($"isRelod LoadItems : {_codeId} etc0 : {_etc0}");

        // dictionary는 _codeId에 해당하는 Dictionary<string, string>입니다.
        tmp = dictionary.AsEnumerable();
      }
      else if (string.IsNullOrWhiteSpace(_etc0) && appData.GlobalDic.TryGetValue(_codeId, out var dictionary2)) {
        // dictionary는 _codeId에 해당하는 Dictionary<string, string>입니다.

        Console.WriteLine($"Finder LoadItems : {_codeId} etc0 : {_etc0}");

        tmp = dictionary2.AsEnumerable();
      }
      else {


        Console.WriteLine($"LoadData LoadItems : {_codeId} etc0 : {_etc0}");

        // _codeId에 해당하는 딕셔너리가 없을 경우 처리
        var data = await JsiniService.GetList<Dictionary<string, string>>(new Dictionary<string, string>() {
                { "stp", "sp_projCommon" },
                { "code_id", _codeId },
                { "etc0", Etc0 }
            });


        List<CommonCode> dic = new List<CommonCode>();
        foreach (var d in data.Data) {

          dic.Add(

            new CommonCode() {
              Cd_Id = d["cd_id"],
              Cd_Name = d["cd_name"],
              Cd_Desc = d["cd_desc"],
              //Others = d,
            }

            );
        }

        if(!appData.GlobalDic.ContainsKey(_codeId)) {
          appData.GlobalDic.Add(_codeId, dic);
        }
        tmp = dic.AsEnumerable();

        //tmp = Enumerable.Range(0, 100).Select(i => new CommonCode() { Cd_Id = $"ID{i}", Cd_Name = $"Name{i}" });

      }
    }
    else {
      // If CodeId is empty or GlobalDic is null, provide a default or empty list
      tmp = (new List<CommonCode>()).AsEnumerable();
    }

    if (IsAll) {
      //tmp.Insert(0, new CommonCode() { Cd_Id="", Cd_Name="All", Cd_Desc="All items" });
      
    }

    Data = tmp;
    //if (!IsAll && tmp.Count > 0) {
      /*
      var dt =  base.LoadData;

      this.selectedIndex = 0;
      this.SelectedItem = tmp[0];
      this.SelectedItemsText = tmp[0].Value;
      this.Value = tmp[0].Key;

      await Task.Run(async  () => {
          this.selectedIndex = 0;
          this.SelectedItem = tmp[0];
          this.SelectedItemsText = tmp[0].Value;
          this.Value = tmp[0].Key;
          await Task.Delay(10);
      });

      StateHasChanged();
      */
    //}
  }



}
