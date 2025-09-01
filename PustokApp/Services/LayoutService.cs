using PustokApp.Data;
using PustokApp.Models;

namespace PustokApp.Services
{
    public class LayoutService
        (PustokDbContex pustokDbContex)
    {
        private readonly PustokDbContex _pustokDbContex = pustokDbContex;
        public Dictionary<string, string> GetSettings()
        {
            return _pustokDbContex.Settings.ToDictionary(s => s.Key, s => s.Value);
        }

    }
}
