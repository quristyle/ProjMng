﻿using Microsoft.AspNetCore.Mvc;
using ProjMngServer.Services;
using ProjModel;
using System.Collections.Generic;

namespace ProjMngServer.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProjController : ControllerBase {
  private readonly ProjService _projService;

  public ProjController(ProjService projService) {
    _projService = projService;
  }


  //[HttpPost]
  //public ActionResult<ResultInfo<Dictionary<string, object>>> PostBody([FromBody] Dictionary<string, string> param) {
  //  string req_cname = param.TryGetValue("req_cname", out var stpValue) && stpValue != null ? stpValue.ToString().ToLower() : string.Empty;
  //  var data = _projService.GetData(req_cname,param);
  //  return Ok(data);
  //}

  [HttpPost]
  public ActionResult<ResultInfo<Dictionary<string, object>>> PostBody([FromBody] RequestDto dto) {
    ResultInfo<dynamic> data = null;
    if (
      dto.MultyData == null || dto.MultyData.Count <= 0) {
      data = _projService.GetData(dto); 
    }
    else { 
      
      data = _projService.ExcuteMultyData(dto); 
    
    }
    return Ok(data);
  }


  //[HttpPost]
  //[Route("multydata")]
  //public ActionResult<ResultInfo<Dictionary<string, object>>> PostBodyMulty([FromBody] Dictionary<string, string> param) {
  //  string req_cname = param.TryGetValue("req_cname", out var stpValue) && stpValue != null ? stpValue.ToString().ToLower() : string.Empty;
  //  object req_multydata = param.TryGetValue("req_multydata", out var stpValue2) ;
  //  var data = _projService.GetData(req_cname, param);
  //  return Ok(data);
  //}
  //[HttpPost]
  //[Route("fast")]
  //public ActionResult<Dictionary<string, object>> Fast([FromBody] Dictionary<string, string> param) {

  //  string req_cname = param.TryGetValue("req_cname", out var stpValue) && stpValue != null ? stpValue.ToString().ToLower() : string.Empty;

  //  var data = _projService.GetFastData(req_cname, param);
  //  return Ok(data);
  //}

  [HttpPost]
  [Route("multy")]
  public ActionResult<Dictionary<string, object>> PostForm([FromForm] IFormCollection formData) {
    var parameters = formData.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToString());


    string req_cname = parameters.TryGetValue("req_cname", out var stpValue) && stpValue != null ? stpValue.ToString().ToLower() : string.Empty;

    var data = _projService.GetData(req_cname,parameters);
    return Ok(data);
  }



}