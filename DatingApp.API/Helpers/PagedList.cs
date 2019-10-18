using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Helpers
{
    public class PagedList<T> : List<T>
    {
        public int CurrentPage { get; set; } // numero della pagina
        public int TotalPages { get; set; }
        public int PageSize { get; set; } // utenti da visualizzare
        public int TotalCount { get; set; }

        public PagedList(List<T> items, int count, int pageNumber, int pageSize) // count conta gli utenti totali della pagina
        {
            TotalCount = count;
            PageSize = pageSize;
            CurrentPage = pageNumber;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            this.AddRange(items);
        }

        public static async Task<PagedList<T>> CreateAsync(IQueryable<T> source,
            int pageNumber, int pageSize)
        {
            var count = await source.CountAsync();
            // se per esempio vogliamo la pagina 2 di 5 utenti la formula sarà: (2-1) * 5 (quindi skippa i primi 5) e da i prossimi 5
            var items = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
            return new PagedList<T>(items, count, pageNumber, pageSize);
        }
    }
}