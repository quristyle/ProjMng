﻿using Microsoft.AspNetCore.Components;
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

  protected List<IDictionary<string, object>> ordersToInsert = new List<IDictionary<string, object>>();
  protected List<IDictionary<string, object>> ordersToUpdate = new List<IDictionary<string, object>>();

  protected DataGridEditMode editMode = DataGridEditMode.Single;

  public IDictionary<string, string> cols { get; set; } = new Dictionary<string, string>();

  [Parameter] public EventCallback<IDictionary<string, object>> SaveBtnEvent { get; set; }
  [Parameter] public EventCallback<IDictionary<string, object>> DeleteBtnEvent { get; set; }
  [Parameter] public EventCallback<IDictionary<string, object>> ActionBtnEvent { get; set; }
  // 편집 가능여부
  [Parameter] public bool IsEdit { get; set; } = false;
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
        Console.WriteLine($" qurigrid set : {value}");
        SItemsChanged.InvokeAsync(SItems);
      }
    }
  }
  [Parameter] public EventCallback<IList<IDictionary<string, object>>> SItemsChanged { get; set; }
  [Parameter] public ResultInfo<Dictionary<string, object>> ReqData { get; set; }

  protected override void OnParametersSet() {
    if (ReqData != null) {
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
    ordersToInsert.Clear();
    ordersToUpdate.Clear();
  }

  protected void Reset(IDictionary<string, object> order) {
    ordersToInsert.Remove(order);
    ordersToUpdate.Remove(order);
  }


  protected async Task EditRow(IDictionary<string, object> order) {
    if (!ordersGrid.IsValid) return;

    if (editMode == DataGridEditMode.Single) {
      Reset();
    }

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


  protected async Task InsertRow() {
    if (!ordersGrid.IsValid) return;

    if (editMode == DataGridEditMode.Single) {
      Reset();
    }

    var order = CreateData();



    ordersToInsert.Add(order);
    await ordersGrid.InsertRow(order);
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




  protected async Task ActionRow(IDictionary<string, object> order) {
  
    if (orders.Contains(order)) {
      await ActionBtnEvent.InvokeAsync(order);
    }
  }





  protected async Task InsertAfterRow(IDictionary<string, object> row) {
    if (!ordersGrid.IsValid) return;

    if (editMode == DataGridEditMode.Single) {
      Reset();
    }

    var order = CreateData();
    ordersToInsert.Add(order);
    await ordersGrid.InsertAfterRow(order, row);
  }

  protected async Task OnCreateRow(IDictionary<string, object> order) {

    await DataSave(order);
    ordersToInsert.Remove(order);
  }





  [Parameter] public EventCallback<IDictionary<string, object>> RowClickEvent { get; set; }



  protected async Task OnRowClick(IDictionary<string, object> e) {

    await RowClickEvent.InvokeAsync(e);

  }


  protected async Task OnDoubleClick(DataGridRowMouseEventArgs<IDictionary<string, object>> e) {
    await EditRow(e.Data);
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