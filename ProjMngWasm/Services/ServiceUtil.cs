using Newtonsoft.Json.Linq;
using ProjMngWasm.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ProjMngWasm.Services;
public static class ServiceUtil {

public static T CreateReq<T>(string qName, Dictionary<string, object> dic) where T : class, IReq {
    Req req = new Req() { QueryName = qName };
    if (dic != null && dic.Count > 0)    {
        foreach (var d in dic)        {
            req.QueryParameters.Add(new QueryParameter() { ParameterName = d.Key, ParameterValue = d.Value });
        }
    }

    if (typeof(T).IsAssignableFrom(typeof(Req)) )    {
        return req as T;
    }
    else    {
        throw new InvalidCastException($"Cannot cast type 'Req' to '{typeof(T).FullName}'. T should be Req or interface of Req.");
    }
}

}