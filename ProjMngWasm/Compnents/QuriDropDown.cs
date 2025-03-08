using System;
using Microsoft.AspNetCore.Components;
using Radzen.Blazor;

namespace ProjMngWasm.Compnents;

public class QuriDropDown<CommonCode> : RadzenDropDown<CommonCode> {

  public string _codeId;
  [Parameter] public string CodeId{get{ return _codeId; }set{ _codeId = value; LoadData();}}


  public static IDictionary<string, IDictionary<string, string>> GlobalDic { get; set; } = new Dictionary<string, IDictionary<string, string>>();
  IDictionary<string, string> codeList = new Dictionary<string, string>() { { "11", "quristyle all" }, 
  { "12", "quristyle wait" }, 
  { "13", "ui userAll complete" } ,
  { "14", "ui userAll wait" } ,
  { "15", "ui userAll " } ,
  }; //No longer needed here

  IDictionary<string, string> projlist = new Dictionary<string, string>() { 
  { "hanju_dev", "hanju_dev" }, 
  { "hanju_prod", "hanju_prod" }, 
  { "funeralfr", "funeralfr" }, 
  { "jsini", "jsini" }, 
  }; //No longer needed here


  protected override async Task OnInitializedAsync()    {
        await base.OnInitializedAsync();
        TextProperty="Value" ;
        ValueProperty="Key";

        GlobalDic.Add("projuser", codeList);

        GlobalDic.Add("projlist", projlist);
    }

    protected override  async void OnParametersSet()    {
      base.OnParametersSet();
    await  LoadData();
    }


async Task LoadData(){

        if (!string.IsNullOrEmpty(_codeId) && GlobalDic != null)        {
            if (!GlobalDic.ContainsKey(_codeId))            {
                // If the CodeId doesn't exist, provide a default or empty list
                // 존재 하지 ᄋ않으면 서버에서 구해온다.
                Data = new List<KeyValuePair<string, string>>();
            }
            else            {
                Data = GlobalDic[_codeId].Select(kvp => new KeyValuePair<string, string>(kvp.Key, kvp.Value)).ToList();
            }
        }
        else        {
            // If CodeId is empty or GlobalDic is null, provide a default or empty list
            Data = new List<KeyValuePair<string, string>>();
        }
}
  

}
