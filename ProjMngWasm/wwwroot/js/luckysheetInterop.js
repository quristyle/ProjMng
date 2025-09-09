window.luckysheetInterop = {
  create: function (elementId, data) {
    // 기본 설정
    luckysheet.create({
      container: elementId,
      title: 'Blazor Luckysheet',
      lang: 'en',
      data: data || [
        {
          name: 'Sheet1',
          celldata: [
            { r: 0, c: 0, v: { v: "Hello" } },
            { r: 0, c: 1, v: { v: "Blazor" } },
            { r: 1, c: 0, v: { v: 123 } }
          ]
        }
      ]
    });
  },

  getData: function () {
    return luckysheet.getAllSheets();
  }
  ,

    // DB에서 불러온 JSON으로 Luckysheet 다시 그리기
    setData: function (elementId, ttl, data) {
    // 기존 Luckysheet 영역 비우기
    document.getElementById(elementId).innerHTML = "";

    // 새로 생성
    luckysheet.create({
      container: elementId,
      title: ttl,
      lang: 'en',
      data: data
    });
  }



};
