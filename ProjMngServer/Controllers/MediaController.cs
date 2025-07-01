using Microsoft.AspNetCore.Mvc;
using ProjMngServer.Services;
using ProjModel;
using System.Collections.Generic;

namespace ProjMngServer.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MediaController : ControllerBase {
  private readonly ProjService _projService;

  public MediaController(ProjService projService) {
    _projService = projService;
  }

  //[HttpPost]
  //public ActionResult<Dictionary<string, object>> PostBody([FromBody] Dictionary<string, string> parameters) {

  //  string req_cname = parameters.TryGetValue("req_cname", out var stpValue) && stpValue != null ? stpValue.ToString().ToLower() : string.Empty;

  //  var data = _projService.GetMdData(req_cname,parameters);
  //  return Ok(data);
  //}




  [HttpPost]
  public ActionResult<ResultInfo<Dictionary<string, object>>> PostBody([FromBody] RequestDto dto) {
    //ResultInfo<dynamic> data = null;
    //if (
      //dto.MultyData == null || dto.MultyData.Count <= 0) {
    var  data = _projService.GetMdData(dto);
    //}
    //else {

    //  data = _projService.ExcuteMultyData(dto);

    //}
    return Ok(data);
  }



}