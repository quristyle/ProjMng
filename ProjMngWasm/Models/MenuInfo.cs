namespace ProjMngWasm.Models {
  public class MenuInfo : BaseModel {

    public string owner_id { get; set; }
    public string mnu_cd { get; set; }
    public string mnu_nm { get; set; }
    public string disp_seq { get; set; }
    public string parent_mnu_cd { get; set; }
    public string mnu_grp_yn { get; set; }
    public string pgm_id { get; set; }
    public string mnu_id { get; set; }
    public string use_yn { get; set; }
    public string pgm_ty { get; set; }

    public bool Expanded { get; set; }
    public List<MenuInfo> children { get; set; } = new List<MenuInfo>();


  }


  public class BaseModel {
    public string cre_id { get; set; }  
    public string cre_dt { get; set; }
    public string mod_id { get; set; }
    public string mod_dt { get; set; }

  }


  }
