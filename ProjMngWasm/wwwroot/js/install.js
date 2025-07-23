let deferredInstallPrompt = null;
let installButton = null;


window.addEventListener('load', () => {
  //installButton = document.querySelector("#install");
  //installButton.addEventListener('click', installPWA );
});


//window.addEventListener( 'beforeinstallprompt', saveBeforeInstallPromptEvent );


function saveBeforeInstallPromptEvent(evt) {
  //  evt.prevenDefault();
  deferredInstallPrompt = evt;
  //installButton.removeAttribute('hidden');
}



function installPWA() {
  deferredInstallPrompt.prompt();
}




// wwwroot/js/excelDownload.js
window.downloadExcel2 = (data, filename) => {

  console.log('data :', data, filename);


  const ws = XLSX.utils.aoa_to_sheet(data);  // 데이터 변환
  const wb = XLSX.utils.book_new();
  XLSX.utils.book_append_sheet(wb, ws, "Sheet1");

  // 엑셀 파일 다운로드
  XLSX.writeFile(wb, filename);
};




//import ExcelJS from 'exceljs';

window.downloadExcel = async (data, filename) => {
 

  const workbook = new ExcelJS.Workbook();
  const worksheet = workbook.addWorksheet('Sheet1');

  
  // 데이터 채우기
  data.forEach((row, rowIndex) => {
    const excelRow = worksheet.addRow(row);

    
    // 첫 번째 행(헤더)에 스타일 지정
    if (rowIndex === 0) {
      excelRow.eachCell((cell) => {
        cell.font = { bold: true, size: 9 };
        cell.fill = {
          type: 'pattern',
          pattern: 'solid',
          fgColor: { argb: 'FFD3D3D3' } // 회색 (연한 회색: LightGray)
        };
        cell.alignment = { vertical: 'middle', horizontal: 'center' };
        cell.border = {
          top: { style: 'thin' },
          left: { style: 'thin' },
          bottom: { style: 'thin' },
          right: { style: 'thin' }
        };
      });
    }
    else {
      excelRow.eachCell((cell) => {
        cell.font = { size: 9 };
        cell.alignment = { vertical: 'middle' };
        cell.border = {
          top: { style: 'thin' },
          left: { style: 'thin' },
          bottom: { style: 'thin' },
          right: { style: 'thin' }
        };
      });
    }
    
  });


  /*
  // 자동 열 너비 조절
  worksheet.columns.forEach((column) => {
    const lengths = column.values.map(v => (v ? v.toString().length : 0));
    const maxLength = Math.max(...lengths, 10);
    column.width = maxLength + 2;
  });
  */

  if (data.length > 0) {
    // 열 너비 수동 계산
    data[0].forEach((_, colIndex) => {
      let maxLength = 5;
      data.forEach((row) => {
        const cellValue = row[colIndex];
        if (cellValue) {
          const len = cellValue.toString().length;
          if (len > maxLength) maxLength = len;
        }
      });
      worksheet.getColumn(colIndex + 1).width = maxLength + 3;
    });
  }

  worksheet.eachRow((row, rowNumber) => {
    row.height = 25;  // 모든 행의 높이를 25pt로
  });


  // 엑셀 파일 버퍼 생성
  const buffer = await workbook.xlsx.writeBuffer();

  // 브라우저 다운로드
  const blob = new Blob([buffer], {
    type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet'
  });



  const url = URL.createObjectURL(blob);

  const a = document.createElement('a');
  a.href = url;
  a.download = filename;
  a.click();

  URL.revokeObjectURL(url);
};



function getval(obj, i, j) {
  obj[i][j]
}


window.downloadExcel_cust = async (data, bkcolors, bolds, lines, merges, widths, filename) => {


  console.log('downloadExcel_cust :', bkcolors, bolds, lines, merges, widths, filename, data);

  const workbook = new ExcelJS.Workbook();
  const worksheet = workbook.addWorksheet('Sheet1');

  // 데이터 채우기
  data.forEach((row, rowIndex) => {
    const excelRow = worksheet.addRow(row);

      excelRow.eachCell((cell, cellIndex) => {
        cell.font = { size: 9 };
        cell.alignment = { vertical: 'middle' };



        console.log('eachCell :', rowIndex, cellIndex);


        // bolds에 "rowIndex,cellIndex" 형태로 포함되어 있는지 체크
        if (bolds != null &&  bolds.includes(`${rowIndex},${cellIndex - 1}`)) {
          cell.font.bold = true;
          cell.alignment.horizontal = 'center' ;
        }

        if (lines != null && lines.includes(`${rowIndex},${cellIndex - 1}`)) {
          cell.border = {
            top: { style: 'thin' },
            left: { style: 'thin' },
            bottom: { style: 'thin' },
            right: { style: 'thin' }
          };
        }


        if (bkcolors != null && bkcolors.includes(`${rowIndex},${cellIndex - 1}`)) {
          cell.fill = {
            type: 'pattern',
            pattern: 'solid',
            fgColor: { argb: 'FFD3D3D3' } // 회색 (연한 회색: LightGray)
          };
        }



        //cell.border = {
        //  top: { style: 'thin' },
        //  left: { style: 'thin' },
        //  bottom: { style: 'thin' },
        //  right: { style: 'thin' }
        //};
      });

  });



  if (widths != null) {

    widths.forEach((c, cIdx) => {
      worksheet.getColumn(cIdx + 1).width = c + 3;
    }); 

  }



  if (merges != null) {
    merges.forEach(mergeStr => {
      // "0,0,4,0" → [0,0,4,0]
      const [rowStart, colStart, rowEnd, colEnd] = mergeStr.split(',').map(Number);
      // ExcelJS는 1-based index이므로 +1
      worksheet.mergeCells(
        rowStart + 1,
        colStart + 1,
        rowEnd + 1,
        colEnd + 1
      );
    });
  }





  worksheet.eachRow((row, rowNumber) => {
    row.height = 25;  // 모든 행의 높이를 25pt로
  });


  // 엑셀 파일 버퍼 생성
  const buffer = await workbook.xlsx.writeBuffer();

  // 브라우저 다운로드
  const blob = new Blob([buffer], {
    type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet'
  });

  const url = URL.createObjectURL(blob);

  const a = document.createElement('a');
  a.href = url;
  a.download = filename;
  a.click();

  URL.revokeObjectURL(url);
};




window.disableF5 = function () {



  window.addEventListener('keydown', function (e) {

    console.log('ffffffffffffffffffffffffffffff55555555555555555555555555555');
    // F5: keyCode 116, 또는 e.key === 'F5'
    if (e.key === 'F5' || e.keyCode === 116) {
      e.preventDefault();
    }
  }, true);
};




window.copyClipboard = function (text) {
  window.focus();
  navigator.clipboard.writeText(text).then(function () {
    console.log('Text copied to clipboard');
  }).catch(function (error) {
    console.error('Could not copy text: ', error);
  });
};






