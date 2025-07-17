let deferredInstallPrompt = null;
let installButton = null;


window.addEventListener('load', () => {
  //installButton = document.querySelector("#install");
  //installButton.addEventListener('click', installPWA );
});


//window.addEventListener( 'beforeinstallprompt', saveBeforeInstallPromptEvent );


function saveBeforeInstallPromptEvent ( evt ) {
//  evt.prevenDefault();
  deferredInstallPrompt = evt;
  //installButton.removeAttribute('hidden');
}



function installPWA ( ) {
  deferredInstallPrompt.prompt();
}




// wwwroot/js/excelDownload.js
window.downloadExcel = (data, filename) => {
  const ws = XLSX.utils.aoa_to_sheet(data);  // 데이터 변환
  const wb = XLSX.utils.book_new();
  XLSX.utils.book_append_sheet(wb, ws, "Sheet1");

  // 엑셀 파일 다운로드
  XLSX.writeFile(wb, filename);
};



