using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaQuotes.Services.GeoInformationService.Application.Services
{
    public interface ILoadDatabase
    {
        Task LoadDataAsync(string filePath);
    }
}
