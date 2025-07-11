using BlazorMonaco.Editor;
using ProjModel;
using WasmShear;

namespace ProjMngWasm.Commons {
  public class AppProjData {


    /*
     | Language Option (`Language`) | 설명                        |
| ---------------------------- | ------------------------- |
| `abap`                       | ABAP                      |
| `apex`                       | Salesforce Apex           |
| `azcli`                      | Azure CLI                 |
| `bat` / `cmd`                | Windows Batch             |
| `bicep`                      | Azure Bicep               |
| `c`                          | C 언어                      |
| `cpp` / `c++`                | C++                       |
| `csharp` / `cs`              | C#                        |
| `clojure`                    | Clojure                   |
| `coffeescript` / `coffee`    | CoffeeScript              |
| `css`                        | CSS                       |
| `dockerfile`                 | Dockerfile                |
| `fsharp` / `fs`              | F#                        |
| `go`                         | Go 언어                     |
| `graphql` / `gql`            | GraphQL                   |
| `handlebars` / `hbs`         | Handlebars                |
| `html`                       | HTML                      |
| `ini`                        | INI 파일                    |
| `java`                       | Java                      |
| `javascript` / `js`          | JavaScript                |
| `json`                       | JSON                      |
| `julia`                      | Julia                     |
| `kotlin`                     | Kotlin                    |
| `less`                       | LESS CSS                  |
| `lua`                        | Lua                       |
| `markdown` / `md`            | Markdown                  |
| `mysql`                      | MySQL                     |
| `pgsql`                      | PostgreSQL (※ 쿼리 편집기에 유용) |
| `objective-c` / `objc`       | Objective-C               |
| `pascal`                     | Pascal                    |
| `perl`                       | Perl                      |
| `php`                        | PHP                       |
| `plaintext`                  | 일반 텍스트                    |
| `powershell` / `ps`          | PowerShell                |
| `python` / `py`              | Python                    |
| `r`                          | R 언어                      |
| `razor`                      | Razor (.cshtml)           |
| `ruby` / `rb`                | Ruby                      |
| `rust`                       | Rust                      |
| `scss`                       | SCSS                      |
| `shell` / `sh` / `bash`      | Shell Script              |
| `sql`                        | 일반 SQL                    |
| `pgsql`                      | PostgreSQL SQL (하이라이팅 다름) |
| `swift`                      | Swift                     |
| `typescript` / `ts`          | TypeScript                |
| `vb`                         | Visual Basic              |
| `xml`                        | XML                       |
| `yaml` / `yml`               | YAML                      |

     */






    public static StandaloneEditorConstructionOptions PgsqlOption { get; set; } =
       new StandaloneEditorConstructionOptions {
         AutomaticLayout = true,
         Language = "pgsql",
         Theme = "vs-dark",
         StickyScroll = new EditorStickyScrollOptions() {
           Enabled = false,
         }
       };
    public static StandaloneEditorConstructionOptions HtmlOption { get; set; } =
       new StandaloneEditorConstructionOptions {
         AutomaticLayout = true,
         Language = "html",
         Theme = "vs-dark",
         StickyScroll = new EditorStickyScrollOptions() {
           Enabled = false,
         }
       };

    public static StandaloneEditorConstructionOptions FnPgsqlOtion(StandaloneCodeEditor editor) {
      return PgsqlOption;
    }

    public static StandaloneEditorConstructionOptions FnHtmlOption(StandaloneCodeEditor editor) {
      return HtmlOption;
    }

  }
}
