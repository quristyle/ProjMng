using BlazorMonaco.Editor;
using ProjModel;

namespace ProjMngWasm.Commons {
  public class AppData {
    public IDictionary<string, List<CommonCode>> GlobalDic { get; set; } = new Dictionary<string, List<CommonCode>>();

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
