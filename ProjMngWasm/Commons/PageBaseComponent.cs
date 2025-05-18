using System;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using ProjModel;

namespace ProjMngWasm;

public class PageBaseComponent: BaseComponent {




  protected override async Task OnInitializedAsync() {
   await AuthCheck();
    //StateHasChanged();
  }



  //protected override void OnInitialized() {
  //  base.OnInitialized();
  //}


}
