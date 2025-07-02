
namespace ProjModel;


public class RequestDto {
  public string ProcName { get; set; }
  public string ProcType { get; set; } = "srch";
  public bool IsFast { get; set; }
  public bool IsProjDb { get; set; } = false;
  public DateTime Start { get; set; }
  public IDictionary<string, string> MainParam { get; set; }
  public List<Dictionary<string, object>> MultyData { get; set; } = new ();



}


