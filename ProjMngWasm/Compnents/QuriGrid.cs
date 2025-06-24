using Microsoft.AspNetCore.Components;
using ProjModel;
using Radzen.Blazor;
using WasmShear.Services;

namespace ProjMngWasm.Compnents {
  public class QuriGrid<TItem> : RadzenDataGrid<TItem> {

    [Inject] protected DevService? devService { get; set; }
    public QuriGrid() {
      Style = "height:100%;";
      FilterCaseSensitivity = Radzen. FilterCaseSensitivity.CaseInsensitive;
      LogicalFilterOperator = Radzen.LogicalFilterOperator.And;
      AllowFiltering = true;
      AllowColumnResize = true;
      FilterMode = Radzen.FilterMode.Simple;
      SelectionMode = Radzen.DataGridSelectionMode.Single;
      AllowPaging = false;
      AllowSorting = true;
      PagerHorizontalAlign = Radzen.HorizontalAlign.Left;

      ShowPagingSummary = true;
    }

    //public List<T>? Data { get; set; }
    public async Task<IEnumerable<TItem>> Load(string procName, Dictionary<string, string> dic) {

      IsLoading = true;
      await InvokeAsync(StateHasChanged);

      RequestDto rd = new RequestDto() { 
      ProcName=procName, MainParam = dic
      };


      var ri = await devService.GetList<TItem>(rd);
      Data = ri.Data.ToList();

      IsLoading = false;
      //await Reload();
      await InvokeAsync(StateHasChanged);
      return Data;
    }


  }
}
