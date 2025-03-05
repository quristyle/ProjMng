namespace ProjModel {
  public class Devsqlresp {
    public string Dsl_id { get; set; }
    public string Dsl_type { get; set; }
    public string Dsl_cd { get; set; }
    public string Dsl_query { get; set; }
    public string Comm { get; set; }
    public string Proj { get; set; }
    public string DbConnectionString { get; set; }
  }


  public class DbInfo {

    public string Db_ip { get; set; }
    public string Db_port { get; set; }
    public string Db_database { get; set; }
    public string Db_id { get; set; }
    public string Db_pwd { get; set; }
    public string Db_cert { get; set; }
    public string Db_comm { get; set; }
    public string Db_nick { get; set; }
    public string Db_type { get; set; }
  }

  
  public class ProjectInfo {

    public string prj_rid { get; set; }
    public string prj_name { get; set; }
    public string prj_desc { get; set; }
    public string prj_sdt { get; set; }
    public string prj_edt { get; set; }
    public string prj_nick { get; set; }
    public string prj_type { get; set; }
    
  }


  public class CommonCode {

    public string Code { get; set; }
    public string Name { get; set; }
  }

  }
