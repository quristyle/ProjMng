

namespace skgRestApi.Models;

public class ResultInfo<T> {
  //public int? Code { get; set; } = 0;

  //public IDictionary<string, object>? Res { get; set; }
/// <summary>
/// 결과 칼럼 정보
/// </summary>
  public IDictionary<string, string>? Cols { get; set; }
/// <summary>
/// 결과 데이터 목록
/// </summary>
  public List<T>? Data { get; set; }

}


