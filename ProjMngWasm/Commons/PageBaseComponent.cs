using System;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using ProjModel;

namespace ProjMngWasm;

public class PageBaseComponent: BaseComponent {


  //[Inject] AppData appData { get; set; }
  [Parameter] public Action? OnRequestRemove { get; set; }
  [Parameter] public Action? OnRequestStateHasChange { get; set; }

  




  protected override async Task OnInitializedAsync() {
   await AuthCheck();
    //StateHasChanged();
  }



  //protected override void OnInitialized() {
  //  base.OnInitialized();
  //}


}
