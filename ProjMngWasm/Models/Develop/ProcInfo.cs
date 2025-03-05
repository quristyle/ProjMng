using Radzen.Blazor;

namespace ProjMngWasm.Models.Develop {
  public class ProcInfo {
    public string ProcedureName { get; set; }
    public string Description { get; set; }
    public string Routine_Definition { get; set; }
  }

  public class TableInfo {
    public string TableName { get; set; }
    public string Description { get; set; }

  }



  public class ColumnInfo {
    public string ColunmName { get; set; }
    public string Description { get; set; }
    public string Type_NM { get; set; }
    public string Max_LENGTH { get; set; }
    public string NullAble { get; set; }
    public string Ident { get; set; }

  }


}
