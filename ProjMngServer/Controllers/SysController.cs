﻿using Microsoft.AspNetCore.Mvc;
using ProjMngServer.Services;
using System.Collections.Generic;

namespace ProjMngServer.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SysController : ControllerBase {
  private readonly SysService _sysService;

  public SysController(SysService sysService) {
    _sysService = sysService;
  }

  [HttpPost]
  public ActionResult<Dictionary<string, object>> PostBody([FromBody] Dictionary<string, string> parameters) {

    string req_cname = parameters.TryGetValue("req_cname", out var stpValue) && stpValue != null ? stpValue.ToString().ToLower() : string.Empty;

    var data = _sysService.AppDataClear(req_cname,parameters);
    return Ok(data);
  }


}