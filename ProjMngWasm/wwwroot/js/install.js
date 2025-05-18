let deferredInstallPrompt = null;
let installButton = null;


window.addEventListener('load', () => {


console.log('lllllllllllllllllllllllllllllload');

  installButton = document.querySelector("#install");
console.log('installButton', installButton);
  installButton.addEventListener('click', installPWA );
  
});


window.addEventListener( 'beforeinstallprompt', saveBeforeInstallPromptEvent );


function saveBeforeInstallPromptEvent ( evt ) {
//  evt.prevenDefault();
  deferredInstallPrompt = evt;
  installButton.removeAttribute('hidden');
}



function installPWA ( ) {

  deferredInstallPrompt.prompt();
}

