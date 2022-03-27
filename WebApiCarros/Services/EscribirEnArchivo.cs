namespace WebApiCarros.Services
{
    //IHostedService permite crear un timer y poder escribir o soreescribir, y en dado caso crear un archivo
    //en dado caso que no sirva
    public class EscribirEnArchivo : IHostedService
    {
        private readonly IWebHostEnvironment env;
        private readonly string nombreArchivo = "Frase.txt";
        private Timer timer;

        public EscribirEnArchivo(IWebHostEnvironment env)
        {
            this.env = env;
        }

        //Se ejecuta cuando cargamos la aplicacion 1 vez
        public Task StartAsync(CancellationToken cancellationToken)
        {
            //Se ejecuta cuando cargamos la aplicacion 1 vez
            timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromMinutes(2));
            Escribir("Proceso Iniciado de la Tarea 3");
            return Task.CompletedTask;
        }

        // Se ejecuta cuando detenemos la aplicacion aunque puede que no se ejecute por algun error.
        public Task StopAsync(CancellationToken cancellationToken)
        {
            // Se ejecuta cuando detenemos la aplicacion aunque puede que no se ejecute por algun error. 
            timer.Dispose();
            Escribir("Proceso Finalizado de la Tarea 3");
            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {
            //Escribir("Proceso en ejecucion: " + DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss"));
            Escribir("El Profe Gustavo Rodriguez es el mejor --->" + DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss"));
        }
        private void Escribir(string msg)
        {
            //Define la ruta con ContentRootPath, que es la ruta del Path por defecto que tiene la aplicación
            //y luego en la carpeta entra y se guarda en el archivo.
            var ruta = $@"{env.ContentRootPath}\wwwroot\{nombreArchivo}";
            //Escribe en el archivo con la ruta, y escibe en el archivo el mensaje que es el que pusimos en el metodo DoWork
            using (StreamWriter writer = new StreamWriter(ruta, append: true)) { writer.WriteLine(msg); }
        }

        internal void sobrescribir(string msg, string arch)
        {
            var ruta = $@"{env.ContentRootPath}\wwwroot\{msg}";
            using (StreamWriter writer = new StreamWriter(ruta, append: true)) { writer.WriteLine(arch + DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss")); }
            throw new NotImplementedException();
        }
    }
}