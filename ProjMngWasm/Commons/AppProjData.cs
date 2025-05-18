using BlazorMonaco.Editor;
using ProjModel;
using WasmShear;

namespace ProjMngWasm.Commons {
  public class AppProjData: AppData {
    
    public StandaloneEditorConstructionOptions PgsqlOtion { get; set; } =
       new StandaloneEditorConstructionOptions {
         AutomaticLayout = true,
         Language = "pgsql",
         Theme = "vs-dark",
         StickyScroll = new EditorStickyScrollOptions() {
           Enabled = false,
         }
       };

    public StandaloneEditorConstructionOptions FnPgsqlOtion(StandaloneCodeEditor editor) {
      return PgsqlOtion;
    }


  }
}
