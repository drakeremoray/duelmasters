using DuelMastersApi.Data.Models;

namespace DuelMastersApi.Services
{
    public interface ITokenService
    {
        string CreateToken(Player player);
    }
}
