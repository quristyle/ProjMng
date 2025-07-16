using Microsoft.AspNetCore.Components;
using ProjModel;
using Radzen;
using Radzen.Blazor;
using System.Linq;
using WasmShear;
using WasmShear.Services;

namespace ProjMngWasm.Compnents {
  public class QuriGrid<TItem> : RadzenDataGrid<TItem> {

    [Inject] protected DevService? devService { get; set; }
    [Inject] protected AppData? appData { get; set; }
    public QuriGrid() {
      Style = "height:100%;";
      FilterCaseSensitivity = Radzen. FilterCaseSensitivity.CaseInsensitive;
      LogicalFilterOperator = Radzen.LogicalFilterOperator.And;
      //AllowFiltering = true;   // 해당 옵션 동작시 방향키로 이동시 선택이 올바르게 되지 않는 문제가 발생함.
      //AllowColumnResize = true;
      FilterMode = Radzen.FilterMode.Simple;
      SelectionMode = Radzen.DataGridSelectionMode.Single;
      AllowPaging = false;
      AllowSorting = true;
      PagerHorizontalAlign = Radzen.HorizontalAlign.Left;

      ShowPagingSummary = true;


    }




    IRadzenFormComponent editor;
    bool editorFocused;
    string columnEditing;
    List<KeyValuePair<string, string>> editedFields = new List<KeyValuePair<string, string>>();
    List<TItem> ordersToUpdate = new List<TItem>();



    [Parameter]    public Func<TItem, string> KeySelector { get; set; }



    //string columnEditing;
    //List<KeyValuePair<string, string>> editedFields = new List<KeyValuePair<string, string>>();
    //List<TableInfo> ordersToUpdate = new List<TableInfo>();
    public async Task OnCellClickCehck(DataGridCellMouseEventArgs<TItem> args) {
      if (!this.IsValid ||
          (ordersToUpdate.Contains(args.Data) && columnEditing == args.Column.Property)) return;

      // Record the previous edited field, if you're not using IRevertibleChangeTracking to track object changes
      if (ordersToUpdate.Any()) {
        //var tname = (ordersToUpdate.First() as TableInfo).TableName;
        //editedFields.Add(new(tname, columnEditing));

        if (KeySelector != null)
          editedFields.Add(new(KeySelector(ordersToUpdate.First()), columnEditing));



        await Update();
      }

      // This sets which column is currently being edited.
      columnEditing = args.Column.Property;

      // This sets the Item to be edited.
      await EditRow2(args.Data);
    }



    // void Reset(TableInfo order = null)    {
    //     editorFocused = false;

    //     if (order != null)        {
    //         ordersToUpdate.Remove(order);
    //     }
    //     else        {
    //         ordersToUpdate.Clear();
    //     }
    // }

    public async Task Update() {
      editorFocused = false;

      if (ordersToUpdate.Any()) {
        await this.UpdateRow(ordersToUpdate.First());
      }
    }

    async Task EditRow2(TItem order) {
      Reset();

      ordersToUpdate.Add(order);

      await this.EditRow(order);
    }






    protected override async Task OnAfterRenderAsync(bool firstRender) {
      await base.OnAfterRenderAsync(firstRender);

      if (!editorFocused && editor != null) {
        editorFocused = true;

        try {
          await editor.FocusAsync();
        }
        catch {
          //
        }
      }
    }




    public bool IsEditing(string columnName, TItem order) {
      // Comparing strings is quicker than checking the contents of a List, so let the property check fail first.
      return columnEditing == columnName && ordersToUpdate.Contains(order);
    }

    public void Reset() {
      Reset(default);
    }

    public void Reset(TItem order) {
      editorFocused = false;

      if (!EqualityComparer<TItem>.Default.Equals(order, default)) {
        ordersToUpdate.Remove(order);
      }
      else {
        ordersToUpdate.Clear();
      }
    }



    /// <summary>
    /// resultinfo data list binding 
    /// </summary>
    /// <param name="procName"></param>
    /// <param name="dic"></param>
    /// <returns></returns>
    public async Task<IEnumerable<TItem>> Load(string procName, Dictionary<string, string> dic) {

      IsLoading = true;
      await InvokeAsync(StateHasChanged);


      RequestDto rd = appData.CreateDto(procName, dic);
      //rd.IsProjDb = true;
      //rd.IsFast = isFast;
      //rd.MultyData = mlist;


      //RequestDto rd = new RequestDto() { 
      //ProcName=procName, MainParam = dic
      //};


      var ri = await devService.GetList<TItem>(rd);
      if(ri.Code < 0) {
        Console.WriteLine($" Grd Load Error : {ri.Message}");
      }
      else {
        Data = ri.Data.ToList();
      }

      IsLoading = false;
      //await Reload();
      await InvokeAsync(StateHasChanged);
      return Data;
    }


  }
}
