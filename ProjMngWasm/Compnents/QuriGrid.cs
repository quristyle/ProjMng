using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.JSInterop;
using ProjModel;
using Radzen;
using Radzen.Blazor;
using System.Linq;
using System.Text.RegularExpressions;
using WasmShear;
using WasmShear.Services;
using static System.Runtime.InteropServices.Marshalling.IIUnknownCacheStrategy;

namespace ProjMngWasm.Compnents {
  public class QuriGrid<TItem> : RadzenDataGrid<TItem> {

    [Inject] protected DevService? devService { get; set; }
    [Inject] protected AppData? appData { get; set; }
    [Inject] protected IJSRuntime? jsRuntime { get; set; }






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




    //protected override async Task OnAfterRenderAsync(bool firstRender) {
    //  await base.OnAfterRenderAsync(firstRender);

    //  Console.WriteLine($" rrrrrrrrrrrrrrrquriGrid..OnAfterRenderAsync : {editorFocused} ");


    //  /*
    //  //if (!editorFocused && editor != null) {
    //    if (!editorFocused ) {
    //      editorFocused = true;

    //    try {

    //      Console.WriteLine($" FocusAsync..start");
    //      //await editor.FocusAsync();
    //      //await this.FocusAsync();
    //      Console.WriteLine($" FocusAsync..end");
    //    }
    //    catch(Exception eee) {

    //      Console.WriteLine($" fouce...... {eee.Message}");
    //      //
    //    }
    //  }
    //    */
    //}
    private bool _initialized;

    /*
     RadzenDataGrid<TItem>.CellClick은 일반적인 delegate 이벤트가 아니라
    EventCallback<DataGridCellMouseEventArgs<TItem>> 형태의 [Parameter] 속성이기 때문에

    RadzenDataGrid<T>의 CellClick은 [Parameter]이므로 일반적인 이벤트처럼 += 할 수 없습니다.

따라서 위처럼 EventCallback.Factory.Create로 동적으로 주입해야 합니다.

OnAfterRenderAsync 내에서 최초 렌더링 시점에만 바인딩하면 외부 속성 선언 없이도 작동합니다.



     
     */
    protected override async Task OnAfterRenderAsync(bool firstRender) { // 이거 중요하다... parameter 방식의 이벤트 처럼 보이는 것들.... 이런 방식으로 내부 구현 가능.
      if (!_initialized && firstRender) {
        _initialized = true;

        // CellClick에 내부 핸들러 주입
        this.CellClick = EventCallback.Factory.Create<DataGridCellMouseEventArgs<TItem>>(this, OnInternalCellClick);
      }

      await base.OnAfterRenderAsync(firstRender);
    }





    private async Task OnInternalCellClick(DataGridCellMouseEventArgs<TItem> args) {
      Console.WriteLine($"[QuriGrid 내부] Cell 클릭됨 - {args.Column?.Property}");



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
      //await EditRow2(args.Data);




      // 사용자 정의 이벤트에도 전달
      //if (QuriCellClick.HasDelegate) {
      //  await QuriCellClick.InvokeAsync(args);
      //}
    }
















    protected override void OnInitialized() {
      base.OnInitialized();

      // FooterTemplate이 지정되지 않은 경우 기본 템플릿 할당
      // FooterTemplate이 지정되지 않은 경우 기본 템플릿 할당
      if (base.FooterTemplate == null) {
        base.FooterTemplate = builder => {
          builder.OpenElement(0, "div");
          builder.AddAttribute(1, "class", "action-btn px-1 py-1");

          builder.OpenElement(2, "a");
          builder.AddAttribute(3, "class", "action float-start pe-2 pt-1");
          builder.AddAttribute(4, "onclick", EventCallback.Factory.Create(this, () => ExportXlsx()));
          builder.AddAttribute(5, "title", "excel");
          builder.AddMarkupContent(6, "<i class=\"bi bi-file-earmark-excel\"></i>");
          builder.CloseElement();

          builder.OpenElement(7, "a");
          builder.AddAttribute(8, "class", "action float-end ps-2 pe-1 pt-1");
          builder.AddAttribute(9, "onclick", EventCallback.Factory.Create(this, () => ToggleFilter()));
          builder.AddAttribute(10, "title", "filter");
          builder.AddMarkupContent(11, "<i class=\"bi bi-filter\"></i>");
          builder.CloseElement();

          builder.OpenElement(12, "span");
          builder.AddAttribute(13, "class", "action float-end p-1");
          builder.AddContent(14, $"{DataCount()} items");
          builder.CloseElement();

          builder.CloseElement();
        };
      }
    }




    public void ToggleFilter() {
      AllowFiltering = !AllowFiltering;
    }
    public int DataCount() {
      if (this.Data == null) { return 0; }
      return this.Data.Count();
    }


    public async Task ExportXlsx() {

      var columnsToExport = this.ColumnsCollection.Where(c => c.GetVisible() && !string.IsNullOrEmpty(c.Property));
      /*
      //var OrderBy = ordersGrid.Query.OrderBy;
      //var Filter = ordersGrid.Query.Filter;

      Console.WriteLine($" export xlsx : {Select}");
      */

      var Select = string.Join(",", this.ColumnsCollection
                   .Where(c => c.GetVisible() && !string.IsNullOrEmpty(c.Property))
                   .Select(c => c.Property.Contains(".") ? $"{c.Property} as {c.Property.Replace(".", "_")}" : c.Property));


      Console.WriteLine($" export2 xlsx : {Select}");




      IEnumerable<TItem> rows = this.Data;

      List<string[]> data = new();

      List<string> aaa = new();
      foreach (var col in columnsToExport) {
        aaa.Add(col.Title);
      }
      data.Add(aaa.ToArray());

      
      foreach (var row in rows) {
        List<string> bbb = new();
        foreach (var col in columnsToExport) {
          // col.Property와 동일한 이름의 프로퍼티 또는 필드 값을 가져옴
          var propInfo = typeof(TItem).GetProperty(col.Property);
          object? value = null;
          if (propInfo != null) {
            value = propInfo.GetValue(row);
          }
          else {
            // 프로퍼티가 없으면 필드도 시도
            var fieldInfo = typeof(TItem).GetField(col.Property);
            if (fieldInfo != null) {
              value = fieldInfo.GetValue(row);
            }
          }
          bbb.Add(value?.ToString() ?? "");
        }
        data.Add(bbb.ToArray());
      }

      
      //  return;


      // 엑셀에 들어갈 데이터 (예시)
      // var data = new[]
      // {
      //     new[] { "이름", "나이", "직업" },
      //     new[] { "홍길동", "25", "개발자" },
      //     new[] { "김철수", "30", "디자이너" },
      //     new[] { "박영희", "28", "마케팅" }
      // };

      // JavaScript로 엑셀 다운로드 함수 호출
      await jsRuntime.InvokeVoidAsync("downloadExcel", data.ToArray(), "엑셀파일.xlsx");




    }

    public string GetProp(string property) {

      string pattern = @"\[""(.*?)""\]";
      Match match = Regex.Match(property, pattern);

      if (match.Success) {
        string extractedValue = match.Groups[1].Value;
        return extractedValue; // 출력: prj_rid
      }
      return string.Empty;


    }





    //IRadzenFormComponent editor;
    public bool editorFocused;
    string columnEditing;
    List<KeyValuePair<string, string>> editedFields = new List<KeyValuePair<string, string>>();
    List<TItem> ordersToUpdate = new List<TItem>();



    [Parameter]    public Func<TItem, string> KeySelector { get; set; }

    public bool GetEditorFocused() {
      return editorFocused;
    }

    public void SetEditorFocused(bool val) {
       editorFocused = val;
    }



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

      Console.WriteLine($" Update editorFocused : {editorFocused}");

      if (ordersToUpdate.Any()) {
        var sItem = ordersToUpdate.First();
        await this.UpdateRow(sItem);
        if( sItem is BaseModel) {
          (sItem as BaseModel).isChanged = false;
        }
      }
    }

    async Task EditRow2(TItem order) {
      Reset();

      ordersToUpdate.Add(order);

      await this.EditRow(order);
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
      Console.WriteLine($" Reset editorFocused : {editorFocused}");

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

      Console.WriteLine($"Load :1");
      IsLoading = true;
      await InvokeAsync(StateHasChanged);

      Console.WriteLine($"Load :2");

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

      Console.WriteLine($"Load :3");
      IsLoading = false;
      //await Reload();
      await InvokeAsync(StateHasChanged);

      Console.WriteLine($"Load :4");
      return Data;
    }


  }
}
