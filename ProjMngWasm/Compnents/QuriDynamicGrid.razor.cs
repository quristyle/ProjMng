using Microsoft.AspNetCore.Components;
using ProjModel;
using Radzen.Blazor;
using Radzen;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace ProjMngWasm.Compnents;
public class QuriDynamicGridBase : BaseComponent {

  protected RadzenDataGrid<IDictionary<string, object>> ordersGrid;
  //protected IEnumerable<IDictionary<string, object>> orders;
  protected List<Dictionary<string, object>> orders;
  //protected IEnumerable<Dictionary<string, object>> _data { get; set; } = new List<Dictionary<string, object>>();

  //protected List<IDictionary<string, object>> ordersToInsert = new List<IDictionary<string, object>>();
  protected List<IDictionary<string, object>> ordersToUpdate = new List<IDictionary<string, object>>();

  protected DataGridEditMode editMode = DataGridEditMode.Single;

  public IDictionary<string, string> cols { get; set; } = new Dictionary<string, string>();

  [Parameter] public EventCallback<IDictionary<string, object>> CopyBtnEvent { get; set; }
  [Parameter] public EventCallback<IDictionary<string, object>> AddBtnEvent { get; set; }
  [Parameter] public EventCallback<IDictionary<string, object>> SaveBtnEvent { get; set; }
  [Parameter] public EventCallback<IDictionary<string, object>> DeleteBtnEvent { get; set; }
  [Parameter] public EventCallback<IDictionary<string, object>> ActionBtnEvent { get; set; }
  // 편집 가능여부
  //[Parameter] public bool IsEdit { get; set; } = false;
  // 동작 버턴 활성 여부


  public bool IsSave {
    get {
      return SaveBtnEvent.HasDelegate;
    }
  }
  public bool IsDelete {
    get {
      return DeleteBtnEvent.HasDelegate;
    }
  }

  public bool IsAction {
    get {
      return ActionBtnEvent.HasDelegate;
    }
  }


  [Parameter] public string HiddenCols { get; set; }
  private string[] _hiddenColArr = { };



  bool _isLoading { get; set; }
  [Parameter]  public bool IsLoading { get; set; }


  [Parameter] public EventCallback<bool> IsLoadingChanged { get; set; }


  public IList<IDictionary<string, object>> _selectedItems { get; set; }

  [Parameter]
  public IList<IDictionary<string, object>> SItems {
    get { return _selectedItems; }
    set {
      if (value != _selectedItems) {
        _selectedItems = value;
        //Console.WriteLine($" qurigrid set : {value}");
        SItemsChanged.InvokeAsync(SItems);
      }
    }
  }
  [Parameter] public EventCallback<IList<IDictionary<string, object>>> SItemsChanged { get; set; }
  [Parameter] public ResultInfo<Dictionary<string, object>> ReqData { get; set; }
  
  public int RecordCount { get; set; } = 0;

  protected override void OnParametersSet() {
    if (ReqData != null) {
      RecordCount = ReqData.Data.Count;
      orders = ReqData.Data;
      cols = ReqData.Cols;
    }

    //if (SItems != _selectedItems) {
    //  _selectedItems = SItems;
    //  Console.WriteLine($" qurigrid set2 : {SItems}");
    //  SItemsChanged.InvokeAsync(SItems);
    //}


    if (_isLoading != IsLoading) {
      _isLoading = IsLoading;

      IsLoadingChanged.InvokeAsync(IsLoading);
    }

    if (!string.IsNullOrEmpty(HiddenCols)) {
      _hiddenColArr = HiddenCols.Split(',');
    }


  }





  protected bool isHiddenCol(string colname) {
    bool result = false;
    foreach (string a in _hiddenColArr) {
      if (colname == a) { result = true; break; }
    }
    return result;
  }





  protected void Reset() {
    //ordersToInsert.Clear();
    ordersToUpdate.Clear();
  }

  protected void Reset(IDictionary<string, object> order) {
    //ordersToInsert.Remove(order);
    ordersToUpdate.Remove(order);
    ordersToUpdate.Clear();
  }


  protected async Task EditRow(IDictionary<string, object> order) {
    if (!ordersGrid.IsValid) return;

    //if (editMode == DataGridEditMode.Single) {
    //  Reset();
    //}
    order["quri_ischange"] = true;
    ordersToUpdate.Add(order);
    await ordersGrid.EditRow(order);
  }


  protected void CancelEdit(IDictionary<string, object> order) {
    Reset(order);
    ordersGrid.CancelEditRow(order);

  }



  Dictionary<string, object> CreateData() {

    var order = new Dictionary<string, object>();

    foreach (var c in cols) {
      order.Add(c.Key, "");
    }




    return order;
  }


  //protected async Task InsertRow() {
  //  if (!ordersGrid.IsValid) return;

  //  if (editMode == DataGridEditMode.Single) {
  //    Reset();
  //  }

  //  var order = CreateData();



  //  ordersToInsert.Add(order);
  //  await ordersGrid.InsertRow(order);
  //}



  protected async Task ConfDeleteRow(IDictionary<string, object> order) {

    var a = await DialogService.Confirm("Are You Delete?", "Qutions?", new ConfirmOptions() { OkButtonText = "Delete", CancelButtonText = "Cancel" });

    if(a == true) {
     await DeleteRow(order);
    }
  }


  protected async Task DeleteRow(IDictionary<string, object> order) {
    Reset(order);

    if (orders.Contains(order)) {

      orders.Remove((Dictionary<string, object>)order);

      await DeleteBtnEvent.InvokeAsync(order);

    }
    else {
      ordersGrid.CancelEditRow(order);
    }

    await ordersGrid.Reload();
    StateHasChanged();

  }

  public async Task Reload() {

    for (int i = ordersToUpdate.Count - 1; i >= 0; i--) {
      ordersGrid.CancelEditRow(ordersToUpdate[i]);
    }
    Reset();
    //await ordersGrid.Reload();
    StateHasChanged();
  }



  protected async Task ActionRow(IDictionary<string, object> order) {
  
    if (orders.Contains(order)) {
      await ActionBtnEvent.InvokeAsync(order);
    }
  }





  protected async Task InsertAfterRow(IDictionary<string, object> row) {
    if (!ordersGrid.IsValid) return;

    //if (editMode == DataGridEditMode.Single) {
    //  Reset();
    //}

    int findIndex = 0;
    var firstItem = SItems?.FirstOrDefault();
    if (firstItem != null) {
      findIndex = orders.FindIndex(order => order.SequenceEqual(firstItem)); // 항목이 존재 하는데 -1 리턴한다. 고쳐라..
      Console.WriteLine($"Index of the first item in SItems within orders: {findIndex}");
    }
    else {
      Console.WriteLine("SItems is empty or null.");
    }

    if (findIndex < 0) findIndex = 0;

    //SItems

    //ordersGrid

    var order = CreateData();
    //ordersToInsert.Add(order);

    orders.Insert(findIndex, order);

    await ordersGrid.EditRow(order);


    RecordCount = orders.Count;

    await ordersGrid.RefreshDataAsync();

    //await ordersGrid.InsertAfterRow(order, row);

    //await ordersGrid.RowSelect.InvokeAsync(row);

    //await ordersGrid.InsertRow(order);
    await AddBtnEvent.InvokeAsync(order);

    //StateHasChanged();


  }


  protected async Task CopyAfterRow(IDictionary<string, object> row) {
    if (!ordersGrid.IsValid) return;

    if (editMode == DataGridEditMode.Single) {
      Reset();
    }

    int findIndex = 0;
    var firstItem = SItems?.FirstOrDefault();
    if (firstItem != null) {
      findIndex = orders.FindIndex(order => order.SequenceEqual(firstItem)); // 항목이 존재 하는데 -1 리턴한다. 고쳐라..
      Console.WriteLine($"Index of the first item in SItems within orders: {findIndex}");

      // 이때만 copy 하자..


      if (findIndex < 0) findIndex = 0;

      var order = CreateData();



      foreach ( var o in firstItem) {

        if (isHiddenCol(o.Key)) { continue; } // 숨긴 칼럼은 복제 하지 않는다.
        order[o.Key] = o.Value;
      }


      orders.Insert(findIndex+1, order);

      await ordersGrid.EditRow(order);

      RecordCount = orders.Count;

      await ordersGrid.RefreshDataAsync();

      await CopyBtnEvent.InvokeAsync(order);

    }
    else {
      Console.WriteLine("SItems is empty or null.");
    }


    //await AddBtnEvent.InvokeAsync(order);

  }







  protected async Task OnCreateRow(IDictionary<string, object> order) {

    await DataSave(order);
    //ordersToInsert.Remove(order);
  }





  [Parameter] public EventCallback<IDictionary<string, object>> RowClickEvent { get; set; }



  protected async Task OnRowClick(IDictionary<string, object> e) {

    await RowClickEvent.InvokeAsync(e);

  }


  protected async Task OnDoubleClick(DataGridRowMouseEventArgs<IDictionary<string, object>> e) {
    if( IsSave) {
      await EditRow(e.Data);
    }
  }

  protected async Task SaveRow(IDictionary<string, object> order) {
    await ordersGrid.UpdateRow(order);
  }


  protected async Task OnUpdateRow(IDictionary<string, object> order) {
    await DataSave(order);
    Reset(order);
  }

  protected async Task DataSave(IDictionary<string, object> order) {
    await SaveBtnEvent.InvokeAsync(order);
  }




}