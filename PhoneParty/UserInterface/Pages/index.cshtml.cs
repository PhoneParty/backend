using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PhoneParty.Pages;

public class IndexModel : PageModel
{
    public void OnGet()
    {
        
    }
    
    public IActionResult OnPostGoToLobby(string lobbyId)
    {
        return RedirectToPage($"/Lobby/{lobbyId}");
    }
}