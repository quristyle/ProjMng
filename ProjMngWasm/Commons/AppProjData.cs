using BlazorMonaco.Editor;
using ProjModel;
using WasmShear;

namespace ProjMngWasm.Commons {
  public class AppProjData {
    
    public static StandaloneEditorConstructionOptions PgsqlOtion { get; set; } =
       new StandaloneEditorConstructionOptions {
         AutomaticLayout = true,
         Language = "pgsql",
         Theme = "vs-dark",
         StickyScroll = new EditorStickyScrollOptions() {
           Enabled = false,
         }
       };

    public static StandaloneEditorConstructionOptions FnPgsqlOtion(StandaloneCodeEditor editor) {
      return PgsqlOtion;
    }


  }
}
