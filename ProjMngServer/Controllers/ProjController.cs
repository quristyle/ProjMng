using Microsoft.AspNetCore.Mvc;
using ProjMngServer.Services;
using System.Collections.Generic;

namespace ProjMngServer.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProjController : ControllerBase {
  private readonly ProjService _projService;

  public ProjController(ProjService projService) {
    _projService = projService;
  }


  [HttpPost]
  public ActionResult<Dictionary<string, object>> PostBody([FromBody] Dictionary<string, string> parameters) {
    var data = _projService.GetData(parameters);
    return Ok(data);
  }

  [HttpPost]
  [Route("multy")]
  public ActionResult<Dictionary<string, object>> PostForm([FromForm] IFormCollection formData) {
    var parameters = formData.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToString());
    var data = _projService.GetData(parameters);
    return Ok(data);
  }


  [HttpGet]
  public ActionResult<Dictionary<string, object>> Get() {
    var parameters = Request.Query.ToDictionary(q => q.Key, q => q.Value.ToString());
    var data = _projService.GetData(parameters);
    return Ok(data);
  }

}