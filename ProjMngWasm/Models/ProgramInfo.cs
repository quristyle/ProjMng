namespace ProjMngWasm.Models;

  public class ProgramInfo {
    public ProgramInfo(string _componentType, string projname) {
      ComponentType = Type.GetType(_componentType);
      ProjName = projname;
    }
    public string? ProjName { get; set; }
  public string? ProjId { get; set; }
  public string? Url { get; set; }
  public string? ProjPath { get; set; }
    public Type? ComponentType { get; set; }
  }