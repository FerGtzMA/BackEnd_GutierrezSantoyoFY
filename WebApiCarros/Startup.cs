using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using WebApiCarros.Filtros;
using WebApiCarros.Services;
/*El Middleware es una cola de peticiones o requerimientos*/
using WebApiCarros.Middlewares;
using WebApiCarros.Controllers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;

namespace WebApiCarros
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            //Principio solid , nuestras clases deberian depender de abstracciones y no tipos concretos
            //var alumnosController = new AlumnosController(new ApplicationDbContext());
            Configuration = configuration;
        }
        public IConfiguration Configuration { get; }




        public void ConfigureServices(IServiceCollection services)
        {
            //Se tiene la opción de definir el FiltroDeExcepcion
            //y lo que hace es que si ocurre un error lo va a marcar
            //Y es un FILTRO GLOBAL
            services.AddControllers(opciones =>
            {
                opciones.Filters.Add(typeof(FiltroDeExcepcion));
            }).AddJsonOptions(x =>
            x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

            // Se encarga de configurar ApplicationDbContext como un servicio
            services.AddDbContext<AplicationDbContext>(options =>
            //options.UseSqlServer(Configuration.GetConnectionString("defaultConnection")));
            options.UseSqlServer("defaultConnection"));

            //Transient da nueva instancia de la clase declarada,
            //sirve para funciones que ejecutan una funcionalidad y listo, sin tener
            //que mantener información que será reutilizada en otros lugares
            services.AddTransient<IService, ServiceA>();
            //services.AddTransient<ServiceA>();
            services.AddTransient<ServiceTransient>();
            //Scoped el tiempo de vida de la clase declarada aumenta, sin embargo, Scoped da diferentes instancia
            //de acuerdo a cada quien mande la solicitud es decir Gustavo tiene su intancia y Alumno otra
            //services.AddScoped<IService, ServiceA>();
            services.AddScoped<ServiceScoped>();
            //Singleton se tiene la misma instancia siempre para todos los usuarios en todos los días,
            //todos los usuarios que hagan una petición van a tener la misma info compartida entre todos 
            //services.AddSingleton<IService, ServiceA>();
            services.AddSingleton<ServiceSingleton>();
            services.AddTransient<FiltroDeAccion>();
            services.AddHostedService<EscribirEnArchivo>();
            services.AddResponseCaching();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "WebAPICarros", Version = "v1" });
            });


/*--------------------------------------------------------------------------------------------------------------------
            services.AddControllers().AddJsonOptions(x => 
            x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            services.AddDbContext<AplicationDbContext>(options =>
            options.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=ClaseWebApis"));

            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "WebAPICarros", Version = "v1" });
            });
*///------------------------------------------------------------------------------------------------------------------
        }
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
        {
            /*
            //Use me permite agregar mi propio proceso sin afectar a los demas como Run
            app.Use(async (context, siguiente) =>
            {
                using (var ms = new MemoryStream())
                {
                    //Se asigna el body del response en una variable y se le da el valor de memorystream
                    var bodyOriginal = context.Response.Body;
                    context.Response.Body = ms;

                    //Permite continuar con la linea
                    await siguiente.Invoke();

                    //Guardamos lo que le respondemos al cliente en el string
                    ms.Seek(0, SeekOrigin.Begin);
                    string response = new StreamReader(ms).ReadToEnd();
                    ms.Seek(0, SeekOrigin.Begin);

                    //Leemos el stream y lo colocamos como estaba
                    await ms.CopyToAsync(bodyOriginal);
                    context.Response.Body = bodyOriginal;

                    logger.LogInformation(response);
                }
            });
            */
            //Metodo para utilizar la clase middleware propia
            app.UseMiddleware<ResponseHttpMiddleware>();

            //Metodo para utilizar la clase middleware sin exponer la clase. 
            //app.UseResponseHttpMiddleware();


            //Atrapara todas las peticiones http que mandemos y retornar un string
            //Para detener todos los otros middleware se utiliza la funcion RUN

            //Para condicionar la ejecucion del middleware segun una ruta especifica se utiliza Map
            //Al utilizar Map permite que en lugar de ejecutar linealmente podemos agregar rutas especificas para
            // nuestro middleware
            
            app.Map("/maping", app =>
            {
                //Esto permite ejecutar a nivel del middleware una acción con el Run
                //Pero al momento de ejecutar ya no va a cargar lo que le sigue
                app.Run(async context =>
                {
                    //va a interceptar las peticiones de toda la aplicación, despues
                    //de esta linea no se ejecutara lo demás si es que este no estuviera 
                    //adentro del       app.Map
                    //Se ejecutara y rompera las demás lineas de codigo si es que en el url
                    //se pone /maping
                    await context.Response.WriteAsync("Interceptando las peticiones");
                });
            });
            

            //Todo lo que tenga la palabra app. o .Use es parte del middleware
            // Configure the HTTP request pipeline.
            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseResponseCaching();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

        }
    }
}
