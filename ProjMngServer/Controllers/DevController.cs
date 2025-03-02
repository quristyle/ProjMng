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
  [Route("body")]
  public ActionResult<Dictionary<string, object>> PostBody([FromBody] Dictionary<string, object> parameters) {
    var data = _devService.GetData(parameters);
    return Ok(data);
  }

  [HttpPost]
  public ActionResult<Dictionary<string, object>> PostForm([FromForm] IFormCollection formData) {
    var parameters = formData.ToDictionary(kvp => kvp.Key, kvp => (object)kvp.Value.ToString());
    var data = _devService.GetData(parameters);
    return Ok(data);
  }


  [HttpGet]
  public ActionResult<Dictionary<string, object>> Get() {
    var parameters = Request.Query.ToDictionary(q => q.Key, q => (object)q.Value.ToString());
    var data = _devService.GetData(parameters);
    return Ok(data);
  }

}

