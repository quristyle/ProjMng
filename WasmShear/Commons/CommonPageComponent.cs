using System;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using ProjModel;

namespace WasmShear.Commons;

public class CommonPageComponent: CommonComponent {




  protected override async Task OnInitializedAsync() {
   await AuthCheck();
    //StateHasChanged();
  }



  //protected override void OnInitialized() {
  //  base.OnInitialized();
  //}


}
