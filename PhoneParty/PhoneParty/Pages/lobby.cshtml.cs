using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;

namespace PhoneParty.Pages;

public class LobbyModel : PageModel
{
    [BindProperty(SupportsGet = true)]
    public string LobbyId { get; set; }

    public void OnGet()
    {
        // Здесь можно добавить логику инициализации, если нужно
    }
}