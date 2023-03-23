using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ExampleOpenAiPlugin.Pages.Posts;

public class IndexModel : PageModel
{
    [FromQuery]
    public bool Static { get; set; }
    [FromQuery]
    public string? Author { get; set; }
    [FromQuery]
    public string? Tag { get; set; }
}