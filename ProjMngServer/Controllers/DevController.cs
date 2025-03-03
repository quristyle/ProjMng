using Microsoft.AspNetCore.Mvc;
using ProjMngServer.Services;
using System.Collections.Generic;

namespace ProjMngServer.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DevController : ControllerBase {
  private readonly DevService _devService;

  public DevController(DevService devService) {
    _devService = devService;
  }


  [HttpPost]
  public ActionResult<Dictionary<string, object>> PostBody([FromBody] Dictionary<string, string> parameters) {
    var data = _devService.GetData(parameters);
    return Ok(data);
  }

  [HttpPost]
  [Route("multy")]
  public ActionResult<Dictionary<string, object>> PostForm([FromForm] IFormCollection formData) {
    var parameters = formData.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToString());
    var data = _devService.GetData(parameters);
    return Ok(data);
  }


  [HttpGet]
  public ActionResult<Dictionary<string, object>> Get() {
    var parameters = Request.Query.ToDictionary(q => q.Key, q => q.Value.ToString());
    var data = _devService.GetData(parameters);
    return Ok(data);
  }

}