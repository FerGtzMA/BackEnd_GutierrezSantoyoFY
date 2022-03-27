namespace WebApiCarros.Services
{
    public interface IService
    {
        //Estos métodos sin los que tienen que ir implementados obligatoriamente en ambas clases
        void EjecutarJob();
        Guid GetScoped();
        Guid GetSingleton();
        Guid GetTransient();
    }
    //Ha esta interface se le crearon 2 clses
    //Esta es la clase A, la primera clase
    public class ServiceA : IService
    {
        private readonly ServiceScoped serviceScoped;
        private readonly ServiceSingleton serviceSingleton;

        private readonly ILogger<ServiceA> logger;
        private readonly ServiceTransient serviceTransient;

        //Service A depende de ILogger, Transient, Scoped, Singleton
        public ServiceA(ILogger<ServiceA> logger, ServiceTransient serviceTransient,
            ServiceScoped serviceScoped,
            ServiceSingleton serviceSingleton)
        {
            this.logger = logger;
            this.serviceTransient = serviceTransient;
            this.serviceScoped = serviceScoped;
            this.serviceSingleton = serviceSingleton;
        }

        public Guid GetTransient() { return serviceTransient.guid; }
        public Guid GetScoped() { return serviceScoped.guid; }
        public Guid GetSingleton() { return serviceSingleton.guid; }

        public void EjecutarJob()
        {
        }
    }

    //Esta es la clase A, la segunda clase
    public class ServiceB : IService
    {
        //ServiceB no depende de nada
        public void EjecutarJob()
        {
        }

        public Guid GetScoped()
        {
            throw new NotImplementedException();
        }

        public Guid GetSingleton()
        {
            throw new NotImplementedException();
        }

        public Guid GetTransient()
        {
            throw new NotImplementedException();
        }
    }

    public class ServiceTransient
    {
        public Guid guid = Guid.NewGuid();
    }

    public class ServiceScoped
    {
        public Guid guid = Guid.NewGuid();
    }

    public class ServiceSingleton
    {
        public Guid guid = Guid.NewGuid();
    }
}
