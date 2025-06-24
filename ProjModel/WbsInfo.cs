
namespace ProjModel;


public class WbsInfo : BaseModel {

  public string prj_rid { get; set; }
  public string wbs_id { get; set; }
  public string proc_id { get; set; }
  public string gb1 { get; set; }
  public string gb2 { get; set; }
  public string proc_nm { get; set; }
  public string proc_tp { get; set; }
  public string proc_lvl { get; set; }
  public string build_user { get; set; }
  public string build_status { get; set; }
  public string dev_user { get; set; }
  public DateTime? plan_sdt { get; set; }
  public DateTime? plan_edt { get; set; }
  public DateTime? dev_sdt { get; set; }
  public DateTime? dev_edt { get; set; }
  public string dev_chk { get; set; }
  public string build_chk { get; set; }
  public string build_chk_dt { get; set; }
  public string qc_user { get; set; }
  public string qc_chk { get; set; }
  public string qc_chk_dt { get; set; }
  public string comm { get; set; }
  public string wbs_state { get; set; }
  public bool isComplatge { get; set; }

}




