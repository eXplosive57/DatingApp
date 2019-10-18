namespace DatingApp.API.Helpers
{
    public class UserParams // quando il client chiede una pagina di default sarà sempre la pagina 1 con 10 utenti
    {
        private const int MaxPageSize = 50;
        public int PageNumber { get; set; } = 1;
        private int pageSize = 10;
        public int  PageSize
        {
            get { return pageSize;}
            set { pageSize = (value > MaxPageSize) ? MaxPageSize : value; } // se il valore che richiede l'utente è > del MAX allora ritorna Max, altrimenti il valore richiesto
        }

        public int UserId { get; set; }
        public string Gender { get; set; }

        public int MinAge { get; set; } = 18;
        public int MaxAge { get; set; } = 99;

        public string OrderBy { get; set; }

    }
}