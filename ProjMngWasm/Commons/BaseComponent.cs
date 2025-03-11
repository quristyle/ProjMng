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



  public Dictionary<string, string> JoinDictionaries(IDictionary<string, object> dict1, Dictionary<string, string> dict2) {
    var result = new Dictionary<string, string>();

    var allKeys = dict1.Keys.Union(dict2.Keys);

    foreach (var key in allKeys) {
      if (dict1.ContainsKey(key)) {

        if (dict1[key].GetType() == typeof(DateTime)) {

          result[key] = ((DateTime)dict1[key]).ToString("yyyyMMdd");

        }
        else {
          result[key] = dict1[key] + "";
        }

      }
      else if (dict2.ContainsKey(key)) {
        result[key] = dict2[key];
      }
    }

    return result;
  }












}
