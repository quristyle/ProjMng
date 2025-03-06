using System;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;  
using ProjMngWasm.Models;
using ProjMngWasm.Services;
using ProjModel;

namespace ProjMngWasm;

public class BaseComponent: ComponentBase{

[Inject] protected IJsiniService JsiniService { get; set; }

[Inject] protected IUMSService UmsService { get; set; }

}
