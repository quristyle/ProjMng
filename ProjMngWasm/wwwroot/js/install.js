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

